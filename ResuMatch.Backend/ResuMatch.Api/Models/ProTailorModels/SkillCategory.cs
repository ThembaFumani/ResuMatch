using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models.ProTailorModels
{
    public class SkillCategory
    {
        [JsonPropertyName("CategoryName")]
        public string? CategoryName { get; set; }
        [JsonPropertyName("Skills")]
        public List<string>? Skills { get; set; }
    }
}