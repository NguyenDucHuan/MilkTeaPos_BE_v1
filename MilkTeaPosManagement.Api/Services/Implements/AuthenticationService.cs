
using AutoMapper;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.AuthenticationModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Models.ViewModels;


namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AuthenticationService(IUnitOfWork uow, ITokenGenerator tokenGenerator,
            IAccountService accountService, IMapper mapper)
        {
            _uow = uow;
            _tokenGenerator = tokenGenerator;
            _accountService = accountService;
            _mapper = mapper;
        }


        public async Task<MethodResult<SignInViewModel>> SigninAsync(LoginRequest request)
        {
            var user = await _accountService.GetUserByPhoneOrEmailAsync(request.PhoneOrEmail);
            if (user == null)
            {
                return new MethodResult<SignInViewModel>.Failure("Invalid email", StatusCodes.Status400BadRequest);
            }

            var passwordVerified = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordVerified)
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
