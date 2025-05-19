using ResuMatch.Api.Services.Interfaces;
using ResuMatch.Pipelines;

public class ExtractJobDescriptionSkillsStep : IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisPipelineResult>
{
    private readonly IAIService _aiService;
    private readonly ILogger<ExtractJobDescriptionSkillsStep> _logger;
    public ExtractJobDescriptionSkillsStep(IAIService aiService, ILogger<ExtractJobDescriptionSkillsStep> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<ResumeAnalysisPipelineResult> ProcessAsync(ResumeAnalysisContext context)
    {
        if (string.IsNullOrEmpty(context.JobDescription))
        {
            _logger.LogWarning("Job description is null or empty.");
            context.Error = "Job description is null or empty. Cannot extract skills.";
            return new ResumeAnalysisPipelineResult { AnalysisResult = context.AnalysisResult };
        }

        var skillsJson = await _aiService.ExtractSkillsAsync(context.JobDescription);
        context.JobDescriptionSkills = skillsJson.RootElement.EnumerateArray()
            .Select(element => element.GetString())
            .Where(skill => !string.IsNullOrEmpty(skill))
            .Cast<string>()
            .ToList();

        return new ResumeAnalysisPipelineResult { AnalysisResult = context.AnalysisResult };
    }
}