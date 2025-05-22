using ResuMatch.Api.Data;
using ResuMatch.Api.Models.Configurations;
using ResuMatch.Api.Repositories;
using ResuMatch.Api.Services.Concretes;
using ResuMatch.Api.Services.FileProccessing.Concretes;
using ResuMatch.Api.Services.FileProccessing.Interfaces;
using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Pipelines;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OpenRouterConfig>(options =>
{
    options.ApiKey = builder.Configuration["OpenRouter:ApiKey"];
    options.Model = builder.Configuration["OpenRouter:Model"];
    options.Endpoint = builder.Configuration["OpenRouter:Endpoint"];
});

builder.Services.AddHttpClient();

builder.Services.AddScoped<IPipeline, Pipeline>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, SaveResumeFileStep>(provider =>
{
    var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads"); // This line is the key
    var logger = provider.GetRequiredService<ILogger<SaveResumeFileStep>>();
    return new SaveResumeFileStep(logger, uploadDirectory);
});
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, ExtractResumeTextStep>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, ExtractResumeSkillStep>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, ExtractJobDescriptionSkillsStep>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, MatchSkillsStep>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, GenerateSummaryStep>();
builder.Services.AddScoped<IPipelineStep<PipelineContext, PipelineResult>, CalculateScoreStep>();

builder.Services.AddScoped<IAnalysisService, AnalysisService>(); // Register AnalysisService
builder.Services.AddScoped<IFileProcessorFactory, FileProcessorFactory>();
builder.Services.AddScoped<IResumeAnalysisService, ResumeAnalysisService>();
builder.Services.AddScoped<IAIService, OpenRouterAIService>();
builder.Services.AddScoped<IResumeRepository, ResumeRepository>(); // Register ResumeRepository
builder.Services.AddScoped<IResumeContext, ResumeContext>(); // Register ResumeContext


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
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
