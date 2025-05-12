using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}