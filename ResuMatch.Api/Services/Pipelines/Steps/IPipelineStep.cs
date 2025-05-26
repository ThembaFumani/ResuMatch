public interface IPipelineStep<TContext, TResult>
{
    Task<TResult> ProcessAsync(TContext context);
} 