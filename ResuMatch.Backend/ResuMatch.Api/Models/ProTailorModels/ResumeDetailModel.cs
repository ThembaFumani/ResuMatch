using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models.ProTailorModels
{
    public class ResumeDetailModel
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Contact")]
        public ContactInfo Contact { get; set; }

        [JsonPropertyName("Summary")]
        public string Summary { get; set; }

        [JsonPropertyName("Experience")]
        public List<ExperienceEntry> Experience { get; set; }

        [JsonPropertyName("Education")]
        public List<EducationEntry> Education { get; set; }

        [JsonPropertyName("Skills")]
        public List<SkillCategory>? Skills { get; set; }

        [JsonPropertyName("Projects")]
        public List<ProjectEntry>? Projects { get; set; }

        [JsonPropertyName("Certifications")]
        public List<string>? Certifications { get; set; }
 
        [JsonPropertyName("Awards")]
        public List<string>? Awards { get; set; }

        public ResumeDetailModel()
        {
            Name = string.Empty;
            Contact = new ContactInfo();
            Summary = string.Empty;
            Experience = new List<ExperienceEntry>();
            Education = new List<EducationEntry>();
        }
    }
}