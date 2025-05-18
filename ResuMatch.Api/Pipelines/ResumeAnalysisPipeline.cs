using ResuMatch.Pipelines;

public class ResumeAnalysisPipeline : IResumeAnalysisPipeline
{
    private readonly IEnumerable<IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisContext>> _steps;
    private readonly ILogger<ResumeAnalysisPipeline> _logger;
    public ResumeAnalysisPipeline(IEnumerable<IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisContext>> steps, ILogger<ResumeAnalysisPipeline> logger)
    {
        _steps = steps;
        _logger = logger;
    }

    public Task<ResumeAnalysisPipelineResult> ExecuteAsync(ResumeAnalysisContext context)
    {
        throw new NotImplementedException();
    }
}