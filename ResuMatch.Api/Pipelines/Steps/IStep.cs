using ResuMatch.Pipelines;

public interface IStep
{
    Task<ResumeAnalysisContext> ExecuteAsync(ResumeAnalysisContext context);
}