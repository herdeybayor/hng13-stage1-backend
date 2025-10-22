using System.Text.Json.Serialization;

namespace StringAnalyzer.Models;

// Request DTOs
public class CreateStringRequest
{
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

// Response DTOs
public class StringAnalysisResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
    
    [JsonPropertyName("properties")]
    public PropertiesResponse Properties { get; set; } = new();
    
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;
}

public class PropertiesResponse
{
    [JsonPropertyName("length")]
    public int Length { get; set; }
    
    [JsonPropertyName("is_palindrome")]
    public bool IsPalindrome { get; set; }
    
    [JsonPropertyName("unique_characters")]
    public int UniqueCharacters { get; set; }
    
    [JsonPropertyName("word_count")]
    public int WordCount { get; set; }
    
    [JsonPropertyName("sha256_hash")]
    public string Sha256Hash { get; set; } = string.Empty;
    
    [JsonPropertyName("character_frequency_map")]
    public Dictionary<string, int> CharacterFrequencyMap { get; set; } = new();
}

public class StringListResponse
{
    [JsonPropertyName("data")]
    public List<StringAnalysisResponse> Data { get; set; } = new();
    
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("filters_applied")]
    public Dictionary<string, object>? FiltersApplied { get; set; }
}

public class NaturalLanguageResponse
{
    [JsonPropertyName("data")]
    public List<StringAnalysisResponse> Data { get; set; } = new();
    
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("interpreted_query")]
    public InterpretedQuery InterpretedQuery { get; set; } = new();
}

public class InterpretedQuery
{
    [JsonPropertyName("original")]
    public string Original { get; set; } = string.Empty;
    
    [JsonPropertyName("parsed_filters")]
    public Dictionary<string, object> ParsedFilters { get; set; } = new();
}

public class ErrorResponse
{
    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;
    
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

