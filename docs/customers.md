# Customers Data Guide

This document describes the expanded customer registration model and practical usage.

## Core fields

- `fullName`: customer display name.
- `email`: unique contact email.
- `documentNumber`: government/company document identifier (CPF/CNPJ or equivalent).
- `phoneNumber`: primary phone number for operational contact.
- `customerType`: `individual` or `company`.

## Profile fields

- `birthDate`: optional date of birth for individual customers.
- `street`, `city`, `state`, `postalCode`, `country`: optional address information.
- `notes`: free text for business notes and operational context.

## Why these fields are useful

- Better order support and customer service operations.
- Better filtering and lookup by document and phone.
- Better auditability and CRM readiness for future integrations.

## Recommended validation rules

- Keep `documentNumber` and `phoneNumber` normalized.
- Block duplicate `email` and duplicate `documentNumber`.
- Allow `customerType` only as `individual` or `company`.
- Reject future `birthDate`.
