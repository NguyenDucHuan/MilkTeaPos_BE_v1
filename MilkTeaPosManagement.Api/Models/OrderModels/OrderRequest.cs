using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.OrderModels
{
    public class OrderRequest
    {
        public string? Note { get; set; }
        [Required(ErrorMessage = "Payment method ID is required")]
        public int PaymentMethodId { get; set; }
    }
}
