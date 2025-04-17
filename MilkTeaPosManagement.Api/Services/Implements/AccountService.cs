
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.ViewModels;
using AutoMapper;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;



        public AccountService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<MethodResult<ProfileViewModel>> GetProfileAsync(string email)
        {
            var user = await _uow.GetRepository<Account>().SingleOrDefaultAsync(
                     predicate: p => p.Email == email
                 );

            var result = _mapper.Map<ProfileViewModel>(user);
            return new MethodResult<ProfileViewModel>.Success(result);
        }

        public async Task<Account> GetUserByUserNameOrEmailAsync(string UserNameOrEmail)
        {
            return await _uow.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: p => p.Email == UserNameOrEmail || p.Username == UserNameOrEmail
            );
        }
    }
}
