namespace MilkTeaPosManagement.Api.Models.VoucherMethod
{
    public class VoucherUpdateRequestModel
    {
        public decimal? DiscountAmount { get; set; }
        public string? DiscountType { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
    }
}
