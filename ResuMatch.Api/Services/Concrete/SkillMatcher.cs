using ResuMatch.Api.Models;
using ResuMatch.Api.Services.Interfaces;

namespace ResuMatch.Api.Services.Concretes
{
    public class SkillMatcher : ISkillMatcher
    {
        private readonly ILogger<SkillMatcher> _logger;
        public SkillMatcher(ILogger<SkillMatcher> logger)
        {
            _logger = logger;     
        }

        public AnalysisResult MatchSkills(List<string> resumeSkills, List<string> jobDescriptionSkills)
        {
             _logger.LogInformation("Matching skills...");
            var matchingSkills = resumeSkills.Intersect(jobDescriptionSkills).ToList();
            var missingSkills = jobDescriptionSkills.Except(resumeSkills).ToList();
            int matchScore = (int)((double)matchingSkills.Count / jobDescriptionSkills.Count * 100);

            return new AnalysisResult
            {
                MatchScore = matchScore,
                MatchingSkills = matchingSkills,
                MissingSkills = missingSkills
            };
        }
    }
}