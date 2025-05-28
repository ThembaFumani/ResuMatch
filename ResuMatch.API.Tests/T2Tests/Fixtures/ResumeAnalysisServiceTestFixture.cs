
using Moq;
using ResuMatch.Api.Models;
using ResuMatch.Api.Tests.T2Tests.Fixtures;
using ResuMatch.Pipelines;

namespace ResuMatch.API.Tests.T2Tests.Fixtures
{
    public class ResumeAnalysisServiceTestFixture
    {
        public TestLoggerMocks LoggerMocks { get; private set; }
        public ExternalDependencyMocks ExternalDependencies { get; private set; }
        public PipelineStepMocks PipelineSteps { get; private set; }

        public ResumeAnalysisServiceTestFixture()
        {
            LoggerMocks = new TestLoggerMocks();
            ExternalDependencies = new ExternalDependencyMocks();
            PipelineSteps = new PipelineStepMocks();

            SetupDefaultPipelineStepMocks();
        }
        private void SetupDefaultPipelineStepMocks()
        {
            PipelineSteps.MockSaveResumeFileStep.Setup(s => s.ProcessAsync(It.IsAny<PipelineContext>()))
                .ReturnsAsync(new PipelineResult { AnalysisResult = new AnalysisResult() });
            PipelineSteps.MockExtractResumeTextStep.Setup(s => s.ProcessAsync(It.IsAny<PipelineContext>()))
                .ReturnsAsync(new PipelineResult {  AnalysisResult = new AnalysisResult()  });
            PipelineSteps.MockExtractResumeSkillStep.Setup(s => s.ProcessAsync(It.IsAny<PipelineContext>()))
                .ReturnsAsync(new PipelineResult {  AnalysisResult = new AnalysisResult()  });
            PipelineSteps.MockExtractJobDescriptionSkillsStep.Setup(s => s.ProcessAsync(It.IsAny<PipelineContext>()))
                .ReturnsAsync(new PipelineResult {  AnalysisResult = new AnalysisResult()  });
            PipelineSteps.MockMatchSkillsStep.Setup(s => s.ProcessAsync(It.IsAny<PipelineContext>()))
                .ReturnsAsync(new PipelineResult {  AnalysisResult = new AnalysisResult()  });
            PipelineSteps.MockGenerateSummaryStep.Setup(s => s.ProcessAsync(It.IsAny<PipelineContext>()))
                .ReturnsAsync(new PipelineResult {  AnalysisResult = new AnalysisResult()  });
            PipelineSteps.MockCalculateScoreStep.Setup(s => s.ProcessAsync(It.IsAny<PipelineContext>()))
                .ReturnsAsync(new PipelineResult {  AnalysisResult = new AnalysisResult()  });
        }

        public void ClearInvocations()
        {
            LoggerMocks.ClearInvocations();
            PipelineSteps.ClearInvocations();
            ExternalDependencies.ClearInvocations();
        }
        
    }
}