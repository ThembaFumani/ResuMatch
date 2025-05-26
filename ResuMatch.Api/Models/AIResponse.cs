using System.Text.Json.Serialization;

namespace ResuMatch.Api.Models.Configurations
{
    public class AIResponse
    {
        [JsonPropertyName("choices")]
        public Choice[]? Choices { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("message")]
        public Message? Message { get; set; }
    }
    
    public class Message
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}