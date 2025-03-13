using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ResuMatch.Api.Data.JobDescriptionsDataContext;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Repositories.JobDescriptionRepository
{
    public class JobDescriptionRepository : IJobDescriptionRepository
    {
        private readonly IJobDescriptionsDataContext _jobDescriptionsDataContext;
        public JobDescriptionRepository(IJobDescriptionsDataContext jobDescriptionsDataContext)
        {
            _jobDescriptionsDataContext = jobDescriptionsDataContext;
        }

        public async Task AddAsync(JobDescription jobDescription)
        {
            await _jobDescriptionsDataContext.JobDescriptions.InsertOneAsync(jobDescription);
        }

        public async Task<JobDescription> GetByIdAsync(string title)
        {
            return await _jobDescriptionsDataContext.JobDescriptions.Find(j => j.Title == title).FirstOrDefaultAsync();
        }
    }
}