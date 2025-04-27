

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MilkTeaPosManagement.Api.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MilkTeaPosManagement.Api.Models.Configurations;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Models.AuthenticationModels
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly AuthenticationConfiguration _configuration;
        public TokenGenerator(IOptions<AuthenticationConfiguration> options)
        {
            _configuration = options.Value;
        }
        private async Task<string> GenerateToken(string secretKey, string issuer, string audience, DateTime utcExpirationTime,
            IEnumerable<Claim>? claims)
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                DateTime.Now,
                utcExpirationTime,
                credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AccessToken> GenerateAccessToken(Account user)
        {
            List<Claim> claims =
            [
                new Claim(ClaimTypes.Sid, user.AccountId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            ];
            DateTime expirationTime = DateTime.Now.AddMinutes(_configuration.AccessTokenExpiration);
            return new AccessToken
            {
                Token = await GenerateToken(
                   _configuration.AccessTokenSecret,
                   _configuration.Issuer,
                   _configuration.Audience,
                   expirationTime,
                   claims),
                ExpirationTime = expirationTime
            };
        }

        public async Task<RefreshToken> GenerateRefreshToken()
        {
            DateTime expirationTime = DateTime.Now.AddMinutes(_configuration.RefreshTokenExpiration);
            return new RefreshToken
            {
                Token = await GenerateToken(
                    _configuration.RefreshTokenSecret,
                    _configuration.Issuer,
                    _configuration.Audience,
                    expirationTime, null),
                ExpirationTime = expirationTime
            };
        }

        public async Task<string> GenerateEmailVerificationToken(string email)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email)
            };
            DateTime expirationTime = DateTime.Now.AddMinutes(_configuration.EmailVerificationExpiration);
            return await GenerateToken(
                    _configuration.EmailVerificationSecret,
                    _configuration.Issuer,
                    _configuration.Audience,
                    expirationTime,
                    claims);
        }
    }
}
