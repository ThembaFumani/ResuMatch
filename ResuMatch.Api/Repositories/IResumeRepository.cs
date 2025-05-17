using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Repositories
{
    public interface IResumeRepository
    {
        Task StoreAnalysisResult(AnalysisRequest request, AnalysisResponse result, string filePath);
        Task<string> GetSkillsFromOpenRouter(string jobDescription);
    }
}