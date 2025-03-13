using System.Net.Mime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models
{
    public class MatchResult
    {
        public string? ResumeId { get; set; }
        public double MatchPercentage { get; set; }
        public List<string>? MissingSkills { get; set; }
    }
}