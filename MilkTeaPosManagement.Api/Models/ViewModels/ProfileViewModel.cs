namespace MilkTeaPosManagement.Api.Models.ViewModels
{
    public class ProfileViewModel
    {
        public int AccountId { get; set; }

        public string? Username { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? ImageUrl { get; set; }

        public string? Phone { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
