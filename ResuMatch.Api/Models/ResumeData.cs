using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ResuMatch.Api.Models
{
    public class ResumeData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? FilePath { get; set; }
        public string? ResumeText { get; set; }
        public string? JobDescriptionText { get; set; }
        public AnalysisResult? AnalysisResult { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}