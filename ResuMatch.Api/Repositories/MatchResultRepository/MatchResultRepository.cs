using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ResuMatch.Api.Data.MatchResultsDataContext;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Repositories.MatchResultRepository
{
    public class MatchResultRepository : IMatchResultRepository
    {
        private readonly IMatchResultsContext _matchResultsContext;
        public MatchResultRepository(IMatchResultsContext matchResultsContext)
        {
            _matchResultsContext = matchResultsContext;
        }

        public async Task AddAsync(MatchResult matchResult)
        {
            await _matchResultsContext.MatchResults.InsertOneAsync(matchResult);
        }

        public Task<MatchResult> GetByIdAsync(string id)
        {
            return _matchResultsContext.MatchResults.Find(m => m.ResumeId == id).FirstOrDefaultAsync();
        }
    }
}