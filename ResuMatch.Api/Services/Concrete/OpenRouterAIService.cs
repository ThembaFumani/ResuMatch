using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ResuMatch.Api.Models.Configurations;
using ResuMatch.Api.Repositories;
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

        public async Task<string> GenerateSummaryAsync(string[] details)
        {
            _logger.LogInformation("Generating summary using OpenRouter...");

            if (details == null || details.Length == 0)
            {
                throw new ArgumentException("Details for summary cannot be null or empty.", nameof(details));
            }

            string summaryPrompt = await _promptService.GetExtractSummaryPrompt(details);

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