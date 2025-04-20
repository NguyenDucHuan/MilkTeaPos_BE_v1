using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Api.Models.ViewModels;
using MilkTeaPosManagement.Api.Models.AccountModel;
using MilkTeaPosManagement.Domain.Paginate;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account> GetUserByPhoneOrEmailAsync(string PhoneOrEmail);
        Task<MethodResult<string>> UpdateAvatarAsync(string email, IFormFile avatarFile);
        Task<MethodResult<ProfileViewModel>> GetProfileAsync(string email);
        Task<MethodResult<bool>> ChangePasswordAsync(string email, ChangePasswordRequest request);
        Task<IPaginate<AccountViewModel>> GetAccountsByFilterAsync(AccountFilterModel filter);
        Task<MethodResult<AccountViewModel>> GetAccountByIdAsync(int id);
        Task<MethodResult<AccountViewModel>> CreateAccountAsync(CreateUserRequest request, IFormFile avatarFile);
        Task<MethodResult<AccountViewModel>> UpdateAccountAsync(int id, UpdateUserRequest request, IFormFile? avatarFile);
        Task<MethodResult<AccountViewModel>> UpdateAccountStatusAsync(int id);
    }
}
