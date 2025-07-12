using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ResuMatch.Api.Services.Interfaces
{
    public interface IHttpHandler
    {
        Task<string> GetAsync(string url, CancellationToken cancellationToken = default);
        Task<string> PostAsync<TRequest>(string url, TRequest data, CancellationToken cancellationToken = default);
        Task<string> PostMultipartAsync(string url, MultipartFormDataContent content, CancellationToken cancellationToken = default);
        Task<string> PutAsync<TRequest>(string url, TRequest data, CancellationToken cancellationToken = default);
        Task<string> DeleteAsync(string url, CancellationToken cancellationToken = default);
    }
}