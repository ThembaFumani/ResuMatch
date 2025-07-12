using System.ComponentModel.DataAnnotations;

namespace ResuMatch.Api.Models.ProTailorModels
{
    public class TailorSectionByIdRequest
    {
        [Required(ErrorMessage = "Resume ID is required.")]
        public required string ResumeId { get; set; }

        [Required(ErrorMessage = "Job Description ID is required.")]
        public required string JobDescriptionId { get; set; }

        [Required(ErrorMessage = "Section Type is required (e.g., 'summary', 'experience_bullet').")]
        public required string SectionType { get; set; }
    }
}