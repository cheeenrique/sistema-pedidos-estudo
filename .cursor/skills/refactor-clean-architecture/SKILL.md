---
name: refactor-clean-architecture
description: Refactors backend code toward clean architecture and SOLID principles. Use when reducing coupling, moving business logic to domain/application, enforcing dependency direction, and improving maintainability.
---

# Refactor to Clean Architecture

## Refactor sequence

1. Identify boundary violations (infrastructure leaks into domain/application).
2. Extract interfaces in `Application` for external dependencies.
3. Move business rules from controllers/services into domain or use-case handlers.
4. Simplify orchestration methods and remove mixed responsibilities.
5. Add or update tests to lock desired behavior.

## Done criteria

- Dependency direction is correct
- Responsibilities are clear per layer
- Public contracts remain stable or are versioned
