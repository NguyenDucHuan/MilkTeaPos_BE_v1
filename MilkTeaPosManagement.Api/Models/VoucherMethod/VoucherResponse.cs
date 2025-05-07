namespace MilkTeaPosManagement.Api.Models.VoucherMethod
{
    public class VoucherResponse
    {
        public int VoucherId { get; set; }

        public string? VoucherCode { get; set; }

        public decimal? DiscountAmount { get; set; }

        public string? DiscountType { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public decimal? MinimumOrderAmount { get; set; }

        public bool? Status { get; set; }

        public DateTime? CreateAt { get; set; }

        public int? CreateBy { get; set; }

        public DateTime? UpdateAt { get; set; }

        public int? UpdateBy { get; set; }

        public DateTime? DisableAt { get; set; }

        public int? DisableBy { get; set; }
    }
}
