using ResuMatch.Api.Models.Configurations;
using ResuMatch.Api.DataAccess;
using ResuMatch.Api.Services.Concrete;
using ResuMatch.Api.Services.Concretes;
using ResuMatch.Api.Services.FileProccessing.Concretes;
using ResuMatch.Api.Services.FileProccessing.Interfaces;
using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Pipelines;
using ResuMatch.Api.Services.Pipelines.Steps;
using ResuMatch.Api.Services.Concrete.AIServices;
using System.Text.Json;
using ResuMatch.Api.Services.Interfaces.AiInterfaces;

var builder = WebApplication.CreateBuilder(args);

// Standard setup for Web API (often placed early)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddHttpClient();

builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true,
    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
});

builder.Services.Configure<OpenRouterConfig>(options =>
{
    options.ApiKey = builder.Configuration["OpenRouter:ApiKey"];
    options.Model = builder.Configuration["OpenRouter:Model"];
    options.Endpoint = builder.Configuration["OpenRouter:Endpoint"];
});
builder.Services.Configure<LocalModelConfig>(option =>
{
    option.Endpoint = builder.Configuration["LocalModel:Endpoint"];
    option.Models = builder.Configuration.GetSection("LocalModel:Models").Get<List<LocalModelEntry>>();
    option.Stream = builder.Configuration.GetValue<bool>("LocalModel:Stream");
});

builder.Services.AddScoped<IHttpHandler, HttpHandler>();

builder.Services.AddScoped<IPipeline, Pipeline>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, ExtractResumeTextStep>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, ExtractAnalysisExperienceStep>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, ExtractResumeSkillStep>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, ExtractJobDescriptionSkillsStep>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, MatchSkillsStep>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, GenerateSummaryStep>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, CalculateScoreStep>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, PersistResumeDataStep>();

builder.Services.AddScoped<IFileProcessorFactory, FileProcessorFactory>();
builder.Services.AddScoped<IResumeAnalysisService, ResumeAnalysisService>();
builder.Services.AddScoped<IPromptFileService, PromptFileService>();
builder.Services.AddScoped<IPromptService, PromptService>();
builder.Services.AddScoped<IResumeRepository, ResumeRepository>();
builder.Services.AddScoped<IProTailorService, ProTailorService>();
builder.Services.AddScoped<IProTailorAIService, LlmService>();

bool useLocalModel = builder.Configuration.GetValue<bool>("FeatureFlags:UseLocalModel");

if (useLocalModel)
{
    builder.Services.AddScoped<IAIService, LlmService>();
}
else
{
    builder.Services.AddScoped<IAIService, OpenRouterAIService>();
}


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();