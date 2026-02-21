# Migrations Workflow

Use EF Core migrations to keep schema versioned and reproducible.

## Create initial migration

```powershell
dotnet ef migrations add InitialIdentityAndOrdering --project src/Ordering.Infrastructure --startup-project src/Ordering.Api
```

## Apply migration locally

```powershell
dotnet ef database update --project src/Ordering.Infrastructure --startup-project src/Ordering.Api
```

## Generate SQL script for deployment

```powershell
dotnet ef migrations script --project src/Ordering.Infrastructure --startup-project src/Ordering.Api --output ./artifacts/sql/initial.sql
```

## Team conventions

- Do not edit generated migration code manually unless strictly required.
- One migration per logical change set.
- Keep migration names explicit (example: `AddRefreshTokenTable`).
- Update docs when migration introduces public contract impact.
