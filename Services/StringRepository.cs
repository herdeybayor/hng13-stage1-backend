using StringAnalyzer.Models;

namespace StringAnalyzer.Services;

public interface IStringRepository
{
    bool Exists(string value);
    bool ExistsById(string id);
    void Add(StringAnalysis analysis);
    StringAnalysis? GetByValue(string value);
    StringAnalysis? GetById(string id);
    List<StringAnalysis> GetAll();
    List<StringAnalysis> GetFiltered(StringFilters filters);
    bool Delete(string value);
}

// Using in-memory storage for simplicity - could swap for a real DB later
public class InMemoryStringRepository : IStringRepository
{
    private readonly Dictionary<string, StringAnalysis> _storage = new();
    private readonly object _lock = new();

    public bool Exists(string value)
    {
        lock (_lock)
        {
            return _storage.Values.Any(s => s.Value == value);
        }
    }

    public bool ExistsById(string id)
    {
        lock (_lock)
        {
            return _storage.ContainsKey(id);
        }
    }

    public void Add(StringAnalysis analysis)
    {
        lock (_lock)
        {
            _storage[analysis.Id] = analysis;
        }
    }

    public StringAnalysis? GetByValue(string value)
    {
        lock (_lock)
        {
            return _storage.Values.FirstOrDefault(s => s.Value == value);
        }
    }

    public StringAnalysis? GetById(string id)
    {
        lock (_lock)
        {
            return _storage.TryGetValue(id, out var analysis) ? analysis : null;
        }
    }

    public List<StringAnalysis> GetAll()
    {
        lock (_lock)
        {
            return _storage.Values.ToList();
        }
    }

    public List<StringAnalysis> GetFiltered(StringFilters filters)
    {
        lock (_lock)
        {
            var results = _storage.Values.AsEnumerable();

            if (filters.IsPalindrome.HasValue)
                results = results.Where(s => s.Properties.IsPalindrome == filters.IsPalindrome.Value);

            if (filters.MinLength.HasValue)
                results = results.Where(s => s.Properties.Length >= filters.MinLength.Value);

            if (filters.MaxLength.HasValue)
                results = results.Where(s => s.Properties.Length <= filters.MaxLength.Value);

            if (filters.WordCount.HasValue)
                results = results.Where(s => s.Properties.WordCount == filters.WordCount.Value);

            if (!string.IsNullOrEmpty(filters.ContainsCharacter))
            {
                var searchChar = filters.ContainsCharacter;
                results = results.Where(s => s.Value.Contains(searchChar, StringComparison.OrdinalIgnoreCase));
            }

            return results.ToList();
        }
    }

    public bool Delete(string value)
    {
        lock (_lock)
        {
            var item = _storage.Values.FirstOrDefault(s => s.Value == value);
            if (item != null)
            {
                _storage.Remove(item.Id);
                return true;
            }
            return false;
        }
    }
}

