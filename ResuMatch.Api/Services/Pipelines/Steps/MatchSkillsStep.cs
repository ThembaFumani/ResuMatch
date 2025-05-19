using ResuMatch.Pipelines;

public class MatchSkillsStep : IResumeAnalysisPipelineStep<ResumeAnalysisContext, ResumeAnalysisPipelineResult>
{
    private readonly ILogger<MatchSkillsStep> _logger;

        public MatchSkillsStep(ILogger<MatchSkillsStep> logger)
        {
            _logger = logger;
        }

        public Task<ResumeAnalysisPipelineResult> ProcessAsync(ResumeAnalysisContext context)
        {
            if (context.ResumeSkills == null || context.JobDescriptionSkills == null)
            {
                context.Error = "Cannot match skills.  ResumeSkills or JobDescriptionSkills is null.";
                return Task.FromResult(new ResumeAnalysisPipelineResult { AnalysisResult = context.AnalysisResult });
            }

            context.MatchingSkills = context.ResumeSkills
                .Intersect(context.JobDescriptionSkills, StringComparer.OrdinalIgnoreCase)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            context.MissingSkills = context.JobDescriptionSkills
                .Except(context.ResumeSkills, StringComparer.OrdinalIgnoreCase)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            _logger.LogInformation("Skills matched.");
            
            return Task.FromResult(new ResumeAnalysisPipelineResult { AnalysisResult = context.AnalysisResult });
        }
}