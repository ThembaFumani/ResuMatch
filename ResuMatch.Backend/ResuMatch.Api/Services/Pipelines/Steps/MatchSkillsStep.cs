using System.Text.Json;
using ResuMatch.Api.Models;
using System.Text.RegularExpressions;
using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Pipelines;

public class MatchSkillsStep :  IPipelineStep<PipelineContext, PipelineResult>
{
    private readonly ILogger<MatchSkillsStep> _logger;
    private readonly IAIService _aiService;
    public MatchSkillsStep(ILogger<MatchSkillsStep> logger, IAIService aIService)
    {
        _logger = logger;
        _aiService = aIService;
    }

    private static string ExtractJsonContentFromMarkdown(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return string.Empty;
        }
        var trimmedContent = content.Trim();
        Match match = Regex.Match(trimmedContent, @"```(?:json)?\s*([\s\S]*?)\s*```", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        if (match.Success && match.Groups.Count > 1)
        {
            return match.Groups[1].Value.Trim();
        }
        return trimmedContent;
    }

    public async Task<PipelineResult> ProcessAsync(PipelineContext context)
    {
        if (context.ExtractedResumeText == null || !context.ExtractedResumeText.Any())
        {
            context.Error = "Cannot match skills. ResumeSkills or JobDescriptionSkills is null.";
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }

        if (context.JobDescription == null || !context.JobDescription.Any())
        {
            context.Error = "Cannot match skills. JobDescriptionSkills is null.";
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }

        if (context.AnalysisResult == null)
        {
            context.AnalysisResult = new AnalysisResult();
        }

        try
        {
            var rawAiServiceResponse = await _aiService.GetMatchingSkillsAnalysisAsync(context.ResumeSkills, context.JobDescriptionSkills);
            string contentForSkillsExtraction;

            try
            {
                using (JsonDocument doc = JsonDocument.Parse(rawAiServiceResponse))
                {
                    JsonElement root = doc.RootElement;
                    if (root.TryGetProperty("choices", out JsonElement choicesElement) &&
                        choicesElement.ValueKind == JsonValueKind.Array &&
                        choicesElement.GetArrayLength() > 0)
                    {
                        var firstChoice = choicesElement[0];
                        if (firstChoice.TryGetProperty("message", out JsonElement messageElement) &&
                            messageElement.TryGetProperty("content", out JsonElement contentElement))
                        {
                            contentForSkillsExtraction = contentElement.GetString() ?? string.Empty;
                        }
                        else
                        {
                            _logger.LogWarning("AI response 'choices[0].message.content' not found. Using raw response as content.");
                            contentForSkillsExtraction = rawAiServiceResponse; // Fallback
                        }
                    }
                    else if (root.TryGetProperty("response", out JsonElement responseElement))
                    {
                        contentForSkillsExtraction = responseElement.GetString() ?? string.Empty;
                    }
                    else
                    {
                        _logger.LogInformation("AI response does not have 'choices' or 'response' property. Assuming raw response is the direct content.");
                        contentForSkillsExtraction = rawAiServiceResponse;
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Raw AI response could not be parsed as a JSON object. Assuming it's direct content (possibly markdown-wrapped). Raw response: {RawResponse}", rawAiServiceResponse);
                contentForSkillsExtraction = rawAiServiceResponse;
            }

            if (string.IsNullOrWhiteSpace(contentForSkillsExtraction))
            {
                _logger.LogError("Extracted content for skills is empty or whitespace. Raw AI Response: {RawResponse}", rawAiServiceResponse);
                throw new JsonException("Extracted content for skills was empty or null after initial extraction stage.");
            }

            string cleanedSkillsJson = ExtractJsonContentFromMarkdown(contentForSkillsExtraction);

            if (string.IsNullOrWhiteSpace(cleanedSkillsJson))
            {
                _logger.LogError("Skills JSON content became empty after markdown cleaning. Original extracted content: {OriginalExtractedContent}, Raw AI Response: {RawResponse}", contentForSkillsExtraction, rawAiServiceResponse);
                throw new JsonException("Skills JSON content was empty or null after markdown cleaning.");
            }

            using (JsonDocument skillsDoc = JsonDocument.Parse(cleanedSkillsJson))
            {
                var skillsRoot = skillsDoc.RootElement;
                List<string> matchingSkills = new List<string>();
                List<string> missingSkills = new List<string>();

                if (skillsRoot.TryGetProperty("matching_skills", out JsonElement matchingSkillsElement) &&
                    matchingSkillsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var skillElement in matchingSkillsElement.EnumerateArray())
                        matchingSkills.Add(skillElement.GetString() ?? string.Empty);
                }
                else
                {
                    _logger.LogWarning("No 'matching_skills' array found in AI response.");
                }

                if (skillsRoot.TryGetProperty("missing_skills", out JsonElement missingSkillsElement) &&
                    missingSkillsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var skillElement in missingSkillsElement.EnumerateArray())
                        missingSkills.Add(skillElement.GetString() ?? string.Empty);
                }
                else
                {
                    _logger.LogWarning("No 'missing_skills' array found in AI response.");
                }

                context.AnalysisResult.MatchingSkills = matchingSkills.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                context.AnalysisResult.MissingSkills = missingSkills.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            }

            context.MatchingSkills = context.AnalysisResult.MatchingSkills;
            context.MissingSkills = context.AnalysisResult.MissingSkills;

            return new PipelineResult
            {
                AnalysisResult = context.AnalysisResult
            };
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, "JSON parsing error in MatchSkillsStep. Error: {ErrorMessage}", jsonEx.Message);
            context.Error = $"Error parsing skills analysis JSON: {jsonEx.Message}";
            context.AnalysisResult ??= new AnalysisResult();
            context.AnalysisResult.Error = context.Error;
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error matching skills.");
            context.Error = "An error occurred while matching skills.";
            context.AnalysisResult ??= new AnalysisResult();
            context.AnalysisResult.Error = context.Error;
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }
    }
}