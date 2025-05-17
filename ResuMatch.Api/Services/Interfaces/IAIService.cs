using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface IAIService
    {
        Task<JsonDocument> ExtractSkillsAsync(string jobDescription);
        Task<JsonDocument> ExtractSummaryAsync(string[] extractedSkills);
    }
}