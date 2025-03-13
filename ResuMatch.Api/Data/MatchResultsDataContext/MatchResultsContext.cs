using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Data.MatchResultsDataContext
{
    public class MatchResultsContext : IMatchResultsContext
    {
        private readonly IConfiguration _configuration;
        public MatchResultsContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IMongoCollection<MatchResult>? MatchResults { get; }
    }
}