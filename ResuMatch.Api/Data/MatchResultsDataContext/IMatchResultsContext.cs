using MongoDB.Driver;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Data.MatchResultsDataContext
{
    public interface IMatchResultsContext
    {
        IMongoCollection<MatchResult>? MatchResults { get; }
    }
}