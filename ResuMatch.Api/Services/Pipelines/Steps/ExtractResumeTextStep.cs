using ResuMatch.Api.Services.FileProccessing.Interfaces;
using ResuMatch.Pipelines;

public class ExtractResumeTextStep : IPipelineStep<PipelineContext, PipelineResult>
{
    private readonly IFileProcessorFactory _fileProcessorFactory;
    private readonly ILogger<ExtractResumeTextStep> _logger;
    public ExtractResumeTextStep(IFileProcessorFactory fileProcessorFactory, ILogger<ExtractResumeTextStep> logger)
    {
        _fileProcessorFactory = fileProcessorFactory;
        _logger = logger;
    }

    public async Task<PipelineResult> ProcessAsync(PipelineContext context)
    {
        _logger.LogInformation("Starting text extraction from file: {FilePath}", context.FilePath);
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context), "ResumeAnalysisContext cannot be null.");
        }
        if (string.IsNullOrEmpty(context.FilePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(context.FilePath));
        }
        if (string.IsNullOrEmpty(context.JobDescription))
        {
            throw new ArgumentException("Job description cannot be null or empty.", nameof(context.JobDescription));
        }

        _logger.LogInformation("Extracting text from file: {FilePath}", context.FilePath);
         IFileProcessor fileProcessor = _fileProcessorFactory.CreateProcessor(context.FilePath);
        if (fileProcessor == null)
        {
            throw new InvalidOperationException("File processor is not initialized.");
        }
        context.ExtractedResumeText = await fileProcessor.ExtractTextAsync(context.FilePath);
        _logger.LogInformation("Text extracted successfully from file: {FilePath}", context.FilePath);

        return new PipelineResult { AnalysisResult = context.AnalysisResult };
    }
}