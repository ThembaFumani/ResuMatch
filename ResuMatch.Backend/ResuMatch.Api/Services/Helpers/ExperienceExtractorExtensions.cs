using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization; // For JsonPropertyName
using System.Text.RegularExpressions;

namespace ResuMatch.Api.Services.Helpers // Adjust namespace as needed for your project structure
{
    /// <summary>
    /// Represents the raw experience data extracted by the LLM from resume and job description texts.
    /// </summary>
    public class ExperienceRawData
    {
        [JsonPropertyName("resumeExplicitExperiencePhrases")]
        public List<string> ResumeExplicitExperiencePhrases { get; set; } = new List<string>();

        [JsonPropertyName("resumeJobDurationDates")]
        public List<string> ResumeJobDurationDates { get; set; } = new List<string>();

        [JsonPropertyName("jobDescriptionExplicitExperiencePhrases")]
        public List<string> JobDescriptionExplicitExperiencePhrases { get; set; } = new List<string>();
        // Add if your LLM can extract JD job duration dates:
        // [JsonPropertyName("jobDescriptionJobDurationDates")]
        // public List<string> JobDescriptionJobDurationDates { get; set; } = new List<string>();
    }

    /// <summary>
    /// Provides static methods for extracting and calculating professional experience from text.
    /// </summary>
    public static class ExperienceExtractorExtensions
    {
        /// <summary>
        /// Extracts all numerical years from common experience phrases (e.g., "8+ years", "5.5 years", "2 years in X").
        /// </summary>
        /// <param name="phrases">A list of experience phrases from the LLM.</param>
        /// <returns>A List of extracted integer years, or an empty list if none found.</returns>
        public static List<int> ExtractExplicitYearsFromPhrases(List<string> phrases)
        {
            List<int> extractedYears = new List<int>();
            if (phrases == null || phrases.Count == 0)
            {
                return extractedYears;
            }

            // Regex to capture numbers (integers or decimals) followed by "year" or "yr"
            // Added \+? after (\d+) to optionally match a plus sign
            string pattern = @"\b(\d+)\+?(?:\.\d+)?\s*(?:years?|yr)\b(?:(?:\s+of)?(?:\s+experience)?|\s+in\b)";

            foreach (string phrase in phrases)
            {
                Match match = Regex.Match(phrase, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    // Group 1 contains the whole number part (e.g., "8" from "8+", "3" from "3.5")
                    if (int.TryParse(match.Groups[1].Value, out int years))
                    {
                        extractedYears.Add(years);
                    }
                }
            }
            return extractedYears;
        }

        /// <summary>
        /// Extracts start and end month/year from a date range string (e.g., "MM/YYYY - MM/YYYY" or "MM/YYYY - Present").
        /// </summary>
        /// <param name="dateRange">The date range string.</param>
        /// <param name="startDate">Output: The parsed start date.</param>
        /// <param name="endDate">Output: The parsed end date (or current date if "Present").</param>
        /// <returns>True if parsing was successful, false otherwise.</returns>
        public static bool TryParseDateRange(string dateRange, out DateTime startDate, out DateTime endDate)
        {
            startDate = DateTime.MinValue;
            endDate = DateTime.MinValue;

            // Regex to capture MM/YYYY patterns. Handles "Present" as the end.
            // Group 1: Start Month, Group 2: Start Year
            // Group 3: End Month, Group 4: End Year (if not "Present")
            // Group 5: "Present" keyword
            string pattern = @"(\d{1,2})\/(\d{4})\s*-\s*(?:(\d{1,2})\/(\d{4})|(Present))";
            Match match = Regex.Match(dateRange, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                // Parse Start Date
                if (int.TryParse(match.Groups[1].Value, out int startMonth) &&
                    int.TryParse(match.Groups[2].Value, out int startYear))
                {
                    startDate = new DateTime(startYear, startMonth, 1);
                }
                else
                {
                    return false; // Failed to parse start date
                }

                // Parse End Date
                if (match.Groups[5].Success) // Matched "Present"
                {
                    endDate = DateTime.Now; // Use current date for "Present"
                }
                else if (int.TryParse(match.Groups[3].Value, out int endMonth) &&
                         int.TryParse(match.Groups[4].Value, out int endYear))
                {
                    // Set to last day of the month for full month count
                    endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
                }
                else
                {
                    return false; // Failed to parse end date
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates the total non-overlapping years from a list of date ranges.
        /// </summary>
        /// <param name="dateRanges">List of date range strings from the LLM.</param>
        /// <returns>Total non-overlapping years, rounded down.</returns>
        public static int CalculateTotalNonOverlappingYears(List<string> dateRanges)
        {
            if (dateRanges == null || !dateRanges.Any())
            {
                return 0;
            }

            var periods = new List<(DateTime Start, DateTime End)>();

            foreach (var rangeString in dateRanges)
            {
                if (TryParseDateRange(rangeString, out DateTime start, out DateTime end))
                {
                    periods.Add((start, end));
                }
            }

            if (!periods.Any())
            {
                return 0;
            }

            // Sort periods by start date
            periods = periods.OrderBy(p => p.Start).ToList();

            var mergedPeriods = new List<(DateTime Start, DateTime End)>();
            foreach (var currentPeriod in periods)
            {
                if (!mergedPeriods.Any() || currentPeriod.Start > mergedPeriods.Last().End)
                {
                    // No overlap or current period starts after last merged period ends
                    mergedPeriods.Add(currentPeriod);
                }
                else
                {
                    // Overlap: extend the end date of the last merged period if current one extends further
                    var lastMerged = mergedPeriods.Last();
                    mergedPeriods[mergedPeriods.Count - 1] = (lastMerged.Start, Max(lastMerged.End, currentPeriod.End));
                }
            }

            double totalMonths = 0;
            foreach (var period in mergedPeriods)
            {
                totalMonths += (period.End.Year - period.Start.Year) * 12 + (period.End.Month - period.Start.Month) + 1;
            }

            return (int)Math.Floor(totalMonths / 12.0);
        }

        private static DateTime Max(DateTime d1, DateTime d2)
        {
            return d1 > d2 ? d1 : d2;
        }

        /// <summary>
        /// Infers professional experience in years based on seniority keywords found in the job description.
        /// </summary>
        /// <param name="jobDescriptionText">The full job description text.</param>
        /// <returns>Inferred years of experience, or 0 if no seniority keyword is matched.</returns>
        public static int InferExperienceFromSeniority(string jobDescriptionText)
        {
            if (string.IsNullOrWhiteSpace(jobDescriptionText))
            {
                return 0;
            }

            string lowerText = jobDescriptionText.ToLowerInvariant();

            // Order matters: check for higher seniority first
            if (Regex.IsMatch(lowerText, @"\b(principal|lead|staff|architect)\b"))
            {
                return 10; // 10+ years
            }
            if (Regex.IsMatch(lowerText, @"\b(senior)\b"))
            {
                return 6; // 6+ years
            }
            if (Regex.IsMatch(lowerText, @"\b(intermediate|mid)\b"))
            {
                return 3; // 3+ years
            }
            if (Regex.IsMatch(lowerText, @"\b(junior|entry-level|entry level)\b"))
            {
                return 0; // 0-2 years, taking 0 as lower bound
            }

            return 0; // Default to 0 if no seniority keyword is found
        }
    }
}