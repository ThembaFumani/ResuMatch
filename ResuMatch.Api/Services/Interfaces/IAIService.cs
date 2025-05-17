using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface IAIService
    {
        Task<string> ExtractSkillsAsync(string jobDescription);
    }
}