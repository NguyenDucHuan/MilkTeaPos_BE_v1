using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Models.OrderItemModels;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Controllers
{
    [Route("api/order-item")]
    [ApiController]
    [Authorize]
    public class OrderItemController(IOrderItemService service) : ControllerBase
    {
        private readonly IOrderItemService _service = service;
        [HttpGet("get-cart")]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var result = await _service.GetCartAsync();
                var cartResponse = new List<object>();
                foreach (var item in result.Item1)
                {
                    var toppings = await _service.GetToppingsInCart(item.OrderItemId);
                    var toppingOfProduct = new List<object>();
                    
                    var comboItems = await _service.GetComboItemsInCart(item.OrderItemId);
                    var itemsOfProduct = new List<object>();
                    decimal? toppingPrice = 0;
                    foreach (var topping in toppings)
                    {
                        toppingOfProduct.Add(new
                        {
                            toppingId = topping.ProductId,
                            toppingName = topping.Product.ProductName,
                            toppingPrize = topping.Product.Prize,
                        });
                        toppingPrice += topping.Price;
                    }
                    foreach (var comboItem in comboItems)
                    {
                        itemsOfProduct.Add(new
                        {
                            comboItenId = comboItem.ProductId,
                            comboItemName = comboItem.Product.ProductName
                        });
                    }
                    cartResponse.Add(new
                    {
                        orderItemId = item.OrderItemId,
                        productId = item.ProductId,
                        productName = item.Product.ProductName,
                        sizeId = item.Product.SizeId,
                        prize = item.Product.Prize,
                        quantity = item.Quantity,
                        subPrice = item.Price + toppingPrice,
                        toppings = toppingOfProduct,
                        comboItems = itemsOfProduct
                    });
                }
                return Ok(new
                {
                    cart = cartResponse,
                    offers = result.Item2
                });
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("get-by-order-id/{orderId}")]
        public async Task<IActionResult> GetByOrderId([FromRoute] int orderId)
        {
            var result = await _service.GetOrderitemsByOrderIdAsync(orderId);
            var cartResponse = new List<object>();
            foreach (var item in result)
            {
                var toppings = await _service.GetToppingsInOrder(orderId, item.OrderItemId);
                var toppingOfProduct = new List<object>();
                decimal? toppingPrice = 0;
                foreach (var topping in toppings)
                {
                    toppingOfProduct.Add(new
                    {
                        toppingId = topping.ProductId,
                        toppingName = topping.Product.ProductName,
                        toppingPrize = topping.Product.Prize,
                    });
                    toppingPrice += topping.Product.Prize;
                }
                cartResponse.Add(new
                {
                    orderItemId = item.OrderItemId,
                    productId = item.ProductId,
                    productName = item.Product.ProductName,
                    sizeId = item.Product.SizeId,
                    prize = item.Product.Prize,
                    quantity = item.Quantity,
                    subPrice = item.Product.Prize + toppingPrice,
                    toppings = toppingOfProduct
                });
            }
            return Ok(cartResponse);
        }
        [HttpPost("apply-combo")]
        public async Task<IActionResult> ChangeProductsToCombo([FromBody] int comboId)
        {
            var result = await _service.ChangeProductsToCombo(comboId);
            return result.Match(
                 (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                 Ok
             );
        }
        [HttpGet("get-by-id/{orderItemId}")]
        public async Task<IActionResult> GetById([FromRoute]int orderItemId)
        {
            var result = await _service.GetAnOrderItemByIdAsync(orderItemId);
            return Ok(result);
        }
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> Add([FromBody] OrderItemRequest request)
        {
            var result = await _service.AddToCart(request);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        [HttpDelete("remove-from-cart")]
        public async Task<IActionResult> Delete(int orderItemId, int quantity)
        {
            var result = await _service.RemoveFromCart(orderItemId, quantity);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        [HttpPost("add-quantity")]
        public async Task<IActionResult> Add(int orderItemId, int quantity)
        {
            var result = await _service.AddQuantity(orderItemId, quantity);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        [HttpPut("edit-order-item/{orderItemId}")]
        public async Task<IActionResult> Update(int orderItemId, OrderItemRequest request)
        {
            var result = await _service.UpdateOrderItem(orderItemId, request);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
    }
}
