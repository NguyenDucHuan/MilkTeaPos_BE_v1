using AutoMapper;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Models.ViewModels;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Mapper
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, ProfileViewModel>();
        }

    }
}
