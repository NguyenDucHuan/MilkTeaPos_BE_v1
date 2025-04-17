
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Api.Models.AuthenticationModels;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface ITokenGenerator
    {
        Task<AccessToken> GenerateAccessToken(Account user);
        Task<RefreshToken> GenerateRefreshToken();
        Task<string> GenerateEmailVerificationToken(string email);
    }
}
