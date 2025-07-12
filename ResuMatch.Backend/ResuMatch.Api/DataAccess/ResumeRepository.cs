using System.Text;
using System.Text.Json;
using ResuMatch.Api.Models;
using ResuMatch.Api.Models.ProTailorModels;
using ResuMatch.Api.Services.Interfaces;

namespace ResuMatch.Api.DataAccess
{
    public class ResumeRepository : IResumeRepository
    {
        private readonly ILogger<ResumeRepository> _logger;
        private readonly IHttpHandler _httpHandler;
        private readonly string _resumeExternalAPIBaseUrl;

        public ResumeRepository(
            ILogger<ResumeRepository> logger,
            IHttpHandler httpHandler,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpHandler = httpHandler;
            _resumeExternalAPIBaseUrl = configuration["ResumeExternalAPI:BaseUrl"]
                                        ?? throw new ArgumentNullException("ResumeExternalAPI:BaseUrl is not configured.");
        }

        public Task<string> CacheAsync(string key, string suggestionsResponse)
        {
            try
            {
                var url = $"{_resumeExternalAPIBaseUrl}/api/resumatchexternal/cache/set/{key}";
                var jsonContent = JsonSerializer.Serialize(suggestionsResponse);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                return _httpHandler.PostAsync(url, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching suggestion response for key {Key}: {Message}", key, ex.Message);
                throw;
            }
        }

        public Task<string> GetCachedSuggestionResponseAsync(string key)
        {
            try
            {
                var url = $"{_resumeExternalAPIBaseUrl}/api/resumatchexternal/cache/{key}";
                return _httpHandler.GetAsync(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cached suggestion response for key {Key}: {Message}", key, ex.Message);
                throw;
            }
        }

        public async Task<ResumeData?> GetResumeDataByIdAsync(string resumeId)
        {
            try
            {
                var url = $"{_resumeExternalAPIBaseUrl}/api/resumatchexternal/{resumeId}";
                var response = await _httpHandler.GetAsync(url);

                return JsonSerializer.Deserialize<ResumeData>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resume data for ID {ResumeId}: {Message}", resumeId, ex.Message);
                throw;
            }
        }

        public async Task<ResumeData> PersistData(IFormFile file, string resumeId, string extractedResumeText, string jobDescription, AnalysisResult analysisResult)
        {
            _logger.LogInformation("Persisting resume data to external API...");

            if (file == null || file.Length == 0)
            {
                _logger.LogError("No file provided for upload.");
                throw new ArgumentException("No file provided for upload.");
            }
            if (string.IsNullOrWhiteSpace(extractedResumeText))
            {
                _logger.LogError("Extracted resume text is empty.");
                throw new ArgumentException("Extracted resume text cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(jobDescription))
            {
                _logger.LogError("Job description is empty.");
                throw new ArgumentException("Job description cannot be empty.");
            }
            if (analysisResult == null)
            {
                _logger.LogError("Analysis result is null.");
                throw new ArgumentNullException(nameof(analysisResult), "Analysis result cannot be null.");
            }
            _logger.LogDebug("File: {FileName}, ExtractedText Length: {ExtractedTextLength}, JobDescription Length: {JobDescriptionLength}, AnalysisResult: {@AnalysisResult}",
            file.FileName, extractedResumeText.Length, jobDescription.Length, analysisResult);

            var uploadUrl = $"{_resumeExternalAPIBaseUrl}/api/resumatchexternal/upload";

            using var formData = new MultipartFormDataContent();

            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            formData.Add(fileContent, "file", file.FileName);
            formData.Add(new StringContent(resumeId, Encoding.UTF8), "resumeId");
            formData.Add(new StringContent(file.FileName, Encoding.UTF8), "fileName");
            formData.Add(new StringContent(extractedResumeText, Encoding.UTF8), "extractedResumeText");
            formData.Add(new StringContent(jobDescription, Encoding.UTF8), "jobDescription");
            formData.Add(new StringContent(JsonSerializer.Serialize(analysisResult), Encoding.UTF8, "application/json"), "analysisResult");
            _logger.LogDebug("Sending multipart request to {UploadUrl}", uploadUrl);

            try
            {
                var response = await _httpHandler.PostMultipartAsync(uploadUrl, formData);
                _logger.LogInformation("Resume data persisted successfully.");
                return new ResumeData
                {
                    ResumeDataId = resumeId,
                    // FileName = file.FileName,
                    // ExtractedResumeText = extractedResumeText,
                    // JobDescription = jobDescription,
                    AnalysisResult = analysisResult
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist resume data: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<bool> SaveResumeDetailModelAsync(string resumeId, ResumeDetailModel resumeDetailModel)
        {
            try
            {
                var url = $"{_resumeExternalAPIBaseUrl}/api/resumatchexternal/save/{resumeId}";
                var response = await _httpHandler.PutAsync(url, resumeDetailModel);
                return string.IsNullOrWhiteSpace(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving resume detail model for ID {ResumeId}: {Message}", resumeId, ex.Message);
                throw;
            }
        }
        
        
    }
}