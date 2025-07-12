using ResuMatch.Api.Models;
using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Pipelines;

namespace ResuMatch.Api.Services.Concretes
{
    public class ResumeAnalysisService : IResumeAnalysisService
    {
        private readonly IPipeline _pipeline;
        private readonly ILogger<ResumeAnalysisService>? _logger;

        public ResumeAnalysisService(IPipeline pipeline, ILogger<ResumeAnalysisService>? logger)
        {
            _pipeline = pipeline;
            _logger = logger;
        }

        public async Task<AnalysisResult> ProcessResumeAsync(IFormFile file, string jobDescription)
        {
           try
           {
             var context = new PipelineContext
             {
                 File = file,
                 JobDescription = jobDescription,
                 AnalysisResult = new AnalysisResult() 
             };
 
             var pipelineResult = await _pipeline.ExecuteAsync(context);
             context.AnalysisResult = pipelineResult.AnalysisResult;
 
             if (context == null)
             {
                 _logger.LogError("Pipeline execution returned null context.");
                 throw new InvalidOperationException("Pipeline execution returned null context.");
             }
 
             if (context.Error != null)
             {
                 _logger.LogError("Error processing resume: {ErrorMessage}", context.Error);
                
                 return new AnalysisResult { Error = context.Error };
             }
             _logger.LogInformation("Resume processed successfully.");
             return context.AnalysisResult ?? new AnalysisResult();
           }
           catch (Exception ex)
           {
            _logger?.LogError(ex, "An error occurred while processing the resume.");
            throw;
           }
        }
    }
}