using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ResuMatch.Api.Models.Configurations;
using ResuMatch.Api.DataAccess;
using ResuMatch.Api.Services.Interfaces;

namespace ResuMatch.Api.Services.Concretes
{    public class OpenRouterAIService : IAIService
    {
        private readonly ILogger<OpenRouterAIService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OpenRouterConfig _openRouterConfig;
        private readonly IResumeRepository _resumeRepository;
        private readonly IPromptService _promptService;

        public OpenRouterAIService
        (
         ILogger<OpenRouterAIService> logger,
         IHttpClientFactory httpClientFactory,
         IOptions<OpenRouterConfig> openRouterConfig,
         IResumeRepository resumeRepository,
        IPromptService promptService
        )
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _openRouterConfig = openRouterConfig.Value;
            _resumeRepository = resumeRepository;
            _promptService = promptService;
        }

        public async Task<JsonDocument> ExtractSkillsAsync(string jobDescription)
        {
            _logger.LogInformation("Extracting skills using OpenRouter...");

            if (string.IsNullOrWhiteSpace(jobDescription))
            {
                throw new ArgumentException("Job description cannot be null or empty.", nameof(jobDescription));
            }

            string skillsPrompt = await _promptService.GetExtractSkillsPrompt(jobDescription);
            string skillsResponseContent = await CallOpenRouterAsync(skillsPrompt);
            JsonDocument skillsJson = JsonDocument.Parse(skillsResponseContent);
            string skillsText = skillsJson.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()?.Trim() ?? string.Empty;
            string[] extractedSkills = skillsText?.Split(',')?.Select(s => s.Trim()).ToArray() ?? Array.Empty<string>();

            return JsonDocument.Parse(JsonSerializer.Serialize(extractedSkills, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }

        public async Task<string> GenerateSummaryAsync
        (List<string> matchingSkills,
         List<string> missingSkills,
         decimal resumeExperience,
         decimal jobDesrcitionExperience
        )
        {
            _logger.LogInformation("Generating summary using OpenRouter...");

            if (!matchingSkills.Any() || !missingSkills.Any())
            {
                throw new ArgumentException("Matching and missing skills cannot be empty.", nameof(matchingSkills));
            }

            string summaryPrompt = await _promptService.GetExtractSummaryPrompt(matchingSkills, missingSkills, resumeExperience, jobDesrcitionExperience);

            string summaryResponseContent = await CallOpenRouterAsync(summaryPrompt);
            JsonDocument summaryJson = JsonDocument.Parse(summaryResponseContent);
            string summary = summaryJson.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()?.Trim() ?? string.Empty;
            return summary;
        }

        public async Task<string> GetMatchingSkillsAnalysisAsync(List<string> resumeSkills, List<string> jobDescriptionSkills)
        {
            var prompt = await _promptService.GetMatchingSkillsAnalysisPrompt(resumeSkills, jobDescriptionSkills);

            return await CallOpenRouterAsync(prompt);
        }

        public async Task<string> GetOverallExperienceAnalysisAsync(string resumeText, string jobDescriptionText)
        {
            var prompt = await _promptService.GetOverallExperienceAnalysisPrompt(resumeText, jobDescriptionText);
            var response = await CallOpenRouterAsync(prompt);

            if (string.IsNullOrWhiteSpace(response))
            {
                _logger.LogError("OpenRouter response is null or empty.");
                return "{}";
            }

            try
            {
                using var doc = JsonDocument.Parse(response);
                var choices = doc.RootElement.GetProperty("choices");
                if (choices.GetArrayLength() == 0)
                {
                    _logger.LogError("No choices returned from OpenRouter.");
                    return "{}";
                }

                var content = choices[0].GetProperty("message").GetProperty("content").GetString()?.Trim();
                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogError("OpenRouter content is empty.");
                    return "{}";
                }

                // Remove code block markers if present
                if (content.StartsWith("```"))
                {
                    int firstNewline = content.IndexOf('\n');
                    if (firstNewline >= 0)
                        content = content.Substring(firstNewline + 1);
                    if (content.EndsWith("```"))
                        content = content.Substring(0, content.Length - 3).TrimEnd();
                }

                // Validate JSON
                using var contentDoc = JsonDocument.Parse(content);
                return contentDoc.RootElement.GetRawText();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse overall experience from OpenRouter response: {Response}", response);
                return "{}";
            }
        }

        private async Task<string> CallOpenRouterAsync(string prompt)
        {
            var requestBody = new
            {
                model = _openRouterConfig.Model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            string json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            _logger.LogDebug("OpenRouter request body: {RequestBody}", json);

            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openRouterConfig.ApiKey}");

                if (!string.IsNullOrWhiteSpace(_openRouterConfig.HttpReferer))
                {
                    httpClient.DefaultRequestHeaders.Add("HTTP-Referer", _openRouterConfig.HttpReferer);
                }
                if (!string.IsNullOrWhiteSpace(_openRouterConfig.XTitle))
                {
                    httpClient.DefaultRequestHeaders.Add("X-Title", _openRouterConfig.XTitle);
                }

                var response = await httpClient.PostAsync(_openRouterConfig.Endpoint, new StringContent(json, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("OpenRouter API error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    throw new Exception($"OpenRouter API Error: {response.StatusCode} - {errorContent}");
                }

                string responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("OpenRouter API response: {ResponseContent}", responseContent);
                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenRouter API");
                throw;
            }
        }
    }
}