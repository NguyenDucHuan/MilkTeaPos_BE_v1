using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Models.OrderModels;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Api.Services.Interfaces;

namespace MilkTeaPosManagement.Api.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController(IOrderService service) : ControllerBase
    {
        private readonly IOrderService _service = service;
        [HttpGet("")]
        public async Task<IActionResult> GetAll(OrderSearchModel? searchModel)
        {
            var result = await _service.GetAllOrders(searchModel);
            return Ok(result);
        }
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> Get(int orderId)
        {
            var result = await _service.GetOrderDetail(orderId);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] OrderRequest request)
        {
            var result = await _service.CreateOrder(request);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] int orderId)
        {
            var result = await _service.CancelOrder(orderId);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        [HttpPut]
        public async Task<IActionResult> update([FromBody] int orderId)
        {
            var result = await _service.ConfirmOrder(orderId);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
    }
}
