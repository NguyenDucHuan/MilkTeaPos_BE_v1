using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.OrderItemModels;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<(ICollection<Orderitem>, List<Product>?)> GetCartAsync();
        Task<MethodResult<Orderitem>> ChangeProductsToCombo(int comboId);
        Task<ICollection<Orderitem>> GetOrderitemsByOrderIdAsync(int orderId);
        Task<Orderitem> GetAnOrderItemByIdAsync(int id);
        Task<MethodResult<Orderitem>> AddToCart(OrderItemRequest request);
        Task<MethodResult<Orderitem>> RemoveFromCart(int orderItemId, int quantity);
        Task<MethodResult<object>> ClearCart();
    }
}
