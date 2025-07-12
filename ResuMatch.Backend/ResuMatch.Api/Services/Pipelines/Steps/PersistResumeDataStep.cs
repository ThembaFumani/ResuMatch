using ResuMatch.Api.DataAccess;
using ResuMatch.Pipelines;
//todo: implementation might change or extended (depending if there's a need to save files locally) once MongoDB is integrated
public class PersistResumeDataStep : IPipelineStep<PipelineContext, PipelineResult>
{
    private readonly ILogger<PersistResumeDataStep> _logger;
    private readonly IResumeRepository _resumeRepository;
    public PersistResumeDataStep(ILogger<PersistResumeDataStep> logger, IResumeRepository resumeRepository)
    {
        _logger = logger;
        _resumeRepository = resumeRepository ?? throw new ArgumentNullException(nameof(resumeRepository));
    }

    public async Task<PipelineResult> ProcessAsync(PipelineContext context)
    {
        _logger.LogInformation("Starting data persistence step.");

        if (context.File == null || context.File.Length == 0)
        {
            context.Error = "No file provided in context for persistence.";
            _logger.LogError(context.Error);
            context.AnalysisResult ??= new ResuMatch.Api.Models.AnalysisResult();
            context.AnalysisResult.Error = context.Error;
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }

        if (string.IsNullOrWhiteSpace(context.ExtractedResumeText))
        {
            context.Error = "Extracted resume text is empty in context for persistence.";
            _logger.LogError(context.Error);
            context.AnalysisResult ??= new ResuMatch.Api.Models.AnalysisResult();
            context.AnalysisResult.Error = context.Error;
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }

        if (string.IsNullOrWhiteSpace(context.JobDescription))
        {
            context.Error = "Job description is empty in context for persistence.";
            _logger.LogError(context.Error);
            context.AnalysisResult ??= new ResuMatch.Api.Models.AnalysisResult();
            context.AnalysisResult.Error = context.Error;
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }
        
        context.AnalysisResult ??= new ResuMatch.Api.Models.AnalysisResult();

        try
        {
            _logger.LogInformation("Persisting resume data via repository for file: {FileName}", context.File.FileName);
            var rusumeId = Guid.NewGuid().ToString();
            var resumeData = await _resumeRepository.PersistData(context.File, rusumeId, context.ExtractedResumeText, context.JobDescription, context.AnalysisResult);
            
            _logger.LogInformation("Data persisted successfully for file: {FileName}. External ID (if available): {ResumeId}", context.File.FileName, rusumeId);
           return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during data persistence for file: {FileName}", context.File.FileName);
            context.Error = $"Error persisting data: {ex.Message}";
            context.AnalysisResult.Error = context.Error;
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }
    }
}