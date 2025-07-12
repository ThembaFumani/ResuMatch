using ResuMatch.Api.Models.ProTailorModels;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface IProTailorService
    {
        Task<ResumeDetailModel> ProcessFullTailoringAsync(string resumeId, string jobDescriptionId);
        Task<TailorSectionResponse> TailorResumeSectionAsync(string resumeId, string jobDescriptionId, string sectionType);

    }
}