using Microsoft.AspNetCore.Identity;

namespace ResuMatch.Auth.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthRepository(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> AddUserToRoleAsync(IdentityUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);   
        }

        public async Task<IdentityResult> CreateUserAsync(IdentityUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public Task<IdentityUser?> GetUserByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }
    }
}