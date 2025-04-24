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
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;


        public AccountController(IAccountService userService)
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

        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpGet]
        [Route(Router.UserRoute.GetAllUsers)]
        public async Task<IActionResult> GetAllUsers([FromQuery] AccountFilterModel filter = null)
        {
            if (filter == null)
            {
                filter = new AccountFilterModel();
            }

            var users = await _accountService.GetAccountsByFilterAsync(filter);
            return Ok(users);
        }

        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpGet]
        [Route(Router.UserRoute.GetUserById)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _accountService.GetAccountByIdAsync(id);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }

        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPost]
        [Route(Router.UserRoute.CreateUser)]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserRequest request, IFormFile avatarFile)
        {
            var result = await _accountService.CreateAccountAsync(request, avatarFile);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }

        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPut]
        [Route(Router.UserRoute.UpdateUser)]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromForm] UpdateUserRequest request, IFormFile avatarFile)
        {
            var result = await _accountService.UpdateAccountAsync(id, request, avatarFile);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPut]
        [Route(Router.UserRoute.UpdateUserStatus)]
        public async Task<IActionResult> UpdateUserStatus([FromRoute] int id)
        {
            var result = await _accountService.UpdateAccountStatusAsync(id);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
    }
}
