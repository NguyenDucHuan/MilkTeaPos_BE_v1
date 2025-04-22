using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.OrderModels;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Domain.Paginate;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IPaginate<Order>> GetAllOrders(OrderSearchModel? searchModel);
        Task<MethodResult<Order>> GetOrderDetail(int orderId);
        Task<MethodResult<Order>> CreateOrder(OrderRequest orderRequest);
        Task<MethodResult<Order>> CancelOrder(int orderId);
        Task<MethodResult<Order>> ConfirmOrder(int orderId);
    }
}
