using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Repositories.ResumeRepository
{
    public interface IResumeRepository
    {
        Task AddAsync(Resume resume);
        Task<Resume> GetByIdAsync(string id);
    }
}