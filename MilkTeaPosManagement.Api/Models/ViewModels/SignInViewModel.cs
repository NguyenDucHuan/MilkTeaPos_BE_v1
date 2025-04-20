using MilkTeaPosManagement.Api.Models.AuthenticationModels;

namespace MilkTeaPosManagement.Api.Models.ViewModels
{
    public class SignInViewModel
    {
        public AccessToken AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}
