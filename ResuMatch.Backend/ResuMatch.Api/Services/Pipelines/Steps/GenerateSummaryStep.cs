using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Pipelines;

public class GenerateSummaryStep : IPipelineStep<PipelineContext, PipelineResult>
{
    private readonly IAIService _aiService;
    private readonly ILogger<GenerateSummaryStep> _logger;

    public GenerateSummaryStep(IAIService aiService, ILogger<GenerateSummaryStep> logger)
    {
        _logger = logger;
        _aiService = aiService;
    }

    public async Task<PipelineResult> ProcessAsync(PipelineContext context)
    {
        // if (context.MissingSkills == null || context.MissingSkills.Count == 0)
        // {
        //     context.Error = "No missing skills found.";
        //     return new PipelineResult { AnalysisResult = context.AnalysisResult };
        // }

        var summary = await _aiService.GenerateSummaryAsync
        (context.MatchingSkills,
         context.MissingSkills,
         context.AnalysisResult.ResumeOverallExperience,
         context.AnalysisResult.JobDescriptionOverallExperience);
        if (context.AnalysisResult != null)
        {
            context.AnalysisResult.Summary = summary ?? string.Empty;
        }
        return new PipelineResult { AnalysisResult = context.AnalysisResult };
    }
}