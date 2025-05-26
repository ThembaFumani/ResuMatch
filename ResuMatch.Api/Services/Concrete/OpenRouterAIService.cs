using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ResuMatch.Api.Models.Configurations;
using ResuMatch.Api.Repositories;
using ResuMatch.Api.Services.Interfaces;

namespace ResuMatch.Api.Services.Concretes
{    public class OpenRouterAIService : IAIService
    {
        private readonly ILogger<OpenRouterAIService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OpenRouterConfig _openRouterConfig;
        private readonly IResumeRepository _resumeRepository;

        public OpenRouterAIService
        (
         ILogger<OpenRouterAIService> logger,
         IHttpClientFactory httpClientFactory,
         IOptions<OpenRouterConfig> openRouterConfig,
         IResumeRepository resumeRepository
        )
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _openRouterConfig = openRouterConfig.Value;
            _resumeRepository = resumeRepository;
        }

        public async Task<JsonDocument> ExtractSkillsAsync(string jobDescription)
        {
            _logger.LogInformation("Extracting skills using OpenRouter...");

            if (string.IsNullOrWhiteSpace(jobDescription))
            {
                throw new ArgumentException("Job description cannot be null or empty.", nameof(jobDescription));
            }

            string skillsPrompt = $"Extract the skills from the following job description and return them as a comma-separated list: {jobDescription}";
            string skillsResponseContent = await CallOpenRouterAsync(skillsPrompt);
            JsonDocument skillsJson = JsonDocument.Parse(skillsResponseContent);
            string skillsText = skillsJson.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()?.Trim() ?? string.Empty;
            string[] extractedSkills = skillsText?.Split(',')?.Select(s => s.Trim()).ToArray() ?? Array.Empty<string>();

            return JsonDocument.Parse(JsonSerializer.Serialize(extractedSkills, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }

        public async Task<string> GenerateSummaryAsync(string[] details)
        {
            _logger.LogInformation("Generating summary using OpenRouter...");

            if (details == null || details.Length == 0)
            {
                throw new ArgumentException("Details for summary cannot be null or empty.", nameof(details));
            }

            string summaryPrompt = $@"
            You are an expert career advisor providing feedback to a job seeker.
            Based *EXCLUSIVELY* on the 'Matching Skills' and 'Missing Skills' provided below, generate a concise summary (1-4 sentences maximum).

            **Instructions:**
            1.  **Highlight Strengths**: Emphasize skills listed under 'Matching Skills' as key strengths.
            2.  **Frame Growth Areas**: Briefly and constructively frame skills listed under 'Missing Skills' as opportunities for development or areas to highlight in a cover letter if relevant.
            3.  **No Contradictions**: Ensure the summary *never* contradicts the provided lists. If a skill is listed as 'Missing', do not refer to it as a 'match' or 'strength'.
            4.  **Concise & Direct**: The summary should be a single paragraph, 1-2 sentences. Do not include any introductory or concluding phrases outside the summary itself.

            Skill Matching Analysis:
            {string.Join("\n", details)}

            Example Summary:
            ""Your resume shows strong alignment with key requirements, particularly in C#, SQL, and Azure DevOps. To further enhance your profile, consider focusing on Angular 13 and specific e-commerce platform integrations as areas for future growth.""

            Concise Summary:
            ";

            string summaryResponseContent = await CallOpenRouterAsync(summaryPrompt);
            JsonDocument summaryJson = JsonDocument.Parse(summaryResponseContent);
            string summary = summaryJson.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()?.Trim() ?? string.Empty;
            return summary;
        }

        public Task<string> GetMatchingSkillsAnalysisAsync(List<string> resumeSkills, List<string> jobDescriptionSkills)
        {
            var prompt = $@"
            You are an experienced Senior Software Developer and Hiring Manager. Your task is to meticulously analyze and compare a candidate's resume skills against a job description's required skills. Your goal is to identify precise matches and genuine gaps from a practical, technical perspective.

            **Instructions for Identifying Matching and Missing Skills:**

            1.  **Deep Semantic Understanding**: Go beyond keyword matching. Understand the underlying concepts, technologies, and practices.
                * **Case-Insensitivity**: Treat skills like 'C#' and 'c#' as identical.
                * **Synonyms & Phrasing**: Recognize direct synonyms (e.g., 'JavaScript' and 'JS') and different common phrasings (e.g., 'Agile' and 'Scrum').
                * **Hierarchical & Implied Skills**: If a resume shows strong experience in a specific technology, infer the broader category (e.g., 'Kubernetes' implies 'orchestration', 'Azure DevOps' implies 'Azure cloud services'). If experience in 'Payment Processing' is present, it strongly implies 'third-party services integration' in that domain.
                * **Foundational Skills**: Experience with tools like 'GitHub Actions' implies foundational skills like 'Git'.
                * **Core Competencies**: Ensure fundamental skills like programming languages (C#), frameworks (.NET), and major cloud platforms (Azure, Google Cloud) are accurately matched if demonstrated.

            2.  **Identify Matching Skills**: List all skills from the resume that *clearly and semantically match or strongly imply* a requirement in the job description. These are skills a hiring manager would confidently tick off.

            3.  **Identify Missing Skills**: List only those skills explicitly stated in the job description that have *no semantic equivalent, no strong implication from related experience, and are genuinely absent* from the resume.
                * **CRITICAL RULE**: A skill (or its semantic equivalent) must **NEVER** appear in both 'matching_skills' and 'missing_skills'. If a skill is matched, it cannot be missing. If a broader skill is matched due to specific experience, the broader skill should not be listed as missing.
                * Consider if a broader skill (e.g., 'Application Development') is adequately covered by specific skills (e.g., 'C#', '.NET Core', 'Microservices'). If so, do not list the broader skill as missing.

            **Output Format**:
            Provide your analysis strictly as a JSON object with two distinct arrays: 'matching_skills' and 'missing_skills'.
            Ensure all skills within each array are unique and clearly formatted. Do not include any introductory or concluding text outside the JSON.

            ---
            Resume Skills:
            {string.Join(", ", resumeSkills)}

            Job Description Skills:
            {string.Join(", ", jobDescriptionSkills)}
            ---

            Example JSON Output (reflecting a highly accurate and semantically rich analysis):
            {{
                ""matching_skills"": [""C#"", "".NET Core"", ""SQL"", ""Agile Methodologies"", ""Microservices Architecture"", ""Azure Cloud Services"", ""CI/CD"", ""Docker"", ""Leadership"", ""Code Reviews"", ""System Design"", ""API Security"", ""Problem-Solving"", ""Communication"", ""Time Management"", ""E-commerce Platforms Integration"", ""Git"", ""Orchestration""],
                ""missing_skills"": [""Angular 13"", ""Magento"", ""WooCommerce"", ""Shopify""]
            }}

            Your JSON response:
            ";

            return CallOpenRouterAsync(prompt);
        }

        private async Task<string> CallOpenRouterAsync(string prompt)
        {
            var requestBody = new
            {
                model = _openRouterConfig.Model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            string json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            _logger.LogDebug("OpenRouter request body: {RequestBody}", json);

            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openRouterConfig.ApiKey}");
                var response = await httpClient.PostAsync(_openRouterConfig.Endpoint, new StringContent(json, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("OpenRouter API error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    throw new Exception($"OpenRouter API Error: {response.StatusCode} - {errorContent}");
                }

                string responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("OpenRouter API response: {ResponseContent}", responseContent);
                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenRouter API");
                throw;
            }
        }
    }
}