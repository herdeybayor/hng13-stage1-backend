using System.Security.Cryptography;
using System.Text;
using StringAnalyzer.Models;

namespace StringAnalyzer.Services;

public interface IStringAnalyzerService
{
    StringAnalysis AnalyzeString(string value);
}

public class StringAnalyzerService : IStringAnalyzerService
{
    public StringAnalysis AnalyzeString(string value)
    {
        var sha256 = ComputeSha256(value);
        
        return new StringAnalysis
        {
            Id = sha256,
            Value = value,
            Properties = new StringProperties
            {
                Length = value.Length,
                IsPalindrome = CheckIfPalindrome(value),
                UniqueCharacters = CountUniqueChars(value),
                WordCount = CountWords(value),
                Sha256Hash = sha256,
                CharacterFrequencyMap = BuildCharFrequencyMap(value)
            },
            CreatedAt = DateTime.UtcNow
        };
    }

    private bool CheckIfPalindrome(string str)
    {
        // simple palindrome check - ignore case
        var cleaned = str.ToLowerInvariant();
        int left = 0;
        int right = cleaned.Length - 1;
        
        while (left < right)
        {
            if (cleaned[left] != cleaned[right])
                return false;
            left++;
            right--;
        }
        
        return true;
    }

    private int CountUniqueChars(string str)
    {
        // just count distinct characters
        return str.Distinct().Count();
    }

    private int CountWords(string str)
    {
        // split by whitespace and count non-empty parts
        return str.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private Dictionary<string, int> BuildCharFrequencyMap(string str)
    {
        var freqMap = new Dictionary<string, int>();
        
        foreach (var ch in str)
        {
            var key = ch.ToString();
            if (freqMap.ContainsKey(key))
                freqMap[key]++;
            else
                freqMap[key] = 1;
        }
        
        return freqMap;
    }

    private string ComputeSha256(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        
        // convert to hex string
        var sb = new StringBuilder();
        foreach (var b in hash)
        {
            sb.Append(b.ToString("x2"));
        }
        
        return sb.ToString();
    }
}

