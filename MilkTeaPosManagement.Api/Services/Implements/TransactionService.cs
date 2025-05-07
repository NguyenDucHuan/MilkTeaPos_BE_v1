using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Api.Models.TransactionModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class TransactionService(IUnitOfWork uow) : ITransactionService
    {
        private readonly IUnitOfWork _uow = uow;
        public async Task<MethodResult<Transaction>> UpdateTransactionAsync(int id, TransactionUpdateModel model)
        {
            try
            {
                var paymrentmethod = await _uow.GetRepository<Paymentmethod>().SingleOrDefaultAsync(predicate: PM => PM.PaymentMethodId == model.PaymentMethodId);
                if (paymrentmethod == null)
                {
                    return new MethodResult<Transaction>.Failure("Paymentmethod not valid!", StatusCodes.Status400BadRequest);
                }
                var transaction = await _uow.GetRepository<Transaction>().SingleOrDefaultAsync(predicate: t => t.TransactionId == id);
                if (transaction == null)
                {
                    return new MethodResult<Transaction>.Failure("Transaction not valid!", StatusCodes.Status400BadRequest);
                }
                if (paymrentmethod.MethodName == "Cash" && model.AmountPaid.HasValue && model.AmountPaid.Value <= transaction.Amount)
                {
                    return new MethodResult<Transaction>.Failure("Amount paiid cannot be less than "+ transaction.Amount+"!", StatusCodes.Status400BadRequest);
                }
                transaction.PaymentMethodId = model.PaymentMethodId;
                if (paymrentmethod.MethodName == "Cash" && model.AmountPaid.HasValue && model.AmountPaid.Value > transaction.Amount)
                {
                    transaction.AmountPaid = model.AmountPaid;
                    transaction.ChangeGiven = model.AmountPaid - transaction.Amount;
                    transaction.Status = true;
                }                
                transaction.UpdatedAt = DateTime.Now;
                var newStatus = new Orderstatusupdate
                {
                    OrderStatus = OrderConstant.DELIVERED.ToString(),
                    OrderId = transaction.OrderId,
                    UpdatedAt = DateTime.Now,
                    //AccountId = account.AccountId
                    AccountId = transaction.StaffId
                };

                await _uow.GetRepository<Orderstatusupdate>().InsertAsync(newStatus);

                _uow.GetRepository<Transaction>().UpdateAsync(transaction);

                if (await _uow.CommitAsync() > 0)
                {
                    return new MethodResult<Transaction>.Success(transaction);
                }

                return new MethodResult<Transaction>.Failure(
                    "Failed to update transaction",
                    StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return new MethodResult<Transaction>.Failure(
                    $"Error updating transaction: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}
