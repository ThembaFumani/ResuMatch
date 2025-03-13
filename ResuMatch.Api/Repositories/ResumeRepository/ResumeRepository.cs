using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ResuMatch.Api.Data.MatchResultsDataContext;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Repositories.ResumeRepository
{
    public class ResumeRepository : IResumeRepository
    {
        private readonly IResumesContext _resumesContext;
        public ResumeRepository(IResumesContext resumesContext)
        {
            _resumesContext = resumesContext;
        }

        public async Task AddAsync(Resume resume)
        {
            await _resumesContext.Resumes.InsertOneAsync(resume);
        }

        public async Task<Resume> GetByIdAsync(string id)
        {
            return await _resumesContext.Resumes.Find(r => r.Id == id).FirstOrDefaultAsync();
        }
    }
}