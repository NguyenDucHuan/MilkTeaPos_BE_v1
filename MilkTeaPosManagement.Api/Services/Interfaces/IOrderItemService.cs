﻿using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.OrderItemModels;
using MilkTeaPosManagement.Api.Models.ProductModel;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<ICollection<Orderitem>> GetCartAsync();
        Task<MethodResult<Orderitem>> ChangeProductsToCombo(int comboId);
        Task<ICollection<Orderitem>> GetOrderitemsByOrderIdAsync(int orderId);
        Task<Orderitem> GetAnOrderItemByIdAsync(int id);
        Task<MethodResult<Orderitem>> AddToCart(OrderItemRequest request);
        Task<MethodResult<Orderitem>> RemoveFromCart(int orderItemId, int quantity);
        Task<MethodResult<Orderitem>> AddQuantity(int orderItemId, int quantity);
        Task<ICollection<Orderitem>> GetToppingsInCart(int masterId);
        Task<ICollection<Orderitem>> GetToppingsInOrder(int orderId, int masterId);
        Task<MethodResult<Orderitem>> UpdateOrderItem(int id, OrderItemRequest request);
        Task<ICollection<Orderitem>> GetComboItemsInCart(int masterId);
        Task<ProductResponse> GetProductByIdAsync(int id);
    }
}
