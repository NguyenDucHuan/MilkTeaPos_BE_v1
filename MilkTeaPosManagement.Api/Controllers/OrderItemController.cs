using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Models.OrderItemModels;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Api.Services.Interfaces;

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
            var result = await _service.GetCartAsync();
            return Ok(result);
        }
        [HttpGet("get-by-order-id")]
        public async Task<IActionResult> GetByOrderId([FromBody] int orderId)
        {
            var result = await _service.GetOrderitemsByOrderIdAsync(orderId);
            return Ok(result);
        }
        [HttpPost("apply-combo")]
        public async Task<IActionResult> ChangeProductsToCombo([FromBody] int comboId)
        {
            var result = await _service.ChangeProductsToCombo(comboId);
            return Ok(result);
        }
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById([FromBody] int orderItemId)
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
        public async Task<IActionResult> Delete([FromBody] int productId)
        {
            var result = await _service.RemoveFromCart(productId);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
    }
}
