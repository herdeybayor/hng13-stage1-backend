using Microsoft.AspNetCore.Mvc;
using StringAnalyzer.Models;
using StringAnalyzer.Services;

namespace StringAnalyzer.Controllers;

[ApiController]
[Route("strings")]
public class StringsController : ControllerBase
{
    private readonly IStringAnalyzerService _analyzerService;
    private readonly IStringRepository _repository;
    private readonly INaturalLanguageParser _nlParser;
    private readonly ILogger<StringsController> _logger;

    public StringsController(
        IStringAnalyzerService analyzerService,
        IStringRepository repository,
        INaturalLanguageParser nlParser,
        ILogger<StringsController> logger)
    {
        _analyzerService = analyzerService;
        _repository = repository;
        _nlParser = nlParser;
        _logger = logger;
    }

    [HttpPost]
    public IActionResult CreateString([FromBody] CreateStringRequest request)
    {
        // Validate request body
        if (request == null || request.Value == null)
        {
            return BadRequest(new ErrorResponse
            {
                Error = "Bad Request",
                Message = "Invalid request body or missing 'value' field"
            });
        }

        // Check if value is actually a string (not a number, bool, etc)
        if (string.IsNullOrEmpty(request.Value) && request.Value != string.Empty)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                Error = "Unprocessable Entity",
                Message = "Invalid data type for 'value' (must be string)"
            });
        }

        // Check if string already exists
        if (_repository.Exists(request.Value))
        {
            return Conflict(new ErrorResponse
            {
                Error = "Conflict",
                Message = "String already exists in the system"
            });
        }

        // Analyze and store the string
        var analysis = _analyzerService.AnalyzeString(request.Value);
        _repository.Add(analysis);

        var response = MapToResponse(analysis);
        return StatusCode(201, response);
    }

    [HttpGet("{stringValue}")]
    public IActionResult GetString(string stringValue)
    {
        var analysis = _repository.GetByValue(stringValue);
        
        if (analysis == null)
        {
            return NotFound(new ErrorResponse
            {
                Error = "Not Found",
                Message = "String does not exist in the system"
            });
        }

        var response = MapToResponse(analysis);
        return Ok(response);
    }

    [HttpGet]
    public IActionResult GetAllStrings(
        [FromQuery(Name = "is_palindrome")] bool? isPalindrome,
        [FromQuery(Name = "min_length")] int? minLength,
        [FromQuery(Name = "max_length")] int? maxLength,
        [FromQuery(Name = "word_count")] int? wordCount,
        [FromQuery(Name = "contains_character")] string? containsCharacter)
    {
        // Validate query parameters
        if (minLength.HasValue && minLength.Value < 0)
        {
            return BadRequest(new ErrorResponse
            {
                Error = "Bad Request",
                Message = "min_length must be a positive integer"
            });
        }

        if (maxLength.HasValue && maxLength.Value < 0)
        {
            return BadRequest(new ErrorResponse
            {
                Error = "Bad Request",
                Message = "max_length must be a positive integer"
            });
        }

        if (wordCount.HasValue && wordCount.Value < 0)
        {
            return BadRequest(new ErrorResponse
            {
                Error = "Bad Request",
                Message = "word_count must be a positive integer"
            });
        }

        if (!string.IsNullOrEmpty(containsCharacter) && containsCharacter.Length > 1)
        {
            return BadRequest(new ErrorResponse
            {
                Error = "Bad Request",
                Message = "contains_character must be a single character"
            });
        }

        var filters = new StringFilters
        {
            IsPalindrome = isPalindrome,
            MinLength = minLength,
            MaxLength = maxLength,
            WordCount = wordCount,
            ContainsCharacter = containsCharacter
        };

        var results = _repository.GetFiltered(filters);
        
        var filtersApplied = new Dictionary<string, object>();
        if (isPalindrome.HasValue) filtersApplied["is_palindrome"] = isPalindrome.Value;
        if (minLength.HasValue) filtersApplied["min_length"] = minLength.Value;
        if (maxLength.HasValue) filtersApplied["max_length"] = maxLength.Value;
        if (wordCount.HasValue) filtersApplied["word_count"] = wordCount.Value;
        if (!string.IsNullOrEmpty(containsCharacter)) filtersApplied["contains_character"] = containsCharacter;

        var response = new StringListResponse
        {
            Data = results.Select(MapToResponse).ToList(),
            Count = results.Count,
            FiltersApplied = filtersApplied.Count > 0 ? filtersApplied : null
        };

        return Ok(response);
    }

    [HttpGet("filter-by-natural-language")]
    public IActionResult FilterByNaturalLanguage([FromQuery] string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest(new ErrorResponse
            {
                Error = "Bad Request",
                Message = "Query parameter is required"
            });
        }

        try
        {
            var filters = _nlParser.ParseQuery(query);
            var results = _repository.GetFiltered(filters);

            var parsedFilters = new Dictionary<string, object>();
            if (filters.IsPalindrome.HasValue) parsedFilters["is_palindrome"] = filters.IsPalindrome.Value;
            if (filters.MinLength.HasValue) parsedFilters["min_length"] = filters.MinLength.Value;
            if (filters.MaxLength.HasValue) parsedFilters["max_length"] = filters.MaxLength.Value;
            if (filters.WordCount.HasValue) parsedFilters["word_count"] = filters.WordCount.Value;
            if (!string.IsNullOrEmpty(filters.ContainsCharacter)) parsedFilters["contains_character"] = filters.ContainsCharacter;

            // Check for conflicting filters
            if (filters.MinLength.HasValue && filters.MaxLength.HasValue && filters.MinLength > filters.MaxLength)
            {
                return UnprocessableEntity(new ErrorResponse
                {
                    Error = "Unprocessable Entity",
                    Message = "Query parsed but resulted in conflicting filters"
                });
            }

            var response = new NaturalLanguageResponse
            {
                Data = results.Select(MapToResponse).ToList(),
                Count = results.Count,
                InterpretedQuery = new InterpretedQuery
                {
                    Original = query,
                    ParsedFilters = parsedFilters
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse natural language query: {Query}", query);
            return BadRequest(new ErrorResponse
            {
                Error = "Bad Request",
                Message = "Unable to parse natural language query"
            });
        }
    }

    [HttpDelete("{stringValue}")]
    public IActionResult DeleteString(string stringValue)
    {
        var deleted = _repository.Delete(stringValue);
        
        if (!deleted)
        {
            return NotFound(new ErrorResponse
            {
                Error = "Not Found",
                Message = "String does not exist in the system"
            });
        }

        return NoContent();
    }

    private StringAnalysisResponse MapToResponse(StringAnalysis analysis)
    {
        return new StringAnalysisResponse
        {
            Id = analysis.Id,
            Value = analysis.Value,
            Properties = new PropertiesResponse
            {
                Length = analysis.Properties.Length,
                IsPalindrome = analysis.Properties.IsPalindrome,
                UniqueCharacters = analysis.Properties.UniqueCharacters,
                WordCount = analysis.Properties.WordCount,
                Sha256Hash = analysis.Properties.Sha256Hash,
                CharacterFrequencyMap = analysis.Properties.CharacterFrequencyMap
            },
            CreatedAt = analysis.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
    }
}

