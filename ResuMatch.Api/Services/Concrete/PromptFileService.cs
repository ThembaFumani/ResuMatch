using System.Text.Json;
using ResuMatch.Api.Models;
using ResuMatch.Api.Services.Interfaces;

namespace ResuMatch.Api.Services.Concrete
{
    public class PromptFileService : IPromptFileService
    {
        private readonly ILogger<PromptFileService> _logger;
        private readonly IHostEnvironment _hostEnvironment;
        private PromptFileModel _promptFileModel;
        public PromptFileService(ILogger<PromptFileService> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _promptFileModel = new PromptFileModel();
        }
        
        public async Task<PromptFileModel> LoadPromptFileAsync()
        {
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath,  "prompts.json");
            if (!File.Exists(filePath))
            {
                _logger.LogError("Prompt file not found at path: {FilePath}", filePath);
                throw new FileNotFoundException("Prompt file not found.", filePath);
            }
            try
            {
                await using FileStream openStream = File.OpenRead(filePath);
                var deserializedModel = await JsonSerializer.DeserializeAsync<PromptFileModel>(openStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (deserializedModel == null)
                {
                    _logger.LogError("Failed to deserialize prompt file at path: {FilePath}", filePath);
                    throw new InvalidOperationException("Failed to deserialize prompt file.");
                }
                _promptFileModel = deserializedModel;
                _logger.LogInformation("Prompt file model loaded successfully from path: {FilePath}", filePath);
                return _promptFileModel;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing prompts.json from {FilePath}", filePath);
                throw new InvalidOperationException($"Invalid JSON format in prompts.json: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading prompt file at path: {FilePath}", filePath);
                throw new IOException("Error reading prompt file.", ex);
            }

            throw new NotImplementedException();
        }
    }
}