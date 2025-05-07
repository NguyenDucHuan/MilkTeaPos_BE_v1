using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Services.Interfaces;
using Net.payOS.Types;

namespace MilkTeaPosManagement.Api.Controllers
{
    [Route("api/payOS")]
    [ApiController]
    public class PayOSController(IPayOSService payOSService) : ControllerBase
    {
        private readonly IPayOSService _payOSService = payOSService;
        [AllowAnonymous]
        [HttpPost("")]
        public async Task<IActionResult> CreatePaymentLink(int orderId)
        {
            var result = await _payOSService.CreatePaymentLink(orderId);
            return result.Item1 switch
            {
                0 => BadRequest("ClientId not found"),
                1 => BadRequest("ApiKey not found"),
                2 => BadRequest("ChecksumKey not found"),
                3 => BadRequest("Order not found!!"),
                _ => Ok(result.Item2),
            };
        }
        [HttpGet("{orderCode}")]
        public async Task<ActionResult<IActionResult>> GetPaymentLinkInfomation([FromRoute] long orderCode)
        {
            var result = await _payOSService.GetPaymentLinkInformation(orderCode);
            return result.Item1 switch
            {
                0 => BadRequest("ClientId not found"),
                1 => BadRequest("ApiKey not found"),
                2 => BadRequest("ChecksumKey not found"),
                3 => BadRequest("Payment link not found!!"),
                _ => Ok(result.Item2),
            };
        }
        //[HttpPut("{orderCode}")]
        //public async Task<ActionResult<ResultModel>> CancelOrder([FromRoute] int orderCode)
        //{
        //    var result = await _payOSService.CancelOrder(orderCode);
        //    return Ok(result);
        //}
        [HttpPost("payos_transfer_handler")]
        public async Task<IActionResult> PayOSTransferHandler(WebhookType body)
        {
            var result = await _payOSService.VerifyPaymentWebhookData(body);
            return result.Item1 switch
            {
                0 => BadRequest("ClientId not found"),
                1 => BadRequest("ApiKey not found"),
                2 => BadRequest("ChecksumKey not found"),
                3 => Ok("Payment success"),
                _ => Ok("Payment fail")
            };
        }
    }
}
