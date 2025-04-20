using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Api.Models.ViewModels;
using MilkTeaPosManagement.Api.Models.AccountModel;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account> GetUserByPhoneOrEmailAsync(string PhoneOrEmail);
        Task<MethodResult<string>> UpdateAvatarAsync(string email, IFormFile avatarFile);
        Task<MethodResult<ProfileViewModel>> GetProfileAsync(string email);
        Task<MethodResult<bool>> ChangePasswordAsync(string email, ChangePasswordRequest request);
    }
}
