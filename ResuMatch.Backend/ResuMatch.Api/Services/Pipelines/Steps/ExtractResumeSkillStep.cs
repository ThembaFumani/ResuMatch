using System.Text.Json;
using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Pipelines;

public class ExtractResumeSkillStep : IPipelineStep<PipelineContext, PipelineResult>
{
    private readonly ILogger<ExtractResumeSkillStep> _logger;
    private readonly IAIService _aiService;

    public ExtractResumeSkillStep(ILogger<ExtractResumeSkillStep> logger, IAIService aiService)
    {
        _logger = logger;
        _aiService = aiService;
    }

    public async Task<PipelineResult> ProcessAsync(PipelineContext context)
    {
        if (context.ResumeSkills != null && context.ResumeSkills.Count > 0)
        {
            return new PipelineResult { AnalysisResult = context.AnalysisResult };
        }

        if (context.ExtractedResumeText == null)
        {
            throw new InvalidOperationException("Extracted resume text is null.");
        }

        var skillsJson = await _aiService.ExtractSkillsAsync(context.ExtractedResumeText);
        var skills = new List<string>();
        if (skillsJson != null && skillsJson.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            ExtractOpenRouteResponseSkills(skillsJson, skills);
        }
        else
        {
            ExtractLLMResponseSkills(skillsJson, skills);
        }
        context.ResumeSkills = skills;
        context.AnalysisResult.ExtractedResumeSkills = skills;
        return new PipelineResult { AnalysisResult = context.AnalysisResult };
    }

    private void ExtractLLMResponseSkills(JsonDocument? skillsJson, List<string> skills)
    {
            JsonElement root = skillsJson.RootElement;
            string skillsText = root.GetProperty("response").GetString()?.Trim() ?? string.Empty;

        throw new NotImplementedException();
    }

    private static void ExtractOpenRouteResponseSkills(System.Text.Json.JsonDocument skillsJson, List<string> skills)
    {
        foreach (var element in skillsJson.RootElement.EnumerateArray())
        {
            if (element.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                var skill = element.GetString();
                if (!string.IsNullOrEmpty(skill))
                {
                    skills.Add(skill);
                }
            }
        }
    }
}