# API Reference

Base URL: `http://localhost:5000`

This document summarizes available endpoints and conventions for the backend API.  
Authentication uses **JWT Bearer** tokens. Pagination and filtering follow common query parameters.

---

## Conventions

### Authentication
- Header: `Authorization: Bearer <token>`

### Pagination & Filtering
- `page` (default: 1), `pageSize` (default: 10)
- `sort` (e.g., `name`, `-createdAt` for descending)
- `q` free-text search (when supported)

### Standard Responses
- Success:
  ```json
  { "success": true, "message": "OK", "data": { ... } }
  ```
- Paged:
  ```json
  {
    "success": true,
    "message": "OK",
    "data": { "items": [ ... ], "total": 0, "page": 1, "pageSize": 10 }
  }
  ```
- Error:
  ```json
  { "success": false, "message": "Validation error", "errors": [ { "field": "email", "message": "Invalid" } ] }
  ```

---

## Health

### GET `/health`
Check service health.
- **200 OK**:
  ```json
  { "status": "Healthy" }
  ```

---

## Auth

### POST `/api/auth/login`
Authenticate user and receive a JWT.
- **Body**
  ```json
  { "email": "user@example.com", "password": "P@ssw0rd!" }
  ```
- **200 OK**
  ```json
  { "token": "<jwt>", "expiresIn": 3600 }
  ```
- **401 Unauthorized** when credentials are invalid.

**cURL**
```bash
curl -X POST http://localhost:5000/api/auth/login   -H "Content-Type: application/json"   -d '{ "email": "user@example.com", "password": "P@ssw0rd!" }'
```

---

## Users

> Endpoints require a valid Bearer token unless stated otherwise.

### POST `/api/users`
Create a user.
- **Body**
  ```json
  {
    "name": "Jane Doe",
    "email": "jane@example.com",
    "phone": "+55 61 99999-9999",
    "password": "P@ssw0rd!",
    "role": "Admin"
  }
  ```
- **201 Created**
  ```json
  {
    "id": "guid",
    "name": "Jane Doe",
    "email": "jane@example.com",
    "phone": "+55 61 99999-9999",
    "role": "Admin",
    "status": "Active",
    "createdAt": "2024-01-01T00:00:00Z"
  }
  ```

### GET `/api/users/{id}`
- **200 OK**: returns user by id.  
- **404 Not Found** if not exists.

### DELETE `/api/users/{id}`
- **200 OK** (or **204 No Content**): user soft-deleted or removed.  
- **404 Not Found** if not exists.

### GET `/api/users`
List users with pagination and filtering.
- **Query**: `page`, `pageSize`, `sort`, `q`  
- **200 OK**: paginated list.

**cURL**
```bash
curl -H "Authorization: Bearer <jwt>"   "http://localhost:5000/api/users?page=1&pageSize=10&sort=-createdAt&q=jane"
```

---

## Domain: Sales (Challenge Scope)

> The challenge requires a complete CRUD for **Sales** including items, discounts, totals, status and branch.

### Entity (example)
```json
{
  "number": "S-2025-0001",
  "date": "2025-08-01T10:00:00Z",
  "customer": "ACME Ltd",
  "branch": "BR-DF-001",
  "status": "Active",
  "items": [
    { "sku": "P001", "name": "Product A", "quantity": 2, "unitPrice": 10.50, "discount": 0.0, "total": 21.00 }
  ],
  "totalAmount": 21.00
}
```

### POST `/api/sales`
Create a sale with items.

### GET `/api/sales/{id}`
Return a sale.

### PUT `/api/sales/{id}`
Update header and items (recalculate totals/discounts).

### DELETE `/api/sales/{id}`
Cancel or remove a sale (as required by the challenge).

### GET `/api/sales`
Pagination + filters by `date`, `customer`, `branch`, `status`, and sorting.

---

## Error Handling
- Validation issues return `success=false` with a list of `errors`.
- Unexpected errors return `500` with a generic message (details hidden).

---

## Local Development Tips
- Use **Docker**: `docker-compose up --build`
- Or run via **.NET CLI** in `template/backend/src/Ambev.DeveloperEvaluation.WebApi`
  ```bash
  dotnet run
  ```
- Run tests: `dotnet test`

