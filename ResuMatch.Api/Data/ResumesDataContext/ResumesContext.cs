using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Data.MatchResultsDataContext
{
    public class ResumesContext : IResumesContext
    {
        private readonly IConfiguration _configuration;
        public ResumesContext(IConfiguration configuration)
        {
            _configuration = configuration;
             var connectionStringKey = GetConnectionString();

            var client = new MongoClient(connectionStringKey);
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));
                       
            Resumes = database.GetCollection<Resume>(configuration.GetValue<string>("DatabaseSettings:Collections:Resumes"));
            ResumesContextSeed.SeedData(Resumes);
        }

        private string GetConnectionString()
        {
            return _configuration.GetValue<string>("DatabaseSettings:ConnectionString:Dev");
        }

        public IMongoCollection<Resume>? Resumes { get; }
    }
}