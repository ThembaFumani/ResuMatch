using ResuMatch.Api.Models;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface IResumeAnalysisService
    {
        Task<AnalysisResult> ProcessResumeAsync(IFormFile file, string jobDescription);
    }
}