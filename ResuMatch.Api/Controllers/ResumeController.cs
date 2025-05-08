using Microsoft.AspNetCore.Mvc;
using ResuMatch.Api.Services.Interfaces;

namespace ResuMatch.Api.Controllers
{
    [ApiController]
    [Route("api/resume")]
    public class ResumeController : ControllerBase
    {
        private readonly ILogger<ResumeController>? _logger;
        private readonly IResumeAnalysisService? _resumeAnalysisService;

        public ResumeController(ILogger<ResumeController>? logger, IResumeAnalysisService? resumeAnalysisService)
        {
            _logger = logger;
            _resumeAnalysisService = resumeAnalysisService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadResume(IFormFile file, [FromForm] string jobDescription)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                var result = await _resumeAnalysisService?.ProcessResumeAsync(file, jobDescription);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error processing the resume.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}