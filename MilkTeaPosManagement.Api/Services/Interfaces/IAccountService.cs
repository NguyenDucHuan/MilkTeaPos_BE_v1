using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Api.Models.ViewModels;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account> GetUserByUserNameOrEmailAsync(string UserNameOrEmail);
        Task<MethodResult<ProfileViewModel>> GetProfileAsync(string email);
    }
}
