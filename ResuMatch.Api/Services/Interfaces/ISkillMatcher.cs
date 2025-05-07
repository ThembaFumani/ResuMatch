using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface ISkillMatcher
    {
        Task<AnalysisResponse> MatchSkills(List<string> resumeSkills, List<string> jobDescriptionSkills);

    }
}