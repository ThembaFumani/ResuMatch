using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models.ProTailorModels
{
    public class ExperienceEntry
    {
        [JsonPropertyName("JobTitle")]
        public string? JobTitle { get; set; }
        [JsonPropertyName("Company")]
        public string? Company { get; set; }
        [JsonPropertyName("Location")]
        public string? Location { get; set; }
        [JsonPropertyName("StartDate")]
        public string? StartDate { get; set; }
        [JsonPropertyName("EndDate")]
        public string? EndDate { get; set; }
        [JsonPropertyName("ResponsibilitiesAndAchievements")]
        public List<string>? ResponsibilitiesAndAchievements { get; set; }
        [JsonPropertyName("TechnologiesUsed")]
        public List<string>? TechnologiesUsed { get; set; }
    }
}