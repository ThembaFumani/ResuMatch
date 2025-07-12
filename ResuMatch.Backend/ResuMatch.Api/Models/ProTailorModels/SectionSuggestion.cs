using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models.ProTailorModels
{
    public class SectionSuggestion
    {
        [JsonPropertyName("targetSectionPath")]
        public string TargetSectionPath { get; set; } = string.Empty;

        [JsonPropertyName("originalContentSnippet")]
        public string OriginalContentSnippet { get; set; } = string.Empty;

        [JsonPropertyName("suggestion")]
        public string Suggestion { get; set; } = string.Empty;

        [JsonPropertyName("suggestionType")]
        public string SuggestionType { get; set; } = string.Empty;

        [JsonPropertyName("priority")]
        public string Priority { get; set; } = "Medium";
    }
}