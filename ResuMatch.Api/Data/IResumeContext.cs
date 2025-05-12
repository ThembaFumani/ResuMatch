using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Data
{
    public interface IResumeContext
    {
        IMongoCollection<ResumeData> ResumeData { get; }
    }
}