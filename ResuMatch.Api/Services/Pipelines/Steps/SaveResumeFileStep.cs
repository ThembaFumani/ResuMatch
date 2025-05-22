using ResuMatch.Pipelines;

public class SaveResumeFileStep : IPipelineStep<PipelineContext, PipelineResult>
{
    private readonly ILogger<SaveResumeFileStep> _logger;
    private string _uploadDirectory;
    public SaveResumeFileStep(ILogger<SaveResumeFileStep> logger, string uploadDirectory)
    {
        _logger = logger;
        _uploadDirectory = uploadDirectory;
    }

    public async Task<PipelineResult> ProcessAsync(PipelineContext context)
    {
         if (context.File == null || context.File.Length == 0)
            {
                context.Error = "No file provided.";
                _logger.LogError("No file was provided for upload.");
                return new PipelineResult { AnalysisResult = context.AnalysisResult };
            }

            try
            {
                _logger.LogDebug("Attempting to save file. Upload directory: {UploadDirectory}", _uploadDirectory);

                // Ensure the upload directory exists
                if (!Directory.Exists(_uploadDirectory))
                {
                    Directory.CreateDirectory(_uploadDirectory);
                    _logger.LogInformation("Created upload directory: {UploadDirectory}", _uploadDirectory);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(context.File.FileName);
                context.FilePath = Path.Combine(_uploadDirectory, uniqueFileName); // Set FilePath in context

                _logger.LogDebug("Saving file to path: {FilePath}", context.FilePath);
                using (var stream = new FileStream(context.FilePath, FileMode.Create))
                {
                    await context.File.CopyToAsync(stream); // Save the file
                }
                _logger.LogInformation("File uploaded successfully: {FilePath}", context.FilePath);
                return new PipelineResult { AnalysisResult = context.AnalysisResult };
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error saving file: {FilePath}", context.FilePath);
                context.Error = $"Error saving file: {ex.Message}";
                return new PipelineResult { AnalysisResult = context.AnalysisResult };
            }
            catch (Exception ex) // Catch any other unexpected exceptions
            {
                _logger.LogError(ex, "An unexpected error occurred while saving file: {FilePath}", context.FilePath);
                context.Error = $"An unexpected error occurred: {ex.Message}";
                return new PipelineResult { AnalysisResult = context.AnalysisResult };
            }
    }
}