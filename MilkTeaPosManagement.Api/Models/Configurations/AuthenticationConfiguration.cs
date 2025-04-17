using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaPosManagement.Api.Models.Configurations
{
    public class AuthenticationConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string AccessTokenSecret { get; set; }
        public double AccessTokenExpiration { get; set; }
        public string RefreshTokenSecret { get; set; }
        public double RefreshTokenExpiration { get; set; }
        public string EmailVerificationSecret { get; set; }
        public double EmailVerificationExpiration { get; set; }
    }
}
