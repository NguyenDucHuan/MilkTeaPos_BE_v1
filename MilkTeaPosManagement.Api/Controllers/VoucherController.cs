using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Models.CategoryModels;
using MilkTeaPosManagement.Api.Models.VoucherMethod;
using MilkTeaPosManagement.Api.Services.Implements;
using MilkTeaPosManagement.Api.Services.Interfaces;

namespace MilkTeaPosManagement.Api.Controllers
{
    [Route("api/vouchers")]
    [ApiController]
    public class VoucherController(IVoucherService service) : ControllerBase
    {
        private readonly IVoucherService _service = service;
        [HttpGet("")]
        public async Task<IActionResult> GetVouchers([FromQuery] VoucherSearchModel? filter)
        {            
            var vouchers = await _service.GetVouchersByFilterAsync(filter);
            return Ok(vouchers);
        }
        [HttpGet("/{id}")]
        public async Task<IActionResult> GetVoucherById([FromRoute] int id)
        {
            var result = await _service.GetVoucherByIdAsync(id);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromForm] VoucherCreateRequestModel request)
        {
            var result = await _service.CreateVoucherAsync(request);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }

        [HttpPut]
        public async Task<IActionResult> UpdateVoucher(int id, [FromForm] VoucherUpdateRequestModel request)
        {
            var result = await _service.UpdateVoucherAsync(id, request);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }

        [HttpDelete]
        public async Task<IActionResult> UpdateStatusVoucher(int id)
        {
            var result = await _service.UpdateStatus(id);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                _ => Ok(new { message = "Voucher Update Status successfully" })
            );
        }

    }
}
