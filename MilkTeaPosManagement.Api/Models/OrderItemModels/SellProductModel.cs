using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Models.OrderItemModels
{
    public class SellProductModel
    {
        public virtual Product Product { get; set; }
        public int? TotalQuantitySold { get; set; }
    }
}
