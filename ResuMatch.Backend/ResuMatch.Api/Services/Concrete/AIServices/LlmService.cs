using System.Text.Json;
using ResuMatch.Api.Models.Configurations;
using ResuMatch.Api.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using ResuMatch.Api.Models;
using ResuMatch.Api.Models.ProTailorModels;
using ResuMatch.Api.Services.Interfaces.AiInterfaces;
using ResuMatch.Api.DataAccess;
using ResuMatch.Api.Services.Helpers;

namespace ResuMatch.Api.Services.Concrete.AIServices
{
    public class LlmService : IAIService, IProTailorAIService
    {
        private readonly IPromptService _promptService;
        private readonly ILogger<LlmService> _logger;
        private readonly LocalModelConfig _llmConfig;
        private readonly IHttpHandler _httpHandler;
        private readonly IResumeRepository _resumeRepository;

        private const int MaxRetries = 2;
        private int currentRetry = 0;
        private const int RetryDelayMilliseconds = 500;

        public LlmService
        (
            IPromptService promptService,
            ILogger<LlmService> logger,
            IOptions<LocalModelConfig> llmConfig,
            IHttpHandler httpHandler,
            IResumeRepository resumeRepository
        )
        {
            _promptService = promptService;
            _logger = logger;
            _llmConfig = llmConfig.Value;
            _httpHandler = httpHandler;
            _resumeRepository = resumeRepository;
        }

        public async Task<JsonDocument> ExtractSkillsAsync(string jobDescription)
        {
            string promptTemplate = await _promptService.GetExtractSkillsPrompt(jobDescription);
            string fullPrompt = string.Format(promptTemplate, jobDescription);

            string jsonResponseContent = await CallLlmApiAsync(fullPrompt, "openchat");

            if (string.IsNullOrWhiteSpace(jsonResponseContent))
            {
                _logger.LogError("LLM response for skills extraction is null or empty.");
                throw new InvalidOperationException("Empty response from LLM for skills extraction.");
            }

            using JsonDocument doc = JsonDocument.Parse(jsonResponseContent);
            JsonElement root = doc.RootElement;
            string skillsMarkdown = root.GetProperty("response").GetString()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(skillsMarkdown))
            {
                _logger.LogError("No valid JSON found in LLM skills response: {Response}", skillsMarkdown);
                throw new JsonException("Failed to extract JSON content for skills from LLM response.");
            }

            string[] extractedSkills = skillsMarkdown?.Split(',')?.Select(s => s.Trim()).ToArray() ?? Array.Empty<string>();

