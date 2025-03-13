using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Repositories.JobDescriptionRepository
{
    public interface IJobDescriptionRepository
    {
        Task AddAsync(JobDescription jobDescription);
        Task<JobDescription> GetByIdAsync(string id);
    }
}