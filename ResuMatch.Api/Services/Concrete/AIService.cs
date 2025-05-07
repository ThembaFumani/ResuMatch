using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ResuMatch.Api.Models.Configurations;
using ResuMatch.Api.Repositories;
using ResuMatch.Api.Services.Interfaces;

namespace ResuMatch.Api.Services.Concrete
{
    public class AIService : IAIService
    {
         private readonly ILogger<AIService> _logger;
        private readonly IResumeRepository _resumeRepository;

        public AIService(ILogger<AIService> logger, IResumeRepository resumeRepository)
        {
            _logger = logger;
            _resumeRepository = resumeRepository;
        }

        public async Task<string> ExtractSkillsAsync(string jobDescription)
        {
            _logger.LogInformation("Extracting skills using OpenRouter...");
            string responseContent = await _resumeRepository.GetSkillsFromOpenRouter(jobDescription);
            AIResponse openRouterResponse = JsonSerializer.Deserialize<AIResponse>(responseContent);
            string extractedSkills = openRouterResponse?.Choices?[0]?.Message?.Content ?? "No skills found";
            return extractedSkills;
        }
    }
}