using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models
{
    public class AnalysisRequest
    {
        public string? ResumeText { get; set; }
        public string? JobDescriptionText { get; set; } 
    }
}