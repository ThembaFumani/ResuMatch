using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Data.JobDescriptionsDataContext
{
    public class JobDescriptionsDataContextSeed
    {
        internal static void SeedData(IMongoCollection<JobDescription> jobDescriptions)
        {
            SeedJobDescriptions(jobDescriptions);
        }

        private static void SeedJobDescriptions(IMongoCollection<JobDescription> jobDescriptions)
        {
            throw new NotImplementedException();
        }
    }
}