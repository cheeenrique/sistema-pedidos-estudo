# API Notes

## Base route

- `api/orders`

## Standard response format

### Success envelope

```json
{
  "success": true,
  "message": "Request completed successfully.",
  "data": {},
  "traceId": "00-f7b7024db3f7f04f9f4d2f3e4e5a0db5-6b8f2417d9399999-00"
}
```

### Error envelope

```json
{
  "success": false,
  "errorCode": "ValidationError",
  "message": "One or more validation errors occurred.",
  "statusCode": 400,
  "errors": {
    "sortDirection": [
      "Allowed values: asc, desc."
    ]
  },
  "traceId": "00-f7b7024db3f7f04f9f4d2f3e4e5a0db5-6b8f2417d9399999-00"
}
```

## Global exception handling

Unhandled exceptions are converted to the standard error envelope by middleware.

- `ValidationException` -> `400 ValidationError`
- `ArgumentException` -> `400 BadRequest`
- `InvalidOperationException` -> `400 BusinessRuleViolation`
- `KeyNotFoundException` -> `404 NotFound`
- Any other exception -> `500 InternalServerError`

### Internal server error example (`500`)

```json
{
  "success": false,
  "errorCode": "InternalServerError",
  "message": "An unexpected error occurred.",
  "statusCode": 500,
  "errors": null,
  "traceId": "00-f7b7024db3f7f04f9f4d2f3e4e5a0db5-6b8f2417d9399999-00"
}
```

## Endpoints

### `POST /api/auth/login`

Returns JWT access token for protected APIs.

Default seeded users (development bootstrap):

- `admin` / `Admin123!`
- `sales` / `Sales123!`

#### Request body

```json
{
  "username": "admin",
  "password": "admin123"
}
```

#### Success response (`200 OK`)

```json
{
  "success": true,
  "message": "Authentication successful.",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "very-long-random-token",
    "tokenType": "Bearer",
    "expiresInSeconds": 3600
  },
  "traceId": "00-f7b7024db3f7f04f9f4d2f3e4e5a0db5-6b8f2417d9399999-00"
}
```

### `POST /api/auth/refresh`

Rotates refresh token and returns a new access token + refresh token pair.

#### Request body

```json
{
  "refreshToken": "very-long-random-token"
}
```

### `POST /api/auth/revoke`

Protected endpoint. Revokes refresh token.

#### Request body

```json
{
  "refreshToken": "optional-token-if-you-want-to-revoke-a-specific-one"
}
```

If `refreshToken` is omitted, API revokes latest active refresh token for authenticated user.

### `POST /api/auth/register`

Creates a new identity user and assigns default role `viewer`.

#### Request body

```json
{
  "username": "new-user",
  "email": "new-user@ordering.local",
  "password": "StrongPass123"
}
```

### `GET /api/customers`

Protected endpoint. Returns paginated customer list.

#### Query parameters

- `page` (int, default `1`)
- `pageSize` (int, default `10`, max `100`)
- `search` (optional: full name or email)
- `sortBy` (optional: `createdAtUtc`, `fullName`, `email`)
- `sortDirection` (optional: `asc`, `desc`)

### `POST /api/customers`

Protected endpoint. Creates a customer.

#### Request body

```json
{
  "fullName": "Carlos Henrique",
  "email": "carlos@example.com",
  "documentNumber": "12345678901",
  "phoneNumber": "+55 11 99999-8888",
  "customerType": "individual",
  "birthDate": "1990-08-10",
  "street": "Av. Paulista, 1000",
  "city": "Sao Paulo",
  "state": "SP",
  "postalCode": "01310-100",
  "country": "Brazil",
  "notes": "Prefers WhatsApp contact."
}
```

#### Success response (`201 Created`)

```json
{
  "success": true,
  "message": "Customer created successfully.",
  "data": {
    "customerId": "00000000-0000-0000-0000-000000000001",
    "fullName": "Carlos Henrique",
    "email": "carlos@example.com",
    "documentNumber": "12345678901",
    "phoneNumber": "+55 11 99999-8888",
    "customerType": "individual",
    "birthDate": "1990-08-10T00:00:00Z",
    "city": "Sao Paulo",
    "state": "SP",
    "country": "Brazil",
    "isActive": true
  },
  "traceId": "00-f7b7024db3f7f04f9f4d2f3e4e5a0db5-6b8f2417d9399999-00"
}
```

