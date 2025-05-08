using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ResuMatch.Api.Models.Configurations;
using ResuMatch.Api.Services.Interfaces;

namespace ResuMatch.Api.Services.Concretes
{
    public class OpenRouterAIService : IAIService
    {
        private readonly ILogger<OpenRouterAIService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OpenRouterConfig _openRouterConfig;

        public OpenRouterAIService(ILogger<OpenRouterAIService> logger, IHttpClientFactory httpClientFactory, IOptions<OpenRouterConfig> openRouterConfig)
        {
            _openRouterConfig = openRouterConfig.Value;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> ExtractSkillsAsync(string jobDescription)
        {
            _logger.LogInformation("Extracting skills using OpenRouter...");

            if (string.IsNullOrWhiteSpace(jobDescription))
            {
                throw new ArgumentException("Job description cannot be null or empty.", nameof(jobDescription));
            }

            string prompt = $"Extract the skills from the following job description and return them as a comma-separated list: {jobDescription}";

            var requestBody = new
            {
                model = _openRouterConfig.Model, 
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            string json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions {  
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });

            _logger.LogDebug("OpenRouter request body: {RequestBody}", json);

            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.PostAsync(_openRouterConfig.Endpoint, new StringContent(json, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("OpenRouter API error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                        throw new Exception(response.StatusCode.ToString());
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