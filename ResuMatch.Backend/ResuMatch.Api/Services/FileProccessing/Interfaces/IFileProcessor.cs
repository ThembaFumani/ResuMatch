using System.IO;

namespace ResuMatch.Api.Services.FileProccessing.Interfaces
{
    public interface IFileProcessor
    {
        // Task<string> ExtractTextAsync(string filePath);
        Task<string> ExtractTextAsync(IFormFile file, CancellationToken cancellationToken = default);
    }
}