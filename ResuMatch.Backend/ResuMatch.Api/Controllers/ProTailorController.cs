using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ResuMatch.Api.Models.ProTailorModels;
using ResuMatch.Api.Services.Interfaces;

namespace ResuMatch.Api.Controllers
{
    [ApiController]
    [Route("api/protailor")]
    public class ProTailorController : ControllerBase
    {
        private readonly IProTailorService _proTailorService;
        private readonly ILogger<ProTailorController> _logger;

        public ProTailorController(IProTailorService proTailorService, ILogger<ProTailorController> logger)
        {
            _proTailorService = proTailorService ?? throw new ArgumentNullException(nameof(proTailorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("process-full-tailoring")]
        [ProducesResponseType(typeof(ResumeDetailModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProcessFullTailoring([FromBody] ProcessTailoringRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _logger.LogInformation("ProTailor Controller: Received request to Process Full Tailoring for Resume ID: {ResumeId}, JD ID: {JobDescriptionId}", request.ResumeId, request.JobDescriptionId);
                ResumeDetailModel tailoredResumeDetail = await _proTailorService.ProcessFullTailoringAsync(request.ResumeId, request.JobDescriptionId);

                if (tailoredResumeDetail == null)
                {
                    _logger.LogWarning("ProTailor Service returned null for full tailoring process, likely due to an upstream issue.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Full tailoring process failed: no tailored resume generated.");
                }
                return Ok(tailoredResumeDetail);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "ProTailor Controller: Service-level error during full tailoring process: {Message}", ex.Message);
                if (ex.Message.Contains("not found") || ex.Message.Contains("missing"))
                {
                    return NotFound(ex.Message);
                }
                return BadRequest(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "ProTailor Controller: HTTP request error during full tailoring process.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"External API communication error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProTailor Controller: Critical error during Process Full Tailoring for Resume ID: {ResumeId}, JD ID: {JobDescriptionId}", request?.ResumeId, request?.JobDescriptionId);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("tailor-section-by-id")]
        [ProducesResponseType(typeof(TailorSectionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TailorResumeSectionById([FromBody] TailorSectionByIdRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _logger.LogInformation("ProTailor Controller: Received request to tailor resume section by ID. Resume ID: {ResumeId}, JD ID: {JobDescriptionId}, Section Type: {SectionType}", request.ResumeId, request.JobDescriptionId, request.SectionType);

                // Delegate the entire operation, including data fetching, to the ProTailorService
                TailorSectionResponse response = await _proTailorService.TailorResumeSectionAsync(
                    request.ResumeId,
                    request.JobDescriptionId,
                    request.SectionType
                );

                if (string.IsNullOrWhiteSpace(response.TailoredSection))
                {
                    _logger.LogWarning("ProTailor Service returned empty tailored content for section tailoring request.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to tailor resume section: AI service returned no content.");
                }
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "ProTailor Controller: Service-level error during section tailoring: {Message}", ex.Message);
                if (ex.Message.Contains("not found") || ex.Message.Contains("missing"))
                {
                    return NotFound(ex.Message);
                }
                return BadRequest(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "ProTailor Controller: HTTP request error during section tailoring.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"External API communication error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProTailor Controller: Critical error during Tailor Resume Section by ID.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}