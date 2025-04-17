using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.ViewModels;
using MilkTeaPosManagement.Api.Models.AuthenticationModels;


namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<MethodResult<SignInViewModel>> SigninAsync(LoginRequest request);
    }
}
