---
name: generate-test-suite-dotnet
description: Generates and improves unit and integration test suites for .NET backend code. Use when adding new handlers, domain logic, controllers, regression tests, and test coverage for critical scenarios.
---

# Generate .NET Test Suite

## Unit test workflow

1. Focus on behavior and business outcomes.
2. Use deterministic input data and explicit assertions.
3. Cover happy path, validation failures, and edge cases.

## Integration test workflow

1. Exercise HTTP endpoint contracts end-to-end.
2. Verify persistence effects and response payload.
3. Validate status code and error shape for failure scenarios.

## Quality bar

- Tests are independent and reproducible.
- Names describe behavior clearly.
- No assertions on irrelevant implementation details.
