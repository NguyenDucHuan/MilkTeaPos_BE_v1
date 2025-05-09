using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Models.CashBalanceModel;
using MilkTeaPosManagement.Api.Services.Interfaces;
using System.Security.Claims;

namespace MilkTeaPosManagement.Api.Controllers
{
    [Route("api/cash-balance")]
    [ApiController]
    public class CashBalanceController : Controller
    {
        private readonly ICashBalanceService _cashBalanceService;
        public CashBalanceController(ICashBalanceService cashBalanceService)
        {
            _cashBalanceService = cashBalanceService;
        }


        [HttpGet("get-cash-balance")]
        [Authorize]
        public async Task<IActionResult> GetCashBalance([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _cashBalanceService.GetCashBalanceAsync(startDate, endDate);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        //[HttpGet("get-daily-cash-flow")]
        //public async Task<IActionResult> GetDailyCashFlow([FromQuery] DateTime date)
        //{
        //    var result = await _cashBalanceService.GetTransactionAsyncUseForCashBalance(date);
        //    return result.Match(
        //        (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
        //        Ok
        //    );
        //}
        //[HttpGet("get-cash-flow-history")]
        //public async Task<IActionResult> GetCashFlowHistory([FromQuery] int? userId, [FromQuery] string? flowType, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        //{
        //    var result = await _cashBalanceService.GetCashflowHistoryAsync(userId, flowType, startDate, endDate);
        //    return result.Match(
        //        (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
        //        Ok
        //    );
        //}
        [HttpPost("update-cash-balance")]
        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        public async Task<IActionResult> UpdateCashBalance([FromForm] UpdateCashBalanceRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var result = await _cashBalanceService.UpdateCashBalanceAsync(request.Amount, userId, request.Type, request.Description);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }

    }
}
