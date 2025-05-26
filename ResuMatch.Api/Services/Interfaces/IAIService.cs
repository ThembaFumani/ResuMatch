using System.Text.Json;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface IAIService
    {
        Task<JsonDocument> ExtractSkillsAsync(string jobDescription);
        Task<string> GenerateSummaryAsync(string[] extractedSkills);
        Task<string> GetMatchingSkillsAnalysisAsync(List<string> resumeSkills, List<string> jobDescriptionSkills);
    }
}