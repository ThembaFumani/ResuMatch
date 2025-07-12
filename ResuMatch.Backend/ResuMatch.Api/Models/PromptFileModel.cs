namespace ResuMatch.Api.Models
{
    public class PromptFileModel
    {
        public string? ExtractSkills { get; set; }
        public string? ExtractSummary { get; set; }
        public string? GetMatchingSkillsAnalysis { get; set; }
        public string? GetOverallExperienceAnalysis { get; set; }
        public string? ExtractResumeStructure { get; set; }
        public string? TailorResumeSection { get; set; }
        public string? GenerateTailoringSuggestions { get; set; }
        public string? TailoringMalformedJsonRepair { get; set; }
        public string? ExtractResumeExperience { get; set; }
        public string? ExtractJDExperience { get; set; }
    }
}