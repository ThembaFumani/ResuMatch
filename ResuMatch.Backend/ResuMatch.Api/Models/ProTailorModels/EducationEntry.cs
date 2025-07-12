using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models.ProTailorModels
{
    public class EducationEntry
    {
        [JsonPropertyName("Degree")]
        public string? Degree { get; set; }
        [JsonPropertyName("FieldOfStudy")]
        public string? FieldOfStudy { get; set; }
        [JsonPropertyName("Institution")]
        public string? Institution { get; set; }
        [JsonPropertyName("Location")]
        public string? Location { get; set; }
        [JsonPropertyName("StartDate")]
        public string? StartDate { get; set; }
        [JsonPropertyName("EndDate")]
        public string? EndDate { get; set; }
    }
}