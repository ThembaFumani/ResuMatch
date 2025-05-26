// ResuMatch.Api/Services/Concretes/PromptService.cs
using ResuMatch.Api.Models;
using ResuMatch.Api.Services.Interfaces;
using Microsoft.Extensions.Logging; // Ensure this is present
using System;
using System.Linq; // Ensure this is present for string.Join
using System.Threading.Tasks;

namespace ResuMatch.Api.Services.Concretes
{
    public class PromptService : IPromptService
    {
        private readonly ILogger<PromptService>? _logger;
        private readonly IPromptFileService? _promptFileService;
        private PromptFileModel? _promptFileModel;

        public PromptService(ILogger<PromptService>? logger, IPromptFileService? promptFileService)
        {
            _logger = logger;
            _promptFileService = promptFileService;
        }

        public async Task<string> GetExtractSkillsPrompt(string text)
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(_promptFileModel?.ExtractSkills))
            {
                throw new InvalidOperationException("ExtractSkills prompt is not defined in the prompt file.");
            }
            return string.Format(_promptFileModel.ExtractSkills, text);
        }

        public async Task<string> GetExtractSummaryPrompt(string[] details) // FIXED: Changed to string[]
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(_promptFileModel?.ExtractSummary))
            {
                throw new InvalidOperationException("ExtractSummary prompt is not defined in the prompt file.");
            }
            return string.Format(_promptFileModel.ExtractSummary, string.Join("\n", details)); // FIXED: Added string.Join here
        }

        public async Task<string> GetMatchingSkillsAnalysisPrompt(List<string> resumeSkills, List<string> jobDescriptionSkills)
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(_promptFileModel?.GetMatchingSkillsAnalysis))
            {
                throw new InvalidOperationException("GetMatchingSkillsAnalysis prompt is not defined in the prompt file.");
            }
            
            return string.Format(_promptFileModel.GetMatchingSkillsAnalysis,
                                 string.Join(", ", resumeSkills),
                                 string.Join(", ", jobDescriptionSkills));
        }

        private async Task EnsurePromptsFileLoadedAsync()
        {
            if (_promptFileModel == null)
            {
                try
                {
                    _promptFileModel = await _promptFileService!.LoadPromptFileAsync(); // Consistent method name
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to load prompt file.");
                    throw;
                }
            }
        }
    }
}
