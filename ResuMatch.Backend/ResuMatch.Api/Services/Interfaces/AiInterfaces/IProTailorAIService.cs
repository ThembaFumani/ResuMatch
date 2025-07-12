using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResuMatch.Api.Models;
using ResuMatch.Api.Models.ProTailorModels;

namespace ResuMatch.Api.Services.Interfaces.AiInterfaces
{
    public interface IProTailorAIService
    {
        Task<ResumeDetailModel> ExtractStructuredResumeDataAsync(string combinedInputForAITailoring);
        Task<string> TailorResumeSectionAsync(string resumeSectionText, string jobDescriptionText, string sectionType);
        Task<SuggestionsResponse> GenerateTailoringSuggestionsAsync(AnalysisResult analysisResult, string jobDescriptionText, string rawResumeText);

    }
}