namespace ResuMatch.Api.Services.Interfaces
{
    public interface IFileProcessor
    {
        Task<string> ExtractTextAsync(string filePath);
    }
}         