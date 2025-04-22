using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.ProductModel
{
    public class ProductFilterModel
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public string? ProductType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? Status { get; set; }
        public bool IsShopManager { get; set; } = false;
        public string SortBy { get; set; } = "productid";
        public bool SortAscending { get; set; } = true;
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;
    }
}
