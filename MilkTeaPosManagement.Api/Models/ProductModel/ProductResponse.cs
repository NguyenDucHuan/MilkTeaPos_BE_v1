namespace MilkTeaPosManagement.Api.Models.ProductModel
{
    public class ProductResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public string? ProductType { get; set; }
        public string? SizeId { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreateAt { get; set; }

        // Thông tin bổ sung
        public List<ProductResponse> Sizes { get; set; } = new List<ProductResponse>();
        public List<ComboItemResponse> ComboItems { get; set; } = new List<ComboItemResponse>();
    }

    public class ComboItemResponse
    {
        public int ComboItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int? Quantity { get; set; }
        public int? Discount { get; set; }
        public List<ComboItemResponse> ExtraItems { get; set; } = new List<ComboItemResponse>();
        public int? MasterId { get; internal set; }
    }

    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
