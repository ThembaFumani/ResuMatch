using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using ResuMatch.Api.Data.MatchResultsDataContext;
using ResuMatch.Api.Data.JobDescriptionsDataContext;
using MongoDB.Driver;
using ResuMatch.Api.Repositories.JobDescriptionRepository;
using ResuMatch.Api.Repositories.ResumeRepository;
using ResuMatch.Api.Repositories.MatchResultRepository;

var builder = WebApplication.CreateBuilder(args);
var keyVaultName = builder.Configuration["KeyVaultName"];  
var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
var secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());
builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
builder.Services.AddSingleton(new SecretClient(keyVaultUri, new DefaultAzureCredential()));
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddScoped<IResumesContext, ResumesContext>();
builder.Services.AddScoped<IMatchResultsContext, MatchResultsContext>();
builder.Services.AddScoped<IJobDescriptionsDataContext, JobDescriptionsDataContext>();

builder.Services.AddScoped<IJobDescriptionRepository, JobDescriptionRepository>();
builder.Services.AddScoped<IResumeRepository, ResumeRepository>();
builder.Services.AddScoped<IMatchResultRepository, MatchResultRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
