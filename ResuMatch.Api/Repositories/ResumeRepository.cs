using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ResuMatch.Api.Data;
using ResuMatch.Api.Models;
using ResuMatch.Api.Models.Configurations;

namespace ResuMatch.Api.Repositories
{
    public class ResumeRepository : IResumeRepository
    {
         private readonly ILogger<ResumeRepository> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OpenRouterConfig  _openRouterConfig;
        private readonly IResumeContext _context;

        public ResumeRepository(
            ILogger<ResumeRepository> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<OpenRouterConfig> openRouterConfig,
            IResumeContext context)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _openRouterConfig = openRouterConfig.Value;
            _context = context;
        }

         public async Task StoreAnalysisResult(AnalysisRequest request, AnalysisResult result, string filePath)
        {
            // Use MongoDB through MongoDbContext
            _logger.LogInformation("Storing analysis result in MongoDB for file: {FilePath}", filePath);

            var resumeData = new ResumeData
            {
                FilePath = filePath,
                ResumeText = request.ResumeText,
                JobDescriptionText = request.JobDescriptionText,
                AnalysisResult = result,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _context.ResumeData.InsertOneAsync(resumeData);
                _logger.LogInformation("Analysis result stored successfully in MongoDB.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing analysis result in MongoDB.");
                throw; // Re-throw to be handled in the controller
            }
        }

        public async Task<string> GetSkillsFromOpenRouter(string jobDescription)
        {
            string prompt = $"Extract the skills from the following job description and return them as a comma-separated list: {jobDescription}";
            var requestBody = new
            {
                model = _openRouterConfig.Model,
                messages = new[] { new { role = "user", content = prompt } }
            };
            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.PostAsync(_openRouterConfig.Endpoint, content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"OpenRouter API error: {response.StatusCode} - {errorContent}");
                    throw new Exception($"OpenRouter API request failed: {response.StatusCode} - {errorContent}"); //custom exception
                }
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent; // Return the raw JSON
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenRouter API");
                throw; // Re-throw the exception to be handled by the service/controller
            }
        }
    }
}