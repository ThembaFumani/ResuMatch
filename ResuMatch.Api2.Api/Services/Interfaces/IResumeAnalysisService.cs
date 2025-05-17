using ResuMatch.Api.Models;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface IResumeAnalysisService
    {
        Task<AnalysisResponse> ProcessResumeAsync(IFormFile file, string jobDescription);
    }
}