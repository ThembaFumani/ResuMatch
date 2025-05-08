using ResuMatch.Api.Models;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface ISkillMatcher
    {
     AnalysisResponse MatchSkills(List<string> resumeSkills, List<string> jobDescriptionSkills);
    }
}