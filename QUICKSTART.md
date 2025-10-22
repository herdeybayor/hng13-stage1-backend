# Quick Start Guide

Get up and running in 2 minutes! ⚡

## Prerequisites Check

```bash
dotnet --version
# Should show 8.0.x or higher
```

Don't have .NET 8? [Download it here](https://dotnet.microsoft.com/download/dotnet/8.0)

## Run Locally (3 commands)

```bash
# 1. Restore dependencies
dotnet restore

# 2. Build the project
dotnet build

# 3. Run the app
dotnet run
```

The API will be available at `http://localhost:5000`

## Test It Out

Open your browser and go to:

```
http://localhost:5000/swagger
```

Or use curl:

```bash
# Create a string
curl -X POST http://localhost:5000/strings \
  -H "Content-Type: application/json" \
  -d '{"value":"racecar"}'

# Get it back
curl http://localhost:5000/strings/racecar

# Find palindromes
curl "http://localhost:5000/strings?is_palindrome=true"
```

## Deploy to Railway

**Option 1: CLI Deployment**

```bash
# Install Railway CLI
npm i -g @railway/cli

# Login and deploy
railway login
railway init
railway up
```

**Option 2: GitHub Integration** (Recommended)

1. Push your code to GitHub
2. Visit [Railway](https://railway.app)
3. Click "New Project" → "Deploy from GitHub repo"
4. Select your repository and deploy

Done! Your API is now live 🚀

## Common Commands

```bash
# Run with auto-reload (development)
dotnet watch run

# Build for production
dotnet publish -c Release

# Run tests (if you add them)
dotnet test
```

## Need Help?

- Check the full [README.md](README.md) for detailed documentation
- See [test-requests.http](test-requests.http) for example API calls
- View [PROJECT_OVERVIEW.md](PROJECT_OVERVIEW.md) for architecture details

## Project Structure (Simple)

```
├── Controllers/       → API endpoints (the routes)
├── Services/         → Business logic (the brains)
├── Models/          → Data structures (the shapes)
└── Program.cs       → App setup (the wiring)
```

That's it! You're ready to go. 🎉
