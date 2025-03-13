using MongoDB.Driver;
using ResuMatch.Api.Models;

namespace ResuMatch.Api.Data.MatchResultsDataContext
{
    public interface IResumesContext
    {
        IMongoCollection<Resume>? Resumes { get; }
    }
}