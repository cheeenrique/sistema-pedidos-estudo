---
name: design-usecase-cqrs
description: Designs or refines backend use cases using CQRS and MediatR with clean architecture boundaries. Use when creating new commands, queries, handlers, request/response models, or endpoint-to-use-case mapping.
---

# Design Use Case with CQRS

## Workflow

1. Identify if the request is a command (state change) or query (read model).
2. Create request/response contracts with explicit fields and validation assumptions.
3. Implement handler with one use case responsibility and dependency on abstractions only.
4. Keep business invariants in domain entities or value objects, not in controllers.
5. Map API contracts to command/query contracts in the presentation layer.

## Output checklist

- Clear command/query name
- Minimal handler dependencies
- Domain invariants preserved
- Persistence interaction through repository abstraction
