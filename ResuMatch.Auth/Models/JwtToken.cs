using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Auth.Models
{
    public class JwtToken
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}