# Conventions

## Language and naming

- Use English for all code artifacts.
- Use descriptive names for methods and variables.
- Keep folder and namespace alignment consistent.

## Code structure

- One primary responsibility per class.
- Keep controllers thin and use handlers for orchestration.
- Keep business rules in domain entities/value objects.

## Error handling

- Throw explicit exceptions with meaningful messages.
- Avoid silent catch blocks.
- Map exceptions to consistent HTTP responses in API layer.

## Testing

- Unit tests for domain/application behavior.
- Integration tests for endpoint contracts.
- Name tests as: `Method_ShouldExpectedBehavior_WhenCondition`.

## Pull request quality checklist

- Architecture boundaries respected.
- New behavior covered by tests.
- Public contract changes documented in `docs/api.md`.
- Relevant roadmap tasks updated in `docs/roadmap.md`.
