using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models.Configurations
{
    public class OpenRouterConfig
    {
        public string? ApiKey { get; set; }
        public string? Model { get; set; }
        public string? Endpoint { get; set; }
    }
}