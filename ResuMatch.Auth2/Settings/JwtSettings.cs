using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Auth.Settings
{
    public class JwtSettings
    {
        public string? Issuer { get; set; } = "";
        public string? Audience { get; set; } = "";
        public string? SecretKey { get; set; } = "";
        public static TimeSpan ExiryDuration { get; set; } = TimeSpan.FromMinutes(1);
    }
}