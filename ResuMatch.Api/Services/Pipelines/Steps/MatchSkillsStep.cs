using System.Text.Json;
using ResuMatch.Api.Models;
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

    public async Task<PipelineResult> ProcessAsync(PipelineContext context)
    {
        if (context.ResumeSkills == null || !context.ResumeSkills.Any())
        {
            context.Error = "Cannot match skills.  ResumeSkills or JobDescriptionSkills is null.";
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }

        if (context.JobDescriptionSkills == null || !context.JobDescriptionSkills.Any())
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
            var response = await _aiService.GetMatchingSkillsAnalysisAsync(context.ResumeSkills, context.JobDescriptionSkills);
            
            string innerJsonContent = string.Empty;
            using (JsonDocument doc = JsonDocument.Parse(response))
            {
                if (doc.RootElement.TryGetProperty("choices", out JsonElement choicesElement))
                {
                    var choicesEnumerator = choicesElement.EnumerateArray();
                    if (choicesEnumerator.MoveNext())
                    {
                        var firstChoice = choicesEnumerator.Current;
                        if (firstChoice.TryGetProperty("message", out JsonElement messageElement) &&
                            messageElement.TryGetProperty("content", out JsonElement contentElement))
                        {
                            innerJsonContent = contentElement.GetString() ?? string.Empty;
                        }
                        else
                        {
                            _logger.LogError("AI response did not contain expected 'choices[0].message.content' structure. Raw response: {Response}", response);
                            throw new JsonException("AI response did not contain expected 'choices[0].message.content' structure.");
                        }
                    }
                    else
                    {
                        _logger.LogError("AI response 'choices' array was empty. Raw response: {Response}", response);
                        throw new JsonException("AI response 'choices' array was empty.");
                    }
                }
                else
                {
                    _logger.LogError("AI response did not contain expected 'choices[0].message.content' structure. Raw response: {Response}", response);
                    throw new JsonException("AI response did not contain expected 'choices[0].message.content' structure.");
                }
            }

            if (string.IsNullOrWhiteSpace(innerJsonContent))
            {
                throw new JsonException("AI response content was empty or null after extraction.");
            }

            using (JsonDocument innerDoc = JsonDocument.Parse(innerJsonContent))
            {
                var innerRoot = innerDoc.RootElement;
                List<string> matchingSkills = new List<string>();
                List<string> missingSkills = new List<string>();

                if (innerRoot.TryGetProperty("matching_skills", out JsonElement matchingSkillsElement) &&
                    matchingSkillsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var skillElement in matchingSkillsElement.EnumerateArray())
                    {
                        matchingSkills.Add(skillElement.GetString() ?? string.Empty);
                    }
                }
                else
                {
                     _logger.LogWarning("AI response content did not contain 'matching_skills' array or it was not an array. Raw inner content: {InnerContent}", innerJsonContent);
                }


                if (innerRoot.TryGetProperty("missing_skills", out JsonElement missingSkillsElement) &&
                    missingSkillsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var skillElement in missingSkillsElement.EnumerateArray())
                    {
                        missingSkills.Add(skillElement.GetString() ?? string.Empty);
                    }
                }
                else
                {
                    _logger.LogWarning("AI response content did not contain 'missing_skills' array or it was not an array. Raw inner content: {InnerContent}", innerJsonContent);
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error matching skills.");
            context.Error = "An error occurred while matching skills.";
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }           
    }
}