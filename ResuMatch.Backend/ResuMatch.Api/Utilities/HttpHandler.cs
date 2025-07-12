using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ResuMatch.Api.Services.Interfaces;

namespace ResuMatch.Api.Services.Concrete
{
    public class HttpHandler : IHttpHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpHandler> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public HttpHandler(IHttpClientFactory httpClientFactory, ILogger<HttpHandler> logger, JsonSerializerOptions jsonSerializerOptions)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        private async Task<string> SendRequestAsync(
            HttpMethod method,
            string url,
            HttpContent? content = null,
            CancellationToken cancellationToken = default
        )
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(1000); 
            var request = new HttpRequestMessage(method, url);

            if (content != null)
            {
                request.Content = content;
            }

            try
            {
                var response = await client.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed to URL '{Url}': {Message}", url, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during HTTP request to URL '{Url}': {Message}", url, ex.Message);
                throw;
            }
        }

        public async Task<string> GetAsync(string url, CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync(HttpMethod.Get, url, null, cancellationToken);
        }

        public async Task<string> PostAsync<TRequest>(string url, TRequest data, CancellationToken cancellationToken = default)
        {
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(data, _jsonSerializerOptions),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            return await SendRequestAsync(
                HttpMethod.Post,
                url,
                jsonContent,
                cancellationToken
            );
        }

        public async Task<string> PostMultipartAsync(string url, MultipartFormDataContent content, CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync(HttpMethod.Post, url, content, cancellationToken);
        }

        public async Task<string> PutAsync<TRequest>(string url, TRequest data, CancellationToken cancellationToken = default)
        {
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(data, _jsonSerializerOptions),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            return await SendRequestAsync(
                HttpMethod.Put,
                url,
                jsonContent,
                cancellationToken
            );
        }

        public async Task<string> DeleteAsync(string url, CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync(HttpMethod.Delete, url, null, cancellationToken);
        }
    }
}