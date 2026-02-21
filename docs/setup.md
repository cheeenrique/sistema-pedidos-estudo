# Setup

## Requirements

- .NET 8 SDK
- PostgreSQL 15+ (or compatible)

## Install .NET SDK on Windows

```powershell
winget install Microsoft.DotNet.SDK.8
dotnet --version
```

## Project bootstrap commands

```powershell
dotnet restore
dotnet build
dotnet ef database update --project src/Ordering.Infrastructure --startup-project src/Ordering.Api
dotnet run --project src/Ordering.Api
```

## Docker bootstrap (recommended for local consistency)

```powershell
docker compose up --build db api-dev
```

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- Code updates are automatically reloaded by `dotnet watch` in container.

## Identity bootstrap

On startup, the API seeds default roles and users if they do not exist.

- Roles: `admin`, `sales`, `viewer`, `catalog-manager`
- Users:
  - `admin` with password `Admin123!`
  - `sales` with password `Sales123!`

## Connection string

Default value is in:

- `src/Ordering.Api/appsettings.json`

Use environment-specific configuration for real environments:

- `appsettings.Development.json`
- `appsettings.Production.json`
- environment variables / secret manager

## Optional developer tools

- EF Core CLI:

```powershell
dotnet tool install --global dotnet-ef
```

- Create initial migration (after solution is fully wired):

```powershell
dotnet ef migrations add InitialCreate --project src/Ordering.Infrastructure --startup-project src/Ordering.Api
dotnet ef database update --project src/Ordering.Infrastructure --startup-project src/Ordering.Api
```
