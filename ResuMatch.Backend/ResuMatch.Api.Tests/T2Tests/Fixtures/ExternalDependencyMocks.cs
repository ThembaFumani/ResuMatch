using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using ResuMatch.Api.Models;
using ResuMatch.Api.Models.Configurations;
using ResuMatch.Api.Repositories;
using ResuMatch.Api.Services.Interfaces;

namespace ResuMatch.API.Tests.T2Tests.Fixtures
{
    public class ExternalDependencyMocks
    {
        public Mock<IHttpClientFactory>? MockHttpClientFactory { get; private set; }
        public Mock<IOptions<OpenRouterConfig>>? MockOpenRouterConfig { get; private set; }
        public Mock<IAnalysisService>? MockAnalysisService { get; private set; }
        public Mock<IResumeRepository>? MockResumeRepository { get; private set; }

        public ExternalDependencyMocks()
        {
            MockHttpClientFactory = new Mock<IHttpClientFactory>();
            MockOpenRouterConfig = new Mock<IOptions<OpenRouterConfig>>();
            MockOpenRouterConfig.Setup(o => o.Value)
                .Returns(new OpenRouterConfig
                {
                    Model = "test-model",
                    Endpoint = "http://mock-ai-endpoint.com",
                    ApiKey = "mock-key"
                });

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{ \"choices\": [ { \"message\": { \"content\": \"C#, .NET, MockedAI\" } } ] }")
                });
            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);
            MockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);


            MockAnalysisService = new Mock<IAnalysisService>();

            MockAnalysisService.Setup(s => s.StoreAnalysisResultAsync(It.IsAny<AnalysisRequest>(), It.IsAny<AnalysisResult>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            MockResumeRepository = new Mock<IResumeRepository>();

            MockResumeRepository.Setup(r => r.StoreAnalysisResult(It.IsAny<AnalysisRequest>(), It.IsAny<AnalysisResult>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

        }
        
        public void ClearInvocations()
        {
            MockHttpClientFactory?.Invocations.Clear();
            MockOpenRouterConfig?.Invocations.Clear();
            MockAnalysisService?.Invocations.Clear();
            MockResumeRepository?.Invocations.Clear();
        }
    }
}