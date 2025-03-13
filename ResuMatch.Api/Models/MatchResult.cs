using System.Net.Mime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models
{
    public class MatchResult
    {
        public int Score { get; set; }
        public List<string>? MissingSkills { get; set; }
    }
}