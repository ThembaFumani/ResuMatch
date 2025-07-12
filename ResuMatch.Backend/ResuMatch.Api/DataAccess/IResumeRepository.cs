using ResuMatch.Api.Models;
using ResuMatch.Api.Models.ProTailorModels;

namespace ResuMatch.Api.DataAccess
{
    public interface IResumeRepository
    {
        Task<ResumeData> PersistData(IFormFile file, string resumeId, string extractedResumeText, string jobDescription, AnalysisResult analysisResult);
        Task<ResumeData?> GetResumeDataByIdAsync(string resumeId);
        Task<bool> SaveResumeDetailModelAsync(string resumeId, ResumeDetailModel resumeDetailModel);
        Task<string> CacheAsync(string key, string suggestionsResponse);
        Task<string> GetCachedSuggestionResponseAsync(string key);
    }
}