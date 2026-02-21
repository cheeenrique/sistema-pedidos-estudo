# Architecture

## High-level style

- Clean Architecture with DDD tactical patterns.
- CQRS with MediatR for use case orchestration.
- ASP.NET Core Web API as presentation layer.

## Layers

### Domain (`src/Ordering.Domain`)

- Business rules, entities, value objects, and domain events.
- No dependency on Application, Infrastructure, or API.

### Application (`src/Ordering.Application`)

- Commands, queries, handlers, and use case contracts.
- Depends on Domain only.
- Uses abstractions for persistence/time/external services.

### Infrastructure (`src/Ordering.Infrastructure`)

- EF Core DbContext, repositories, and technical integrations.
- Implements Application abstractions.
- Depends on Application and Domain.

### API (`src/Ordering.Api`)

- Controllers, request/response mapping, middleware pipeline, and DI composition.
- Depends on Application and Infrastructure.

## Dependency direction

- Allowed: `Api -> Application -> Domain`
- Allowed: `Api -> Infrastructure -> Application`
- Forbidden: `Domain -> *`
- Forbidden: `Application -> Api/Infrastructure`

## Current aggregate

- `Order` is the aggregate root.
- `OrderItem` belongs to `Order`.
- Invariants:
  - Only draft orders can be modified.
  - Empty orders cannot be submitted.

## Next architecture upgrades

- Add cross-cutting pipeline behaviors (validation/logging).
- Add Outbox pattern for reliable domain event publishing.
- Add read models for query optimization.
