using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.OrderModels
{
    public class OrderSearchModel
    {
        public int? StaffId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Status { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? SortBy { get; set; }
        public bool? SortAscending { get; set; }
    }
}
