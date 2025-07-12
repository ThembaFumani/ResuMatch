using Moq;
using Microsoft.Extensions.Logging;
using ResuMatch.Api.Services.Concretes;
using ResuMatch.Api.Repositories;

namespace ResuMatch.Api.Tests.T2Tests.Fixtures
{
    public class TestLoggerMocks
    {
        public Mock<ILogger<ResumeAnalysisService>> MockServiceLogger { get; private set; }
        public Mock<ILogger<SaveResumeFileStep>> MockSaveFileStepLogger { get; private set; }
        public Mock<ILogger<ExtractResumeTextStep>> MockExtractTextStepLogger { get; private set; }
        public Mock<ILogger<ExtractResumeSkillStep>> MockExtractResumeSkillStepLogger { get; private set; }
        public Mock<ILogger<ExtractJobDescriptionSkillsStep>> MockExtractJobDescriptionSkillsStepLogger { get; private set; }
        public Mock<ILogger<MatchSkillsStep>> MockMatchSkillsStepLogger { get; private set; }
        public Mock<ILogger<GenerateSummaryStep>> MockGenerateSummaryStepLogger { get; private set; }
        public Mock<ILogger<CalculateScoreStep>> MockCalculateScoreStepLogger { get; private set; }
        public Mock<ILogger<ResumeRepository>> MockResumeRepositoryLogger { get; private set; }
        public Mock<ILogger<AnalysisService>> MockAnalysisServiceLogger { get; private set; }

        public TestLoggerMocks()
        {
            MockServiceLogger = new Mock<ILogger<ResumeAnalysisService>>();
            MockSaveFileStepLogger = new Mock<ILogger<SaveResumeFileStep>>();
            MockExtractTextStepLogger = new Mock<ILogger<ExtractResumeTextStep>>();
            MockExtractResumeSkillStepLogger = new Mock<ILogger<ExtractResumeSkillStep>>();
            MockExtractJobDescriptionSkillsStepLogger = new Mock<ILogger<ExtractJobDescriptionSkillsStep>>();
            MockMatchSkillsStepLogger = new Mock<ILogger<MatchSkillsStep>>();
            MockGenerateSummaryStepLogger = new Mock<ILogger<GenerateSummaryStep>>();
            MockCalculateScoreStepLogger = new Mock<ILogger<CalculateScoreStep>>();
            MockResumeRepositoryLogger = new Mock<ILogger<ResumeRepository>>();
            MockAnalysisServiceLogger = new Mock<ILogger<AnalysisService>>();
        }

        public void ClearInvocations()
        {
            MockServiceLogger.Invocations.Clear();
            MockSaveFileStepLogger.Invocations.Clear();
            MockExtractTextStepLogger.Invocations.Clear();
            MockExtractResumeSkillStepLogger.Invocations.Clear();
            MockExtractJobDescriptionSkillsStepLogger.Invocations.Clear();
            MockMatchSkillsStepLogger.Invocations.Clear();
            MockGenerateSummaryStepLogger.Invocations.Clear();
            MockCalculateScoreStepLogger.Invocations.Clear();
            MockResumeRepositoryLogger.Invocations.Clear();
            MockAnalysisServiceLogger.Invocations.Clear();
        }
    }
}