using ResuMatch.Api.Models;

namespace ResuMatch.Api.Repositories
{
    public interface IResumeRepository
    {
        Task StoreAnalysisResult(AnalysisRequest request, AnalysisResult result, string filePath);
        Task<string> GetSkillsFromOpenRouter(string jobDescription);
    }
}