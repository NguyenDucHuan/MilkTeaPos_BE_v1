namespace MilkTeaPosManagement.Api.Models.AccountModel
{
    public class UpdateUserRequest
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public bool? Status { get; set; }
    }
}
