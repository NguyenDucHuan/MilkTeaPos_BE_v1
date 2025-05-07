using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.VoucherMethod
{
    public class VoucherCreateRequestModel
    {
        [Required(ErrorMessage = "Voucher code is required")]
        public string VoucherCode { get; set; }
        [Required(ErrorMessage = "Discount amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount amount must be greater than 0")]
        public decimal DiscountAmount { get; set; }
        [Required(ErrorMessage = "Discount type is required")]
        public string DiscountType { get; set; }
        [Required(ErrorMessage = "Expiration date is required")]
        public DateTime ExpirationDate { get; set; }
        [Required(ErrorMessage = "Minimum Order Amount is required")]
        public decimal MinimumOrderAmount { get; set; }
    }
}