            return JsonDocument.Parse(JsonSerializer.Serialize(extractedSkills, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }

        public async Task<string> GenerateSummaryAsync(List<string> matchingSkills, List<string> missingSkills, decimal resumeExperience, decimal jobDesrcitionExperience)
        {
            _logger.LogInformation("Generating summary using OpenRouter...");

            // if (!matchingSkills.Any() || !missingSkills.Any())
            // {
            //     throw new ArgumentException("Matching and missing skills cannot be empty.", nameof(matchingSkills));
            // }

            string summaryPrompt = await _promptService.GetExtractSummaryPrompt(matchingSkills, missingSkills, resumeExperience, jobDesrcitionExperience);

            string summaryResponseContent = await CallLlmApiAsync(summaryPrompt, "openchat");
            JsonDocument summaryJson = JsonDocument.Parse(summaryResponseContent);
            string summary = summaryJson.RootElement.GetProperty("response").GetString()?.Trim() ?? string.Empty;
            return summary;
        }

        public Task<string> GetMatchingSkillsAnalysisAsync(List<string> resumeSkills, List<string> jobDescriptionSkills)
        {
            // var prompt = await _promptService.GetMatchingSkillsAnalysisPrompt(resumeSkills, jobDescriptionSkills);
            var matchingSkills = resumeSkills.Intersect(jobDescriptionSkills, StringComparer.OrdinalIgnoreCase).ToList();
            var missingSkills = jobDescriptionSkills.Except(resumeSkills, StringComparer.OrdinalIgnoreCase).ToList();
            var analysisResult = new
            {
                matching_skills = matchingSkills,
                missing_skills = missingSkills
            };

            string jsonOutput = JsonSerializer.Serialize(analysisResult, new JsonSerializerOptions { WriteIndented = true });
            return Task.FromResult(jsonOutput);
            // var response = await CallLlmApiAsync(prompt, "openchat");
            // return response;
        }

        // public async Task<string> GetOverallExperienceAnalysisAsync(string resumeText, string jobDescriptionText)
        // {
        //     string promptTemplate = await _promptService.GetOverallExperienceAnalysisPrompt(resumeText, jobDescriptionText);

        //     string jsonResponseContent = await CallLlmApiAsync(promptTemplate, "openchat");

        //     if (string.IsNullOrWhiteSpace(jsonResponseContent))
        //     {
        //         _logger.LogError("LLM response for overall experience analysis is null or empty.");
        //         return "{}";
        //     }

        //     try
        //     {
        //         using JsonDocument doc = JsonDocument.Parse(jsonResponseContent);
        //         JsonElement root = doc.RootElement;
        //         string responseMarkdown = root.GetProperty("response").GetString()?.Trim() ?? string.Empty;

        //         string extractedJson = ExtractJsonFromMarkdownResponse(responseMarkdown);

        //         if (string.IsNullOrWhiteSpace(extractedJson))
        //         {
        //             _logger.LogError("No valid JSON found in LLM overall experience response: {Response}", responseMarkdown);
        //             return "{}";
        //         }

        //         return extractedJson;
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Failed to parse overall experience from LLM response: {Response}", jsonResponseContent);
        //         return "{}";
        //     }
        // }

        // public async Task<string> GetOverallExperienceAnalysisAsync(string resumeText, string jobDescriptionText)
        // {
        //     int resumeExperience = 0;
        //     int jobDescriptionExperience = 0;

        //     // --- 1. Extract Resume Experience ---
        //     try
        //     {
        //         // Get the specific prompt for resume experience
        //         string resumePromptTemplate = await _promptService.GetExtractResumeExperiencePrompt(resumeText); // Assuming a new method for this specific prompt

        //         // Call LLM for resume experience
        //         string resumeJsonResponseContent = await CallLlmApiAsync(resumePromptTemplate, "openchat");

        //         if (!string.IsNullOrWhiteSpace(resumeJsonResponseContent))
        //         {
        //             using JsonDocument resumeDoc = JsonDocument.Parse(resumeJsonResponseContent);
        //             JsonElement resumeRoot = resumeDoc.RootElement;

        //             // Extract the 'response' field, which contains the actual JSON string from the LLM
        //             string resumeResponseMarkdown = resumeRoot.GetProperty("response").GetString()?.Trim() ?? string.Empty;

        //             // Extract the inner JSON from the markdown (e.g., from ```json{...}```)
        //             string extractedResumeJson = ExtractJsonFromMarkdownResponse(resumeResponseMarkdown);

        //             if (!string.IsNullOrWhiteSpace(extractedResumeJson))
        //             {
        //                 // Parse the extracted inner JSON to get the "OverallExperience" value
        //                 using JsonDocument innerResumeDoc = JsonDocument.Parse(extractedResumeJson);
        //                 if (innerResumeDoc.RootElement.TryGetProperty("OverallExperience", out JsonElement resumeExperienceElement))
        //                 {
        //                     // Ensure it's parsed as an integer
        //                     if (resumeExperienceElement.ValueKind == JsonValueKind.Number)
        //                     {
        //                         resumeExperience = resumeExperienceElement.GetInt32();
        //                     }
        //                     else if (resumeExperienceElement.ValueKind == JsonValueKind.String && int.TryParse(resumeExperienceElement.GetString(), out int parsedInt))
        //                     {
        //                         // Handle cases where it might still return a number as a string
        //                         resumeExperience = parsedInt;
        //                     }
        //                 }
        //             }
        //             else
        //             {
        //                 _logger.LogWarning("No valid JSON found in LLM resume experience response: {Response}", resumeResponseMarkdown);
        //             }
        //         }
        //         else
        //         {
        //             _logger.LogWarning("LLM response for resume experience is null or empty.");
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Failed to extract resume experience from LLM response.");
        //         // Keep resumeExperience as 0 on error
        //     }

        //     // --- 2. Extract Job Description Experience ---
        //     try
        //     {
        //         // Get the specific prompt for JD experience
        //         string jdPromptTemplate = await _promptService.GetExtractJDExperiencePrompt(jobDescriptionText); // Assuming a new method for this specific prompt

        //         // Call LLM for JD experience
        //         string jdJsonResponseContent = await CallLlmApiAsync(jdPromptTemplate, "openchat");

        //         if (!string.IsNullOrWhiteSpace(jdJsonResponseContent))
        //         {
        //             using JsonDocument jdDoc = JsonDocument.Parse(jdJsonResponseContent);
        //             JsonElement jdRoot = jdDoc.RootElement;

        //             // Extract the 'response' field, which contains the actual JSON string from the LLM
        //             string jdResponseMarkdown = jdRoot.GetProperty("response").GetString()?.Trim() ?? string.Empty;

        //             // Extract the inner JSON from the markdown
        //             string extractedJdJson = ExtractJsonFromMarkdownResponse(jdResponseMarkdown);

        //             if (!string.IsNullOrWhiteSpace(extractedJdJson))
        //             {
        //                 // Parse the extracted inner JSON to get the "OverallExperience" value
        //                 using JsonDocument innerJdDoc = JsonDocument.Parse(extractedJdJson);
        //                 if (innerJdDoc.RootElement.TryGetProperty("OverallExperience", out JsonElement jdExperienceElement))
        //                 {
        //                     // Ensure it's parsed as an integer
        //                     if (jdExperienceElement.ValueKind == JsonValueKind.Number)
        //                     {
        //                         jobDescriptionExperience = jdExperienceElement.GetInt32();
        //                     }
        //                     else if (jdExperienceElement.ValueKind == JsonValueKind.String && int.TryParse(jdExperienceElement.GetString(), out int parsedInt))
        //                     {
        //                         // Handle cases where it might still return a number as a string
        //                         jobDescriptionExperience = parsedInt;
        //                     }
        //                 }
        //             }
        //             else
        //             {
        //                 _logger.LogWarning("No valid JSON found in LLM job description experience response: {Response}", jdResponseMarkdown);
        //             }
        //         }
        //         else
        //         {
        //             _logger.LogWarning("LLM response for job description experience is null or empty.");
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Failed to extract job description experience from LLM response.");
        //         // Keep jobDescriptionExperience as 0 on error
        //     }

        //     // --- 3. Combine results into the final expected JSON format ---
        //     var finalResult = new
        //     {
        //         ResumeOverallExperience = resumeExperience,
        //         JobDescriptionOverallExperience = jobDescriptionExperience
        //     };

        //     return JsonSerializer.Serialize(finalResult);
        // }

         public async Task<string> GetOverallExperienceAnalysisAsync(string resumeText, string jobDescriptionText)
        {
            int resumeOverallExperience = 0;
            int jobDescriptionOverallExperience = 0;

            // --- 1. Call LLM to get raw experience data (explicit phrases and date ranges) ---
            ExperienceRawData rawData = null; // Use the ExperienceRawData class from the new helper namespace
            try
            {
                // Use the single GetExperienceRawData prompt constant
                string rawDataPrompt = string.Format(PromptConstants.GetExperienceRawData, resumeText, jobDescriptionText);
                string llmJsonResponseContent = await CallLlmApiAsync(rawDataPrompt, "openchat");

                if (!string.IsNullOrWhiteSpace(llmJsonResponseContent))
                {
                    using JsonDocument doc = JsonDocument.Parse(llmJsonResponseContent);
                    JsonElement root = doc.RootElement;

                    string responseMarkdown = root.GetProperty("response").GetString()?.Trim() ?? string.Empty;
                    string extractedJson = ExtractJsonFromMarkdownResponse(responseMarkdown);

                    if (!string.IsNullOrWhiteSpace(extractedJson))
                    {
                        rawData = JsonSerializer.Deserialize<ExperienceRawData>(extractedJson);
                        _logger.LogInformation("Raw experience data extracted by LLM successfully.");
                    }
                    else
                    {
                        _logger.LogWarning("No valid JSON block found in LLM raw experience data response: {Response}", responseMarkdown);
                    }
                }
                else
                {
                    _logger.LogWarning("LLM response for raw experience data is null or empty.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get raw experience data from LLM response. Error: {ErrorMessage}", ex.Message);
                // rawData will remain null, leading to default experience values
            }

            // Handle case where LLM failed to provide any raw data
            if (rawData == null)
            {
                _logger.LogWarning("LLM failed to provide raw experience data. Defaulting all experience to 0.");
                // Experience variables are already 0, so nothing more to do here.
            }
            else
            {
                // --- 2. Process Resume Experience (C# Logic: Explicit > Dates) ---
                // Call static methods from ExperienceExtractorExtensions
                List<int> explicitResumeYears = ExperienceExtractorExtensions.ExtractExplicitYearsFromPhrases(rawData.ResumeExplicitExperiencePhrases);
                if (explicitResumeYears.Any())
                {
                    resumeOverallExperience = explicitResumeYears.First(); // Prioritize the first found explicit number
                    _logger.LogInformation($"Resume: Found explicit experience: {resumeOverallExperience} years.");
                }
                else
                {
                    resumeOverallExperience = ExperienceExtractorExtensions.CalculateTotalNonOverlappingYears(rawData.ResumeJobDurationDates);
                    _logger.LogInformation($"Resume: No explicit experience. Calculated from dates: {resumeOverallExperience} years.");
                }

                // --- 3. Process Job Description Experience (C# Logic: Explicit > Seniority) ---
                // Call static methods from ExperienceExtractorExtensions
                List<int> explicitJdYears = ExperienceExtractorExtensions.ExtractExplicitYearsFromPhrases(rawData.JobDescriptionExplicitExperiencePhrases);
                if (explicitJdYears.Any())
                {
                    jobDescriptionOverallExperience = explicitJdYears.First(); // Prioritize the first found explicit number
                    _logger.LogInformation($"Job Description: Found explicit experience: {jobDescriptionOverallExperience} years.");
                }
                else
                {
                    // Fallback 1: Calculate from JD job duration dates (if LLM provides them and you add the property)
                    // if (rawData.JobDescriptionJobDurationDates != null && rawData.JobDescriptionJobDurationDates.Any())
                    // {
                    //     jobDescriptionOverallExperience = ExperienceExtractorExtensions.CalculateTotalNonOverlappingYears(rawData.JobDescriptionJobDurationDates);
                    //     _logger.LogInformation($"Job Description: No explicit experience. Calculated from JD dates: {jobDescriptionOverallExperience} years.");
                    // }
                    // else
                    // {
                        // Fallback 2: Infer from Seniority Level (if no explicit or date-based calculation possible for JD)
                        jobDescriptionOverallExperience = ExperienceExtractorExtensions.InferExperienceFromSeniority(jobDescriptionText); // Use original JD text for seniority
                        _logger.LogInformation($"Job Description: No explicit or date-based experience. Inferred from seniority: {jobDescriptionOverallExperience} years.");
                    // }
                }
            }

            // --- 4. Combine results into the final expected JSON format ---
            var finalResult = new
            {
                ResumeOverallExperience = resumeOverallExperience,
                JobDescriptionOverallExperience = jobDescriptionOverallExperience
            };

            return JsonSerializer.Serialize(finalResult);
        }

        public async Task<ResumeDetailModel> ExtractStructuredResumeDataAsync(string combinedInputForAITailoring)
        {
            _logger.LogInformation("Extracting structured resume data using ProTailor AI service.");
            var prompt = await _promptService.GetExtractResumeStructurePrompt(combinedInputForAITailoring);
            var jsonResponseContent = await CallLlmApiAsync(prompt, "openchat");
            string cleanedJsonResponse;
            ParseJsonResponse(jsonResponseContent, out cleanedJsonResponse);

            try
            {
                return DesirializeResumeDetail(cleanedJsonResponse);
            }
            catch (JsonException ex)
            {
                if (currentRetry < MaxRetries)
                {
                    currentRetry++;
                    _logger.LogWarning(ex, "ProTailor LLM Service: JSON parsing failed. Retrying {CurrentRetry}/{MaxRetries}. Response: {ResponseContent}", currentRetry, MaxRetries, jsonResponseContent);
                    await Task.Delay(RetryDelayMilliseconds);
                    var repairPrompt = await _promptService.GetExtractResumeStructurePrompt(cleanedJsonResponse);
                    var repairedJsonResponseContent = await CallLlmApiAsync(repairPrompt, "openchat");
                    string repairedJsonResponse;
                    ParseJsonResponse(repairedJsonResponseContent, out repairedJsonResponse);
                    return DesirializeResumeDetail(repairedJsonResponse);
                }

                _logger.LogError(ex, "Failed to parse JSON response from LLM: {Response}", cleanedJsonResponse);
                throw new JsonException("Failed to parse JSON content from LLM response.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while extracting structured resume data.");
                throw new ApplicationException("An error occurred while extracting structured resume data.", ex);
            }
        }

        public async Task<SuggestionsResponse> GenerateTailoringSuggestionsAsync(AnalysisResult analysisResult, string jobDescriptionText, string rawResumeText)
        {
            _logger.LogInformation("ProTailor LLM Service: Generating tailoring suggestions.");
            var cacheKeyGuid = Guid.NewGuid();
            string analysisResultJson = JsonSerializer.Serialize(analysisResult, new JsonSerializerOptions { WriteIndented = false });
            string prompt = await _promptService.GetGenerateTailoringSuggestionsPrompt(analysisResultJson, jobDescriptionText, rawResumeText);
            string jsonResponseContent = await CallLlmApiAsync(prompt, "openchat");
            string cleanedJsonResponse;

            ParseJsonResponse(jsonResponseContent, out cleanedJsonResponse);
            
            try
                {
                    var desiarizedResponse = DesirializeResponse(cleanedJsonResponse);
                    return desiarizedResponse;
                }
                catch (JsonException ex)
                {
                    if (currentRetry < MaxRetries)
                    {
                        currentRetry++;
                        _logger.LogWarning(ex, "ProTailor LLM Service: JSON parsing failed. Retrying {CurrentRetry}/{MaxRetries}. Response: {ResponseContent}", currentRetry, MaxRetries, jsonResponseContent);
                        await Task.Delay(RetryDelayMilliseconds);
                        var repairPrompt = await _promptService.GetTailoringSuggestionsJsonRepairPrompt(cleanedJsonResponse);
                        var repairedJsonResponseContent = await CallLlmApiAsync(repairPrompt, "openchat");
                        string repairedJsonResponse;
                        ParseJsonResponse(repairedJsonResponseContent, out repairedJsonResponse);
                        return DesirializeResponse(repairedJsonResponse);
                    }
                    _logger.LogError(ex, "ProTailor LLM Service: Failed to parse SuggestionsResponse JSON from LLM response. Response: {ResponseContent}", jsonResponseContent);
                    throw new InvalidOperationException($"Failed to parse tailoring suggestions: Invalid JSON. Error: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ProTailor LLM Service: An unexpected error occurred during tailoring suggestions generation.");
                    throw;
                }
        }

        private ResumeDetailModel DesirializeResumeDetail(string cleanedJsonResponse)
        {
            var resumeDetail = JsonSerializer.Deserialize<ResumeDetailModel>(cleanedJsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new JsonException("Deserialized ResumeDetailModel is null.");

            if (resumeDetail == null)
            {
                _logger.LogError("ProTailor LLM Service: Deserialization returned null SuggestionsResponse object. Cleaned JSON: {CleanedJson}", cleanedJsonResponse);
                throw new InvalidOperationException("Deserialization returned null SuggestionsResponse.");
            }
            currentRetry = 0;
            return resumeDetail;
        }

        private SuggestionsResponse DesirializeResponse(string cleanedJsonResponse)
        {
            var suggestions = JsonSerializer.Deserialize<SuggestionsResponse>(cleanedJsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new JsonException("Deserialized SuggestionsResponse is null.");

            if (suggestions == null)
            {
                _logger.LogError("ProTailor LLM Service: Deserialization returned null SuggestionsResponse object. Cleaned JSON: {CleanedJson}", cleanedJsonResponse);
                throw new InvalidOperationException("Deserialization returned null SuggestionsResponse.");
            }
            currentRetry = 0;
            return suggestions;
        }

        private void ParseJsonResponse(string jsonResponseContent, out string cleanedJsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponseContent))
            {
                _logger.LogError("ProTailor LLM Service: LLM response for tailoring suggestions is null or empty.");
                throw new InvalidOperationException("Empty response from LLM for tailoring suggestions.");
            }
            var doc = JsonDocument.Parse(jsonResponseContent);
            if (!doc.RootElement.TryGetProperty("response", out JsonElement responseElement))
            {
                _logger.LogError("ProTailor LLM Service: LLM response for tailoring suggestions missing 'response' field. Raw: {RawResponse}", jsonResponseContent);
                throw new InvalidOperationException("LLM response is not in the expected format (missing 'response' field).");
            }
            var innerContent = responseElement.GetString()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(innerContent))
            {
                _logger.LogError("Inner LLM response content for tailoring suggestions is empty. Raw: {RawResponse}", innerContent);
                throw new InvalidOperationException("Inner LLM response content for tailoring suggestions is empty.");
            }
            cleanedJsonResponse = ExtractJsonFromMarkdownResponse(innerContent);
            if (string.IsNullOrWhiteSpace(cleanedJsonResponse))
            {
                cleanedJsonResponse = innerContent;
                _logger.LogWarning("ProTailor LLM Service: No JSON markdown block found.Proceeding with inner content as is.");
            }
            _logger.LogDebug("ProTailor LLM Service: Cleaned JSON response (after markdown strip): {CleanedJson}", cleanedJsonResponse);
        }

        public async Task<string> TailorResumeSectionAsync(string resumeSectionText, string jobDescriptionText, string sectionType)
        {
            _logger.LogInformation("ProTailor LLM Service: Calling AI for resume section tailoring. SectionType: {SectionType}", sectionType);
            string prompt = await _promptService.GetTailorResumeSectionPrompt(
                resumeSectionText,
                jobDescriptionText,
                sectionType
            );
            string tailoredContent = await CallLlmApiAsync(prompt, "openchat");

            if (string.IsNullOrWhiteSpace(tailoredContent))
            {
                _logger.LogError("ProTailor LLM Service: LLM response for resume section tailoring is null or empty.");
                throw new InvalidOperationException("Empty response from LLM for resume section tailoring.");
            }

            string cleanedTailoredContent = ExtractJsonFromMarkdownResponse(tailoredContent);
            if (string.IsNullOrWhiteSpace(cleanedTailoredContent) && !string.IsNullOrWhiteSpace(tailoredContent))
            {
                _logger.LogWarning("ProTailor LLM Service: Tailored content became empty after stripping markdown. Original: {OriginalContent}", tailoredContent);
                return tailoredContent;
            }
            return cleanedTailoredContent;
        }

        private async Task<string> CallLlmApiAsync(string prompt, string model)
        {
            string? modelName = GetModelByName(model);

            if (string.IsNullOrWhiteSpace(modelName))
            {
                _logger.LogError("Model name for '{Model}' could not be found in configuration for LLM API call.", model);
                throw new InvalidOperationException($"Model name for '{model}' could not be found in configuration.");
            }

            var requestBody = new
            {
                model = modelName,
                prompt,
                stream = false
            };

            try
            {
                _logger.LogDebug("Calling LLM API endpoint: {Endpoint} with model: {ModelName}", _llmConfig.Endpoint, modelName);
                if (_httpHandler is null)
                {
                    _logger.LogError("HttpHandler cannot be null.");
                    throw new InvalidOperationException("HttpHandler cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(_llmConfig.Endpoint))
                {
                    _logger.LogError("LLM API endpoint is not configured.");
                    throw new InvalidOperationException("LLM API endpoint is not configured.");
                }

                var response = await _httpHandler.PostAsync(_llmConfig.Endpoint, requestBody);

                if (response == null)
                {
                    _logger.LogError("Response cannot be null from HTTP handler.");
                    throw new InvalidOperationException("Response cannot be null from HTTP handler.");
                }

                _logger.LogDebug("Received successful response from {Endpoint}: {ResponseContent}", _llmConfig.Endpoint, response);

                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when calling LLM API endpoint '{Endpoint}'.", _llmConfig.Endpoint);
                throw new ApplicationException($"Failed to communicate with LLM service at '{_llmConfig.Endpoint}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during LLM API call to '{Endpoint}'.", _llmConfig.Endpoint);
                throw;
            }
        }

        private string? GetModelByName(string modelName)
        {
            var model = _llmConfig.Models?.FirstOrDefault(m => m.Name != null && m.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase));
            return model?.Model;
        }

        private static string ExtractJsonFromMarkdownResponse(string markdownResponse)
        {
            // Regex to find content between ```json and ```
            Match match = Regex.Match(markdownResponse, @"```json\s*([\s\S]*?)\s*```", RegexOptions.IgnoreCase);

            if (match.Success && match.Groups.Count > 1)
            {
                // Return the captured group which contains the JSON string
                return match.Groups[1].Value.Trim();
            }

            // If no markdown block is found, check if it's already pure JSON (though less likely if it was wrapped)
            // This fallback might be useful if some responses don't get wrapped.
            if (IsLikelyJson(markdownResponse))
            {
                return markdownResponse;
            }

            return string.Empty;
        }

        private static bool IsLikelyJson(string text)
        {
            return text.StartsWith("{") && text.EndsWith("}") || text.StartsWith("[") && text.EndsWith("]");
        }
    }
}