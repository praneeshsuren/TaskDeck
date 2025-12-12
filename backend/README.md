# TaskDeck Backend

A .NET 8 Web API backend for TaskDeck - a task management application built with Clean Architecture principles.

## Architecture

This project follows Clean Architecture with the following layers:

- **TaskDeck.Api** - Presentation layer (Controllers, Minimal APIs, SignalR Hubs)
- **TaskDeck.Application** - Application layer (Commands, Queries, Services, DTOs)
- **TaskDeck.Domain** - Domain layer (Entities, Value Objects, Enums, Events)
- **TaskDeck.Infrastructure** - Infrastructure layer (Database, External Services, Repositories)
- **TaskDeck.Common** - Shared utilities and constants

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/) (optional, for containerized development)
- [PostgreSQL 15+](https://www.postgresql.org/) (or use Docker)

## Getting Started

### 1. Clone and Setup

```bash
cd backend
cp .env.example .env
# Edit .env with your configuration
```

### 2. Run with Docker

```bash
docker-compose up -d
```

### 3. Run Locally

```bash
# Start PostgreSQL (via Docker or locally)
docker-compose up postgres -d

# Restore dependencies
dotnet restore

# Run migrations
dotnet ef database update --project src/TaskDeck.Infrastructure --startup-project src/TaskDeck.Api

# Run the API
dotnet run --project src/TaskDeck.Api
```

## Project Structure

```
backend/
├─ src/
│  ├─ TaskDeck.Api/           # Web API layer
│  ├─ TaskDeck.Application/   # Business logic
│  ├─ TaskDeck.Domain/        # Domain models
│  ├─ TaskDeck.Infrastructure/# Data access & external services
│  └─ TaskDeck.Common/        # Shared utilities
├─ tests/
│  ├─ TaskDeck.Application.Tests/
│  ├─ TaskDeck.Infrastructure.Tests/
│  └─ TaskDeck.Api.IntegrationTests/
└─ docs/
```

## API Documentation

Once running, access Swagger UI at: `http://localhost:5000/swagger`

## Testing

```bash
dotnet test
```

## License

MIT
