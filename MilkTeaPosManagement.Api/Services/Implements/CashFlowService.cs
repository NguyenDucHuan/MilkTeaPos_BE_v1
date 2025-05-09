using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class CashFlowService : ICashFlowService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CashFlowService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<List<Cashflow>>> GetCashflowHistoryAsync(int? userId, string? flowType, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var cashFlowHistory = await _unitOfWork.GetRepository<Cashflow>().GetListAsync();
                if (userId.HasValue)
                {
                    cashFlowHistory = cashFlowHistory.Where(c => c.UserId == userId.Value).ToList();
                }
                if (!string.IsNullOrEmpty(flowType))
                {
                    cashFlowHistory = cashFlowHistory.Where(c => c.FlowType == flowType).ToList();
                }
                if (startDate.HasValue)
                {
                    cashFlowHistory = cashFlowHistory.Where(c => c.CreatedAt >= startDate.Value).ToList();
                }
                if (endDate.HasValue)
                {
                    cashFlowHistory = cashFlowHistory.Where(c => c.CreatedAt <= endDate.Value).ToList();
                }
                return new MethodResult<List<Cashflow>>.Success(cashFlowHistory.ToList());
            }
            catch (Exception ex)
            {
                return new MethodResult<List<Cashflow>>.Failure("Error retrieving cash flow history.", StatusCodes.Status500InternalServerError);
            }
        }


        public Task<bool> UpdateCashFlowAsync(decimal amount, string flowType)
        {
            throw new NotImplementedException();
        }
        public Task<bool> UpdateCashFlowAsync(Cashflow cashflow)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteCashFlowAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}