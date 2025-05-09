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
            service.AddTransient<IAccountService, AccountService>();
            service.AddTransient<IAuthenticationService, AuthenticationService>();
            service.AddTransient<IOrderService, OrderService>();
            service.AddTransient<IOrderItemService, OrderItemService>();
            service.AddTransient<IPaymentmethodService, PaymentMethodService>();
            service.AddTransient<IStatisticService, StatisticService>();
            service.AddHttpContextAccessor();
            service.AddTransient<ICloudinaryService, CloudinaryService>();
            service.AddTransient<ICategoryService, CategoryService>();
            service.AddTransient<IProductService, ProductService>();
            service.AddTransient<IVoucherService, VoucherService>();
            service.AddTransient<ITransactionService, TransactionService>();
            service.AddTransient<IPayOSService, PayOSService>();
            service.AddTransient<ICashBalanceService, CashBalanceService>();
            return service;
        }
    }
}

