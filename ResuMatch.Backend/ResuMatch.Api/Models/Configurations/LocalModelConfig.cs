namespace ResuMatch.Api.Models.Configurations
{
    public class LocalModelConfig
    {
        public string? Endpoint { get; set; }
        public List<LocalModelEntry>? Models { get; set; }
        public bool Stream { get; set; } = false;
    }
    
     public class LocalModelEntry
    {
        public string? Name { get; set; }
        public string? Model { get; set; }
    }
}