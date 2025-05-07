using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using Net.payOS.Types;
using Net.payOS;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Api.Models.OrderModels;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet.Actions;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class PayOSService(IConfiguration configuration, IUnitOfWork unitOfWork) : IPayOSService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
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

            var order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderId);
            if (order is null)
            {
                return (3, null); //Order not found!!
            }
            var orderStatus = new Orderstatusupdate
            {
                OrderStatus = OrderConstant.SUCCESS.ToString(),
                OrderId = orderId,
                UpdatedAt = DateTime.Now,
                AccountId = order.StaffId,
            };            

            await _unitOfWork.GetRepository<Orderstatusupdate>().InsertAsync(orderStatus);
            if (await _unitOfWork.CommitAsync() <= 0)
            {
                return (3, null); //update fail
            }

            var orderDetails = await _unitOfWork.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.OrderId == orderId, include: oi => oi.Include(i => i.Product));
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
        public async Task<(long, PaymentLinkInformation?)> GetPaymentLinkInformation(int orderCode)
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

            var order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderCode);
            if (order is null)
            {
                return (3, null); //Order not found!!
            }
            var orderStatus = new Orderstatusupdate
            {
                OrderStatus = OrderConstant.CANCELED.ToString(),
                OrderId = orderCode,
                UpdatedAt = DateTime.Now,
                AccountId = order.StaffId,
            };

            await _unitOfWork.GetRepository<Orderstatusupdate>().InsertAsync(orderStatus);
            if (await _unitOfWork.CommitAsync() <= 0)
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

                string responseCode = data.code;
                var orderCode = (int)data.orderCode;

                if (responseCode == "00")
                {
                    var order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == orderCode);
                    if (order is null)
                    {
                        return (3, null); //Order not found!!
                    }
                    var orderStatus = new Orderstatusupdate
                    {
                        OrderStatus = OrderConstant.SUCCESS.ToString(),
                        OrderId = orderCode,
                        UpdatedAt = DateTime.Now,
                        AccountId = order.StaffId,
                    };

                    await _unitOfWork.GetRepository<Orderstatusupdate>().InsertAsync(orderStatus);
                    if (await _unitOfWork.CommitAsync() <= 0)
                    {
                        return (4, null); //fail
                    }
                    return (5, 0); // "Payment success"
                }
                return (4, 1); // "Payment failed"
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (4, -1); // "Payment failed" };
            }
        }
    }
}
