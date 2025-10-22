# String Analyzer API

A RESTful API service built with .NET 8 that analyzes strings and stores their computed properties. This service computes various string metrics including length, palindrome detection, unique character count, word count, SHA-256 hash, and character frequency mapping.

## Features

- ✨ Analyze strings with detailed property computation
- 🔍 Filter strings using structured query parameters
- 💬 Natural language query support
- 🚀 Fast in-memory storage
- 📝 Comprehensive error handling
- 🔒 SHA-256 hashing for unique identification

## Tech Stack

- **Framework**: .NET 8.0 (ASP.NET Core Web API)
- **Language**: C# 12
- **Storage**: In-memory (thread-safe dictionary)
- **Documentation**: Swagger/OpenAPI

## Prerequisites

Before you begin, make sure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A code editor (Visual Studio, VS Code, or Rider recommended)
- Git (for cloning the repository)

## Local Development Setup

### 1. Clone the Repository

```bash
git clone <your-repo-url>
cd hng13-stage1-backend
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Project

```bash
dotnet build
```

### 4. Run the Application

```bash
dotnet run
```

The API will start and be available at:

- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

You can access the Swagger documentation at `http://localhost:5000/swagger` when running in development mode.

## API Endpoints

### 1. Create/Analyze String

Analyzes a string and stores its computed properties.

**Request:**

```http
POST /strings
Content-Type: application/json

{
  "value": "hello world"
}
```

**Success Response (201 Created):**

```json
{
  "id": "b94d27b9934d3e08a52e52d7da7dabfac484efe37a5380ee9088f7ace2efcde9",
  "value": "hello world",
  "properties": {
    "length": 11,
    "is_palindrome": false,
    "unique_characters": 8,
    "word_count": 2,
    "sha256_hash": "b94d27b9934d3e08a52e52d7da7dabfac484efe37a5380ee9088f7ace2efcde9",
    "character_frequency_map": {
      "h": 1,
      "e": 1,
      "l": 3,
      "o": 2,
      " ": 1,
      "w": 1,
      "r": 1,
      "d": 1
    }
  },
  "created_at": "2025-10-22T12:30:45Z"
}
```

**Error Responses:**

- `400 Bad Request`: Invalid request body or missing "value" field
- `409 Conflict`: String already exists in the system
- `422 Unprocessable Entity`: Invalid data type for "value"

### 2. Get Specific String

Retrieves a previously analyzed string by its exact value.

**Request:**

```http
GET /strings/hello%20world
```

**Success Response (200 OK):**

```json
{
  "id": "b94d27b9934d3e08a52e52d7da7dabfac484efe37a5380ee9088f7ace2efcde9",
  "value": "hello world",
  "properties": {
    /* same as above */
  },
  "created_at": "2025-10-22T12:30:45Z"
}
```

**Error Response:**

- `404 Not Found`: String does not exist in the system

### 3. Get All Strings with Filtering

Retrieves all strings with optional filtering.

**Request:**

```http
GET /strings?is_palindrome=true&min_length=3&max_length=10&word_count=1&contains_character=a
```

**Query Parameters:**

- `is_palindrome` (boolean): Filter by palindrome status
- `min_length` (integer): Minimum string length
- `max_length` (integer): Maximum string length
- `word_count` (integer): Exact word count
- `contains_character` (string): Single character to search for

**Success Response (200 OK):**

```json
{
  "data": [
    {
      "id": "hash1",
      "value": "racecar",
      "properties": {
        /* ... */
      },
      "created_at": "2025-10-22T12:30:45Z"
    }
  ],
  "count": 1,
  "filters_applied": {
    "is_palindrome": true,
    "min_length": 3,
    "max_length": 10,
    "word_count": 1,
    "contains_character": "a"
  }
}
```

**Error Response:**

- `400 Bad Request`: Invalid query parameter values or types

### 4. Natural Language Filtering

Filter strings using natural language queries.

**Request:**

```http
GET /strings/filter-by-natural-language?query=all%20single%20word%20palindromic%20strings
```

**Supported Query Examples:**

- `"all single word palindromic strings"` → `word_count=1, is_palindrome=true`
- `"strings longer than 10 characters"` → `min_length=11`
- `"strings containing the letter z"` → `contains_character=z`
- `"palindromic strings that contain the first vowel"` → `is_palindrome=true, contains_character=a`

