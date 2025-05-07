using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Api.Models.TransactionModels;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<MethodResult<Transaction>> UpdateTransactionAsync(int id, TransactionUpdateModel model);
    }
}
