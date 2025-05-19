

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
 // Make sure you have this class

 // Register the factory
//builder.Services.AddScoped<ISkillMatcher, SkillMatcher>(); // Register SkillMatcher
builder.Services.AddScoped<IAnalysisService, AnalysisService>(); // Register AnalysisService

//builder.Services.AddScoped<IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisPipelineResult>, SaveResumeFileStep>();
builder.Services.AddScoped<IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisPipelineResult>, ExtractResumeTextStep>();
builder.Services.AddScoped<IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisPipelineResult>, ExtractResumeSkillStep>();
builder.Services.AddScoped<IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisPipelineResult>, ExtractJobDescriptionSkillsStep>();
builder.Services.AddScoped<IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisPipelineResult>, MatchSkillsStep>();
builder.Services.AddScoped<IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisPipelineResult>, GenerateSummaryStep>();
builder.Services.AddScoped<IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisPipelineResult>, CalculateScoreStep>();
builder.Services.AddScoped<IResumeAnalysisPipeline, ResumeAnalysisPipeline>();

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
