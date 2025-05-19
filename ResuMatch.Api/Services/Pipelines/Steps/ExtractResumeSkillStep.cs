using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Pipelines;

public class ExtractResumeSkillStep : IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisPipelineResult>
{
    private readonly ILogger<ExtractResumeSkillStep> _logger;
    private readonly IAIService _aiService;

    public ExtractResumeSkillStep(ILogger<ExtractResumeSkillStep> logger, IAIService aiService)
    {
        _logger = logger;
        _aiService = aiService;
    }

    public async Task<ResumeAnalysisPipelineResult> ProcessAsync(ResumeAnalysisContext context)
    {
        if (context.ResumeSkills != null && context.ResumeSkills.Count > 0)
        {
            return new ResumeAnalysisPipelineResult { AnalysisResult = context.AnalysisResult };
        }

        if (context.ExtractedResumeText == null)
        {
            throw new InvalidOperationException("Extracted resume text is null.");
        }

        var skillsJson = await _aiService.ExtractSkillsAsync(context.ExtractedResumeText);
        var skills = new List<string>();
        if (skillsJson != null && skillsJson.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            foreach (var element in skillsJson.RootElement.EnumerateArray())
            {
                if (element.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    skills.Add(element.GetString());
                }
            }
        }
        context.ResumeSkills = skills;

        return new ResumeAnalysisPipelineResult { AnalysisResult = context.AnalysisResult };
    }
}