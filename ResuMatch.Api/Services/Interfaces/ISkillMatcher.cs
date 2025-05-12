using ResuMatch.Api.Models;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface ISkillMatcher
    {
     AnalysisResult MatchSkills(List<string> resumeSkills, List<string> jobDescriptionSkills);
    }
}