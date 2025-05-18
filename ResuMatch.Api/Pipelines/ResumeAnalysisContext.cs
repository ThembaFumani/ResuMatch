using System.Collections.Generic;
using ResuMatch.Api.Models;

namespace ResuMatch.Pipelines
{
    public class ResumeAnalysisContext
    {
        public IFormFile? File { get; set; }
        public string? FilePath { get; set; }
        public string? ExtractedResumeText { get; set; }
        public string? JobDescription { get; set; }
        public List<string>? ResumeSkills { get; set; }
        public List<string>? JobDescriptionSkills { get; set; }
        public AnalysisResult? AnalysisResult { get; set; }
    }

    public class ResumeAnalysisPipelineResult
    {
        public AnalysisResult? AnalysisResult { get; set; }
    }
}