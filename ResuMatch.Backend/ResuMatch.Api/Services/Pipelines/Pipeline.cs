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

    public async Task<PipelineResult> ExecuteAsync(PipelineContext context)
    {
        PipelineResult result = new PipelineResult(); 
        foreach (var step in _steps)
        {
            result = await step.ProcessAsync(context);
            if (result == null)
            {
                _logger.LogError("Pipeline execution failed at step: {Step}", step.GetType().Name);
                throw new InvalidOperationException("Pipeline execution failed.");
            }
        }

        return result;
    }
}