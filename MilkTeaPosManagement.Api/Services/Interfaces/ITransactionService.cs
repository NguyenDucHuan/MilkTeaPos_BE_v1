using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Api.Models.TransactionModels;
using MilkTeaPosManagement.Api.Models.VoucherMethod;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Domain.Paginate;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface ITransactionService
    {
        //Task<IPaginate<Transaction>?> GetAll();
        Task<MethodResult<Transaction>> GetTransactionByIdAsync(int id);
        //Task<MethodResult<Transaction>> CreateTransactionAsync(VoucherCreateRequestModel request, int userId);
        Task<MethodResult<TransactionResponse>> UpdateTransactionAsync(int id, TransactionUpdateModel model);

        Task<MethodResult<List<TransactionResponse>>> GetTransactionAsyncUseForCashBalance(DateTime date);

    }
}
