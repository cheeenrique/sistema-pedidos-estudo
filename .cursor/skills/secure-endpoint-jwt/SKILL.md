---
name: secure-endpoint-jwt
description: Applies JWT authentication and authorization best practices to ASP.NET Core APIs. Use when creating login flows, protecting endpoints, defining claims/policies, refresh token handling, or validating token security settings.
---

# Secure Endpoint with JWT

## Security workflow

1. Configure token validation parameters centrally.
2. Define authorization policies by capability (claims), not by hardcoded role checks.
3. Keep refresh token lifecycle secure (rotation, expiration, revocation).
4. Return consistent unauthorized and forbidden responses.
5. Never log credentials, tokens, or secrets.

## Minimum checks

- HTTPS required
- Token issuer and audience validated
- Short-lived access token and managed refresh token
