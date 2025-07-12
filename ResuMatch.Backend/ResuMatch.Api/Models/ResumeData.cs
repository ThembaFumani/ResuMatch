using System.Text.Json.Serialization;

namespace ResuMatch.Api.Models
{
    public class ResumeData
    {
        public string? Id { get; set; }
        public string? FilePath { get; set; }
        public string? ResumeText { get; set; }
        public string? JobDescriptionText { get; set; }
        [JsonPropertyName("analysisResults")]
        public AnalysisResult? AnalysisResult { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ResumeDataId { get; set; }
    }
}