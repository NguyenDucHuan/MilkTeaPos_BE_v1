using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.TransactionModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using AutoMapper;
using MilkTeaPosManagement.Api.Constants;
using System.Reflection;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class CashBalanceService : ICashBalanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CashBalanceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        public async Task<MethodResult<BalanceResponse>> GetCashBalanceAsync(DateTime? startDate, DateTime? endDate)
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
                    cashBalance = await _unitOfWork.GetRepository<Cashbalance>().SingleOrDefaultAsync();
                }
                var balanceResponse = new BalanceResponse
                {
                    Amount = cashBalance.Amount,
                    UpdatedAt = cashBalance.UpdatedAt
                };
                var paymentMethod = await _unitOfWork.GetRepository<Paymentmethod>().SingleOrDefaultAsync(predicate: pm => pm.MethodName == "Cash");
                if (paymentMethod == null)
                {
                    return new MethodResult<BalanceResponse>.Failure("Payment method not found.", StatusCodes.Status404NotFound);
                }
                var transactions = await _unitOfWork.GetRepository<Transaction>().GetListAsync(
                    predicate: t => t.TransactionDate.HasValue &&
                    t.TransactionDate.Value.Date >= startDate &&
                    t.TransactionDate.Value.Date <= endDate &&
                    (t.TransactionType == TransactionTypeConstant.CASH_IN ||
                    t.TransactionType == TransactionTypeConstant.CASH_OUT ||
                    t.PaymentMethodId == paymentMethod.PaymentMethodId),
                    orderBy: t => t.OrderByDescending(t => t.TransactionDate)
                    );
                if (transactions == null || !transactions.Any())
                {
                    return new MethodResult<BalanceResponse>.Failure("No transactions found for the specified date", StatusCodes.Status404NotFound);
                }

                balanceResponse.ClosingBalance = transactions.FirstOrDefault()?.BeforeCashBalance;
                balanceResponse.OpeningBalance = transactions.LastOrDefault()?.AfterCashBalance;
                balanceResponse.Amount = Math.Round(balanceResponse.Amount ?? 0, 2);
                balanceResponse.UpdatedAt = cashBalance.UpdatedAt;
                balanceResponse.CashInTotal = transactions.Where(t => t.TransactionType == TransactionTypeConstant.CASH_IN || t.PaymentMethodId == paymentMethod.PaymentMethodId).Sum(t => t.Amount);
                balanceResponse.CashOutTotal = transactions.Where(t => t.TransactionType == TransactionTypeConstant.CASH_OUT).Sum(t => t.Amount);
                balanceResponse.Transactions = _mapper.Map<List<TransactionResponse>>(transactions);
                return new MethodResult<BalanceResponse>.Success(balanceResponse);
            }
            catch (Exception ex)
            {
                return new MethodResult<BalanceResponse>.Failure("Cash balance not found.", StatusCodes.Status400BadRequest);
            }
        }
        //public async Task<MethodResult<List<TransactionResponse>>> GetTransactionAsyncUseForCashBalance(DateTime date)
        //{
        //    try
        //    {

        //        var PaymentMethod = await _unitOfWork.GetRepository<Paymentmethod>().GetListAsync();
        //        if (PaymentMethod == null || !PaymentMethod.Any())
        //        {
        //            return new MethodResult<List<TransactionResponse>>.Failure("No payment methods found", StatusCodes.Status404NotFound);
        //        }
        //        var CashPaymentMethod = PaymentMethod.FirstOrDefault(pm => pm.MethodName == "Cash");
        //        var transactions = await _unitOfWork.GetRepository<Domain.Models.Transaction>().GetListAsync(
        //            predicate: t => t.TransactionDate.HasValue && t.TransactionDate.Value.Date == date.Date &&
        //            (t.PaymentMethodId == CashPaymentMethod.PaymentMethodId ||
        //            t.TransactionType == TransactionTypeConstant.CASH_IN ||
        //            t.TransactionType == TransactionTypeConstant.CASH_OUT)
        //        );
        //        if (transactions == null || !transactions.Any())
        //        {
        //            return new MethodResult<List<TransactionResponse>>.Failure("No transactions found for the specified date", StatusCodes.Status404NotFound);
        //        }
        //        var transactionResponses = _mapper.Map<List<TransactionResponse>>(transactions);
        //        var transactionList = transactionResponses.ToList();

        //        return new MethodResult<List<TransactionResponse>>.Success(transactionList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new MethodResult<List<TransactionResponse>>.Failure($"Error retrieving transactions: {ex.Message}", StatusCodes.Status500InternalServerError);
        //    }
        //}
        //public async Task<MethodResult<List<Transaction>>> GetCashflowHistoryAsync(int? userId, string? flowType, DateTime? startDate, DateTime? endDate)
        //{
        //    try
        //    {
        //        var paymentMethods = await _unitOfWork.GetRepository<Paymentmethod>().SingleOrDefaultAsync(predicate: p => p.MethodName == "Cash");
        //        var cashPaymentMethodId = paymentMethods?.PaymentMethodId;

        //        var transactionHistoryList = await _unitOfWork.GetRepository<Transaction>().GetListAsync(
        //            predicate: t => t.TransactionDate.HasValue && t.TransactionDate.Value.Date >= startDate && t.TransactionDate.Value.Date <= endDate &&
        //            (userId == null || t.StaffId == userId) &&
        //            (t.TransactionType == TransactionTypeConstant.CASH_IN || t.TransactionType == TransactionTypeConstant.CASH_OUT || t.PaymentMethodId == cashPaymentMethodId),
        //            orderBy: t => t.OrderByDescending(t => t.TransactionDate)
        //            );

        //        if (transactionHistoryList == null || !transactionHistoryList.Any())
        //        {
        //            return new MethodResult<List<Transaction>>.Failure("No cash flow history found for the specified criteria.", StatusCodes.Status404NotFound);
        //        }
        //        if (flowType != null)
        //        {
        //            transactionHistoryList = transactionHistoryList.Where(t => t.TransactionType == flowType).ToList();
        //        }
        //        var transactionList = _mapper.Map<List<Transaction>>(transactionHistoryList);
        //        return new MethodResult<List<Transaction>>.Success(transactionList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new MethodResult<List<Transaction>>.Failure("Error retrieving cash flow history.", StatusCodes.Status500InternalServerError);
        //    }
        //}
        public async Task<MethodResult<Cashbalance>> UpdateCashBalanceAsync(decimal amount, int userId, string type, string description)
        {
            try
            {
                var cashBalance = await _unitOfWork.GetRepository<Cashbalance>().SingleOrDefaultAsync();
                await _unitOfWork.BeginTransactionAsync();
                if (cashBalance == null)
                {
                    var CashBalance = new Cashbalance
                    {
                        Amount = 0,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.GetRepository<Cashbalance>().InsertAsync(CashBalance);
                    await _unitOfWork.CommitAsync();
                    cashBalance = await _unitOfWork.GetRepository<Cashbalance>().SingleOrDefaultAsync();
                }
                var transaction = new Transaction
                {
                    Amount = amount,
                    TransactionDate = DateTime.UtcNow,
                    StaffId = userId,
                    BeforeCashBalance = cashBalance.Amount,
                    TransactionType = type,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Description = description,
                };
                if (amount <= 0)
                {
                    return new MethodResult<Cashbalance>.Failure("Amount must be greater than zero.", StatusCodes.Status400BadRequest);
                }
                if (string.IsNullOrEmpty(type))
                {
                    return new MethodResult<Cashbalance>.Failure("Transaction type is required.", StatusCodes.Status400BadRequest);
                }
                if (type != TransactionTypeConstant.CASH_IN && type != TransactionTypeConstant.CASH_OUT)
                {
                    return new MethodResult<Cashbalance>.Failure("Invalid transaction type. (Typeneed CashOut/ CashOut) ", StatusCodes.Status400BadRequest);
                }
                if (type == TransactionTypeConstant.CASH_IN)
                {
                    cashBalance.Amount += amount;
                }
                else if (type == TransactionTypeConstant.CASH_OUT)
                {
                    if (cashBalance.Amount < amount)
                    {
                        return new MethodResult<Cashbalance>.Failure("Số dư không đủ", StatusCodes.Status400BadRequest);
                    }
                    cashBalance.Amount -= amount;
                }
                else
                {
                    return new MethodResult<Cashbalance>.Failure("Invalid transaction type.", StatusCodes.Status400BadRequest);
                }
                transaction.AfterCashBalance = cashBalance.Amount;
                transaction.Status = true;
                await _unitOfWork.GetRepository<Transaction>().InsertAsync(transaction);
                cashBalance.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.GetRepository<Cashbalance>().UpdateAsync(cashBalance);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();
                return new MethodResult<Cashbalance>.Success(cashBalance);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new MethodResult<Cashbalance>.Failure($"Error updating cash balance: {ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
