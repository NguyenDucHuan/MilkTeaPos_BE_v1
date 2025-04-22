namespace MilkTeaPosManagement.Api.Models.ProductModel
{
    public class CreateExtraProductRequest
    {
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public decimal? Price { get; set; }
        public bool? Status { get; set; }
    }
}
