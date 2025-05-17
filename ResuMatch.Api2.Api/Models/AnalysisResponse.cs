using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models
{
    public class AnalysisResponse
    {
        public int MatchScore { get; set; }
        public List<string>? MatchingSkills { get; set; }
        public List<string>? MissingSkills { get; set; }
    }
}