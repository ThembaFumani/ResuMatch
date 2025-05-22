using ResuMatch.Pipelines;

public class Pipeline : IPipeline
{
    private readonly IEnumerable<IPipelineStep<PipelineContext, PipelineResult>> _steps;
    private readonly ILogger<Pipeline> _logger;
    public Pipeline(IEnumerable<IPipelineStep<PipelineContext, PipelineResult>> steps, ILogger<Pipeline> logger)
    {
        _steps = steps;
        _logger = logger;
    }

    public void AddStep(IPipelineStep<PipelineContext, PipelineResult> step)
    {
        _steps.Append(step);
    }

    public async Task<PipelineResult> ExecuteAsync(PipelineContext context)
    {
        PipelineResult result = null;
        foreach (var step in _steps)
        {
            result = await step.ProcessAsync(context);
            if (result == null)
            {
                _logger.LogError("Pipeline execution failed at step: {Step}", step.GetType().Name);
                throw new InvalidOperationException("Pipeline execution failed.");
            }
            // Optionally update context if needed for next step
            //context = result.AnalysisResult != null ? new PipelineContext { AnalysisResult = result.AnalysisResult } : context;
        }

        return result;
    }
}