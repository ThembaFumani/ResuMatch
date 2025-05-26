using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models
{
    public class PromptFileModel
    {
        public string? ExtractSkills { get; set; }
        public string? ExtractSummary { get; set; }
        public string? GetMatchingSkillsAnalysis { get; set; }
    }
}