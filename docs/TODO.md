# Backend Delivery TODO

## Phase A - Security Foundation

- [x] Configure JWT settings and token validation.
- [x] Add authentication and authorization middleware in API pipeline.
- [x] Add auth endpoint for login and token issuance.
- [x] Protect business endpoints with `[Authorize]`.

## Phase B - Customer API

- [x] Add `Customer` domain model and persistence mapping.
- [x] Add create/get/list customer use cases.
- [x] Add customer request validation and controller.
- [x] Apply standardized success/error envelope.

## Phase C - Product API

- [x] Add `Product` domain model and persistence mapping.
- [x] Add create/get/list product use cases.
- [x] Add product filters and pagination.
- [x] Apply standardized success/error envelope.

## Phase D - Orders Evolution

- [x] Add cancel order use case and endpoint.
- [x] Keep order endpoints protected and consistent.
- [x] Validate business rules with explicit error messages.

## Phase E - Documentation

- [x] Update `docs/api.md` with auth, customers, products, and orders changes.
- [ ] Update `docs/roadmap.md` to reflect delivered milestones.
