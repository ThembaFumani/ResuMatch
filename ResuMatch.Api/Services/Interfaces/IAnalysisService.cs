using ResuMatch.Api.Models;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface IAnalysisService
    {
        Task StoreAnalysisResultAsync(AnalysisRequest request, AnalysisResult result, string filePath);
    }
}