using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;
using System;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class CashBalanceService : ICashBalanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CashBalanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<MethodResult<Cashbalance>> GetCashBalanceAsync(int userId)
        {
            try
            {
                var cashBalance = await _unitOfWork.GetRepository<Cashbalance>().SingleOrDefaultAsync();
                if (cashBalance == null)
                {
                    var CashBalance = new Cashbalance
                    {
                        Amount = 0,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.GetRepository<Cashbalance>().InsertAsync(CashBalance);
                    await _unitOfWork.CommitAsync();
                }
                return new MethodResult<Cashbalance>.Success(cashBalance);
            }
            catch (Exception ex)
            {
                return new MethodResult<Cashbalance>.Failure("Cash balance not found.", StatusCodes.Status400BadRequest);
            }

        }


        public Task<MethodResult<List<Cashflow>>> GetCashflowHistoryAsync(int userId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCashBalanceAsync(decimal amount, string flowType)
        {
            throw new NotImplementedException();
        }
    }
}
