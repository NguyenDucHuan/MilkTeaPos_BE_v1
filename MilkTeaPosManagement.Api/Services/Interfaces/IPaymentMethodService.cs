using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IPaymentmethodService
    {
        Task<ICollection<Paymentmethod>> GetAllPaymentmethodAsync();
        Task<MethodResult<Paymentmethod>> GetPaymentmethodByIdAsync(int paymentmethodId);
        Task<MethodResult<Paymentmethod>> AddAPaymentmethodAsync(PaymentMethodRequest paymentmethodRequest);
        Task<MethodResult<Paymentmethod>> UpdateAPaymentmethodAsync(PaymentMethodRequest paymentMethodRequest);
        Task<MethodResult<Paymentmethod>> DeleteAPaymentmethodAsync(int paymentmethodId);
    }
}
