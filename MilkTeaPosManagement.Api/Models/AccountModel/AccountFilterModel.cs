using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.AccountModel
{
    public class AccountFilterModel
    {
        public string SearchTerm { get; set; }
        public string Role { get; set; }
        public bool? Status { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedFrom { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedTo { get; set; }
        public string SortBy { get; set; } = "accountid";
        public bool SortAscending { get; set; } = true;
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;
    }
}
