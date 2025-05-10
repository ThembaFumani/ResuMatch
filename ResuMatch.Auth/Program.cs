using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ResuMatch.Auth.Data;
using ResuMatch.Auth.Repositories;
using ResuMatch.Auth.Services;
using ResuMatch.Auth.Settings;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["KeyVaultName"];  
var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
var secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());
builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
builder.Services.AddSingleton(new SecretClient(keyVaultUri, new DefaultAzureCredential()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOtions =>
    {
        sqlOtions.EnableRetryOnFailure();
    });
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
var secret = secretClient.GetSecret("JwtSecretKey");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret.Value.Value)),
                    ClockSkew = TimeSpan.Zero
                };
            });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
 