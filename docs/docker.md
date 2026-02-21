# Docker Setup

This project provides Docker setup for development (hot reload) and production-like runtime.

## Services

- `db`: PostgreSQL 16
- `api-dev`: .NET SDK container with `dotnet watch` (hot reload)
- `api`: production-like API container (profile `prod`)

## Development mode (hot reload)

From project root:

```powershell
# Optional first step: copy template env file
copy .env.example .env

docker compose up --build db api-dev
```

API endpoints:

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- PostgreSQL: `localhost:5432`

### Live update workflow

1. Keep `docker compose up` running.
2. Edit any `.cs` file in your local project.
3. `dotnet watch` inside `api-dev` detects changes and reloads automatically.

No manual image rebuild is required for code-only changes.

When `DATABASE_RUN_MIGRATIONS_ON_STARTUP=true`, API automatically applies pending EF migrations on startup.

## Environment variables

Project-level variables are centralized in `.env`.

- Tracked template: `.env.example`
- Local private file: `.env` (ignored by git)

## Production-like mode

```powershell
docker compose --profile prod up --build db api
```

API endpoint:

- API: `http://localhost:8081`

## Useful commands

```powershell
# Stop and remove containers
docker compose down

# Stop and remove containers + volumes (reset database)
docker compose down -v

# Follow API logs
docker compose logs -f api-dev

# Run EF migration command inside dev container
docker compose exec api-dev dotnet ef migrations add InitialIdentityAndOrdering --project src/Ordering.Infrastructure --startup-project src/Ordering.Api
```
