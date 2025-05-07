namespace MilkTeaPosManagement.Api.Models.TransactionModels
{
    public class TransactionUpdateModel
    {
        public int PaymentMethodId { get; set; }
        public decimal? AmountPaid { get; set; }
    }
}
