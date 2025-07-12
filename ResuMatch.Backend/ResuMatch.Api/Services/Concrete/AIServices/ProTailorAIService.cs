using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ResuMatch.Api.Models;
using ResuMatch.Api.Models.Configurations;
using ResuMatch.Api.Models.ProTailorModels;
using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Api.Services.Interfaces.AiInterfaces;

namespace ResuMatch.Api.Services.Concrete.AIServices
{
    public class ProTailorAIService //: IProTailorAIService
    {
        private readonly IPromptService _promptService;
        private readonly ILogger<ProTailorAIService> _logger;
        private readonly LocalModelConfig _llmConfig;
        private readonly IHttpHandler? _httpHandler;

        public ProTailorAIService(IPromptService promptService, ILogger<ProTailorAIService> logger, IOptions<LocalModelConfig> llmConfig)
        {
            _promptService = promptService;
            _logger = logger;
            _llmConfig = llmConfig.Value;
        }

        public async Task<ResumeDetailModel> ExtractStructuredResumeDataAsync(string combinedInputForAITailoring)
        {
            _logger.LogInformation("Extracting structured resume data using ProTailor AI service.");
            var prompt = await _promptService.GetExtractResumeStructurePrompt(combinedInputForAITailoring);
            var jsonResponseContent = await CallLlmApiAsync(prompt, "mistral");

            if (string.IsNullOrWhiteSpace(jsonResponseContent))
            {
                _logger.LogError("LLM response for structured resume data extraction is null or empty.");
                throw new InvalidOperationException("Empty response from LLM for structured resume data extraction.");
            }

            try
            {
                return JsonSerializer.Deserialize<ResumeDetailModel>(jsonResponseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                }) ?? throw new JsonException("Deserialized ResumeDetailModel is null.");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse JSON response from LLM: {Response}", jsonResponseContent);
                throw new JsonException("Failed to parse JSON content from LLM response.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while extracting structured resume data.");
                throw new ApplicationException("An error occurred while extracting structured resume data.", ex);
            }
        }

        public Task<string> GenerateTailoringSuggestionsAsync(AnalysisResult analysisResult, string jobDescriptionText, string rawResumeText)
        {
            throw new NotImplementedException();
        }

        public Task<string> TailorResumeSectionAsync(string resumeSectionText, string jobDescriptionText, string sectionType)
        {
            throw new NotImplementedException();
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
    }
}