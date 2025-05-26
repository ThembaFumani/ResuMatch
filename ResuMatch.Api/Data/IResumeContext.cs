using MongoDB.Driver;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Data
{
    public interface IResumeContext
    {
        IMongoCollection<ResumeData> ResumeData { get; }
    }
}