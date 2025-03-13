using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ResuMatch.Auth.Repositories
{
    public interface IAuthRepository
    {
        Task<IdentityUser?> GetUserByEmailAsync(string email);
        Task<IdentityResult> CreateUserAsync(IdentityUser user, string password);
        Task<IdentityResult> AddUserToRoleAsync(IdentityUser user, string role);
    }
}