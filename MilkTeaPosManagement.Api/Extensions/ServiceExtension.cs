
using MilkTeaPosManagement.Api.Models.AuthenticationModels;
using MilkTeaPosManagement.Api.Services.Implements;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.GenericRepositories;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;
namespace MilkTeaPosManagement.Api.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddService(this IServiceCollection service)
        {
            service.AddTransient<IUnitOfWork, UnitOfWork<MilTeaPosDbContext>>();
            service.AddTransient<ITokenGenerator, TokenGenerator>();
            service.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            service.AddTransient<IUserService, UserService>();
            service.AddTransient<IAuthenticationService, AuthenticationService>();
            return service;
        }
    }
}

