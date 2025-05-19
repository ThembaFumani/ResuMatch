using System.Threading.Tasks;

namespace ResuMatch.Pipelines
{
    public interface IResumeAnalysisPipeline
    {
        Task<ResumeAnalysisPipelineResult> ExecuteAsync(ResumeAnalysisContext context);
    }
}