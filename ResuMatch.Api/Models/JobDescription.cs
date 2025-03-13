using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models
{
    public class JobDescription
    {
        public string? Title { get; set; }
        public string Description { get; set; }
        public List<string> RequiredSkills { get; set; }
    }
}