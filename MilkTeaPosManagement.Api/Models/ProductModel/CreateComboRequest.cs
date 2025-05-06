namespace MilkTeaPosManagement.Api.Models.ProductModel
{
    public class CreateComboRequest
    {
        public string ComboName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public decimal Price { get; set; }
        public bool? Status { get; set; }
        public bool? ToppingAllowed { get; set; }
        public List<ComboItemRequest> ComboItems { get; set; } = new List<ComboItemRequest>();
    }

    public class ComboItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public int? Discount { get; set; }
        public List<ExtraItemRequest>? ExtraItems { get; set; }
    }

    public class ExtraItemRequest
    {
        public int ExtraProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public int? Discount { get; set; }
    }
}
