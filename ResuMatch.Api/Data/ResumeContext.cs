using MongoDB.Driver;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Data
{
    public class ResumeContext : IResumeContext
    {
        public ResumeContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("MongoDB:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("MongoDB:DatabaseName"));
            Resumes = database.GetCollection<ResumeData>(configuration.GetValue<string>("MongoDB:Resumes"));
        }
        public IMongoCollection<ResumeData> Resumes { get; }
    }
}