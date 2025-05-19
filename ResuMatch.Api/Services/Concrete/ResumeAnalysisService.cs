using System.Text.Json;
using ResuMatch.Api.Models;
using ResuMatch.Api.Services.FileProccessing.Interfaces;
using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Pipelines;
using IFileProcessor = ResuMatch.Api.Services.FileProccessing.Interfaces.IFileProcessor;

namespace ResuMatch.Api.Services.Concretes
{
    public class ResumeAnalysisService : IResumeAnalysisService
    {
        private readonly IResumeAnalysisPipeline _pipeline;
        private readonly ILogger<ResumeAnalysisService>? _logger;

        public ResumeAnalysisService(IResumeAnalysisPipeline pipeline, ILogger<ResumeAnalysisService>? logger)
        {
            _pipeline = pipeline;
            _logger = logger;
        }

        public async Task<AnalysisResult> ProcessResumeAsync(IFormFile file, string jobDescription)
        {
            var context = new ResumeAnalysisContext
            {
                File = file,
                JobDescription = jobDescription
            };

            var result = await _pipeline.ExecuteAsync(context);

            if (context == null)
            {
                _logger.LogError("Pipeline execution returned null context.");
                throw new InvalidOperationException("Pipeline execution returned null context.");
            }

            if (context.Error != null)
            {
                _logger.LogError("Error processing resume: {ErrorMessage}", context.Error);
                // Consider throwing an exception here or returning a specific error result
                return new AnalysisResult { Error = context.Error }; // Or throw new Exception(context.Error);
            }
            _logger.LogInformation("Resume processed successfully.");
            return context.AnalysisResult;
        }

        // public async Task<AnalysisResult> ProcessResumeAsync(IFormFile file, string jobDescription)
        // {
        //     string filePath = null;
        //     try
        //     {
        //         var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        //         var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        //         filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //         using (var stream = new FileStream(filePath, FileMode.Create))
        //         {
        //             await file.CopyToAsync(stream);
        //         }
        //         _logger.LogInformation("File uploaded successfully: {FilePath}", filePath);

        //         IFileProcessor fileProcessor = _fileProcessorFactory.CreateProcessor(filePath);
        //         if (fileProcessor == null)
        //         {
        //             throw new InvalidOperationException("File processor is not initialized.");
        //         }
        //         var extractedResumeText = await fileProcessor.ExtractTextAsync(filePath);
        //         _logger.LogInformation("Text extracted successfully from file: {FilePath}", filePath);

        //         var resumeSkillsResponse = await _aiService.ExtractSkillsAsync(extractedResumeText);
        //         _logger.LogInformation("Skills extracted from resume using AI.");

        //         var jobDescriptionResponse = await _aiService.ExtractSkillsAsync(jobDescription);
        //         _logger.LogInformation("Skills extracted successfully from AI service.");

        //         var resumeSkills = JsonSerializer.Deserialize<string[]>(resumeSkillsResponse)?.ToList() ?? new List<string>();
        //         var jobDescriptionSkills = JsonSerializer.Deserialize<string[]>(jobDescriptionResponse)?.ToList() ?? new List<string>();

        //         var matchedSkillsResult = _skillMatcher.MatchSkills(resumeSkills, jobDescriptionSkills);

        //         // **Generate summary of the matching results**
        //         var summaryResponse = await _aiService.ExtractSummaryAsync(new string[] {
        //             $"Extracted Skills from Resume: {string.Join(", ", matchedSkillsResult.ExtractedResumeSkills)}",
        //             $"Required Skills from Job Description: {string.Join(", ", matchedSkillsResult.ExtractedJobDescriptionSkills)}",
        //             $"Matching Skills: {string.Join(", ", matchedSkillsResult.MatchingSkills)}",
        //             $"Missing Skills: {string.Join(", ", matchedSkillsResult.MissingSkills)}",
        //             $"Match Score: {matchedSkillsResult.MatchScore}",
        //             "Based on this analysis, write a concise summary (1-2 sentences) for a job seeker, highlighting the alignment between their skills and the job requirements, and also mentioning any significant gaps."
        //         });

        //         string summary = JsonSerializer.Deserialize<string[]>(summaryResponse)?.FirstOrDefault() ?? string.Empty;

        //         var analysisRequest = new AnalysisRequest
        //         {
        //             ResumeText = extractedResumeText,
        //             JobDescriptionText = jobDescription,
        //         };
        //         //await _analysisService.StoreAnalysisResultAsync(analysisRequest, matchedSkillsResult, filePath);

        //         // Create and return the final AnalysisResponse with the summary
        //         return new AnalysisResult
        //         {
        //             MatchScore = matchedSkillsResult.MatchScore,
        //             MatchingSkills = matchedSkillsResult.MatchingSkills,
        //             MissingSkills = matchedSkillsResult.MissingSkills,
        //             Summary = summary, 
        //             ExtractedResumeSkills = matchedSkillsResult.ExtractedResumeSkills,
        //             ExtractedJobDescriptionSkills = matchedSkillsResult.ExtractedJobDescriptionSkills 
        //         };
        //     }
        //     finally
        //     {
        //         if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        //         {
        //             try
        //             {
        //                 File.Delete(filePath);
        //                 _logger.LogInformation("Uploaded file deleted successfully: {FilePath}", filePath);
        //             }
        //             catch (IOException ex)
        //             {
        //                 _logger.LogError(ex, "Error deleting uploaded file: {FilePath}", filePath);
        //             }
        //         }
        //     }
        // }
    }
}