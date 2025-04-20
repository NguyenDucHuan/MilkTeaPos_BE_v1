using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.OrderModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Domain.Paginate;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class OrderService(IUnitOfWork uow) : IOrderService
    {
        private readonly IUnitOfWork _uow = uow;
        public async Task<IPaginate<Order>> GetAllOrders(OrderSearchModel? search)
        {
            if (search == null || (search != null && search.PaymentMethodId == null && search.StaffId == null))
            {
                return await _uow.GetRepository<Order>().GetPagingListAsync(orderBy: o => o.OrderByDescending(od => od.CreateAt));
            }
            else if (search.StaffId == null && search.PaymentMethodId != null)
            {
                return await _uow.GetRepository<Order>().GetPagingListAsync(orderBy: o => o.OrderByDescending(od => od.CreateAt), predicate: o => o.PaymentMethodId == search.PaymentMethodId);
            }
            else if (search.StaffId != null && search.PaymentMethodId == null)
            {
                return await _uow.GetRepository<Order>().GetPagingListAsync(orderBy: o => o.OrderByDescending(od => od.CreateAt), predicate: o => o.StaffId == search.StaffId);
            }
            return await _uow.GetRepository<Order>().GetPagingListAsync(orderBy: o => o.OrderByDescending(od => od.CreateAt), predicate: o => o.StaffId == search.StaffId && o.PaymentMethodId == search.PaymentMethodId);
        }
        //public async Task<IPaginate<Order>> GetOrdersByStaffId(int staffId)
        //{
        //    return await _uow.GetRepository<Order>().GetPagingListAsync(orderBy: o => o.OrderByDescending(od => od.CreateAt), predicate: o => o.StaffId == staffId);
        //}
        //public async Task<IPaginate<Order>> GetOrdersByPaymentMethodId(int methodId)
        //{
        //    return await _uow.GetRepository<Order>().GetPagingListAsync(orderBy: o => o.OrderByDescending(od => od.CreateAt), predicate: o => o.PaymentMethodId == methodId);
        //}
        public async Task<MethodResult<Order>> GetOrderDetail(int orderId)
        {
            var order = await _uow.GetRepository<Order>().SingleOrDefaultAsync(
                predicate: o => o.OrderId == orderId, include: o => o.Include(od => od.PaymentMethod).Include(od => od.Orderstatusupdates).Include(od => od.Orderitems).ThenInclude(oi => oi.Product));
            if (order == null)
            {
                return new MethodResult<Order>.Failure("Order not found!", StatusCodes.Status400BadRequest);
            }
            return new MethodResult<Order>.Success(order);
        }
        public async Task<MethodResult<Order>> CreateOrder(OrderRequest orderRequest)
        {
            var orderItems = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.OrderId == null);
            decimal? totalAmount = 0;
            if (orderItems == null && orderItems.Count == 0)
            {
                return new MethodResult<Order>.Failure("Order not have any product!", StatusCodes.Status400BadRequest);
            }
            foreach (var item in orderItems)
            {
                totalAmount += item.Price;
            }
            var orders = await _uow.GetRepository<Order>().GetListAsync();
            var orderId = orders.Count > 0 ? orders.Last().OrderId + 1 : 1;
            var order = new Order
            {
                OrderId = orderId,
                TotalAmount = totalAmount,
                CreateAt = DateTime.Now,
                Note = orderRequest.Note,
                StaffId = orderRequest.StaffId,
                PaymentMethodId = orderRequest.PaymentMethodId
            };
            var status = await _uow.GetRepository<Orderstatusupdate>().GetListAsync();
            var statusId = status.Count > 0 ? status.Last().OrderStatusUpdateId + 1 : 1;
            var orderStatus = new Orderstatusupdate
            {
                OrderStatusUpdateId = statusId,
                OrderStatus = OrderConstant.PENDING.ToString(),
                OrderId = orderId,
                UpdatedAt = DateTime.Now,
                AccountId = orderRequest.StaffId
            };
            await _uow.GetRepository<Order>().InsertAsync(order);
            await _uow.GetRepository<Orderstatusupdate>().InsertAsync(orderStatus);
            if (await _uow.CommitAsync() > 0)
            {
                var setOrder = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == order.OrderId, include: o => o.Include(od => od.Orderstatusupdates));
                return new MethodResult<Order>.Success(setOrder);
            }
            return new MethodResult<Order>.Failure("Create order not success!", StatusCodes.Status400BadRequest);

        }
        public async Task<MethodResult<Order>> CancelOrder(int orderId)
        {
            var orderStatus = await _uow.GetRepository<Orderstatusupdate>().SingleOrDefaultAsync(predicate: s => s.OrderId == orderId, include: s => s.Include(os => os.Order));
            if (orderStatus == null)
            {
                return new MethodResult<Order>.Failure("Order not found!", StatusCodes.Status400BadRequest);
            }
            orderStatus.OrderStatus = OrderConstant.CANCELED.ToString();
            _uow.GetRepository<Orderstatusupdate>().UpdateAsync(orderStatus);
            if (await _uow.CommitAsync() > 0)
            {
                var setOrder = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderId, include: o => o.Include(od => od.Orderstatusupdates));
                return new MethodResult<Order>.Success(setOrder);
            }
            return new MethodResult<Order>.Failure("Order cannot be canceled!", StatusCodes.Status400BadRequest);

        }
    }
}
