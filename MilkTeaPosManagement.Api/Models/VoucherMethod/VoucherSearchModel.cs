namespace MilkTeaPosManagement.Api.Models.VoucherMethod
{
    public class VoucherSearchModel
    {
        //public string? VoucherCode { get; set; }
        public decimal? MinDiscountAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        //public string? DiscountType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? SortBy { get; set; }
        public bool? SortAscending { get; set; }
    }
}
