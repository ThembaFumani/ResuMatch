using Moq;
using ResuMatch.Api.Models;
using ResuMatch.Pipelines;

namespace ResuMatch.API.Tests.T2Tests.Fixtures
{
    public class PipelineStepMocks
    {
        public Mock<IPipelineStep<PipelineContext, PipelineResult>>? MockSaveResumeFileStep { get; private set; }
        public Mock<IPipelineStep<PipelineContext, PipelineResult>>? MockExtractResumeTextStep { get; private set; }
        public Mock<IPipelineStep<PipelineContext, PipelineResult>>? MockExtractResumeSkillStep { get; private set; }
        public Mock<IPipelineStep<PipelineContext, PipelineResult>>? MockExtractJobDescriptionSkillsStep { get; private set; }
        public Mock<IPipelineStep<PipelineContext, PipelineResult>>? MockMatchSkillsStep { get; private set; }
        public Mock<IPipelineStep<PipelineContext, PipelineResult>>? MockGenerateSummaryStep { get; private set; }
        public Mock<IPipelineStep<PipelineContext, PipelineResult>>? MockCalculateScoreStep { get; private set; }
   
        public PipelineStepMocks()
        {
            MockSaveResumeFileStep = new Mock<IPipelineStep<PipelineContext, PipelineResult>>();
            MockExtractResumeTextStep = new Mock<IPipelineStep<PipelineContext, PipelineResult>>();
            MockExtractResumeSkillStep = new Mock<IPipelineStep<PipelineContext, PipelineResult>>();
            MockExtractJobDescriptionSkillsStep = new Mock<IPipelineStep<PipelineContext, PipelineResult>>();
            MockMatchSkillsStep = new Mock<IPipelineStep<PipelineContext, PipelineResult>>();
            MockGenerateSummaryStep = new Mock<IPipelineStep<PipelineContext, PipelineResult>>();
            MockCalculateScoreStep = new Mock<IPipelineStep<PipelineContext, PipelineResult>>();
        }
        public void ClearInvocations()
        {
            MockSaveResumeFileStep?.Invocations.Clear();
            MockExtractResumeTextStep?.Invocations.Clear();
            MockExtractResumeSkillStep?.Invocations.Clear();
            MockExtractJobDescriptionSkillsStep?.Invocations.Clear();
            MockMatchSkillsStep?.Invocations.Clear();
            MockGenerateSummaryStep?.Invocations.Clear();
            MockCalculateScoreStep?.Invocations.Clear();
        }
    }
}