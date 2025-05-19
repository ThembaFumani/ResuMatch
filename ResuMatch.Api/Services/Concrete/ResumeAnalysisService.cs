using ResuMatch.Api.Models;
using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Pipelines;

namespace ResuMatch.Api.Services.Concretes
{
    public class ResumeAnalysisService : IResumeAnalysisService
    {
        private readonly IResumeAnalysisPipeline _pipeline;
        private readonly ILogger<ResumeAnalysisService>? _logger;

        public ResumeAnalysisService(IResumeAnalysisPipeline pipeline, ILogger<ResumeAnalysisService>? logger)
        {
            _pipeline = pipeline;
            _logger = logger;
        }

        public async Task<AnalysisResult> ProcessResumeAsync(IFormFile file, string jobDescription)
        {
            var context = new ResumeAnalysisContext
            {
                File = file,
                JobDescription = jobDescription
            };

            context = await _pipeline.ExecuteAsync(context);

            if (context == null)
            {
                _logger.LogError("Pipeline execution returned null context.");
                throw new InvalidOperationException("Pipeline execution returned null context.");
            }

            if (context.Error != null)
            {
                _logger.LogError("Error processing resume: {ErrorMessage}", context.Error);
                // Consider throwing an exception here or returning a specific error result
                return new AnalysisResult { Error = context.Error }; // Or throw new Exception(context.Error);
            }
            _logger.LogInformation("Resume processed successfully.");
            return context.AnalysisResult;
        }
    }
}