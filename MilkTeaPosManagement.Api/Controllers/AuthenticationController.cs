﻿using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Routes;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.Api.Models.AuthenticationModels;

namespace MilkTeaPosManagement.Api.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [Route(Router.AtuthenticationRoute.Login)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authenticationService.SigninAsync(request);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
    }
}
