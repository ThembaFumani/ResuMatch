using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models.ProTailorModels
{
    public class TailorSectionResponse
    {
        [JsonPropertyName("TailoredSection")]
        public string TailoredSection { get; set; } = string.Empty;
    }
}