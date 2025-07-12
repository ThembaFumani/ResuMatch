using System.Text.Json;
using System.Text.Json.Serialization;
using ResuMatch.Api.Models;
using ResuMatch.Api.Models.ProTailorModels;
using ResuMatch.Api.Services.Interfaces;

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

        public async Task<string> GetExtractResumeStructurePrompt(string combinedInputForAITailoring)
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(_promptFileModel?.ExtractResumeStructure))
            {
                throw new InvalidOperationException("ExtractResumeStructure prompt is not defined in the prompts.json for ProTailor.");
            }
            string rawPromptTemplate = PromptConstants.ExtractResumeStructure;//_promptFileModel.ExtractResumeStructure;
            string resumeDetailModelSchema = JsonSerializer.Serialize(new ResumeDetailModel
            {
                Name = string.Empty,
                Contact = new ContactInfo(),
                Summary = string.Empty,
                Experience = new List<ExperienceEntry>(),
                Education = new List<EducationEntry>()
            }, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Converters = { new JsonStringEnumConverter() }
            }).Replace("{", "{{").Replace("}", "}}");
            return string.Format(
                rawPromptTemplate.Replace("**<YOUR_RESUME_MODEL_JSON_SCHEMA_HERE>**", resumeDetailModelSchema),
                combinedInputForAITailoring
            );
        }

        public async Task<string> GetExtractSkillsPrompt(string text)
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(PromptConstants.ExtractSkills))
            {
                throw new InvalidOperationException("ExtractSkills prompt is not defined in the prompt file.");
            }
            return string.Format(PromptConstants.ExtractSkills, text);
        }

        public async Task<string> GetExtractSummaryPrompt(
            List<string> matchingSkills,
            List<string> missingSkills,
            decimal resumeExperience,
            decimal jobDesrcitionExperience)
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(PromptConstants.ExtractSummary))
            {
                throw new InvalidOperationException("ExtractSummary prompt is not defined in the prompt file.");
            }

            // Prepare the skill analysis JSON for the prompt
            var skillAnalysis = $"{{ \"matching_skills\": [{string.Join(", ", matchingSkills.Select(s => $"\"{s}\""))}], \"missing_skills\": [{string.Join(", ", missingSkills.Select(s => $"\"{s}\""))}] }}";

            return string.Format(
                PromptConstants.ExtractSummary,
                skillAnalysis,
                resumeExperience,
                jobDesrcitionExperience
            );
        }

        public async Task<string> GetGenerateTailoringSuggestionsPrompt(string analysisResultJson, string jobDescriptionText, string rawResumeText)
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(PromptConstants.GenerateTailoringSuggestions))
            {
                throw new InvalidOperationException("GenerateTailoringSuggestions prompt is not defined in the prompts.json for ProTailor.");
            }
            string rawPromptTemplate = PromptConstants.GenerateTailoringSuggestions;
            string suggestionsResponseSchema = JsonSerializer.Serialize(new SuggestionsResponse(), new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Converters = { new JsonStringEnumConverter() }
            }).Replace("{", "{{").Replace("}", "}}");
            string sectionSuggestionSchema = JsonSerializer.Serialize(new SectionSuggestion(), new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Converters = { new JsonStringEnumConverter() }
            }).Replace("{", "{{").Replace("}", "}}");
            string experienceBulletSuggestionSchema = JsonSerializer.Serialize(new ExperienceBulletSuggestion(), new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Converters = { new JsonStringEnumConverter() }
            }).Replace("{", "{{").Replace("}", "}}");
            return string.Format(
                rawPromptTemplate
                    .Replace("**<YOUR_SUGGESTIONS_RESPONSE_JSON_SCHEMA_HERE>**", suggestionsResponseSchema)
                    .Replace("**<YOUR_SECTION_SUGGESTION_JSON_SCHEMA_HERE>**", sectionSuggestionSchema)
                    .Replace("**<YOUR_EXPERIENCE_BULLET_SUGGESTION_JSON_SCHEMA_HERE>**", experienceBulletSuggestionSchema),
                analysisResultJson,
                jobDescriptionText,
                rawResumeText
            );
        }

        public async Task<string> GetMatchingSkillsAnalysisPrompt(List<string> matchingSkills, List<string> missingSkills)
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(PromptConstants.GetMatchingSkillsAnalysis))
            {
                throw new InvalidOperationException("GetMatchingSkillsAnalysis prompt is not defined in the prompt file.");
            }

            var matchingSkillsFormatted = string.Join(", ", matchingSkills.Select(s => $"\"{s}\""));
            var missingSkillsFormatted = string.Join(", ", missingSkills.Select(s => $"\"{s}\""));
            var prompt = string.Format(PromptConstants.GenerateTailoringSuggestions, matchingSkillsFormatted, missingSkillsFormatted);
            return prompt;
        }

        public async Task<string> GetOverallExperienceAnalysisPrompt(string resumeText, string jobDescriptionText)
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(PromptConstants.GetOverallExperienceAnalysis))
            {
                throw new InvalidOperationException("GetOverallExperienceAnalysis prompt is not defined in the prompt file.");
            }
            var result = string.Format(PromptConstants.GetOverallExperienceAnalysis, resumeText, jobDescriptionText);

            return result;
        }

        public async Task<string> GetTailoringSuggestionsJsonRepairPrompt(string malformedJson)
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(_promptFileModel?.TailoringMalformedJsonRepair))
            {
                throw new InvalidOperationException("TailoringSuggestionsJsonRepair prompt is not defined in the prompts.json for ProTailor.");
            }
            var prompt = _promptFileModel.TailoringMalformedJsonRepair;
            
            return string.Format(
                prompt,
                GetSuggestionResponseModelDummySchema(),
                malformedJson
            );
        }
        
        private string GetSuggestionResponseModelDummySchema()
        {
            return JsonSerializer.Serialize(new SuggestionsResponse
            {
                OverallExplanation = string.Empty,
                SkillSectionSuggestions = new List<SectionSuggestion>(),
                ExperienceSuggestions = new List<ExperienceBulletSuggestion>(),
                GeneralTips = new List<SectionSuggestion>(),
                ActionVerbsToConsider = new List<string>(),
                SummarySuggestions = new List<SectionSuggestion>(),
                KeywordsToAdd = new List<string>(),
                MissingSkillSuggestions = new List<SectionSuggestion>()
            }, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Converters = { new JsonStringEnumConverter() }
            }).Replace("{", "{{").Replace("}", "}}"); 
        }

        public async Task<string> GetTailoringResumeDetailJsonRepairPrompt(string malformedJson)
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(PromptConstants.TailoringMalformedJsonRepair))
            {
                throw new InvalidOperationException("TailoringSuggestionsJsonRepair prompt is not defined in the prompts.json for ProTailor.");
            }
            //
            var prompt = PromptConstants.TailoringMalformedJsonRepair;
            
            return string.Format(
                prompt,
                GetResumeDetailsModelDummySchema(),
                malformedJson
            );
        }

        private string GetResumeDetailsModelDummySchema()
        {
            return JsonSerializer.Serialize(new ResumeDetailModel
            {
                Name = string.Empty,
                Contact = new ContactInfo(),
                Summary = string.Empty,
                Experience = new List<ExperienceEntry>(),
                Education = new List<EducationEntry>(),
                Skills = new List<SkillCategory>(),
                Projects = new List<ProjectEntry>(),
                Certifications = new List<string>(),
                Awards = new List<string>(),
            }, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Converters = { new JsonStringEnumConverter() }
            }).Replace("{", "{{").Replace("}", "}}");
        }

        public async Task<string> GetTailorResumeSectionPrompt(string resumeSectionText, string jobDescriptionText, string sectionType)
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(PromptConstants.GetTailorResumeSection))

            {
                throw new InvalidOperationException("GetTailorResumeSection prompt is not defined in the prompts.json for ProTailor.");
            }
            return string.Format(
                PromptConstants.GetTailorResumeSection,
                resumeSectionText,
                jobDescriptionText,
                sectionType
            );
        }

        private async Task EnsurePromptsFileLoadedAsync()
        {
            if (_promptFileModel == null)
            {
                try
                {
                    _promptFileModel = await _promptFileService!.LoadPromptFileAsync();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to load prompt file.");
                    throw;
                }
            }
        }

        public async Task<string> GetExtractResumeExperiencePrompt(string resumeText)
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(PromptConstants.ExtractResumeExperience))
            {
                throw new InvalidOperationException("ExtractResumeExperience prompt is not defined in the prompts.json for ProTailor.");
            }

            return string.Format(PromptConstants.ExtractResumeExperience, resumeText);
        }

        public async Task<string> GetExtractJDExperiencePrompt(string jobDescriptionText)
        {
            await EnsurePromptsFileLoadedAsync();
            if (string.IsNullOrEmpty(PromptConstants.ExtractJDExperience))
            {
                throw new InvalidOperationException("ExtractResumeExperience prompt is not defined in the prompts.json for ProTailor.");
            }

            return string.Format(PromptConstants.ExtractJDExperience, jobDescriptionText);
        }
    }
}
