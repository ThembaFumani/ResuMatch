using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResuMatch.Api.Models;
using ResuMatch.Api.Repositories;
using ResuMatch.Api.Services.Interfaces;

namespace ResuMatch.Api.Services.Concretes
{
    public class AnalysisService : IAnalysisService
    {
        private readonly ILogger<AnalysisService>? _logger;
        private readonly IResumeRepository? _resumeRepository;

        public async Task StoreAnalysisResultAsync(AnalysisRequest request, AnalysisResponse result, string filePath)
        {
            _logger?.LogInformation("Storing analysis result using AnalysisService.");
            if (_resumeRepository == null)
            {
                throw new InvalidOperationException("Resume repository is not initialized.");
            }
            await _resumeRepository.StoreAnalysisResult(request, result, filePath);
        }
    }
}