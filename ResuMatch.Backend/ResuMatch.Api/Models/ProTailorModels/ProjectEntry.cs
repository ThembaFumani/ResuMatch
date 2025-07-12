using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models.ProTailorModels
{
    public class ProjectEntry
    {
        [JsonPropertyName("ProjectName")]
        public string? ProjectName { get; set; }
        [JsonPropertyName("Type")]
        public string? Type { get; set; }
        [JsonPropertyName("Location")]
        public string? Location { get; set; }
        [JsonPropertyName("StartDate")]
        public string? StartDate { get; set; }
        [JsonPropertyName("EndDate")]
        public string? EndDate { get; set; }
        [JsonPropertyName("Role")]
        public string? Role { get; set; }
        [JsonPropertyName("KeyContributions")]
        public List<string>? KeyContributions { get; set; }
        [JsonPropertyName("TechnologiesUsed")]
        public List<string>? TechnologiesUsed { get; set; }
    }
}