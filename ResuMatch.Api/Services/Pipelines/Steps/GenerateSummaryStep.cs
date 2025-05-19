using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Pipelines;

public class GenerateSummaryStep : IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisPipelineResult>
{
    private readonly IAIService _aiService;
    private readonly ILogger<GenerateSummaryStep> _logger;

    public GenerateSummaryStep(IAIService aiService, ILogger<GenerateSummaryStep> logger)
    {
        _logger = logger;
        _aiService = aiService;
    }

    public async Task<ResumeAnalysisPipelineResult> ProcessAsync(ResumeAnalysisContext context)
    {
        if (context.MissingSkills == null || context.MissingSkills.Count == 0)
        {
            context.Error = "No missing skills found.";
            return new ResumeAnalysisPipelineResult { AnalysisResult = context.AnalysisResult };
        }

        var summary = await _aiService.ExtractSummaryAsync(context.MissingSkills.ToArray());
        var summaryValue = summary.RootElement.GetProperty("summary").GetString();
        context.AnalysisResult.Summary = summaryValue ?? string.Empty;
        return new ResumeAnalysisPipelineResult { AnalysisResult = context.AnalysisResult };
    }
}