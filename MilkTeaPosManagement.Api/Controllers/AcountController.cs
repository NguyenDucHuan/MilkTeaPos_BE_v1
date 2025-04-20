using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Services.Interfaces;
using System.Security.Claims;

using MilkTeaPosManagement.Api.Routes;
using Microsoft.AspNetCore.Authorization;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Services.Implements;


namespace MilkTeaPosManagement.Api.Controllers
{
    public class AcountController : ControllerBase
    {
        private readonly IAccountService _accountService;


        public AcountController(IAccountService userService)
        {
            _accountService = userService;
        }
        [HttpGet]
        [Route(Router.UserRoute.Profile)]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null) return Unauthorized();

            var result = await _accountService.GetProfileAsync(email);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        //[Authorize]
        //[HttpPut("profile")]
        //public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        //{
        //    // Get current user's email from claims
        //    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        //    if (string.IsNullOrEmpty(userEmail))
        //    {
        //        return Unauthorized();
        //    }

        //    var result = await _accountService.UpdateProfileAsync(userEmail, request);

        //    if (!result.IsSuccess)
        //    {
        //        return StatusCode(result.StatusCode, new { message = result.Message });
        //    }

        //    return Ok(result.Data);
        //}

    }
}
