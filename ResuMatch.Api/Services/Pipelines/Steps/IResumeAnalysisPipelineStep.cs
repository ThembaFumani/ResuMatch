using ResuMatch.Pipelines;

public interface IResumeAnalysisPipelineStep<TContext, TResult>
{
    Task<TResult> ProcessAsync(TContext context);
} 