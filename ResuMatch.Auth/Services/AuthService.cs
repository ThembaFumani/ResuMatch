using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ResuMatch.Auth.Models;
using ResuMatch.Auth.Repositories;
using ResuMatch.Auth.Settings;

namespace ResuMatch.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AuthService> _logger;
        private readonly SecretClient _keyVaultClient;
        public AuthService
        (IAuthRepository authRepository, 
        SignInManager<IdentityUser> signInManager, 
        ILogger<AuthService> logger,
        SecretClient keyVaultClient)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
            _logger = logger;
            _keyVaultClient = keyVaultClient;
        }

        public async Task<JwtToken> LoginAsync(LoginModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    throw new ArgumentNullException(nameof(model.Email), "Email cannot be null or empty");
                }
                
                var user = await _authRepository.GetUserByEmailAsync(model.Email) ?? throw new Exception("Invalid credentials");
                if (string.IsNullOrEmpty(model.Password))
                {
                    throw new ArgumentNullException(nameof(model.Password), "Password cannot be null or empty");
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (!result.Succeeded)
                {
                    throw new Exception("Invalid credentials");
                }

                return GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login: {ex.Message}");
                throw;
            }
        }

        public async Task<JwtToken> RegisterAsync(RegisterModel model)
        {
            try 
            {
                var existingUser = await _authRepository.GetUserByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    throw new Exception("User already exists");
                }

                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _authRepository.CreateUserAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user");
                }
                else
                {
                    await _authRepository.AddUserToRoleAsync(user, "User");
                }

                return GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during regirstation: {ex.Message}");
                throw;
            }
        }

        private JwtToken GenerateJwtToken(IdentityUser user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, "User")
                };
                 
                var secret = GetSecretFromKeyVault("JwtSecretKey").Result; 
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiration = DateTime.UtcNow.AddDays(1);

                var token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: expiration,
                    signingCredentials: creds
                );

                return new JwtToken
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = expiration
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during token generation: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetSecretFromKeyVault(string secretName)
        {
            try
            {
                var secret = await _keyVaultClient.GetSecretAsync(secretName);
                return secret.Value.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving secret {secretName} from Key Vault", secretName);
                throw new Exception("Error retrieving secrets from Key Vault.");
            }
        }
    }
}