using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.OrderModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Domain.Paginate;
using Net.payOS;
using Net.payOS.Types;
using System.IdentityModel.Tokens.Jwt;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class OrderService(IUnitOfWork uow, IConfiguration configuration, IMapper mapper) : IOrderService
    {
        private readonly IUnitOfWork _uow = uow;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;
        public async Task<(long, IPaginate<Order>?, string?)> GetAllOrders(OrderSearchModel? search)
        {
            if (search == null)
            {
                return (4, await _uow.GetRepository<Order>().GetPagingListAsync(include: o => o.Include(od => od.Orderstatusupdates).Include(od => od.Staff).Include(od => od.Orderitems).Include(od => od.Voucherusages).ThenInclude(vu => vu.Voucher).Include(od => od.Transactions).ThenInclude(t => t.PaymentMethod), page: 1, size: 10, orderBy: o => o.OrderByDescending(od => od.CreateAt)), null);
            }
            //if (search.Status != null && search.Status != OrderConstant.PENDING && search.Status != OrderConstant.SHIPPED && search.Status != OrderConstant.DELIVERED && search.Status != OrderConstant.CANCELED)
            //{
            //    return (0, null, "Status must be in [PENDING, SHIPPED, DELIVERED, CANCELED]");
            //}
            var staff = await _uow.GetRepository<Domain.Models.Account>().SingleOrDefaultAsync(predicate: acc => acc.AccountId == search.StaffId);
            if (search.StaffId != null && staff == null)
            {
                return (0, null, "Staff not found");
            }
            var list = await _uow.GetRepository<Order>().GetPagingListAsync(include: o => o.Include(od => od.Orderstatusupdates).Include(od => od.Staff).Include(od => od.Orderitems).Include(od => od.Voucherusages).ThenInclude(vu => vu.Voucher).Include(od => od.Transactions).ThenInclude(t => t.PaymentMethod),
                                                                        predicate: o => (!search.StaffId.HasValue || o.StaffId == search.StaffId) &&
                                                                                        (!search.FromDate.HasValue || o.CreateAt.Value.Date >= search.FromDate.Value.Date) &&
                                                                                        (!search.ToDate.HasValue || o.CreateAt.Value.Date <= search.ToDate.Value.Date) &&
                                                                                        (!search.MinPrice.HasValue || o.TotalAmount >= search.MinPrice) &&
                                                                                        (!search.MaxPrice.HasValue || o.TotalAmount >= search.MaxPrice) &&
                                                                                        (string.IsNullOrEmpty(search.Status) || o.Orderstatusupdates.FirstOrDefault().OrderStatus.ToLower().Equals(search.Status.ToLower())),
                                                                        page: search.Page.HasValue ? (int)search.Page : 1,
                                                                        size: search.PageSize.HasValue ? (int)search.PageSize : 10,
                                                                        orderBy: o => ((search.SortAscending.HasValue && search.SortAscending.Value) ? ((string.IsNullOrEmpty(search.SortBy) || search.SortBy.ToLower().Equals("createat")) ? o.OrderBy(od => od.CreateAt)
                                                                                                                                                                                                                                            : search.SortBy.ToLower().Equals("orderid") ? o.OrderBy(od => od.OrderId)
                                                                                                                                                                                                                                            : search.SortBy.ToLower().Equals("totalamount") ? o.OrderBy(od => od.TotalAmount)
                                                                                                                                                                                                                                            : search.SortBy.ToLower().Equals("staffid") ? o.OrderBy(od => od.StaffId)
                                                                                                                                                                                                                                                                                                : o.OrderBy(od => od.Orderstatusupdates.FirstOrDefault().OrderStatus))
                                                                                                                                                     : ((string.IsNullOrEmpty(search.SortBy) || search.SortBy.ToLower().Equals("createat")) ? o.OrderByDescending(od => od.CreateAt)
                                                                                                                                                                                                                                            : search.SortBy.ToLower().Equals("orderid") ? o.OrderByDescending(od => od.OrderId)
                                                                                                                                                                                                                                            : search.SortBy.ToLower().Equals("totalamount") ? o.OrderByDescending(od => od.TotalAmount)
                                                                                                                                                                                                                                            : search.SortBy.ToLower().Equals("staffid") ? o.OrderByDescending(od => od.StaffId)
                                                                                                                                                                                                                                                                                                : o.OrderByDescending(od => od.Orderstatusupdates.FirstOrDefault().OrderStatus)))
                                                                        );
            var clientId = _configuration["payOS:ClientId"];
            if (clientId == null)
            {
                return (1, null, "ClientId not found"); //ClientId not found
            }
            var apiKey = _configuration["payOS:ApiKey"];
            if (apiKey == null)
            {
                return (2, null, "APIKEY not found"); //ApiKey not found
            }
            var checksumKey = _configuration["payOS:ChecksumKey"];
            if (checksumKey == null)
            {
                return (3, null, "ChecksumKey not found"); //ChecksumKey not found
            }

            PayOS _payOS = new(clientId, apiKey, checksumKey);
            foreach (var item in list.Items)
            {
                if (item.Transactions.FirstOrDefault() != null && item.Transactions.FirstOrDefault()?.PaymentMethodId == 3)
                {
                    PaymentLinkInformation paymentLinkInformation = await _payOS.getPaymentLinkInformation(item.OrderId);
                    if (paymentLinkInformation.status == "CANCELLED" && (item.Orderstatusupdates.OrderByDescending(o => o.UpdatedAt).Take(1).FirstOrDefault()?.OrderStatus == "PREPARING" || item.Orderstatusupdates.OrderByDescending(o => o.UpdatedAt).Take(1).FirstOrDefault()?.OrderStatus == "PENDING"))
                    {
                        var stt = OrderConstant.CANCELLED.ToString();
                        var orderStatus = new Orderstatusupdate
                        {
                            OrderStatus = stt[..1].ToUpper() + stt[1..].ToLower(),
                            OrderId = item.OrderId,
                            UpdatedAt = DateTime.Now,
                            AccountId = item.StaffId,
                        };

                        await _uow.GetRepository<Orderstatusupdate>().InsertAsync(orderStatus);
                        var transaction = item.Transactions.FirstOrDefault();
                        transaction.Status = false;
                        transaction.TransactionDate = DateTime.Now;
                        transaction.UpdatedAt = DateTime.Now;

                        _uow.GetRepository<Domain.Models.Transaction>().UpdateAsync(transaction);
                        if (await _uow.CommitAsync() <= 0)
                        {
                            return (4, null, "Cannot update statuses"); //fail
                        }
                    }
                    else if (paymentLinkInformation.status == "PAID" && (item.Orderstatusupdates.OrderByDescending(o => o.UpdatedAt).Take(1).FirstOrDefault()?.OrderStatus == "PREPARING" || item.Orderstatusupdates.OrderByDescending(o => o.UpdatedAt).Take(1).FirstOrDefault()?.OrderStatus == "PENDING"))
                    {
                        var stt = OrderConstant.SUCCESS.ToString();
                        var orderStatus = new Orderstatusupdate
                        {
                            OrderStatus = stt[..1].ToUpper() + stt.Substring(1).ToLower(),
                            OrderId = item.OrderId,
                            UpdatedAt = DateTime.Now,
                            AccountId = item.StaffId,
                        };

                        await _uow.GetRepository<Orderstatusupdate>().InsertAsync(orderStatus);
                        var transaction = item.Transactions.FirstOrDefault();
                        transaction.Status = true;
                        transaction.TransactionDate = DateTime.Now;
                        transaction.UpdatedAt = DateTime.Now;

                        _uow.GetRepository<Domain.Models.Transaction>().UpdateAsync(transaction);
                        if (await _uow.CommitAsync() <= 0)
                        {
                            return (4, null, "Cannot update statuses"); //fail
                        }
                    }
                }
            }
            return (4, list, null);
        }
        //public async Task<IPaginate<Order>> GetOrdersByStaffId(int staffId)
        //{
        //    return await _uow.GetRepository<Order>().GetPagingListAsync(orderBy: o => o.OrderByDescending(od => od.CreateAt), predicate: o => o.StaffId == staffId);
        //}
        //public async Task<IPaginate<Order>> GetOrdersByPaymentMethodId(int methodId)
        //{
        //    return await _uow.GetRepository<Order>().GetPagingListAsync(orderBy: o => o.OrderByDescending(od => od.CreateAt), predicate: o => o.PaymentMethodId == methodId);
        //}
        public async Task<(long, Order?, string?)> GetOrderDetail(int orderId)
        {
            var order = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderId, include: o => o.Include(od => od.Orderstatusupdates).Include(od => od.Staff).Include(od => od.Orderitems).ThenInclude(oi => oi.Product).Include(od => od.Voucherusages).ThenInclude(vu => vu.Voucher).Include(od => od.Transactions).ThenInclude(t => t.PaymentMethod));
            if (order == null)
            {
                return (400, null, "Order not found!");
            }
            return (200, order, null);
        }
        public async Task<MethodResult<OrderResponse>> CreateOrder(OrderRequest orderRequest, int userId)
        {

            var orderItems = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.OrderId == null);
            decimal? totalAmount = 0;
            if (orderItems == null || orderItems?.Count == 0)
            {
                return new MethodResult<OrderResponse>.Failure("Order not have any product!", StatusCodes.Status400BadRequest);
            }
            var account = await _uow.GetRepository<Domain.Models.Account>().SingleOrDefaultAsync(predicate: a => a.AccountId == userId);

            //var account = await GetCurrentUser();
            //if (account == null)
            //{
            //    return new MethodResult<Order>.Failure("Login required!", StatusCodes.Status400BadRequest);
            //}
            //var paymrentmethod = await _uow.GetRepository<Paymentmethod>().SingleOrDefaultAsync(predicate: PM => PM.PaymentMethodId == orderRequest.PaymentMethodId);
            //if (paymrentmethod == null)
            //{
            //    return new MethodResult<Order>.Failure("Paymentmethod not valid!", StatusCodes.Status400BadRequest);
            //}
            if (account == null)
            {
                return new MethodResult<OrderResponse>.Failure("Login required!", StatusCodes.Status400BadRequest);
            }
            foreach (var item in orderItems)
            {
                totalAmount += item.Price;
            }
            var orders = await _uow.GetRepository<Order>().GetListAsync();
            var orderId = orders != null && orders.Count > 0 ? orders.Last().OrderId + 1 : 1;

            if (!string.IsNullOrEmpty(orderRequest.VoucherCode))
            {
                var voucher = await _uow.GetRepository<Voucher>().SingleOrDefaultAsync(predicate: v => v.VoucherCode.ToLower().Equals(orderRequest.VoucherCode.ToLower()));
                if (voucher == null)
                {
                    return new MethodResult<OrderResponse>.Failure("Voucher not found!", StatusCodes.Status400BadRequest);
                }
                if (voucher.ExpirationDate < DateTime.Now)
                {
                    return new MethodResult<OrderResponse>.Failure("Voucher not valid!", StatusCodes.Status400BadRequest);
                }
                if (voucher.MinimumOrderAmount > totalAmount)
                {
                    return new MethodResult<OrderResponse>.Failure("Not eligible to use voucher!", StatusCodes.Status400BadRequest);
                }
                var desc = voucher.DiscountType.ToUpper() == DiscountTypeConstant.AMOUNT.ToString() ? voucher.DiscountAmount : totalAmount * voucher.DiscountAmount;

                totalAmount = desc > totalAmount ? 0 : totalAmount - desc;
            }
            var order = new Order
            {
                OrderId = orderId,
                TotalAmount = totalAmount,
                CreateAt = DateTime.Now,
                Note = orderRequest.Note,
                StaffId = userId,
                //StaffId = account.AccountId,
                //PaymentMethodId = orderRequest.PaymentMethodId
            };

            await _uow.GetRepository<Order>().InsertAsync(order);
            if (await _uow.CommitAsync() > 0)
            {
                foreach (var item in orderItems)
                {
                    item.OrderId = orderId;
                    _uow.GetRepository<Orderitem>().UpdateAsync(item);
                }
                if (await _uow.CommitAsync() == 0)
                {
                    return new MethodResult<OrderResponse>.Failure("Create order not success!", StatusCodes.Status400BadRequest);
                }
                var status = await _uow.GetRepository<Orderstatusupdate>().GetListAsync();
                var statusId = status != null && status.Count > 0 ? status.Last().OrderStatusUpdateId + 1 : 1;
                var orderStatus = new Orderstatusupdate
                {
                    OrderStatusUpdateId = statusId,
                    OrderStatus = OrderConstant.PENDING.GetType().ToString(),
                    OrderId = orderId,
                    UpdatedAt = DateTime.Now,
                    //AccountId = account.AccountId
                    AccountId = userId,
                };
                await _uow.GetRepository<Orderstatusupdate>().InsertAsync(orderStatus);
                if (await _uow.CommitAsync() <= 0)
                {
                    return new MethodResult<OrderResponse>.Failure("Create order not success!", StatusCodes.Status400BadRequest);
                }
                var setOrder = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == order.OrderId, include: o => o.Include(od => od.Orderstatusupdates).Include(od => od.Staff));
                var odResp = _mapper.Map<OrderResponse>(setOrder);
                if (!string.IsNullOrEmpty(orderRequest.VoucherCode))
                {
                    var voucher = await _uow.GetRepository<Voucher>().SingleOrDefaultAsync(predicate: v => v.VoucherCode.ToLower().Equals(orderRequest.VoucherCode.ToLower()));
                    if (voucher == null)
                    {
                        return new MethodResult<OrderResponse>.Failure("Voucher not found!", StatusCodes.Status400BadRequest);
                    }
                    if (voucher.ExpirationDate < DateTime.Now)
                    {
                        return new MethodResult<OrderResponse>.Failure("Voucher not valid!", StatusCodes.Status400BadRequest);
                    }
                    if (voucher.MinimumOrderAmount > totalAmount)
                    {
                        return new MethodResult<OrderResponse>.Failure("Not eligible to use voucher!", StatusCodes.Status400BadRequest);
                    }

                    var voucherUsage = new Voucherusage
                    {
                        VoucherId = voucher.VoucherId,
                        OrderId = orderId,
                        AmountUsed = totalAmount,
                        UsedAt = DateTime.Now
                    };
                    await _uow.GetRepository<Voucherusage>().InsertAsync(voucherUsage);
                    if (!(await _uow.CommitAsync() > 0))
                    {
                        return new MethodResult<OrderResponse>.Failure("Create order not success!", StatusCodes.Status400BadRequest);
                    }
                    odResp.Voucher = voucher;
                }

                var transaction = new Domain.Models.Transaction
                {
                    Amount = totalAmount,
                    TransactionType = TransactionTypeConstant.PAY,
                    OrderId = orderId,
                    StaffId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = false
                };
                await _uow.GetRepository<Domain.Models.Transaction>().InsertAsync(transaction);
                if (await _uow.CommitAsync() <= 0)
                {
                    return new MethodResult<OrderResponse>.Failure("Create order not success!", StatusCodes.Status400BadRequest);
                }


                return new MethodResult<OrderResponse>.Success(odResp);
            }

            return new MethodResult<OrderResponse>.Failure("Create order not success!", StatusCodes.Status400BadRequest);
        }
        public async Task<MethodResult<Order>> CancelOrder(int orderId, int constant)
        {
            var orderStatuses = await _uow.GetRepository<Orderstatusupdate>().GetListAsync(predicate: s => s.OrderId == orderId, include: s => s.Include(os => os.Order), orderBy: o => o.OrderByDescending(os => os.UpdatedAt));
            var orderStatus = orderStatuses.FirstOrDefault();
            if (orderStatus == null)
            {
                return new MethodResult<Order>.Failure("Order not found!", StatusCodes.Status400BadRequest);
            }
            if (constant < 1 || constant > 4)
            {
                return new MethodResult<Order>.Failure("Not found status!", StatusCodes.Status400BadRequest);
            }
            if (orderStatus.OrderStatus == "SUCCESS")
            {
                return new MethodResult<Order>.Failure("Order success can not be update status!", StatusCodes.Status400BadRequest);
            }
            if (orderStatus.OrderStatus == "CANCELLED")
            {
                return new MethodResult<Order>.Failure("Order canceled can not be update status!", StatusCodes.Status400BadRequest);
            }

            var newStt = constant == 1 ? "PENDING" : constant == 2 ? "PREPARING" : constant == 3 ? "SUCCESS" : "CANCELLED";
            //var existedStt = await _uow.GetRepository<Orderstatusupdate>().SingleOrDefaultAsync(predicate: s => s.OrderId == orderId && s.OrderStatus == newStt);
            var isExisted = newStt == orderStatus.OrderStatus;
            var oldStt = orderStatus.OrderStatus == "PENDING" ? 1 : orderStatus.OrderStatus == "PREPARING" ? 2 : orderStatus.OrderStatus == "SUCCESS" ? 3 : 4;
            if (constant < oldStt)
            {
                return new MethodResult<Order>.Failure("Order status can not be update from '" + orderStatus.OrderStatus + "' to '" + newStt + "'!", StatusCodes.Status400BadRequest);
            }

            if (!isExisted)
            {
                var status = await _uow.GetRepository<Orderstatusupdate>().GetListAsync();
                var statusId = status != null && status.Count > 0 ? status.Last().OrderStatusUpdateId + 1 : 1;
                var newStatus = new Orderstatusupdate
                {
                    OrderStatusUpdateId = statusId,
                    OrderStatus = constant.ToString(),
                    OrderId = orderId,
                    UpdatedAt = DateTime.Now,
                    //AccountId = account.AccountId
                    AccountId = orderStatus.AccountId
                };

                await _uow.GetRepository<Orderstatusupdate>().InsertAsync(newStatus);
                if (await _uow.CommitAsync() > 0)
                {
                    var setOrder = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderId, include: o => o.Include(od => od.Orderstatusupdates).Include(od => od.Staff));
                    return new MethodResult<Order>.Success(setOrder);
                }
            }
            var setOrderNotChange = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderId, include: o => o.Include(od => od.Orderstatusupdates).Include(od => od.Staff));
            return new MethodResult<Order>.Success(setOrderNotChange);
        }

        //public async Task<MethodResult<Order>> ConfirmOrder(int orderId)
        //{
        //    var orderStatuses = await _uow.GetRepository<Orderstatusupdate>().GetListAsync(predicate: s => s.OrderId == orderId, include: s => s.Include(os => os.Order), orderBy: o => o.OrderByDescending(os => os.UpdatedAt));
        //    var orderStatus = orderStatuses.FirstOrDefault();
        //    if (orderStatus == null)
        //    {
        //        return new MethodResult<Order>.Failure("Order not found!", StatusCodes.Status400BadRequest);
        //    }
        //    if (orderStatus.OrderStatus == "Success")
        //    {
        //        return new MethodResult<Order>.Failure("Order success can not be paid!", StatusCodes.Status400BadRequest);
        //    }
        //    if (orderStatus.OrderStatus == "Canceled")
        //    {
        //        return new MethodResult<Order>.Failure("Order canceled can not be paid!", StatusCodes.Status400BadRequest);
        //    }

        //    var status = await _uow.GetRepository<Orderstatusupdate>().GetListAsync();
        //    var statusId = status != null && status.Count > 0 ? status.Last().OrderStatusUpdateId + 1 : 1;
        //    var newStatus = new Orderstatusupdate
        //    {
        //        OrderStatusUpdateId = statusId,
        //        OrderStatus = OrderConstant.SUCCESS.ToString(),
        //        OrderId = orderId,
        //        UpdatedAt = DateTime.Now,
        //        //AccountId = account.AccountId
        //        AccountId = orderStatuses.First().AccountId
        //    };

        //    await _uow.GetRepository<Orderstatusupdate>().InsertAsync(newStatus);
        //    if (await _uow.CommitAsync() > 0)
        //    {
        //        var setOrder = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderId, include: o => o.Include(od => od.Orderstatusupdates).Include(od => od.Staff));
        //        return new MethodResult<Order>.Success(setOrder);
        //    }
        //    return new MethodResult<Order>.Failure("Order cannot be paid!", StatusCodes.Status400BadRequest);

        //}

        //public async Task<Account?> GetCurrentUser()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    if (httpContext != null && httpContext.Request.Headers.ContainsKey("Authorization"))
        //    {
        //        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        //        if (token == "null")
        //        {
        //            return null;
        //        }
        //        if (!string.IsNullOrEmpty(token))
        //        {
        //            var handler = new JwtSecurityTokenHandler();
        //            var jwtToken = handler.ReadJwtToken(token);
        //            var idClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sid");
        //            if (idClaim != null)
        //            {
        //                var account = await _uow.GetRepository<Account>().SingleOrDefaultAsync(predicate: a => a.AccountId.ToString().Equals(idClaim.Value));
        //                if (account != null)
        //                {
        //                    return account;
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}
    }
}
