using ResuMatch.Api.Services.FileProccessing.Interfaces;
using ResuMatch.Pipelines;

public class ExtractResumeTextStep : IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisContext>
{
    private readonly IFileProcessor _fileProcessor;
    private readonly ILogger<ExtractResumeTextStep> _logger;
    public ExtractResumeTextStep(IFileProcessor fileProcessorFactory, ILogger<ExtractResumeTextStep> logger)
    {
        _fileProcessor = fileProcessorFactory;
        _logger = logger;
    }

    public Task<ResumeAnalysisContext> ProcessAsync(ResumeAnalysisContext context)
    {
        // IFileProcessor fileProcessor = _fileProcessor.CreateProcessor(context.FilePath);
        // if (fileProcessor == null)
        // {
        //     throw new InvalidOperationException("File processor is not initialized.");
        // }
        // context.ExtractedResumeText = await _fileProcessor.ExtractTextAsync(context.FilePath);
        // _logger.LogInformation("Text extracted successfully from file: {FilePath}", context.FilePath);
        // return context;
        throw new NotImplementedException("File processing is not implemented yet.");
    }
}