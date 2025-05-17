using System.Collections.Generic;

namespace ResuMatch.Pipelines
{
    public class ResumeAnalysisContext
    {
        public string? ResumeText { get; set; }
        public string? JobDescriptionText { get; set; }
        public List<string> ExtractedResumeSkills { get; set; } = new List<string>();
        public List<string> ExtractedJobDescriptionSkills { get; set; } = new List<string>();
        public List<string> MatchingSkills { get; set; } = new List<string>();
        public List<string> MissingSkills { get; set; } = new List<string>();
        public string? Summary { get; set; }
        public int MatchScore { get; set; }
    }
}