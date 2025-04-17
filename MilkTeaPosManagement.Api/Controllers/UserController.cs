using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Services.Interfaces;
using System.Security.Claims;

using MilkTeaPosManagement.Api.Routes;
using Microsoft.AspNetCore.Authorization;
using MilkTeaPosManagement.Api.Constants;


namespace MilkTeaPosManagement.Api.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IAccountService _userService;

        public UserController(IAccountService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        [Route(Router.UserRoute.Profile)]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null) return Unauthorized();

            var result = await _userService.GetProfileAsync(email);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        //[HttpPatch]
        //[Route(Router.UserRoute.UpdateProfile)]
        //[Authorize]
        //public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        //{
        //    var email = User.FindFirst(ClaimTypes.Email)?.Value;
        //    if (email == null) return Unauthorized();

        //    var result = await _userService.UpdateProfileAsync(email, request);
        //    return result.Match(
        //        (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
        //        Ok
        //    );
        //}

    }
}
