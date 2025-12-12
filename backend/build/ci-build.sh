#!/bin/bash
set -e

echo "=== TaskDeck CI Build ==="

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore

# Build the solution
echo "Building solution..."
dotnet build --configuration Release --no-restore

# Run tests
echo "Running tests..."
dotnet test --configuration Release --no-build --verbosity normal

# Publish
echo "Publishing API..."
dotnet publish src/TaskDeck.Api/TaskDeck.Api.csproj --configuration Release --no-build --output ./publish

echo "=== Build Complete ==="
