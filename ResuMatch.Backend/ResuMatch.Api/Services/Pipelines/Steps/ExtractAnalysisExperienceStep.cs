using System.Text.Json;
using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Pipelines;

namespace ResuMatch.Api.Services.Pipelines.Steps
{
    public class ExtractAnalysisExperienceStep : IPipelineStep<PipelineContext, PipelineResult>
    {
        private readonly ILogger<ExtractAnalysisExperienceStep> _logger;
        private readonly IAIService _aiService;
        public ExtractAnalysisExperienceStep(ILogger<ExtractAnalysisExperienceStep> logger, IAIService aiService)
        {
            _aiService = aiService;
            _logger = logger;
        }

        public async Task<PipelineResult> ProcessAsync(PipelineContext context)
        {
            if (context == null)
            {
                _logger.LogError("Pipeline context is null.");
                throw new ArgumentNullException(nameof(context), "Pipeline context cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(context.JobDescription) || string.IsNullOrWhiteSpace(context.ExtractedResumeText))
            {
                _logger.LogError("Job description or extracted resume text is empty.");
                context.Error = "Job description or extracted resume text cannot be empty.";
                context.AnalysisResult ??= new ResuMatch.Api.Models.AnalysisResult();
                context.AnalysisResult.Error = context.Error;
                return new PipelineResult { AnalysisResult = context.AnalysisResult };
            }

            // Call the refactored method
            var aiResponse = await _aiService.GetOverallExperienceAnalysisAsync(context.ExtractedResumeText, context.JobDescription);

            // Map the AI response to AnalysisResult
            context.AnalysisResult ??= new Models.AnalysisResult();
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(aiResponse);
                var root = doc.RootElement;

                // Directly get integer values, as per our prompt and GetOverallExperienceAnalysisAsync's goal
                if (root.TryGetProperty("ResumeOverallExperience", out var resumeExpElement) && resumeExpElement.ValueKind == JsonValueKind.Number)
                {
                    context.AnalysisResult.ResumeOverallExperience = resumeExpElement.GetInt32();
                }
                else
                {
                    // Log if the expected integer format isn't received, but default to 0
                    _logger.LogWarning("ResumeOverallExperience not found or not in expected integer format in AI response: {Response}", aiResponse);
                    context.AnalysisResult.ResumeOverallExperience = 0;
                }

                if (root.TryGetProperty("JobDescriptionOverallExperience", out var jdExpElement) && jdExpElement.ValueKind == JsonValueKind.Number)
                {
                    context.AnalysisResult.JobDescriptionOverallExperience = jdExpElement.GetInt32();
                }
                else
                {
                    // Log if the expected integer format isn't received, but default to 0
                    _logger.LogWarning("JobDescriptionOverallExperience not found or not in expected integer format in AI response: {Response}", aiResponse);
                    context.AnalysisResult.JobDescriptionOverallExperience = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse AI experience analysis response: {Response}", aiResponse);
                context.AnalysisResult.Error = "Failed to parse experience analysis.";
            }

            return new PipelineResult
            {
                AnalysisResult = context.AnalysisResult
            };
        }
    }
}