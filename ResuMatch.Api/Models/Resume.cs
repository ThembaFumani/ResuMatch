using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models
{
    public class Resume
    {
        public string? Id { get; set; }
        public string? Text { get; set; }
        public List<string>? Skills { get; set; }   
    }
}