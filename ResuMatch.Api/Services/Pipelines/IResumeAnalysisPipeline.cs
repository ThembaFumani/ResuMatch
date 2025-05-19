using System.Threading.Tasks;

namespace ResuMatch.Pipelines
{
    public interface IResumeAnalysisPipeline
    {
        Task<ResumeAnalysisContext> ExecuteAsync(ResumeAnalysisContext context);
    }
}