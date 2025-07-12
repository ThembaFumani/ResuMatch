using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResuMatch.Api.Models
{
    public static class PromptConstants
    {
        /// <summary>
        /// Prompt for extracting a standardized list of distinct technical and soft skills from text.
        /// </summary>
        public const string ExtractSkills = @"
You are an expert talent recruiter. Your task is to extract a standardized, highly accurate list of distinct technical and soft skills from the provided text. This output is critical for precise resume analysis and job matching.

**Input:**
- Text: {0}

**CORE OVERRIDING RULES (ADHERE TO THESE ABSOLUTELY):**

1.  **VERBATIM EXTRACTION & NO HALLUCINATION:** Only extract skills that are **EXPLICITLY PRESENT** in the 'Text'. **DO NOT INFER, INVENT, OR HALLUCINATE** any skills that are not directly mentioned or clearly implied within the text. If a term is not a skill, omit it.
2.  **OUTPUT FORMAT - ABSOLUTE MANDATE:** Your entire response **MUST BE ONLY** a single, comma-separated string of skills. No introductory sentences, no greetings, no explanations, no concluding remarks, no line breaks, no bullet points, and no category headings whatsoever (e.g., 'Technical Skills:', 'Soft Skills:', 'Key Skills:', 'Software Development').

**Detailed Instructions for Skill Identification:**

* **SKILL SPECIFICITY (CRITICAL):** Focus on concrete, actionable skills, as a human expert would. **DO NOT INCLUDE GENERIC TERMS** such as 'Development', 'Testing', 'Coding', 'Documentation', 'Implementation', 'Support', 'Maintenance', 'Architecture' (unless directly followed by a specific type, e.g., 'Microservices Architecture'), 'Solution Design', 'Problem-Solving' (unless specified as 'Analytical Problem-Solving'), 'Leadership' (unless specified as 'Technical Leadership'). ONLY include these if they are truly distinct and specific skills for the context.
* **DISTINCTNESS & CONCISENESS:** Ensure each extracted skill is distinct and unique. Use concise terms or short, standard phrases (e.g., 'C#', 'SQL', 'RabbitMQ', 'Agile', 'Mentoring').
* **CASING:** Preserve the original casing from the text where applicable, for accurate downstream matching and consistency.

**Output Format**:
Provide your analysis *strictly* as a comma-separated string of skills, matching the Example Output provided.
Do not include any introductory or concluding text, explanations, or apologies outside this specific format.

---
Text:
{0}
---

**Example Of Desired High-Quality Output (Strictly This Format, No Deviations):**
`C#, ASP.NET, MongoDB, SQL, Web API, Microservices, CI/CD, Agile, TDD, SOLID Principles, Docker, Kubernetes, Azure DevOps, Google Cloud Platform, REST API, XML, JSON, RabbitMQ, Service Orchestration, Technical Leadership, Mentoring, Design Patterns, SignalR, WCF, Octopus Deploy, Payment Integrations, Cryptography, Elasticsearch, MS SQL Server, Angular, React, Vue, Redis, Unit Testing, Sports Betting Markets, Gambling Regulations, Communication, Multitasking, Time Management, Attention to Detail, Adaptability, Ownership, Initiative, Resilience, Team Orientation, Integrity, Innovation`";

        /// <summary>
        /// Prompt for generating a concise, actionable summary for a job seeker.
        /// </summary>
        public const string ExtractSummary = @"
YOU ARE AN EXPERT CAREER ADVISOR. Your task is to generate a concise, actionable summary (1-3 sentences maximum) for a job seeker, based *strictly* on the provided experience and skill analysis.

**Instructions (ADHERE STRICTLY):**

1.  **Experience Qualification Assessment (CRITICAL & PRIMARY RULE):**
    * First, compare the candidate's `ResumeOverallExperience` to the `JobDescriptionOverallExperience`.
    * **Significantly Overqualified:** If `ResumeOverallExperience` is **more than double** `JobDescriptionOverallExperience` (e.g., 10 years experience for a 4-year role), your entire summary **MUST ONLY** state this fact clearly and professionally (e.g., ""You are significantly overqualified for this role based on your extensive experience.""). **DO NOT** mention strengths, growth areas, or any other feedback.
    * **Significantly Underqualified:** If `ResumeOverallExperience` is **less than half** `JobDescriptionOverallExperience` (e.g., 2 years experience for a 5-year role), your entire summary **MUST ONLY** state this fact constructively (e.g., ""You are currently underqualified for this role based on the required experience.""). **DO NOT** mention strengths, growth areas, or any other feedback.
    * **Well-Matched (Proceed):** If the candidate is neither significantly overqualified nor significantly underqualified, proceed to Step 2 for a detailed summary.

2.  **Highlight Key Strengths (If Well-Matched):**
    * Emphasize **1-3 most relevant and impactful key skills** as strengths. These skills MUST be chosen **exclusively from the 'matching_skills' list** in the 'Skill Matching Analysis'.
    * Prioritize specific technical skills unless a general skill is explicitly highlighted as a primary focus in the job description.

3.  **Encourage Growth Areas (If Well-Matched):**
    * Constructively mention **1-2 skills** as opportunities for development or growth. These skills MUST be chosen **exclusively from the 'missing_skills' list** in the 'Skill Matching Analysis'.
    * Phrase these as constructive advice to encourage the candidate's professional development in these areas.

4.  **Tone and Format:**
    * Maintain a positive, supportive, and professional tone throughout the summary.
    * The summary **MUST be 1-3 sentences maximum**.
    * **DO NOT** include any introductory phrases (e.g., ""Based on our analysis..."") or concluding remarks.
    * **DO NOT** include bullet points or lists; output a continuous paragraph.

5.  **Strict Data Adherence:**
    * **No Contradictions:** Ensure you **NEVER** mention any skill from 'missing_skills' as a strength, or any from 'matching_skills' as a weakness.
    * **Source of Truth:** Only use the provided `Skill Matching Analysis`, `ResumeOverallExperience`, and `JobDescriptionOverallExperience`. **DO NOT invent or infer any information not present in the input.**

**ABSOLUTELY NO INTERNAL THOUGHT PROCESSES OR REASONING:**
**DO NOT include any thinking process, internal monologue, or reasoning steps (e.g., within `<think>` tags or similar constructs). Your response MUST be ONLY the final summary.**

**Input Data:**
Skill Matching Analysis:
{0}
ResumeOverallExperience: {1}
JobDescriptionOverallExperience: {2}

Concise Summary:";

        /// <summary>
        /// Prompt for analyzing resume skills against job description skills to produce matching and missing lists.
        /// </summary>
        public const string GetMatchingSkillsAnalysis = @"
You are a principal Technical Recruiter and Senior Software Engineer. Your task is to meticulously analyze a candidate's resume against a job description. Your ultimate goal is to produce two *mutually exclusive* lists of skills: 'matching_skills' and 'missing_skills', ensuring no skill or its direct synonym/variant appears in both lists.

**Input:**
- Resume Skills: {0}
- Job Description Skills: {1}

**CORE OVERRIDING RULES (PRIORITIZE THESE ABOVE ALL ELSE):**

1.  **NO CONTRADICTIONS - ABSOLUTE MANDATE**: A skill (or its direct semantic equivalent/covering skill) **MUST NOT AND CANNOT** appear in both 'matching_skills' and 'missing_skills'. If a skill is deemed matching, it is not missing. If it is missing, it is not matching. This is the **most critical rule** and any violation is a fatal error.
2.  **STRICT PRESENCE FOR MATCHING**: For a skill to be in 'matching_skills', it MUST be explicitly mentioned in the resume or undeniably implied by strong, direct evidence of related technologies/experience in the resume. Do NOT infer a match if the resume does not strongly support it.
3.  **STRICT ABSENCE FOR MISSING**: For a skill to be in 'missing_skills', it MUST be explicitly required by the job description AND genuinely absent from the resume, meaning no direct semantic equivalent or strongly implied related experience (as per Rule 2) in the resume fulfills that specific job description requirement.

**Detailed Instructions for Skill Identification:**

* **Accuracy First**: Base your analysis *strictly* on the provided resume and job description text. Do not invent skills or infer experience that isn't clearly supported by the resume text.
* **Semantic Understanding & Synonyms:**
    * Case-Insensitive: Treat 'C#' and 'c#' as identical.
    * Direct Synonyms & Phrasing: Recognize common direct synonyms (e.g., 'JavaScript'/'JS', 'Agile'/'Scrum'). Also, consider common variations like 'Azure' vs. 'Microsoft Azure' as the same skill for matching/missing purposes.
* **Foundational & Hierarchical Skills (Use Conservatively but Accurately):**
    * If a resume lists 'GitHub Actions', it implies 'Git'. If 'ASP.NET Core' is listed, it implies '.NET'. Match these foundational elements if they satisfy a JD requirement.
    * A broader skill from the JD can be considered 'matching' if the resume demonstrates *significant, explicit experience* with multiple specific technologies/tasks that collectively and undeniably fulfill that broader requirement (e.g., extensive 'Azure DevOps CI/CD pipelines', 'Azure App Services', 'Azure Functions' experience can collectively match a JD requirement for 'Azure Cloud Services').
    * A specific technology in the resume (e.g., 'Kubernetes') can match a more general JD requirement (e.g., 'Container Orchestration').
    * For cloud platforms and DevOps tools, strong evidence of specific services or processes (e.g., 'Azure DevOps pipelines', 'Azure SQL', 'Google Cloud Build') should be sufficient to match broader requirements (e.g., 'Microsoft Azure', 'CI/CD'). Direct matches of named tools (e.g., 'Azure DevOps') should always be prioritized as 'matching'.
* **Handling Specific Version Requirements**: If a specific version of a technology is required (e.g., 'Angular 16 or higher'), and the resume does not explicitly mention that technology *at all*, it MUST be considered a 'missing_skill'. Do not infer the presence of a technology (like 'Angular') if it is not explicitly mentioned or strongly implied by directly related, equivalent experience in the resume.
* **Generic vs. Specific Skills**: Prioritize specific technical skills over very generic ones (e.g., 'Software Development') when placing them in 'matching_skills' or 'missing_skills'. If 'Software Development' is a JD requirement and specific development skills (C#, .NET) are present, it should be considered 'matching'. However, if it's missing specific evidence, it can be 'missing'. Ensure consistency.

**Output Format**:
Provide your analysis *strictly* as a JSON object with two distinct arrays: 'matching_skills' and 'missing_skills'.
All skills within each array must be unique and clearly formatted strings. The order of skills within the arrays does not matter.
Do not include any introductory or concluding text, explanations, or apologies outside the JSON structure.

---
Resume Skills:
{0}

Job Description Skills:
{1}
---

Example JSON Output (reflecting accurate, conservative, and non-contradictory analysis where skills are *mutually exclusive*):
{{
    ""matching_skills"": [""C#"", "".NET Core"", ""SQL Server"", ""Agile Methodologies"", ""Microservices Architecture"", ""Azure DevOps"", ""CI/CD"", ""Git"", ""Problem-Solving"", ""Communication""],
    ""missing_skills"": [""Angular 13"", ""Kubernetes (if not implied by other tools)"", ""NoSQL Databases (e.g., Cosmos DB)"", ""Advanced TypeScript""]
}}

Your JSON response:";

        /// <summary>
        /// Prompt for determining overall years of professional experience from resume and job description.
        /// This prompt is now superseded by the `GetExperienceRawData` prompt for a hybrid approach.
        /// </summary>
        public const string GetOverallExperienceAnalysis = @"
<context>
YOU ARE A PRINCIPAL TECHNICAL RECRUITER AND AN EXPERT AT PRECISELY EXTRACTING AND CALCULATING NUMERICAL EXPERIENCE DATA.
</context>

<task>
Your task is to accurately determine the overall years of professional experience for both the resume and the job description.

**Instructions (ADHERE STRICTLY AND SEQUENTIALLY):**

1.  **PRIMARY EXTRACTION: EXPLICIT OVERALL EXPERIENCE (HIGHEST PRIORITY):**
    * For **EACH** of the provided texts (Resume Text and Job Description Text), **first and foremost**, meticulously search for an **explicitly stated single number** representing overall years of professional experience. Look for patterns such as:
        * 'X+ years of experience'
        * 'X years experience'
        * 'at least X years'
        * 'X-Y years'
        * 'X.5 years'
        * 'over X years'
        * 'X years in [role/field]'
        * And any other clear numerical statement of total experience.
    * **CRITICAL EXTRACTION RULE:** When you identify such a pattern, **extract ONLY the numerical part as a pure, whole digit sequence**.
        * For '8+ years', extract `8`.
        * For '3.5 years', extract `3` (always truncate/round down to the nearest whole number integer; DO NOT round up).
        * For '1-2 years', extract `1` (always the lower bound).
        * For '2 years in Web Developer', extract `2`.
    * **IF AN EXPLICIT NUMBER IS FOUND FOR A TEXT (RESUME OR JD), STOP PROCESSING THAT TEXT'S EXPERIENCE AND USE THAT NUMBER.** Do NOT perform date-based calculation for that text.

2.  **SECONDARY CALCULATION: INFERENCE FROM DATES (IF PRIMARY EXTRACTION FAILS):**
    * **ONLY if no explicit overall years of experience are found** in a given text (Resume Text or Job Description Text) from Instruction 1, then proceed to calculate the overall experience by analyzing the **start and end dates** of individual experience entries within that specific text.
    * **Calculation Method:** For each distinct experience entry, determine its duration in months. Sum the durations of all distinct, non-overlapping experience entries. Treat 'Present' or similar current indicators as the current date for calculation.
    * **Convert to Years (CRITICAL):** Convert the total sum of months into a whole number of years. Always **round down/truncate** any fractional years (e.g., 30 months = 2 years, 18 months = 1 year). Do NOT round up.
    * **Overlap Handling (CRITICAL):** If there are overlapping experience periods (e.g., two jobs at the same time), **count the overlapping period only once** towards the total overall experience. Calculate the unique active employment duration.

3.  **DEFAULT VALUE:** If neither explicit experience nor calculable dates yield a number for a text, use `0` (zero) for that text's experience value.

</task>

<input>
Resume Text:
{0}

Job Description Text:
{1}
</input>

<output>
**Strict Output Format (JSON Only):** Respond **ONLY** with a JSON object. Do not include any introductory text, explanations, or conversational filler before or after the JSON.

**Expected JSON Output Format (CRITICAL: VALUES MUST BE PLAIN JSON INTEGERS, NOT OBJECTS):**
```json
{{
  ""ResumeOverallExperience"": 0,
  ""JobDescriptionOverallExperience"": 0
}}
```
</output>

Your JSON output:";

        /// <summary>
        /// Prompt for extracting structured resume data.
        /// </summary>
        public const string ExtractResumeStructure = @"
YOU ARE AN EXPERT RESUME PARSER, EXTRACTOR, AND TAILOR. Your primary task is to **STRICTLY EXTRACT ALL RELEVANT INFORMATION** from the provided 'Resume Content' and output it as a JSON object that adheres to the following exact structure. Your goal is to **populate every field solely based on data present in the input**. DO NOT HALLUCINATE OR INVENT INFORMATION.

**Example JSON Output Structure (ADHERE TO THIS EXACTLY):**
```json
{{
  ""Name"": """",
  ""Contact"": {{
    ""Email"": """",
    ""Phone"": """",
    ""LinkedIn"": ""[https://linkedin.com/in/example](https://linkedin.com/in/example)"",
    ""Portfolio"": ""[https://example-portfolio.com](https://example-portfolio.com)"",
    ""Location"": """"
  }},
  ""Summary"": """",
  ""Experience"": [
    {{
      ""Company"": """",
      ""Position"": """",
      ""Location"": """",
      ""Duration"": """",
      ""ResponsibilitiesAndAchievements"": [
        """"
      ],
      ""KeyTechnologiesUsed"": [
        """"
      ]
    }}
  ],
  ""Education"": {{
      ""University"": """",
      ""Degree"": """",
      ""Status"": """"
  }},
  ""Skills"": [
    {{
      ""SoftSkills"": [
        """"
      ],
      ""HardSkills"": [
        """"
      ],
      ""DevOpsSkills"": [
        """"
      ]
    }}
  ],
  ""Projects"": [
    {{
      ""Name"": """",
      ""Description"": """",
      ""TechnologiesUsed"": [
        """"
      ],
      ""URL"": """"
    }}
  ],
  ""Certifications"": [
    {{
      ""Name"": """",
      ""IssuingBody"": """",
      ""Year"": """"
    }}
  ],
  ""Awards"": [
    """"
  ]
}}
```

This structure represents a detailed resume model. 

**Resume Content:**
{0}

**IMPORTANT:** If the 'Resume Content' includes tailoring suggestions (provided in JSON format), you MUST use those suggestions to guide the extraction and rewriting process, making the outputted structured resume as tailored as possible. Focus on integrating keywords, action verbs, and addressing skill gaps based on the suggestions.

**Fields Description (Referencing the Structure Above):**

**MANDATORY FIELDS (MUST BE POPULATED IN JSON, even if empty based on the structure):**
- Name (string)
- Contact (object, containing at least Email or Phone)
- Summary (string)
- Experience (list of objects)
- Education (object)

**OPTIONAL FIELDS (Include in JSON if present in resume, otherwise can be omitted or empty based on the structure):**
- Skills (list of objects)
- Projects (list of objects)
- Certifications (list of objects)
- Awards (list of strings)

**Population Rules:**
**For all list fields (e.g., Experience, Skills, Projects, Certifications, Awards), if no entries are found in the Resume Content, provide an empty array `[]` .**
**For all string fields, if no data is found, provide an empty string `""` .**
**For object fields (e.g., Contact, Education), if no data is found, provide an empty object `{}` with empty string/array sub-properties as per their structure.**

**ABSOLUTE OUTPUT RULE:**
**YOUR RESPONSE MUST BE A SINGLE JSON OBJECT THAT STARTS DIRECTLY WITH `{\""Name\"": ...`, WITHOUT ANY ENCLOSING ROOT KEY LIKE `\""TailoredResume\""` OR ANYTHING SIMILAR. ABSOLUTELY NO `null` VALUES ARE ALLOWED FOR ANY FIELD; USE EMPTY STRINGS `\""` OR EMPTY ARRAYS `[]` AS APPROPRIATE. DO NOT INCLUDE ANY OTHER TEXT, EXPLANATIONS, APOLOGIES, OR THINKING PROCESSES BEFORE OR AFTER THE JSON.**

Your JSON output:";

        /// <summary>
        /// Prompt for rewriting a specific resume section to align with a job description.
        /// </summary>
        public const string GetTailorResumeSection = @"
You are an expert resume writer and career coach. Your task is to rewrite a specific section of a resume to perfectly align with a given job description, enhancing its relevance and impact.

**INPUT DATA:**
- Original Resume Section Text: {0}
- Full Job Description Text: {1}
- Section Type: {2} (e.g., 'summary', 'experience_bullet', 'achievements', 'skills_statement')

**INSTRUCTIONS:**

1.  **Analyze Relevance:** Carefully read the 'Original Resume Section Text' and the 'Full Job Description Text'. Identify keywords, responsibilities, and desired outcomes from the job description that are relevant to the 'Section Type'.
2.  **Rewrite for Alignment:** Rewrite the 'Original Resume Section Text' to incorporate and highlight the identified keywords and responsibilities. Ensure the rewritten text directly addresses the job description's needs for the specified 'Section Type'.
3.  **Maintain Tone & Style:** Keep the tone professional, results-oriented, and concise. Maintain the original format (e.g., if it was a bullet point, keep it a bullet point; if it was a paragraph, keep it a paragraph).
4.  **Enhance Impact (if applicable):** For 'experience_bullet' or 'achievements' types, try to quantify achievements or emphasize impact where possible, drawing connections to the job description.
5.  **Focus Solely on the Section:** Only rewrite the provided 'Original Resume Section Text'. Do NOT generate any other resume sections, introductions, or conclusions.
6.  **DO NOT INFER:** Do NOT invent experience or skills not present in the original resume. Focus on rephrasing and re-contextualizing what's already there.
7.  **ABSOLUTE OUTPUT RULE:** Your response should **ONLY** be the rewritten resume section text. Do NOT include any explanations, apologies, conversational filler, or Markdown code blocks (e.g., no ```json``` or ```text``` wrappers) before or after the rewritten text.

Rewritten Resume Section:";

        /// <summary>
        /// Prompt for generating highly specific, actionable, and location-aware suggestions for resume tailoring.
        /// </summary>
        public const string GenerateTailoringSuggestions = @"
YOU ARE AN ELITE CAREER COACH. Your task is to provide **highly specific, actionable, and location-aware suggestions** for tailoring a resume to a given job description. You must analyze the 'Skills Analysis Result', the 'Job Description Text', and the 'Original Raw Resume Text' to pinpoint areas for improvement and generate suggestions in a structured JSON format that allows for precise frontend highlighting.

**INPUT DATA:**
- Skills Analysis Result (JSON): {0}
- Job Description Text: {1}
- Original Raw Resume Text: {2}

**INSTRUCTIONS FOR GENERATING SUGGESTIONS:**
1.  **Analyze Context Deeply:** Use the `Original Raw Resume Text` to understand the exact wording and structure of the candidate's current resume. This is *critical* for generating accurate `originalContentSnippet` values.
2.  **Identify Opportunities:** Compare the `Skills Analysis Result` (especially 'Missing Skills' and 'MatchScore') and the `Job Description Text` against the 'Original Raw Resume Text'. Pinpoint specific sentences, phrases, or bullet points in the original resume that could be improved or added.
3.  **Generate Granular Suggestions (JSON Object Format):**
    * For each suggestion, you MUST create a JSON object adhering to the `SectionSuggestion` (or `ExperienceBulletSuggestion`) schema provided below. 
    * **`targetSectionPath` (CRITICAL):** Provide a precise JSONPath-like string indicating the exact location within the *final `ResumeDetailModel`* (the tailored structured CV) where the suggestion applies. Examples:
        * `""Summary""`
        * `""Experience[0].ResponsibilitiesAndAchievements[2]""` (for the 3rd bullet point of the 1st experience entry)
        * `""Education[1].Degree""`
        * `""Skills[0].SoftSkills[1]""` (for the 2nd soft skill in the 1st skill category object)
        * If a suggestion is for adding a new bullet to an existing experience, target the array itself, e.g., `""Experience[X].ResponsibilitiesAndAchievements""`.
    * **`originalContentSnippet` (CRITICAL):** Extract a *small, relevant phrase or sentence* from the `Original Raw Resume Text` that directly corresponds to the `targetSectionPath`. This helps the user see what exact piece of text the suggestion is for. If the suggestion is to *add* entirely new content, this field can be an empty string `""""`.
    * **`suggestion`:** The concrete, actionable advice or proposed rewrite.
    * **`suggestionType`:** Categorize the advice (e.g., `""KeywordInsertion""`, `""ActionVerb""`, `""Quantification""`, `""SkillGapAddressing""`, `""RephraseForClarity""`, `""HighlightRelevance""`).
    * **`priority`:** Assign `""High""`, `""Medium""`, or `""Low""` based on impact.
4.  **Categorize and Aggregate:** Group these individual `SectionSuggestion` objects into the appropriate lists (`SummarySuggestions`, `ExperienceSuggestions`, `SkillSectionSuggestions`, `MissingSkillSuggestions`) within the main `SuggestionsResponse` object.
5.  **Global Suggestions:** Populate `GeneralTips`, `KeywordsToAdd`, and `ActionVerbsToConsider` with broader, non-location-specific advice. Ensure `KeywordsToAdd` and `ActionVerbsToConsider` are direct arrays of strings, not arrays of objects.
6.  **Overall Explanation:** Provide a brief `OverallExplanation` summarizing the general strategy behind all suggestions.

**JSON Schema Representation (ADHERE TO THIS EXACT STRUCTURE FOR YOUR OUTPUT):**

**`SuggestionsResponse` Schema:**
```json
{{
  ""GeneralTips"": [
    {{
      ""targetSectionPath"": ""string"",
      ""originalContentSnippet"": ""string"",
      ""suggestion"": ""string"",
      ""suggestionType"": ""string"",
      ""priority"": ""string""
    }}
  ],
  ""KeywordsToAdd"": [
    ""string""
  ],\n  ""ActionVerbsToConsider"": [\n    ""string""\n  ],\n  ""SummarySuggestions"": [\n    {{\n      ""targetSectionPath"": ""string"",\n      ""originalContentSnippet"": ""string"",\n      ""suggestion"": ""string"",\n      ""suggestionType"": ""string"",\n      ""priority"": ""string""\n    }}\n  ],\n  ""ExperienceSuggestions"": [\n    {{\n      ""targetSectionPath"": ""string"",\n      ""originalContentSnippet"": ""string"",\n      ""suggestion"": ""string"",\n      ""suggestionType"": ""string"",\n      ""priority"": ""string""\n    }}\n  ],\n  ""SkillSectionSuggestions"": [\n    {{\n      ""targetSectionPath"": ""string"",\n      ""originalContentSnippet"": ""string"",\n      ""suggestion"": ""string"",\n      ""suggestionType"": ""string"",\n      ""priority"": ""string""\n    }}\n  ],\n  ""MissingSkillSuggestions"": [\n    {{\n      ""targetSectionPath"": ""string"",\n      ""originalContentSnippet"": ""string"",\n      ""suggestion"": ""string"",\n      ""suggestionType"": ""string"",\n      ""priority"": ""string""\n    }}\n  ],\n  ""OverallExplanation"": ""string""\n}}\n```\n\n**ABSOLUTE OUTPUT RULE:**\n**ONLY RESPOND WITH THE JSON OBJECT THAT STRICTLY ADHERES TO THE `SuggestionsResponse` SCHEMA, STARTING DIRECTLY WITH `{{\""GeneralTips\"": [...]`. ABSOLUTELY NO `null` VALUES ARE ALLOWED FOR ANY FIELD; USE EMPTY STRINGS `\""` OR EMPTY ARRAYS `[]` AS APPROPRIATE. DO NOT INCLUDE ANY OTHER TEXT, EXPLANATIONS, APOLOGIES, OR THINKING PROCESSES BEFORE OR AFTER THE JSON.**\n\nYour JSON output:";

        /// <summary>
        /// Prompt for repairing malformed JSON for resume tailoring suggestions.
        /// </summary>
        public const string TailoringMalformedJsonRepair = @"
The following JSON text contains errors or does not fully conform to the provided schema. Your task is to correct it and return ONLY the valid JSON object. Ensure strict adherence to the schema provided.

SCHEMA:
```json
{0}
```

MALFORMED JSON:
```
{1}
```

Return ONLY the corrected, valid JSON object, ALONG WITH THE DATA MAPPED TO THE CORRECT PROPERTIES. Do NOT include any other text, explanations, or markdown fences.";

        /// <summary>
        /// Prompt for extracting overall professional experience from resume text.
        /// This prompt is intended for the hybrid approach where C# code performs the final calculation.
        /// </summary>
        public const string ExtractResumeExperience = @"
<context>
YOU ARE A PRINCIPAL TECHNICAL RECRUITER AND AN EXPERT AT PRECISELY EXTRACTING AND CALCULATING NUMERICAL EXPERIENCE DATA.
</context>

<task>
Your task is to accurately determine the overall years of professional experience from the provided resume text.

**Instructions (ADHERE STRICTLY AND SEQUENTIALLY):**

1.  **PRIMARY EXTRACTION: EXPLICIT OVERALL EXPERIENCE (HIGHEST PRIORITY):**
    * First and foremost, meticulously search for an **explicitly stated single number** representing overall years of professional experience within the Resume Text. Look for patterns such as:
        * 'X+ years of experience'
        * 'X years experience'
        * 'at least X years'
        * 'X-Y years'
        * 'X.5 years'
        * 'over X years'
        * 'X years in [role/field]'
        * And any other clear numerical statement of total experience.
    * **CRITICAL EXTRACTION RULE:** When you identify such a pattern, **extract ONLY the numerical part as a pure, whole digit integer**.
        * For '8+ years', output `8`.
        * For '3.5 years', output `3` (always truncate/round down to the nearest whole number integer; DO NOT round up).
        * For '1-2 years', output `1` (always the lower bound).
        * For '2 years in Web Developer', output `2`.
    * **IF AN EXPLICIT NUMBER IS FOUND, STOP PROCESSING AND USE THAT NUMBER.** Do NOT perform date-based calculation.

2.  **SECONDARY CALCULATION: INFERENCE FROM DATES (IF PRIMARY EXTRACTION FAILS):**
    * **ONLY if no explicit overall years of experience are found** from Instruction 1, then proceed to calculate the overall experience by analyzing the **start and end dates** of individual experience entries within the Resume Text.
    * **Calculation Method:** For each distinct experience entry, determine its duration in months. Sum the durations of all distinct, non-overlapping experience entries. Treat 'Present' or similar current indicators as the current date for calculation.
    * **Convert to Years (CRITICAL):** Convert the total sum of months into a whole number of years. Always **round down/truncate** any fractional years (e.g., 30 months = 2 years, 18 months = 1 year). Do NOT round up.
    * **Overlap Handling (CRITICAL):** If there are overlapping experience periods (e.g., two jobs at the same time), **count the overlapping period only once** towards the total overall experience. Calculate the unique active employment duration.

3.  **DEFAULT VALUE:** If neither explicit experience nor calculable dates yield a number, output `0` (zero).

</task>

<input>
Resume Text:
{0}
</input>

<output>
**Strict Output Format (JSON Only):** Respond **ONLY** with a JSON object containing a single key `""OverallExperience""` with the extracted integer value. Do not include any introductory text, explanations, or conversational filler before or after the JSON.

**Expected JSON Output Format (CRITICAL: VALUE MUST BE A PLAIN JSON INTEGER, NOT AN OBJECT):**
```json
{{
  ""OverallExperience"": 0
}}
```
</output>

Your JSON output:";

        /// <summary>
        /// Prompt for extracting overall professional experience from job description text.
        /// This prompt is intended for the hybrid approach where C# code performs the final calculation.
        /// </summary>
        public const string ExtractJDExperience = @"
<context>
YOU ARE A PRINCIPAL TECHNICAL RECRUITER AND AN EXPERT AT PRECISELY EXTRACTING NUMERICAL EXPERIENCE DATA.
</context>

<task>
Your task is to accurately determine the overall years of professional experience from the provided job description text.

**Instructions (ADHERE STRICTLY AND SEQUENTIALLY):**

1.  **PRIMARY EXTRACTION: EXPLICIT OVERALL EXPERIENCE (HIGHEST PRIORITY):**
    * First and foremost, meticulously search for an **explicitly stated single number** representing overall years of professional experience within the Job Description Text. Look for patterns such as:
        * 'X+ years of experience'
        * 'X years experience'
        * 'at least X years'
        * 'X-Y years'
        * 'X.5 years'
        * 'over X years'
        * 'X years in [role/field]'
        * And any other clear numerical statement of total experience.
    * **CRITICAL EXTRACTION RULE:** When you identify such a pattern, **extract ONLY the numerical part as a pure, whole digit integer**.
        * For '10+ years', output `10`.
        * For '3.5 years', output `3` (always truncate/round down to the nearest whole number integer; DO NOT round up).
        * For '1-2 years', output `1` (always the lower bound).
        * For '5 years in Software Development', output `5`.
    * **IF AN EXPLICIT NUMBER IS FOUND, STOP PROCESSING AND USE THAT NUMBER.** Do NOT perform date-based calculation.

2.  **SECONDARY CALCULATION: INFERENCE FROM DATES (IF PRIMARY EXTRACTION FAILS):**
    * **ONLY if no explicit overall years of experience are found** from Instruction 1, then proceed to calculate the overall experience by analyzing the **start and end dates** of individual experience entries within the Job Description Text.
    * **Calculation Method:** For each distinct experience entry, determine its duration in months. Sum the durations of all distinct, non-overlapping experience entries. Treat 'Present' or similar current indicators as the current date for calculation.
    * **Convert to Years (CRITICAL):** Convert the total sum of months into a whole number of years. Always **round down/truncate** any fractional years (e.g., 30 months = 2 years, 18 months = 1 year). Do NOT round up.
    * **Overlap Handling (CRITICAL):** If there are overlapping experience periods (e.g., two jobs at the same time), **count the overlapping period only once** towards the total overall experience. Calculate the unique active employment duration.

3.  **DEFAULT VALUE:** If neither explicit experience nor calculable dates yield a number, output `0` (zero).

</task>

<input>
Job Description Text:
{0}
</input>

<output>
**Strict Output Format (JSON Only):** Respond **ONLY** with a JSON object containing a single key `""OverallExperience""` with the extracted integer value. Do not include any introductory text, explanations, or conversational filler before or after the JSON.

**Expected JSON Output Format (CRITICAL: VALUE MUST BE A PLAIN JSON INTEGER, NOT AN OBJECT):**
```json
{{
  ""OverallExperience"": 0
}}
```
</output>

Your JSON output:";

        /// <summary>
        /// This is the new prompt for the hybrid approach, designed to extract raw experience data
        /// (explicit phrases and date ranges) without performing calculations.
        /// Your C# code will then handle the calculation and prioritization.
        /// </summary>
        public const string GetExperienceRawData = @"
<context>
YOU ARE A HIGHLY ACCURATE DATA EXTRACTION AGENT. Your sole purpose is to identify and extract all relevant raw experience data from text, without performing any calculations or interpretations.
</context>

<task>
Your task is to extract all explicit mentions of overall professional experience (e.g., 'X years experience') and all individual job duration date ranges (e.g., 'MM/YYYY - MM/YYYY' or 'MM/YYYY - Present') from both the provided resume and job description texts.

**Instructions (ADHERE STRICTLY):**

1.  **Extract Explicit Overall Experience Phrases (Resume):** Identify any phrases in the Resume Text that explicitly state the candidate's total professional experience (e.g., '8+ years of experience', '5 years experience in software development'). Collect these as a list of strings.
2.  **Extract Job Duration Date Ranges (Resume):** Identify all start and end dates for each job entry in the Resume Text. Extract these as a list of strings, preserving the exact format (e.g., '01/2017 - 07/2019', '07/2024 - Present').
3.  **Extract Explicit Overall Experience Phrases (Job Description):** Identify any phrases in the Job Description Text that explicitly state the required total professional experience (e.g., '10+ years of professional software development experience', 'at least 3 years experience'). Collect these as a list of strings.
4.  **No Interpretation or Calculation:** Absolutely **DO NOT** perform any numerical calculations, date difference calculations, rounding, or prioritization. Just extract the raw text phrases and date ranges as they appear.
5.  **Handle Missing Data:** If no relevant data is found for a specific list (e.g., no explicit experience phrases in the JD), return an empty array `[]` for that list.

</task>

<input>
Resume Text:
{0}

Job Description Text:
{1}
</input>

<output>
**Strict Output Format (JSON Only):** Respond **ONLY** with a JSON object. Do not include any introductory text, explanations, or conversational filler before or after the JSON.

**Expected JSON Output Format (CRITICAL: All values must be JSON arrays of strings):**
```json
{{
  ""resumeExplicitExperiencePhrases"": [],
  ""resumeJobDurationDates"": [],
  ""jobDescriptionExplicitExperiencePhrases"": []
}}
```
</output>

Your JSON output:";
    }
}
