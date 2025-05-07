using Net.payOS.Types;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IPayOSService
    {
        Task<(long, CreatePaymentResult?)> CreatePaymentLink(int orderId);
        Task<(long, PaymentLinkInformation?)> GetPaymentLinkInformation(int orderCode);
        Task<(long, PaymentLinkInformation?)> CancelOrder(int orderCode);
        Task<(long, int?)> VerifyPaymentWebhookData(WebhookType body);
    }
}
