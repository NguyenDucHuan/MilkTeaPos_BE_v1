using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Models.OrderItemModels;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Api.Services.Interfaces;

namespace MilkTeaPosManagement.Api.Controllers
{
    [Route("api/order-item")]
    [ApiController]
    public class OrderItemController(IOrderItemService service) : ControllerBase
    {
        private readonly IOrderItemService _service = service;
        [HttpGet("get-cart")]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var result = await _service.GetCartAsync();
                return Ok(new
                {
                    cart = result.Item1,
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
            return Ok(result);
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
        [HttpDelete("clear-cart")]
        public async Task<IActionResult> Remove()
        {
            var result = await _service.ClearCart();
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
    }
}
