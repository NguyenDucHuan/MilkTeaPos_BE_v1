using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<Account> GetUserByUserNameOrEmailAsync(string UserNameOrEmail);
    }
}
