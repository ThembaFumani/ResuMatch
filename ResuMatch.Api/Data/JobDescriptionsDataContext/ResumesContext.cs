using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Data.JobDescriptionsDataContext
{
    public class JobDescriptionsDataContext : IJobDescriptionsDataContext
    {
        private readonly IConfiguration _configuration;
        public JobDescriptionsDataContext(IConfiguration configuration)
        {
            _configuration = configuration;
             var connectionStringKey = GetConnectionString();

            var client = new MongoClient(connectionStringKey);
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));
                       
            JobDescriptions = database.GetCollection<JobDescription>(configuration.GetValue<string>("DatabaseSettings:Collections:JobDescriptions"));
            JobDescriptionsDataContextSeed.SeedData(JobDescriptions);
        }

        private string GetConnectionString()
        {
            return _configuration.GetValue<string>("DatabaseSettings:ConnectionString:Dev");
        }

        public IMongoCollection<JobDescription>? JobDescriptions { get; }
    }
}