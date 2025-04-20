namespace MilkTeaPosManagement.Api.Models.ViewModels
{
    public class AccountViewModel
    {

        public int AccountId { get; set; }

        public string? Username { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? ImageUrl { get; set; }

        public string? Phone { get; set; }

        public string? Role { get; set; }

        public bool? Status { get; set; }

        public string? Address { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
