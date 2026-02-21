#!/bin/sh
set -eu

echo "Waiting for PostgreSQL..."
until pg_isready -h db -p 5432 -U postgres > /dev/null 2>&1; do
  sleep 1
done

echo "PostgreSQL is ready."

echo "Restoring dependencies..."
dotnet restore /src/src/Ordering.Api/Ordering.Api.csproj

echo "Applying migrations if available..."
if ls /src/src/Ordering.Infrastructure/Migrations/*.cs > /dev/null 2>&1; then
  dotnet ef database update --project src/Ordering.Infrastructure --startup-project src/Ordering.Api
else
  echo "No EF migrations found. Development startup will rely on EnsureCreated."
fi

echo "Starting API with hot reload..."
dotnet watch --project src/Ordering.Api run --urls http://0.0.0.0:8080
