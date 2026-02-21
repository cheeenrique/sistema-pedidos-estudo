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
