namespace StringAnalyzer.Models;

public class StringAnalysis
{
    public string Id { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public StringProperties Properties { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class StringProperties
{
    public int Length { get; set; }
    public bool IsPalindrome { get; set; }
    public int UniqueCharacters { get; set; }
    public int WordCount { get; set; }
    public string Sha256Hash { get; set; } = string.Empty;
    public Dictionary<string, int> CharacterFrequencyMap { get; set; } = new();
}

