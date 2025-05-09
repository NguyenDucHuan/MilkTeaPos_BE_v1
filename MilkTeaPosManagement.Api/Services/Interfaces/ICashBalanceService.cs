using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.TransactionModels;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface ICashBalanceService
    {
        Task<MethodResult<BalanceResponse>> GetCashBalanceAsync(DateTime? startDate, DateTime? endDate);
        //Task<MethodResult<List<TransactionResponse>>> GetTransactionAsyncUseForCashBalance(DateTime date);
        //Task<MethodResult<List<Transaction>>> GetCashflowHistoryAsync(int? userId, string? flowType, DateTime? startDate, DateTime? endDate);
        Task<MethodResult<Cashbalance>> UpdateCashBalanceAsync(decimal amount, int userId, string Type, string description);

    }
}
