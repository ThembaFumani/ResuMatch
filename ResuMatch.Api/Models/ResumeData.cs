using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public AnalysisResponse? AnalysisResult { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}