using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface ICashBalanceService
    {
        Task<MethodResult<Cashbalance>> GetCashBalanceAsync(int userId);
        Task<MethodResult<List<Cashflow>>> GetCashflowHistoryAsync(int? useridm, string? flowType, DateTime? startDate, DateTime? endDate);
        Task<bool> UpdateCashBalanceAsync(decimal amount, string flowType);
    }
}
