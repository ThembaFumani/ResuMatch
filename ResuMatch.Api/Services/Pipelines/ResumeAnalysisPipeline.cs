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

    public async Task<ResumeAnalysisContext> ExecuteAsync(ResumeAnalysisContext context)
    {
        foreach (var step in _steps)
        {
            context = await step.ProcessAsync(context);
            if (context == null)
            {
                _logger.LogError("Pipeline execution failed at step: {Step}", step.GetType().Name);
                throw new InvalidOperationException("Pipeline execution failed.");
            }
        }

        return new ResumeAnalysisContext 
        {
            AnalysisResult = context.AnalysisResult
        };
    }
}