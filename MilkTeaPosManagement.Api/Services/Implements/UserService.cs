
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.DAL.UnitOfWorks;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;

        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Account> GetUserByUserNameOrEmailAsync(string UserNameOrEmail)
        {
            return await _uow.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: p => p.Email == UserNameOrEmail || p.Username == UserNameOrEmail
            );
        }
    }
}
