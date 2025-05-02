using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.OrderModels;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Domain.Paginate;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IOrderService
    {
        Task<(long, IPaginate<Order>?, string?)> GetAllOrders(OrderSearchModel? searchModel);
        Task<(long, Order?, string?)> GetOrderDetail(int orderId);
        Task<MethodResult<Order>> CreateOrder(OrderRequest orderRequest);
        Task<MethodResult<Order>> CancelOrder(int orderId);
        Task<MethodResult<Order>> ConfirmOrder(int orderId);
    }
}
