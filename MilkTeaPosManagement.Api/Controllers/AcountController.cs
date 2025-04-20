using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Services.Interfaces;
using System.Security.Claims;

using MilkTeaPosManagement.Api.Routes;
using Microsoft.AspNetCore.Authorization;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Services.Implements;
using MilkTeaPosManagement.Api.Models.AccountModel;


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
        [Authorize]
        [HttpPut]
        [Route(Router.UserRoute.UpdateAvatar)]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatarFile)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _accountService.UpdateAvatarAsync(userEmail, avatarFile);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        [Authorize]
        [HttpPut]
        [Route(Router.UserRoute.ChangePassword)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _accountService.ChangePasswordAsync(userEmail, request);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                _ => Ok(new { message = "Password changed successfully" })
            );
        }
    }
}
