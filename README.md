# Ordering System - Professional Backend (.NET 8)

Learning-first and production-ready backend foundation with C#, ASP.NET Core, and clean architecture.

## Learning goals

- Apply Clean Architecture with tactical DDD.
- Use CQRS with MediatR for use case orchestration.
- Evolve security with JWT and refresh tokens.
- Implement observability, tests, and performance techniques.

## Structure

- `src/Ordering.Domain`: pure business rules.
- `src/Ordering.Application`: use cases and contracts.
- `src/Ordering.Infrastructure`: persistence and technical integrations.
- `src/Ordering.Api`: HTTP layer (controllers, middleware, DI).
- `tests/Ordering.UnitTests`: unit tests.
- `tests/Ordering.IntegrationTests`: integration tests.
- `.cursor/rules`: persistent agent rules.
- `.cursor/skills`: reusable project skills.

## Current status

- Clean Architecture + DDD tactical patterns + CQRS with MediatR.
- APIs implemented: `auth`, `orders`, `customers`, `products`.
- Standard response envelope for success and errors.
- JWT authentication + ASP.NET Identity + role-based policies.
- Refresh token flow with rotation and revocation.
- Docker setup for dev hot reload and production-like runtime.

## Implementation roadmap

1. Domain model and core order use cases.
2. EF Core + PostgreSQL + migrations.
3. JWT + Identity + policy-based authorization.
4. Caching (Memory/Redis), rate limiting, and background services.
5. Unit, integration, and architecture tests.

## Local prerequisite

Install .NET 8 SDK:

- Windows (winget): `winget install Microsoft.DotNet.SDK.8`
- Verify: `dotnet --version`

## Commands when SDK is available

Restore:

`dotnet restore`

Build:

`dotnet build`

Run API:

`dotnet run --project src/Ordering.Api`

## Docker commands

### First setup

```powershell
copy .env.example .env
```

### Development mode (hot reload)

```powershell
docker compose up --build db api-dev
```

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`

### Production-like mode

```powershell
docker compose --profile prod up --build db api
```

- API: `http://localhost:8081`

### Useful Docker commands

```powershell
# Build API image only
docker compose build api

# Follow dev API logs
docker compose logs -f api-dev

# Stop containers
docker compose down

# Stop and remove volumes (database reset)
docker compose down -v

# Run migration command inside dev container
docker compose exec api-dev dotnet ef migrations add MigrationName --project src/Ordering.Infrastructure --startup-project src/Ordering.Api
```

## Database startup behavior

- Docker uses `.env` with `DATABASE_RUN_MIGRATIONS_ON_STARTUP=true`.
- On API startup, pending EF migrations are automatically applied.
