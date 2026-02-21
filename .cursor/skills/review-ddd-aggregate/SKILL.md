---
name: review-ddd-aggregate
description: Reviews aggregate design and entity behavior using DDD tactical patterns. Use when implementing or refactoring aggregate roots, value objects, invariants, and transactional consistency rules.
---

# Review DDD Aggregate

## Review points

- Aggregate root protects invariant boundaries.
- External code does not mutate child collections directly.
- State transitions are explicit methods with guard clauses.
- Value objects are immutable and equality-safe.
- Domain events represent business facts, not technical events.

## Suggested output format

- Critical invariants at risk
- Design improvements
- Suggested code adjustments
