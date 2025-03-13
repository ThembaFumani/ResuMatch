using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Repositories.MatchResultRepository
{
    public interface IMatchResultRepository
    {
        Task AddAsync(MatchResult matchResult);
        Task<MatchResult> GetByIdAsync(string id);
    }
}