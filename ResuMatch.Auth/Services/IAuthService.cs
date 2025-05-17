using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResuMatch.Auth.Models;

namespace ResuMatch.Auth.Services
{
    public interface IAuthService
    {
        Task<JwtToken> RegisterAsync(RegisterModel model);
        Task<JwtToken> LoginAsync(LoginModel model);
        
    }
}