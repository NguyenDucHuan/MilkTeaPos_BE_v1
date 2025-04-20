using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.OrderModels
{
    public class OrderSearchModel
    {
        public int? StaffId { get; set; }
        public int? PaymentMethodId { get; set; }
    }
}
