using System.Text.Json;
using ResuMatch.Api.Models;
using ResuMatch.Api.Models.Configurations;
using ResuMatch.Api.Services.FileProccessing.Interfaces;
using ResuMatch.Api.Services.Interfaces;
using IFileProcessor = ResuMatch.Api.Services.FileProccessing.Interfaces.IFileProcessor;

namespace ResuMatch.Api.Services.Concretes
{
    public class ResumeAnalysisService : IResumeAnalysisService
    {
        private readonly ILogger<ResumeAnalysisService> _logger;
        private readonly IAIService _aiService;
        private readonly ISkillMatcher _skillMatcher;
        private readonly IAnalysisService _analysisService;
        private readonly IFileProcessorFactory _fileProcessorFactory;
        

        public ResumeAnalysisService(
            ILogger<ResumeAnalysisService> logger,
            IAIService aiService,
            ISkillMatcher skillMatcher,
            IAnalysisService analysisService,
            IFileProcessorFactory fileProcessorFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _skillMatcher = skillMatcher ?? throw new ArgumentNullException(nameof(skillMatcher));
            _analysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
            _fileProcessorFactory = fileProcessorFactory ?? throw new ArgumentNullException(nameof(fileProcessorFactory));
        }
        public async Task<AnalysisResponse> ProcessResumeAsync(IFormFile file, string jobDescription)
        {
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
               await file.CopyToAsync(stream);
            }
            _logger.LogInformation("File uploaded successfully: {FilePath}", filePath);

            IFileProcessor fileProcessor = _fileProcessorFactory.CreateProcessor(filePath);
            if (fileProcessor == null)
            {
                throw new InvalidOperationException("File processor is not initialized.");
            }
            var extractedText = await fileProcessor.ExtractTextAsync(filePath);
            _logger.LogInformation("Text extracted successfully from file: {FilePath}", filePath);

            var aiExtractedSkills = await _aiService.ExtractSkillsAsync(jobDescription);
            _logger.LogInformation("Skills extracted successfully from AI service.");

            var resumeSkills = JsonSerializer.Deserialize<AIResponse>(aiExtractedSkills)?.Choices[0]?.Message?.Content.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
            var jobDescriptionSkills = JsonSerializer.Deserialize<AIResponse>(aiExtractedSkills)?.Choices[0]?.Message?.Content.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
            
            var matchedSkills = _skillMatcher.MatchSkills(resumeSkills, jobDescriptionSkills);

            var analysisRequest = new AnalysisRequest
            {
                ResumeText = extractedText,
                JobDescriptionText = jobDescription,
            };
            await _analysisService.StoreAnalysisResultAsync(analysisRequest, matchedSkills, filePath);

            return matchedSkills;
        }
    }
}