using ResuMatch.Api.Services.FileProccessing.Interfaces;

namespace ResuMatch.Api.Services.FileProccessing.Concretes
{
    public class FileProcessorFactory : IFileProcessorFactory
    {
        private readonly ILoggerFactory? _loggerFactory;
        public FileProcessorFactory(ILoggerFactory? loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IFileProcessor CreateProcessor(string filePath)
        {
            var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
            switch (fileExtension)
            {
                case ".pdf":
                    return new PdfFileProcessor(_loggerFactory?.CreateLogger<PdfFileProcessor>());
                
                // Add cases for other file types if needed
                default:
                    throw new NotSupportedException($"File type '{fileExtension}' is not supported.");
            }
        }
    }
}