**Success Response (200 OK):**

```json
{
  "data": [
    /* array of matching strings */
  ],
  "count": 3,
  "interpreted_query": {
    "original": "all single word palindromic strings",
    "parsed_filters": {
      "word_count": 1,
      "is_palindrome": true
    }
  }
}
```

**Error Responses:**

- `400 Bad Request`: Unable to parse natural language query
- `422 Unprocessable Entity`: Query parsed but resulted in conflicting filters

### 5. Delete String

Deletes a string from the system.

**Request:**

```http
DELETE /strings/hello%20world
```

**Success Response:**

- `204 No Content` (empty body)

**Error Response:**

- `404 Not Found`: String does not exist in the system

## Project Structure

```
hng13-stage1-backend/
├── Controllers/
│   └── StringsController.cs       # API endpoints
├── Models/
│   ├── StringAnalysis.cs          # Core domain model
│   ├── DTOs.cs                    # Request/Response DTOs
│   └── StringFilters.cs           # Filter model
├── Services/
│   ├── StringAnalyzerService.cs   # String analysis logic
│   ├── StringRepository.cs        # Data storage
│   └── NaturalLanguageParser.cs   # NL query parser
├── Program.cs                     # Application entry point
├── appsettings.json              # Configuration
└── StringAnalyzer.csproj         # Project file
```

## Dependencies

This project uses minimal dependencies to keep it lightweight:

- **Swashbuckle.AspNetCore** (6.5.0) - Swagger/OpenAPI documentation

All other functionality is built using .NET 8's built-in libraries.

## Environment Variables

This application doesn't require any environment variables for local development. However, when deploying to production, you may want to configure:

- `ASPNETCORE_ENVIRONMENT`: Set to "Production" for production deployments
- `ASPNETCORE_URLS`: Specify the URLs the app should listen on (e.g., `http://0.0.0.0:8080`)

## Deployment to Railway

This project is configured for Railway deployment with the included `railway.json` configuration file.

### Option 1: Deploy via Railway CLI

1. Install the Railway CLI:

```bash
npm i -g @railway/cli
```

2. Login to Railway:

```bash
railway login
```

3. Initialize and deploy:

```bash
railway init
railway up
```

4. Get your deployment URL:

```bash
railway domain
```

### Option 2: Deploy via GitHub Integration

1. Push your code to GitHub
2. Go to [Railway](https://railway.app)
3. Click "New Project" → "Deploy from GitHub repo"
4. Select your repository
5. Railway will automatically detect the .NET project and deploy it

The `railway.json` file is already configured with the correct settings:

- Builder: NIXPACKS (auto-detects .NET)
- Start command: `dotnet run --urls http://0.0.0.0:$PORT`
- Restart policy: ON_FAILURE with max 10 retries

## Testing the API

You can test the API using curl, Postman, or any HTTP client. Here are some example curl commands:

### Create a string

```bash
curl -X POST http://localhost:5000/strings \
  -H "Content-Type: application/json" \
  -d '{"value":"racecar"}'
```

### Get a string

```bash
curl http://localhost:5000/strings/racecar
```

### Filter strings

```bash
curl "http://localhost:5000/strings?is_palindrome=true"
```

### Natural language query

```bash
curl "http://localhost:5000/strings/filter-by-natural-language?query=single%20word%20palindromic%20strings"
```

### Delete a string

```bash
curl -X DELETE http://localhost:5000/strings/racecar
```

## Development Notes

- The application uses in-memory storage, so data will be lost when the application restarts. For production use, consider integrating with a database (PostgreSQL, MongoDB, etc.)
- Thread-safe operations are ensured using locks in the repository layer
- The natural language parser uses regex patterns and keyword matching - it can be extended with more sophisticated NLP techniques
- SHA-256 hashing is used for unique identification of strings

## Troubleshooting

### Port already in use

If you get a port conflict error, you can specify a different port:

```bash
dotnet run --urls "http://localhost:5500"
```

### .NET SDK not found

Make sure you have .NET 8 SDK installed:

```bash
dotnet --version
```

### Build errors

Try cleaning and rebuilding:

```bash
dotnet clean
dotnet restore
dotnet build
```

## License

This project is part of the HNG13 Backend Wizards Stage 1 task.

## Author

Built with care and attention to detail.
