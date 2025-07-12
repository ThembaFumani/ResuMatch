using ResuMatch.Api.Services.FileProccessing.Interfaces;
using ResuMatch.Pipelines;
using System;
using ResuMatch.Api.Models;
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
            _logger.LogError($"{nameof(context)} is null.");
            var analysisResult = new AnalysisResult
            {
                Error = "No file provided in the context for text extraction."
            };
            return new PipelineResult { AnalysisResult = analysisResult };
        }

        context.AnalysisResult ??= new AnalysisResult();
        if (context.File == null || context.File.Length == 0)
        {
            context.Error = "No file provided in the context for text extraction.";
            context.AnalysisResult.Error = context.Error;
            _logger.LogError("File is null in the context.");
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }
    
        if (string.IsNullOrEmpty(context.JobDescription))
        {
            context.Error = "Job description is null or empty in the context.";
            _logger.LogError("Job description is null or empty in the context.");
            context.AnalysisResult.Error = context.Error;
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }

        _logger.LogInformation("Extracting text from file: {FilePath}", context.File.FileName);
       
        IFileProcessor fileProcessor;
        try
        {
            fileProcessor =  _fileProcessorFactory.CreateProcessor(context.File.FileName);
        }
        catch (Exception ex)
        {
            context.Error = $"Error creating file processor: {ex.Message}";
            _logger.LogError(ex, $"Error creating file processor for file: {context.File.FileName}");
            context.AnalysisResult.Error = context.Error;
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }
        if (fileProcessor == null)
        {
            throw new InvalidOperationException("File processor is not initialized.");
        }

        try
        {
            // The fileProcessor is now responsible for handling the IFormFile.
            context.ExtractedResumeText = await fileProcessor.ExtractTextAsync(context.File);

            _logger.LogInformation("Text extraction completed for file: {FileName}. Extracted text length: {Length}",
                context.File.FileName, context.ExtractedResumeText?.Length ?? 0);

            if (string.IsNullOrEmpty(context.ExtractedResumeText))
            {
                _logger.LogWarning("Extracted text is null or empty for file: {FileName}", context.File.FileName);
                context.Error = "Failed to extract any text from the resume.";
                context.AnalysisResult.Error = context.Error;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during text extraction for file: {FileName}", context.File.FileName);
            context.Error = $"Error extracting text from resume '{context.File.FileName}': {ex.Message}";
            context.AnalysisResult.Error = context.Error;
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }
        return new PipelineResult { AnalysisResult = context.AnalysisResult };
    }
}