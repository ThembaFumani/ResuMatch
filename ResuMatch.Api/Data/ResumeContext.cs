using MongoDB.Driver;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Data
{
    public class ResumeContext : IResumeContext
    {
        public ResumeContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString:Dev"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));
            ResumeData = database.GetCollection<ResumeData>(configuration.GetValue<string>("DatabaseSettings:Collections:ResumeData"));
        }
        public IMongoCollection<ResumeData> ResumeData { get; }
    }
}