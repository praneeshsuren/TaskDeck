# TaskDeck Architecture

## Overview

TaskDeck is built using Clean Architecture principles, ensuring separation of concerns and maintainability.

## Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                      Presentation                           │
│                    (TaskDeck.Api)                           │
│        Controllers, Minimal APIs, SignalR Hubs              │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      Application                            │
│                  (TaskDeck.Application)                     │
│     Commands, Queries, Services, DTOs, Interfaces           │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                        Domain                               │
│                   (TaskDeck.Domain)                         │
│        Entities, Value Objects, Enums, Events               │
└─────────────────────────────────────────────────────────────┘
                              ▲
                              │
┌─────────────────────────────────────────────────────────────┐
│                     Infrastructure                          │
│                 (TaskDeck.Infrastructure)                   │
│     Persistence, Repositories, Auth, External Services      │
└─────────────────────────────────────────────────────────────┘
```

## Layer Responsibilities

### TaskDeck.Api (Presentation)

- HTTP request handling
- Authentication/Authorization middleware
- Request/Response DTOs
- SignalR hubs for real-time communication
- Swagger/OpenAPI documentation

### TaskDeck.Application

- Business logic orchestration
- CQRS pattern with MediatR
- Input validation with FluentValidation
- Application-level DTOs
- Service interfaces (ports)

### TaskDeck.Domain

- Domain entities
- Value objects
- Domain enums
- Domain events
- Business rules

### TaskDeck.Infrastructure

- Entity Framework Core DbContext
- Repository implementations
- External service integrations (Firebase)
- JWT token handling
- SignalR message publishing

### TaskDeck.Common

- Shared exceptions
- Utility classes
- Constants

## Key Technologies

| Component | Technology |
|-----------|------------|
| Framework | .NET 8 |
| Web API | ASP.NET Core Minimal APIs / Controllers |
| Database | PostgreSQL |
| ORM | Entity Framework Core 8 |
| CQRS | MediatR |
| Validation | FluentValidation |
| Authentication | JWT / Firebase Auth |
| Real-time | SignalR |
| Testing | xUnit, Moq |
| Containerization | Docker |

## Data Flow

1. **Request comes in** → API Controller/Endpoint
2. **DTO mapping** → Command/Query created
3. **MediatR dispatch** → Handler executes
4. **Business logic** → Uses repositories via interfaces
5. **Data access** → Infrastructure repositories
6. **Response** → DTO returned to client

## Security

- JWT Bearer authentication
- Firebase authentication integration
- Role-based authorization
- CORS policy configuration

## Real-time Updates

- SignalR hub for task updates
- Group-based notifications (per project)
- Client subscription management
