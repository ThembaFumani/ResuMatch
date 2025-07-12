using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models.ProTailorModels
{
    public class ProcessTailoringRequest
    {
        [Required(ErrorMessage = "Resume ID is required.")]
        public required string ResumeId { get; set; }
        public string? JobDescriptionId { get; set; }
    }
}