### `GET /api/customers/{customerId}`

Protected endpoint. Returns customer details.

### `GET /api/products`

Protected endpoint. Returns paginated product list.

#### Query parameters

- `page` (int, default `1`)
- `pageSize` (int, default `10`, max `100`)
- `search` (optional: sku or name)
- `isActive` (optional bool)
- `sortBy` (optional: `createdAtUtc`, `sku`, `name`, `price`)
- `sortDirection` (optional: `asc`, `desc`)

### `POST /api/products`

Protected endpoint. Creates a product.

### `GET /api/products/{productId}`

Protected endpoint. Returns product details.

### `GET /api/orders?page=1&pageSize=10`

Returns paginated order list.

#### Query parameters

- `page` (int, default `1`)
- `pageSize` (int, default `10`, max `100`)
- `status` (optional: `Draft`, `Submitted`, `Paid`, `Shipped`, `Delivered`, `Cancelled`)
- `customerId` (optional GUID)
- `createdFromUtc` (optional datetime in UTC)
- `createdToUtc` (optional datetime in UTC)
- `sortBy` (optional: `createdAtUtc`, `customerId`, `status`)
- `sortDirection` (optional: `asc`, `desc`)

#### Success response (`200 OK`)

```json
{
  "success": true,
  "message": "Request completed successfully.",
  "data": {
    "items": [
      {
        "orderId": "00000000-0000-0000-0000-000000000999",
        "customerId": "00000000-0000-0000-0000-000000000001",
        "createdAtUtc": "2026-02-21T18:05:12.917Z",
        "status": "Submitted",
        "totalAmount": 60.00,
        "itemsCount": 1
      }
    ],
    "pagination": {
      "page": 1,
      "pageSize": 10,
      "totalCount": 1,
      "totalPages": 1
    }
  },
  "traceId": "00-f7b7024db3f7f04f9f4d2f3e4e5a0db5-6b8f2417d9399999-00"
}
```

### `POST /api/orders`

Creates a new order and submits it.

#### Request body

```json
{
  "customerId": "00000000-0000-0000-0000-000000000001",
  "items": [
    {
      "productId": "00000000-0000-0000-0000-000000000101",
      "quantity": 2,
      "unitPrice": 30.00
    }
  ]
}
```

#### Success response (`201 Created`)

```json
{
  "success": true,
  "message": "Order created successfully.",
  "data": {
    "orderId": "00000000-0000-0000-0000-000000000999",
    "totalAmount": 60.00,
    "status": "Submitted"
  },
  "traceId": "00-f7b7024db3f7f04f9f4d2f3e4e5a0db5-6b8f2417d9399999-00"
}
```

### `GET /api/orders/{orderId}`

Returns detailed order information.

#### Success response (`200 OK`)

```json
{
  "success": true,
  "message": "Request completed successfully.",
  "data": {
    "orderId": "00000000-0000-0000-0000-000000000999",
    "customerId": "00000000-0000-0000-0000-000000000001",
    "createdAtUtc": "2026-02-21T18:05:12.917Z",
    "status": "Submitted",
    "totalAmount": 60.00,
    "items": [
      {
        "productId": "00000000-0000-0000-0000-000000000101",
        "quantity": 2,
        "unitPrice": 30.00,
        "lineTotal": 60.00
      }
    ]
  },
  "traceId": "00-f7b7024db3f7f04f9f4d2f3e4e5a0db5-6b8f2417d9399999-00"
}
```

#### Not found response (`404 Not Found`)

```json
{
  "success": false,
  "errorCode": "NotFound",
  "message": "Order was not found.",
  "statusCode": 404,
  "errors": null,
  "traceId": "00-f7b7024db3f7f04f9f4d2f3e4e5a0db5-6b8f2417d9399999-00"
}
```
### `POST /api/orders/{orderId}/cancel`

Protected endpoint. Cancels an order when business rules allow it.
