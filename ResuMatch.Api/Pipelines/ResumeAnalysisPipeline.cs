using ResuMatch.Pipelines;

public class ResumeAnalysisPipeline : IResumeAnalysisPipeline
{
    
    public ResumeAnalysisPipeline()
    {
        
    }

    public Task<ResumeAnalysisContext> ExecuteAsync(ResumeAnalysisContext context)
    {
        throw new NotImplementedException();
    }
}