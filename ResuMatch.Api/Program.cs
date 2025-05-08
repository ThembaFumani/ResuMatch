

using ResuMatch.Api.Models.Configurations;
using ResuMatch.Api.Repositories;
using ResuMatch.Api.Services.Concretes;
using ResuMatch.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OpenRouterConfig>(options =>
{
    options.ApiKey = builder.Configuration["OpenRouter:ApiKey"];
    options.Model = builder.Configuration["OpenRouter:Model"]; 
    options.Endpoint = builder.Configuration["OpenRouter:Endpoint"];
});

builder.Services.AddHttpClient();

builder.Services.AddScoped<IResumeRepository, ResumeRepository>(); // Register the repository
builder.Services.AddScoped<IAIService, OpenRouterAIService>(); // Register the OpenRouterAIService
builder.Services.AddScoped<ISkillMatcher, SkillMatcher>();
builder.Services.AddScoped<IAnalysisService, AnalysisService>();
builder.Services.AddScoped<IResumeAnalysisService, ResumeAnalysisService>();
builder.Services.AddSingleton<IFileProcessor>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


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
