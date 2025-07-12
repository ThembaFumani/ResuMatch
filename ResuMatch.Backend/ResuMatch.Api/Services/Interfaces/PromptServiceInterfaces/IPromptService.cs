namespace ResuMatch.Api.Services.Interfaces
{
    public interface IPromptService
    {
        Task<string> GetExtractSkillsPrompt(string jobDescription);
        Task<string> GetExtractSummaryPrompt(List<string> matchingSkills, List<string> missingSkills, decimal resumeExperience, decimal jobDesrcitionExperience);
        Task<string> GetMatchingSkillsAnalysisPrompt(List<string> resumeSkills, List<string> jobDescriptionSkills);
        Task<string> GetOverallExperienceAnalysisPrompt(string resumeText, string jobDescriptionText);
        Task<string> GetExtractResumeStructurePrompt(string combinedInputForAITailoring);
        Task<string> GetTailorResumeSectionPrompt(string resumeSectionText, string jobDescriptionText, string sectionType);
        Task<string> GetGenerateTailoringSuggestionsPrompt(string analysisResultJson, string jobDescriptionText, string rawResumeText);
        Task<string> GetTailoringSuggestionsJsonRepairPrompt(string malformedJson);
        Task<string> GetTailoringResumeDetailJsonRepairPrompt(string malformedJson);
        Task<string> GetExtractResumeExperiencePrompt(string resumeText);
        Task<string> GetExtractJDExperiencePrompt(string jobDescriptionText);
    }
}