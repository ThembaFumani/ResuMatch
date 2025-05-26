namespace ResuMatch.Api.Services.Interfaces
{
    public interface IPromptService
    {
        Task<string> GetExtractSkillsPrompt(string jobDescription);
        Task<string> GetExtractSummaryPrompt(string[] extractedSkills);
        Task<string> GetMatchingSkillsAnalysisPrompt(List<string> resumeSkills, List<string> jobDescriptionSkills);
    }
}