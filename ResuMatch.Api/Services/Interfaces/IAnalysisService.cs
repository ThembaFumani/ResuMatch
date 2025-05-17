using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface IAnalysisService
    {
        Task StoreAnalysisResultAsync(AnalysisRequest request, AnalysisResult result, string filePath);
    }
}