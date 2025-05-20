using ResuMatch.Pipelines;

public class CalculateScoreStep : IPipelineStep<PipelineContext, PipelineResult>
{
    private readonly ILogger<CalculateScoreStep> _logger;

    public CalculateScoreStep(ILogger<CalculateScoreStep> logger)
    {
        _logger = logger;
    }    
    Task<PipelineResult> IPipelineStep<PipelineContext, PipelineResult>.ProcessAsync(PipelineContext context)
    {
        if (context.JobDescriptionSkills == null || context.JobDescriptionSkills.Count == 0)
        {
            context.AnalysisResult.MatchScore = 0;
            _logger.LogInformation("Job Description Skills is null or empty. Score set to 0.");
            return Task.FromResult(new PipelineResult { AnalysisResult = context.AnalysisResult });
        }

        context.AnalysisResult.MatchScore = (int)((double)(context.MatchingSkills?.Count ?? 0) / context.JobDescriptionSkills.Count * 100);
        _logger.LogInformation("Score Calculated.");
        return Task.FromResult(new PipelineResult { AnalysisResult = context.AnalysisResult });
    }
}
