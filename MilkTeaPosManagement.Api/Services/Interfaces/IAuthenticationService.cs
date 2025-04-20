using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.AuthenticationModels;
using MilkTeaPosManagement.Api.Models.ViewModels;


namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<MethodResult<SignInViewModel>> SigninAsync(LoginRequest request);
    }
}
