namespace ResuMatch.Api.Models
{
    public class AnalysisResult
    {
        public int MatchScore { get; set; }
        public List<string>? MatchingSkills { get; set; }
        public List<string>? MissingSkills { get; set; }
        public string? Summary { get; set; }
        public List<string>? ExtractedResumeSkills { get; set; }
        public List<string>? ExtractedJobDescriptionSkills { get; set; }
        public string? Error { get; internal set; }
        public decimal ResumeOverallExperience { get; set; }
        public decimal JobDescriptionOverallExperience { get; set; }
    }
}