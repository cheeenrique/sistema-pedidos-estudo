# Roadmap

## Phase 1 - Core Foundation

- [x] Base project structure with clean layers.
- [x] Initial domain aggregate for order flow.
- [x] Initial create-order use case.
- [ ] Build solution file and verify compile/test pipeline.

## Phase 2 - Persistence and Reliability

- [ ] EF Core migrations and PostgreSQL schema evolution.
- [ ] Transaction boundaries and concurrency handling.
- [ ] Domain event dispatch strategy.

## Phase 3 - Security and Identity

- [x] JWT authentication.
- [x] Refresh token flow with rotation and revocation.
- [x] ASP.NET Identity integration.
- [x] Policy-based authorization.

## Phase 4 - Testing and Quality

- [ ] Full unit tests for domain and application use cases.
- [ ] Integration tests for HTTP contract + persistence.
- [ ] Architecture tests for dependency rules.

## Phase 5 - Performance and Operations

- [ ] Redis caching and memory cache strategy.
- [ ] Rate limiting and resilience patterns.
- [ ] Health checks, metrics, and tracing.
- [ ] Background services for async processing.
