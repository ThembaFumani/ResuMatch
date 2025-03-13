using MongoDB.Driver;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Data.JobDescriptionsDataContext
{
    public interface IJobDescriptionsDataContext
    {
        IMongoCollection<JobDescription>? JobDescriptions { get; }
    }
}