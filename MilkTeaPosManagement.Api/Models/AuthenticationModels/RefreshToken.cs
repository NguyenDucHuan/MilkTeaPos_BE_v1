namespace MilkTeaPosManagement.Api.Models.AuthenticationModels
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
