# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY TaskDeck.sln .
COPY src/TaskDeck.Api/TaskDeck.Api.csproj src/TaskDeck.Api/
COPY src/TaskDeck.Domain/TaskDeck.Domain.csproj src/TaskDeck.Domain/
COPY src/TaskDeck.Application/TaskDeck.Application.csproj src/TaskDeck.Application/
COPY src/TaskDeck.Infrastructure/TaskDeck.Infrastructure.csproj src/TaskDeck.Infrastructure/
COPY src/TaskDeck.Common/TaskDeck.Common.csproj src/TaskDeck.Common/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY src/ src/

# Build and publish
WORKDIR /src/src/TaskDeck.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user
RUN adduser --disabled-password --gecos "" appuser

# Copy published app
COPY --from=build /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

USER appuser

EXPOSE 5000

ENTRYPOINT ["dotnet", "TaskDeck.Api.dll"]
