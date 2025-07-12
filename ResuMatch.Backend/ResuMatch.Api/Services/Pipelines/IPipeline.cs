using System.Threading.Tasks;

namespace ResuMatch.Pipelines
{
    public interface IPipeline
    {
        Task<PipelineResult> ExecuteAsync(PipelineContext context);
    }
}