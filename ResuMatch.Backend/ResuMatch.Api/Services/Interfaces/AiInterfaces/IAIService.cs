using System.Text.Json;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface IAIService
    {
        Task<JsonDocument> ExtractSkillsAsync(string jobDescription);
        Task<string> GenerateSummaryAsync(List<string> matchingSkills, List<string> missingSkills, decimal resumeExperience, decimal jobDesrcitionExperience);
        Task<string> GetMatchingSkillsAnalysisAsync(List<string> resumeSkills, List<string> jobDescriptionSkills);
        Task<string> GetOverallExperienceAnalysisAsync(string resumeText, string jobDescriptionText);
    }
}