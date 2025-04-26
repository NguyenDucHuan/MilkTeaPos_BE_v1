using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Api.Routes;
using MilkTeaPosManagement.Api.Services.Interfaces;

namespace MilkTeaPosManagement.Api.Controllers
{
    [Route("api/payment-methods")]
    [ApiController]
    public class PaymentMethodController(IPaymentmethodService service) : ControllerBase
    {
        private readonly IPaymentmethodService _service = service;
        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllPaymentmethodAsync();
            return Ok(result);
        }
        [Authorize]
        [HttpGet("get-by-id/{paymentMethodId}")]
        public async Task<IActionResult> GetById([FromRoute]int paymentMethodId)
        {
            var result = await _service.GetPaymentmethodByIdAsync(paymentMethodId);
            return Ok(result);
        }
        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] PaymentMethodRequest request)
        {
            var result = await _service.AddAPaymentmethodAsync(request);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPut("")]
        public async Task<IActionResult> Update([FromBody] PaymentMethodRequest request)
        {
            var result = await _service.UpdateAPaymentmethodAsync(request);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpDelete("")]
        public async Task<IActionResult> Delete([FromBody] int paymentMethodId)
        {
            var result = await _service.DeleteAPaymentmethodAsync(paymentMethodId);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
    }
}
