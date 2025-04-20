using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.PaymentMethodModels
{
    public class PaymentMethodRequest
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Payment method name is required")]
        public string MethodName { get; set; } = null!;
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = null!;
    }
}
