﻿
using AutoMapper;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.AuthenticationModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.Api.ViewModels;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Api.Constants;


namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AuthenticationService(IUnitOfWork uow, ITokenGenerator tokenGenerator,
            IUserService userService, IMapper mapper)
        {
            _uow = uow;
            _tokenGenerator = tokenGenerator;
            _userService = userService;
            _mapper = mapper;
        }


        public async Task<MethodResult<SignInViewModel>> SigninAsync(LoginRequest request)
        {
            var user = await _userService.GetUserByUserNameOrEmailAsync(request.Email);
            if (user == null)
            {
                return new MethodResult<SignInViewModel>.Failure("Invalid email", StatusCodes.Status400BadRequest);
            }

            //var correctedPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            var correctedPassword = string.Equals(request.Password, user.PasswordHash, StringComparison.OrdinalIgnoreCase);
            if (!correctedPassword)
            {
                return new MethodResult<SignInViewModel>.Failure("Password is not correct", 400);
            }

            if (user.Status == UserConstant.USER_STATUS_INACTIVE)
            {
                return new MethodResult<SignInViewModel>.Failure("Your account has been inactivated", 400);
            }

            var result = new SignInViewModel
            {
                AccessToken = await _tokenGenerator.GenerateAccessToken(user),
                RefreshToken = await _tokenGenerator.GenerateRefreshToken()
            };

            return new MethodResult<SignInViewModel>.Success(result);
        }

    }
}
