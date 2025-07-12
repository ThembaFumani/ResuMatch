using System.Text.Json;
using ResuMatch.Api.DataAccess;
using ResuMatch.Api.Models.ProTailorModels;
using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Api.Services.Interfaces.AiInterfaces;

namespace ResuMatch.Api.Services.Concrete
{
    public class ProTailorService : IProTailorService
    {
        private readonly IProTailorAIService _aiService;
        private readonly IResumeRepository _resumeRepository;
        private readonly ILogger<ProTailorService> _logger;

        public ProTailorService(IProTailorAIService aiService, IResumeRepository resumeRepository, ILogger<ProTailorService> logger)
        {
            _aiService = aiService;
            _resumeRepository = resumeRepository;
            _logger = logger;
        }

        public async Task<ResumeDetailModel> ProcessFullTailoringAsync(string resumeId, string jobDescriptionId)
        {
            _logger.LogInformation("ProTailor Service: Starting full tailoring process for Resume ID: {ResumeId}, JD ID: {JobDescriptionId}", resumeId, jobDescriptionId);

            var resumeData = await _resumeRepository.GetResumeDataByIdAsync(resumeId);
            if (resumeData == null || string.IsNullOrWhiteSpace(resumeData.ResumeText) || string.IsNullOrWhiteSpace(resumeData.JobDescriptionText) || resumeData.AnalysisResult == null)
            {
                _logger.LogError("ProTailor Service: Missing essential data (raw resume/JD text or analysis result) from ResumeData for ID: {ResumeId}. Cannot proceed.", resumeId);
                throw new InvalidOperationException($"Could not fetch complete ResumeData for ID '{resumeId}'. Ensure raw texts and analysis result exist.");
            }

            SuggestionsResponse tailoringSuggestions = await _aiService.GenerateTailoringSuggestionsAsync(
                resumeData.AnalysisResult,
                resumeData.JobDescriptionText,
                resumeData.ResumeText
            );
            
            _logger.LogInformation("ProTailor Service: Tailoring suggestions generated. Explanation: {Explanation}", tailoringSuggestions.OverallExplanation);

            string combinedInputForAITailoring = $"Original Resume Text:\n```\n{resumeData.ResumeText}\n```\n\nTailoring Suggestions (JSON):\n```json\n{JsonSerializer.Serialize(tailoringSuggestions, new JsonSerializerOptions { WriteIndented = true })}\n```";
            ResumeDetailModel tailoredResumeDetail = await _aiService.ExtractStructuredResumeDataAsync(combinedInputForAITailoring);
            _logger.LogInformation("ProTailor Service: Tailored ResumeDetailModel generated.");

            if (tailoredResumeDetail == null)
            {
                _logger.LogError("ProTailor Service: Tailored ResumeDetailModel generation returned null.");
                throw new InvalidOperationException("Tailored ResumeDetailModel generation failed.");
            }

            bool savedSuccessfully = await _resumeRepository.SaveResumeDetailModelAsync(resumeId, tailoredResumeDetail);
            if (!savedSuccessfully)
            {
                _logger.LogError("ProTailor Service: Failed to persist tailored ResumeDetailModel via external API for ID: {ResumeId}", resumeId);
                throw new InvalidOperationException($"Failed to save tailored ResumeDetailModel to external API for ID '{resumeId}'.");
            }
            _logger.LogInformation("ProTailor Service: Tailored ResumeDetailModel persisted successfully for ID: {ResumeId}.", resumeId);

            _logger.LogInformation("ProTailor Service: Full tailoring process completed for Resume ID: {ResumeId}, JD ID: {JobDescriptionId}", resumeId, jobDescriptionId);
            return tailoredResumeDetail;
        }

        public async Task<TailorSectionResponse> TailorResumeSectionAsync(string resumeId, string jobDescriptionId, string sectionType)
        {
            _logger.LogInformation("ProTailor Service: Starting TailorResumeSectionByIdAsync for Resume ID: {ResumeId}, JD ID: {JobDescriptionId}, Section Type: {SectionType}", resumeId, jobDescriptionId, sectionType);

            var resumeData = await _resumeRepository.GetResumeDataByIdAsync(resumeId);
            if (resumeData == null || string.IsNullOrWhiteSpace(resumeData.ResumeText) || string.IsNullOrWhiteSpace(resumeData.JobDescriptionText))
            {
                _logger.LogError("ProTailor Service: Missing raw resume text or job description for ID: {ResumeId}. Cannot tailor section.", resumeId);
                throw new InvalidOperationException($"Resume or Job Description data not found for ID: {resumeId}. Cannot tailor section.");
            }

            string rawResumeText = resumeData.ResumeText;
            string rawJobDescriptionText = resumeData.JobDescriptionText;
            ResumeDetailModel structuredResume = await _aiService.ExtractStructuredResumeDataAsync($"Original Resume Text:\n```\n{rawResumeText}\n```");

            if (structuredResume == null)
            {
                _logger.LogError("ProTailor Service: Failed to generate structured resume from raw text for ID: {ResumeId}.", resumeId);
                throw new InvalidOperationException($"Failed to generate structured resume from raw text for ID: {resumeId}.");
            }

            string resumeSectionToTailor = GetSectionTextFromStructuredResume(structuredResume, sectionType);
            if (string.IsNullOrWhiteSpace(resumeSectionToTailor))
            {
                _logger.LogError("ProTailor Service: Could not find specified section '{SectionType}' in structured resume for ID: {ResumeId}.", sectionType, resumeId);
                throw new InvalidOperationException($"Could not find specified section '{sectionType}' in structured resume for ID: {resumeId}.");
            }

            string tailoredContent = await _aiService.TailorResumeSectionAsync(
                resumeSectionToTailor,
                rawJobDescriptionText,
                sectionType
            );

            if (string.IsNullOrWhiteSpace(tailoredContent))
            {
                _logger.LogWarning("ProTailor Service: AI tailoring returned empty content for section.");
                throw new InvalidOperationException("AI tailoring returned no content for the section.");
            }

            _logger.LogInformation("ProTailor Service: Section tailoring completed for Resume ID: {ResumeId}, Section Type: {SectionType}", resumeId, sectionType);
            return new TailorSectionResponse { TailoredSection = tailoredContent };
        }

        private string GetSectionTextFromStructuredResume(ResumeDetailModel structuredResume, string sectionType)
        {
            switch (sectionType.ToLowerInvariant())
            {
                case "summary":
                    return structuredResume.Summary ?? string.Empty;
                case "experience_bullet":
                    // This is a simplified example. You might want to pass an index in the request
                    // to tailor a specific bullet, e.g., "experience_bullet[0]" for the first.
                    // For now, it takes the first bullet of the first experience entry.
                    if (structuredResume.Experience != null && structuredResume.Experience.Any() &&
                        structuredResume.Experience[0].ResponsibilitiesAndAchievements != null &&
                        structuredResume.Experience[0].ResponsibilitiesAndAchievements.Any())
                    {
                        return structuredResume.Experience[0].ResponsibilitiesAndAchievements[0];
                    }
                    return string.Empty;
                // Add more cases for other sections (e.g., "skills_overall", "education_degree")
                default:
                    return string.Empty;
            }
        }
    }
}