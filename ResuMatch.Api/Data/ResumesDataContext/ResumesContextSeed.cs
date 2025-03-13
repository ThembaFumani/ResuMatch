using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Data.MatchResultsDataContext
{
    public class ResumesContextSeed
    {
        internal static void SeedData(IMongoCollection<Resume> resumes)
        {
            SeedMatchResults(resumes);
        }

        private static void SeedMatchResults(IMongoCollection<Resume> resumes)
        {
            throw new NotImplementedException();
        }
    }
}