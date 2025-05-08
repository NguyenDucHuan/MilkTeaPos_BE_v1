using Microsoft.Extensions.Configuration;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using Net.payOS.Types;
using Net.payOS;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Api.Helper;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class PayOSService(IUnitOfWork uow, IConfiguration configuration) : IPayOSService
    {
        private readonly IUnitOfWork _uow = uow;
        private readonly IConfiguration _configuration = configuration;
        public async Task<(long, CreatePaymentResult?)> CreatePaymentLink(int orderId)
        {
            var clientId = _configuration["payOS:ClientId"];
            if (clientId == null)
            {
                return (0, null); //ClientId not found
            }
            var apiKey = _configuration["payOS:ApiKey"];
            if (apiKey == null)
            {
                return (1, null); //ApiKey not found
            }
            var checksumKey = _configuration["payOS:ChecksumKey"];
            if (checksumKey == null)
            {
                return (2, null); //ChecksumKey not found
            }

            PayOS _payOS = new(
                clientId,
                apiKey,
                checksumKey
            );

            var order = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderId);
            if (order is null)
            {
                return (3, null); //Order not found!!
            }

            var orderDetails = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.OrderId == orderId);
            List<ItemData> items = [];
            if (orderDetails is not null)
            {
                foreach (var orderDetail in orderDetails)
                {
                        items.Add(new ItemData(orderDetail.Product.ProductName, (int)orderDetail.Quantity,(int) orderDetail.Price));
                }
            }
            long expiredAt = (long)(DateTime.UtcNow.AddMinutes(10) - new DateTime(1970, 1, 1)).TotalSeconds;

            PaymentData paymentData = new(
                orderCode: orderId,
                amount: (int)order.TotalAmount,
                description: "Thanh toan hoa don",
                items: items,
                cancelUrl: "http://localhost:5173",
                returnUrl: "http://localhost:5173",
                expiredAt: expiredAt
            );

            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

            return (4, createPayment);
        }
        public async Task<(long, PaymentLinkInformation?)> GetPaymentLinkInformation(long orderCode)
        {
            var clientId = _configuration["payOS:ClientId"];
            if (clientId == null)
            {
                return (0, null); //ClientId not found
            }
            var apiKey = _configuration["payOS:ApiKey"];
            if (apiKey == null)
            {
                return (1, null); //ApiKey not found
            }
            var checksumKey = _configuration["payOS:ChecksumKey"];
            if (checksumKey == null)
            {
                return (2, null); //ChecksumKey not found
            }

            PayOS _payOS = new(clientId, apiKey, checksumKey);
            PaymentLinkInformation paymentLinkInformation = await _payOS.getPaymentLinkInformation(orderCode);
            return paymentLinkInformation == null ?
                (3, null) //not found
                : (4, paymentLinkInformation);
        }
        public async Task<(long, PaymentLinkInformation?)> CancelOrder(int orderCode)
        {
            var clientId = _configuration["payOS:ClientId"];
            if (clientId == null)
            {
                return (0, null); //ClientId not found
            }
            var apiKey = _configuration["payOS:ApiKey"];
            if (apiKey == null)
            {
                return (1, null); //ApiKey not found
            }
            var checksumKey = _configuration["payOS:ChecksumKey"];
            if (checksumKey == null)
            {
                return (2, null); //ChecksumKey not found
            }

            PayOS _payOS = new(clientId, apiKey, checksumKey);
            var getPaymentLinkInformation = await _payOS.getPaymentLinkInformation((long)orderCode);
            if (getPaymentLinkInformation == null)
            {
                return (3, null); //not found
            }
            PaymentLinkInformation paymentLinkInformation = await _payOS.cancelPaymentLink(orderCode);

            var order = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderCode);
            if (order is null)
            {
                return (3, null); //Order not found!!
            }
            var orderStatus = new Orderstatusupdate
            {
                OrderStatus = OrderConstant.CANCELLED.ToString(),
                OrderId = orderCode,
                UpdatedAt = DateTime.Now,
                AccountId = order.StaffId,
            };

            await _uow.GetRepository<Orderstatusupdate>().InsertAsync(orderStatus);
            if (await _uow.CommitAsync() <= 0)
            {
                return (3, null); //update fail
            }
            return (4, paymentLinkInformation);
        }
        public async Task<(long, int?)> VerifyPaymentWebhookData(WebhookType body)
        {
            try
            {
                var clientId = _configuration["payOS:ClientId"];
                if (clientId == null)
                {
                    return (0, null); //ClientId not found
                }
                var apiKey = _configuration["payOS:ApiKey"];
                if (apiKey == null)
                {
                    return (1, null); //ApiKey not found
                }
                var checksumKey = _configuration["payOS:ChecksumKey"];
                if (checksumKey == null)
                {
                    return (2, null); //ChecksumKey not found
                }

                PayOS _payOS = new(clientId, apiKey, checksumKey);
                WebhookData data = _payOS.verifyPaymentWebhookData(body);

                string responseCode = body.code;
                var orderCode = (int)data.orderCode;
                var order = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderCode);
                if (order is null)
                {
                    return (3, null); //Order not found!!
                }
                PaymentLinkInformation paymentLinkInformation = await _payOS.getPaymentLinkInformation(orderCode);
                if (responseCode == "00" && paymentLinkInformation.status == "PAID")
                {                    
                    var orderStatus = new Orderstatusupdate
                    {
                        OrderStatus = OrderConstant.SUCCESS.ToString(),
                        OrderId = orderCode,
                        UpdatedAt = DateTime.Now,
                        AccountId = order.StaffId,
                    };

                    await _uow.GetRepository<Orderstatusupdate>().InsertAsync(orderStatus);
                    var transaction = await _uow.GetRepository<Domain.Models.Transaction>().SingleOrDefaultAsync(predicate: t => t.OrderId == orderCode);
                    if (transaction is null)
                    {
                        return (3, null);
                    }
                    transaction.Status = true;
                    transaction.AmountPaid = transaction.Amount;
                    transaction.ChangeGiven = 0;
                    transaction.TransactionDate = DateTime.Now;
                    transaction.UpdatedAt = DateTime.Now;
                    transaction.PaymentMethodId = 3;
                    _uow.GetRepository<Domain.Models.Transaction>().UpdateAsync(transaction);
                    if (await _uow.CommitAsync() <= 0)
                    {
                        return (4, null); //fail
                    }
                    return (5, 0); // "Payment success"
                } else if (responseCode == "00")
                {
                    var orderStatus = new Orderstatusupdate
                    {
                        OrderStatus = OrderConstant.CANCELLED.ToString(),
                        OrderId = orderCode,
                        UpdatedAt = DateTime.Now,
                        AccountId = order.StaffId,
                    };

                    await _uow.GetRepository<Orderstatusupdate>().InsertAsync(orderStatus);
                    var transaction = await _uow.GetRepository<Domain.Models.Transaction>().SingleOrDefaultAsync(predicate: t => t.OrderId == orderCode);
                    if (transaction is null)
                    {
                        return (3, null);
                    }
                    transaction.Status = false;
                    transaction.AmountPaid = transaction.Amount;
                    transaction.ChangeGiven = 0;
                    transaction.TransactionDate = DateTime.Now;
                    transaction.UpdatedAt = DateTime.Now;
                    transaction.PaymentMethodId = 3;
                    _uow.GetRepository<Domain.Models.Transaction>().UpdateAsync(transaction);
                    if (await _uow.CommitAsync() <= 0)
                    {
                        return (4, null); //fail
                    }
                    return (5, 0);
                }
                return (4, 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (4, -1); // "Payment failed" };
            }
        }
    }
}
