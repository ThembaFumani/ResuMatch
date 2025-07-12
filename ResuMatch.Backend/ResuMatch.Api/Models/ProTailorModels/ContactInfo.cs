using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models.ProTailorModels
{
    public class ContactInfo
    {
        [JsonPropertyName("Phone")]
        public string? Phone { get; set; }
        [JsonPropertyName("Email")]
        public string? Email { get; set; }
        [JsonPropertyName("Location")]
        public string? Location { get; set; }
        [JsonPropertyName("LinkedInUrl")]
        public string? LinkedInUrl { get; set; }
        [JsonPropertyName("GitHubUrl")]
        public string? GitHubUrl { get; set; }  
    }
}