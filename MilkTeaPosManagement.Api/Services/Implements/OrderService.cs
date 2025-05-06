using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.OrderModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Domain.Paginate;
using System.IdentityModel.Tokens.Jwt;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class OrderService(IUnitOfWork uow, IHttpContextAccessor _httpContextAccessor) : IOrderService
    {
        private readonly IUnitOfWork _uow = uow;
        public async Task<(long, IPaginate<Order>?, string?)> GetAllOrders(OrderSearchModel? search)
        {
            if (search == null)
            {
                return (1, await _uow.GetRepository<Order>().GetPagingListAsync(page: 1, size: 10, orderBy: o => o.OrderByDescending(od => od.CreateAt)), null);
            }
            //if (search.Status != null && search.Status != OrderConstant.PENDING && search.Status != OrderConstant.SHIPPED && search.Status != OrderConstant.DELIVERED && search.Status != OrderConstant.CANCELED)
            //{
            //    return (0, null, "Status must be in [PENDING, SHIPPED, DELIVERED, CANCELED]");
            //}
            var staff = await _uow.GetRepository<Account>().SingleOrDefaultAsync(predicate: acc => acc.AccountId == search.StaffId);
            if (search.StaffId != null && staff == null)
            {
                return (0, null, "Staff not found");
            }
            var payment = await _uow.GetRepository<Paymentmethod>().SingleOrDefaultAsync(predicate: pm => pm.PaymentMethodId == search.PaymentMethodId);
            if (search.PaymentMethodId != null && payment == null)
            {
                return (0, null, "Payment method not found");
            }
            return (1, await _uow.GetRepository<Order>().GetPagingListAsync(include: o => o.Include(od => od.Orderstatusupdates).Include(od => od.Staff).Include(od => od.PaymentMethod).Include(od => od.Orderitems),
                                                                        predicate: o => (!search.StaffId.HasValue || o.StaffId == search.StaffId) &&
                                                                                        (!search.PaymentMethodId.HasValue || o.PaymentMethodId == search.PaymentMethodId) &&
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
                                                                                                                                                                                                                                            : search.SortBy.ToLower().Equals("paymentmethodid") ? o.OrderBy(od => od.PaymentMethodId)
                                                                                                                                                                                                                                                                                                : o.OrderBy(od => od.Orderstatusupdates.FirstOrDefault().OrderStatus))
                                                                                                                                                     : ((string.IsNullOrEmpty(search.SortBy) || search.SortBy.ToLower().Equals("createat")) ? o.OrderByDescending(od => od.CreateAt)
                                                                                                                                                                                                                                            : search.SortBy.ToLower().Equals("orderid") ? o.OrderByDescending(od => od.OrderId)
                                                                                                                                                                                                                                            : search.SortBy.ToLower().Equals("totalamount") ? o.OrderByDescending(od => od.TotalAmount)
                                                                                                                                                                                                                                            : search.SortBy.ToLower().Equals("staffid") ? o.OrderByDescending(od => od.StaffId)
                                                                                                                                                                                                                                            : search.SortBy.ToLower().Equals("paymentmethodid") ? o.OrderByDescending(od => od.PaymentMethodId)
                                                                                                                                                                                                                                                                                                : o.OrderByDescending(od => od.Orderstatusupdates.FirstOrDefault().OrderStatus)))
                                                                        ), null); 
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
            var order = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderId, include: o => o.Include(od => od.PaymentMethod).Include(od => od.Orderstatusupdates).Include(od => od.Staff).Include(od => od.Orderitems).ThenInclude(oi => oi.Product));
            if (order == null)
            {
                return (400, null, "Order not found!");
            }
            return (200, order, null);
        }
        public async Task<MethodResult<Order>> CreateOrder(OrderRequest orderRequest)
        {
            var orderItems = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.OrderId == null);
            decimal? totalAmount = 0;
            if (orderItems == null || orderItems?.Count == 0)
            {
                return new MethodResult<Order>.Failure("Order not have any product!", StatusCodes.Status400BadRequest);
            }
            var account = await _uow.GetRepository<Account>().SingleOrDefaultAsync(predicate: a => a.AccountId == orderRequest.StaffId);
            
            //var account = await GetCurrentUser();
            //if (account == null)
            //{
            //    return new MethodResult<Order>.Failure("Login required!", StatusCodes.Status400BadRequest);
            //}
            var paymrentmethod = await _uow.GetRepository<Paymentmethod>().SingleOrDefaultAsync(predicate: PM => PM.PaymentMethodId == orderRequest.PaymentMethodId);
            if (paymrentmethod == null)
            {
                return new MethodResult<Order>.Failure("Paymentmethod not valid!", StatusCodes.Status400BadRequest);
            }
            if (account == null)
            {
                return new MethodResult<Order>.Failure("Login required!", StatusCodes.Status400BadRequest);
            }
            foreach (var item in orderItems)
            {
                totalAmount += item.Price;
            }
            var orders = await _uow.GetRepository<Order>().GetListAsync();
            var orderId = orders != null && orders.Count > 0 ? orders.Last().OrderId + 1 : 1;
            var order = new Order
            {
                OrderId = orderId,
                TotalAmount = totalAmount,
                CreateAt = DateTime.Now,
                Note = orderRequest.Note,
                StaffId = orderRequest.StaffId,
                //StaffId = account.AccountId,
                PaymentMethodId = orderRequest.PaymentMethodId
            };
            
            await _uow.GetRepository<Order>().InsertAsync(order);
            
            if (await _uow.CommitAsync() > 0)
            {
                foreach (var item in orderItems)
                {
                    item.OrderId = orderId;
                    _uow.GetRepository<Orderitem>().UpdateAsync(item);
                }
                if (await _uow.CommitAsync() <= 0)
                {
                    return new MethodResult<Order>.Failure("Create order not success!", StatusCodes.Status400BadRequest);
                }
                var status = await _uow.GetRepository<Orderstatusupdate>().GetListAsync();
                var statusId = status != null && status.Count > 0 ? status.Last().OrderStatusUpdateId + 1 : 1;
                var orderStatus = new Orderstatusupdate
                {
                    OrderStatusUpdateId = statusId,
                    OrderStatus = OrderConstant.PENDING.ToString(),
                    OrderId = orderId,
                    UpdatedAt = DateTime.Now,
                    //AccountId = account.AccountId
                    AccountId = orderRequest.StaffId,
                };
                await _uow.GetRepository<Orderstatusupdate>().InsertAsync(orderStatus);
                if (await _uow.CommitAsync() > 0)
                {
                    var setOrder = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == order.OrderId, include: o => o.Include(od => od.Orderstatusupdates).Include(od => od.Staff).Include(od => od.PaymentMethod));
                    return new MethodResult<Order>.Success(setOrder);
                }
                return new MethodResult<Order>.Failure("Create order not success!", StatusCodes.Status400BadRequest);
            }

            return new MethodResult<Order>.Failure("Create order not success!", StatusCodes.Status400BadRequest);
        }
        public async Task<MethodResult<Order>> CancelOrder(int orderId)
        {
            var orderStatuses = await _uow.GetRepository<Orderstatusupdate>().GetListAsync(predicate: s => s.OrderId == orderId, include: s => s.Include(os => os.Order));
            foreach (var orderStatus in orderStatuses)
            {
                if (orderStatus == null)
                {
                    return new MethodResult<Order>.Failure("Order not found!", StatusCodes.Status400BadRequest);
                }
                if (orderStatus.OrderStatus == "Success")
                {
                    return new MethodResult<Order>.Failure("Order success can not be canceled!", StatusCodes.Status400BadRequest);
                }
                if (orderStatus.OrderStatus == "Canceled")
                {
                    return new MethodResult<Order>.Failure("Order canceled can not be canceled!", StatusCodes.Status400BadRequest);
                }
            }
            
            var status = await _uow.GetRepository<Orderstatusupdate>().GetListAsync();
            var statusId = status != null && status.Count > 0 ? status.Last().OrderStatusUpdateId + 1 : 1;
            var newStatus = new Orderstatusupdate
            {
                OrderStatusUpdateId = statusId,
                OrderStatus = OrderConstant.CANCELED.ToString(),
                OrderId = orderId,
                UpdatedAt = DateTime.Now,
                //AccountId = account.AccountId
                AccountId = orderStatuses.First().AccountId
            };

            await _uow.GetRepository<Orderstatusupdate>().InsertAsync(newStatus);
            if (await _uow.CommitAsync() > 0)
            {
                var setOrder = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderId, include: o => o.Include(od => od.Orderstatusupdates).Include(od => od.Staff));
                return new MethodResult<Order>.Success(setOrder);
            }
            return new MethodResult<Order>.Failure("Order cannot be canceled!", StatusCodes.Status400BadRequest);
            
        }
        public async Task<MethodResult<Order>> ConfirmOrder(int orderId)
        {
            var orderStatuses = await _uow.GetRepository<Orderstatusupdate>().GetListAsync(predicate: s => s.OrderId == orderId, include: s => s.Include(os => os.Order));
            foreach (var orderStatus in orderStatuses)
            {
                if (orderStatus == null)
                {
                    return new MethodResult<Order>.Failure("Order not found!", StatusCodes.Status400BadRequest);
                }
                if (orderStatus.OrderStatus == "Success")
                {
                    return new MethodResult<Order>.Failure("Order success can not be paid!", StatusCodes.Status400BadRequest);
                }
                if (orderStatus.OrderStatus == "Canceled")
                {
                    return new MethodResult<Order>.Failure("Order canceled can not be paid!", StatusCodes.Status400BadRequest);
                }
            }

            var status = await _uow.GetRepository<Orderstatusupdate>().GetListAsync();
            var statusId = status != null && status.Count > 0 ? status.Last().OrderStatusUpdateId + 1 : 1;
            var newStatus = new Orderstatusupdate
            {
                OrderStatusUpdateId = statusId,
                OrderStatus = OrderConstant.SUCCESS.ToString(),
                OrderId = orderId,
                UpdatedAt = DateTime.Now,
                //AccountId = account.AccountId
                AccountId = orderStatuses.First().AccountId
            };

            await _uow.GetRepository<Orderstatusupdate>().InsertAsync(newStatus);
            if (await _uow.CommitAsync() > 0)
            {
                var setOrder = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderId, include: o => o.Include(od => od.Orderstatusupdates).Include(od => od.Staff));
                return new MethodResult<Order>.Success(setOrder);
            }
            return new MethodResult<Order>.Failure("Order cannot be paid!", StatusCodes.Status400BadRequest);

        }

        public async Task<Account?> GetCurrentUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (token == "null")
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    var idClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sid");
                    if (idClaim != null)
                    {
                        var account = await _uow.GetRepository<Account>().SingleOrDefaultAsync(predicate: a => a.AccountId.ToString().Equals(idClaim.Value));
                        if (account != null)
                        {
                            return account;
                        }
                    }
                }
            }
            return null;
        }
    }
}
