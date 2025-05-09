using AutoMapper;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Api.Models.TransactionModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;
using Net.payOS.Types;
using Net.payOS;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using MilkTeaPosManagement.Api.Models.VoucherMethod;


namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class TransactionService(IUnitOfWork uow, IMapper mapper, IConfiguration configuration) : ITransactionService
    {
        private readonly IUnitOfWork _uow = uow;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _configuration = configuration;
        public async Task<MethodResult<TransactionResponse>> UpdateTransactionAsync(int id, TransactionUpdateModel model)
        {
            try
            {
                var paymrentmethod = await _uow.GetRepository<Paymentmethod>().SingleOrDefaultAsync(predicate: PM => PM.PaymentMethodId == model.PaymentMethodId);
                if (paymrentmethod == null)
                {
                    return new MethodResult<TransactionResponse>.Failure("Paymentmethod not valid!", StatusCodes.Status400BadRequest);
                }
                var transaction = await _uow.GetRepository<Domain.Models.Transaction>().SingleOrDefaultAsync(predicate: t => t.TransactionId == id);
                if (transaction == null)
                {
                    return new MethodResult<TransactionResponse>.Failure("Transaction not valid!", StatusCodes.Status400BadRequest);
                }
                if (paymrentmethod.MethodName == "Cash" && model.AmountPaid.HasValue && model.AmountPaid.Value <= transaction.Amount)
                {
                    return new MethodResult<TransactionResponse>.Failure("Amount paid cannot be less than " + transaction.Amount + "!", StatusCodes.Status400BadRequest);
                }
                if (paymrentmethod.MethodName == "Cash" && (!model.AmountPaid.HasValue || model.AmountPaid.Value <= 0))
                {
                    return new MethodResult<TransactionResponse>.Failure("Amount paid cannot be less than 0!", StatusCodes.Status400BadRequest);
                }
                if (paymrentmethod.MethodName == "Cash" && model.AmountPaid.HasValue && model.AmountPaid.Value > transaction.Amount)
                {
                    var cashBalance = await _uow.GetRepository<Cashbalance>().SingleOrDefaultAsync();

                    if (cashBalance == null)
                    {
                        var CashBalance = new Cashbalance
                        {
                            Amount = 0,
                            UpdatedAt = DateTime.UtcNow
                        };
                        await _uow.GetRepository<Cashbalance>().InsertAsync(CashBalance);
                        await _uow.CommitAsync();
                        cashBalance = await _uow.GetRepository<Cashbalance>().SingleOrDefaultAsync();
                    }
                    if (cashBalance.Amount < model.AmountPaid - transaction.Amount)
                    {
                        return new MethodResult<TransactionResponse>.Failure("Tiền trong pos ko đủ để thối", StatusCodes.Status400BadRequest);
                    }

                    transaction.PaymentMethodId = model.PaymentMethodId;
                    transaction.AmountPaid = model.AmountPaid;
                    transaction.ChangeGiven = model.AmountPaid - transaction.Amount;
                    transaction.TransactionDate = DateTime.Now;
                    transaction.Status = true;
                    transaction.UpdatedAt = DateTime.Now;
                    transaction.BeforeCashBalance = cashBalance.Amount;
                    transaction.AfterCashBalance = cashBalance.Amount - (model.AmountPaid - transaction.Amount);
                    cashBalance.Amount = cashBalance.Amount - (model.AmountPaid - transaction.Amount);
                    cashBalance.UpdatedAt = DateTime.Now;
                    var newStatus = new Orderstatusupdate
                    {
                        OrderId = transaction.OrderId,
                        UpdatedAt = DateTime.Now,
                        //AccountId = account.AccountId
                        AccountId = transaction.StaffId
                    };
                    await _uow.GetRepository<Orderstatusupdate>().InsertAsync(newStatus);


                    _uow.GetRepository<Cashbalance>().UpdateAsync(cashBalance);
                    _uow.GetRepository<Domain.Models.Transaction>().UpdateAsync(transaction);

                    if (await _uow.CommitAsync() > 0)
                    {
                        var resp = _mapper.Map<TransactionResponse>(transaction);
                        return new MethodResult<TransactionResponse>.Success(resp);
                    }
                }
                if (paymrentmethod.MethodName == "Online")
                {
                    transaction.AmountPaid = transaction.Amount;
                    transaction.ChangeGiven = 0;
                    transaction.PaymentMethodId = 3;
                    _uow.GetRepository<Domain.Models.Transaction>().UpdateAsync(transaction);
                    if (await _uow.CommitAsync() <= 0)
                    {
                        return new MethodResult<TransactionResponse>.Failure("Cannot update statuses", StatusCodes.Status400BadRequest); //fail
                    }
                    var clientId = _configuration["payOS:ClientId"];
                    if (clientId == null)
                    {
                        return new MethodResult<TransactionResponse>.Failure("Client Id not valid!", StatusCodes.Status400BadRequest); //ClientId not found
                    }
                    var apiKey = _configuration["payOS:ApiKey"];
                    if (apiKey == null)
                    {
                        return new MethodResult<TransactionResponse>.Failure("API Key not found!", StatusCodes.Status400BadRequest); //ApiKey not found
                    }
                    var checksumKey = _configuration["payOS:ChecksumKey"];
                    if (checksumKey == null)
                    {
                        return new MethodResult<TransactionResponse>.Failure("Checksum Key not found!", StatusCodes.Status400BadRequest); //ChecksumKey not found
                    }

                    PayOS _payOS = new(
                        clientId,
                        apiKey,
                        checksumKey
                    );

                    var order = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == transaction.OrderId);
                    if (order is null)
                    {
                        return new MethodResult<TransactionResponse>.Failure("Order not found!", StatusCodes.Status400BadRequest); //Order not found!!
                    }

                    var orderDetails = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.OrderId == transaction.OrderId, include: oi => oi.Include(id => id.Product));
                    List<ItemData> items = [];
                    if (orderDetails is not null)
                    {
                        foreach (var orderDetail in orderDetails)
                        {
                            items.Add(new ItemData(orderDetail.Product.ProductName, (int)orderDetail.Quantity, (int)orderDetail.Price));
                        }
                    }
                    long expiredAt = (long)(DateTime.UtcNow.AddMinutes(10) - new DateTime(1970, 1, 1)).TotalSeconds;

                    PaymentData paymentData = new(
                        orderCode: (int)transaction.OrderId,
                        amount: (int)order.TotalAmount,
                        description: "Thanh toan hoa don #" + transaction.OrderId,
                        items: items,
                        cancelUrl: "http://localhost:5173/orders",
                        returnUrl: "http://localhost:5173/orders",
                        expiredAt: expiredAt
                    );

                    CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
                    var resp = _mapper.Map<TransactionResponse>(transaction);
                    resp.PaymentLink = createPayment.checkoutUrl;
                    return new MethodResult<TransactionResponse>.Success(resp);
                }

                return new MethodResult<TransactionResponse>.Failure(
                    "Failed to update transaction",
                    StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return new MethodResult<TransactionResponse>.Failure(
                    $"Error updating transaction: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
        public async Task<MethodResult<Domain.Models.Transaction>> GetTransactionByIdAsync(int id)
        {
            var transaction = await _uow.GetRepository<Domain.Models.Transaction>().SingleOrDefaultAsync(
                predicate: c => c.TransactionId == id
            );

            if (transaction == null)
            {
                return new MethodResult<Domain.Models.Transaction>.Failure("Transaction not found", StatusCodes.Status404NotFound);
            }

            return new MethodResult<Domain.Models.Transaction>.Success(transaction);
        }


    }
}
