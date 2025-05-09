using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface ICashFlowService
    {
        Task<MethodResult<List<Cashflow>>> GetCashflowHistoryAsync(int? userId, string? flowType, DateTime? startDate, DateTime? endDate);
        Task<bool> UpdateCashFlowAsync(decimal amount, string flowType);
    }
}
