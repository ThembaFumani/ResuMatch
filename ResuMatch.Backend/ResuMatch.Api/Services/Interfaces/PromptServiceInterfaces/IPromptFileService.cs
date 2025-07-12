using ResuMatch.Api.Models;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface IPromptFileService
    {
        Task<PromptFileModel> LoadPromptFileAsync();
    }
}