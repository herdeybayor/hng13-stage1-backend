using StringAnalyzer.Models;

namespace StringAnalyzer.Services;

public interface INaturalLanguageParser
{
    StringFilters ParseQuery(string query);
}

public class NaturalLanguageParser : INaturalLanguageParser
{
    public StringFilters ParseQuery(string query)
    {
        var filters = new StringFilters();
        var lowerQuery = query.ToLowerInvariant();

        // Check for palindrome indicators
        if (lowerQuery.Contains("palindrome") || lowerQuery.Contains("palindromic"))
        {
            filters.IsPalindrome = true;
        }

        // Check for word count
        if (lowerQuery.Contains("single word"))
        {
            filters.WordCount = 1;
        }
        else if (lowerQuery.Contains("two word"))
        {
            filters.WordCount = 2;
        }
        else if (lowerQuery.Contains("three word"))
        {
            filters.WordCount = 3;
        }

        // Check for length constraints
        var longerThanMatch = System.Text.RegularExpressions.Regex.Match(lowerQuery, @"longer than (\d+)");
        if (longerThanMatch.Success)
        {
            filters.MinLength = int.Parse(longerThanMatch.Groups[1].Value) + 1;
        }

        var shorterThanMatch = System.Text.RegularExpressions.Regex.Match(lowerQuery, @"shorter than (\d+)");
        if (shorterThanMatch.Success)
        {
            filters.MaxLength = int.Parse(shorterThanMatch.Groups[1].Value) - 1;
        }

        // Check for character containment
        if (lowerQuery.Contains("contain") && lowerQuery.Contains("letter"))
        {
            var letterMatch = System.Text.RegularExpressions.Regex.Match(lowerQuery, @"letter ([a-z])");
            if (letterMatch.Success)
            {
                filters.ContainsCharacter = letterMatch.Groups[1].Value;
            }
        }

        // Special case for vowels
        if (lowerQuery.Contains("first vowel"))
        {
            filters.ContainsCharacter = "a";
        }
        else if (lowerQuery.Contains("vowel"))
        {
            // could be smarter here but keeping it simple
            var vowelMatch = System.Text.RegularExpressions.Regex.Match(lowerQuery, @"vowel ([aeiou])");
            if (vowelMatch.Success)
            {
                filters.ContainsCharacter = vowelMatch.Groups[1].Value;
            }
        }

        return filters;
    }
}

