using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.CategoryModels
{
    public class CategoryFilterModel
    {
        public string SearchTerm { get; set; }
        public bool? Status { get; set; }
        public string SortBy { get; set; } = "categoryid";
        public bool SortAscending { get; set; } = true;
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;
    }
}
