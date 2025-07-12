using System.Text.Json.Serialization;

namespace ResuMatch.Api.Models.ProTailorModels
{
    public class SuggestionsResponse
    {
        [JsonPropertyName("GeneralTips")]
        public List<SectionSuggestion> GeneralTips { get; set; } = new List<SectionSuggestion>();

        [JsonPropertyName("KeywordsToAdd")]
        public List<string> KeywordsToAdd { get; set; } = new List<string>();

        [JsonPropertyName("ActionVerbsToConsider")]
        public List<string> ActionVerbsToConsider { get; set; } = new List<string>();

        [JsonPropertyName("SummarySuggestions")]
        public List<SectionSuggestion> SummarySuggestions { get; set; } = new List<SectionSuggestion>();

        [JsonPropertyName("ExperienceSuggestions")]
        public List<ExperienceBulletSuggestion> ExperienceSuggestions { get; set; } = new List<ExperienceBulletSuggestion>();

        [JsonPropertyName("SkillSectionSuggestions")]
        public List<SectionSuggestion> SkillSectionSuggestions { get; set; } = new List<SectionSuggestion>();

        [JsonPropertyName("MissingSkillSuggestions")]
        public List<SectionSuggestion> MissingSkillSuggestions { get; set; } = new List<SectionSuggestion>();

        [JsonPropertyName("OverallExplanation")]
        public string OverallExplanation { get; set; } = string.Empty;
    }
}