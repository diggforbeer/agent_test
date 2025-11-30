# API Designer Tasks

## Overview
As the API Designer, create comprehensive API specifications, endpoint documentation, and OpenAPI schemas for the Friend Share platform.

---

## Task 1: API Architecture Overview (Priority: Must Have)

**Objective**: Define the overall API architecture and conventions.

### Topics to Cover
- [ ] API versioning strategy
- [ ] URL naming conventions
- [ ] HTTP method usage
- [ ] Request/response formats
- [ ] Error handling patterns
- [ ] Authentication header requirements
- [ ] Rate limiting strategy

### API Conventions
```
Base URL: /api/v1
Content-Type: application/json
Authentication: Bearer JWT tokens
```

### Deliverables
- Output: `docs/api/architecture.md`

---

## Task 2: Authentication API (Priority: Must Have)

**Objective**: Design authentication and authorization endpoints.

### Endpoints Required
```
POST   /api/v1/auth/register       - Register new user
POST   /api/v1/auth/login          - User login
POST   /api/v1/auth/logout         - User logout
POST   /api/v1/auth/refresh        - Refresh access token
POST   /api/v1/auth/forgot-password - Initiate password reset
POST   /api/v1/auth/reset-password - Complete password reset
POST   /api/v1/auth/verify-email   - Verify email address
```

### Documentation Required
- [ ] Request/response schemas
- [ ] Error responses
- [ ] Security considerations
- [ ] Token expiration policies

### Deliverables
- Output: `docs/api/endpoints/authentication.md`
- OpenAPI spec: `docs/api/openapi/auth.yaml`

---

## Task 3: User Profile API (Priority: Must Have)

**Objective**: Design user profile management endpoints.

### Endpoints Required
```
GET    /api/v1/users/me            - Get current user profile
PUT    /api/v1/users/me            - Update current user profile
DELETE /api/v1/users/me            - Delete account
POST   /api/v1/users/me/avatar     - Upload avatar
GET    /api/v1/users/{id}          - Get public user profile
GET    /api/v1/users/{id}/items    - Get user's public items
```

### Deliverables
- Output: `docs/api/endpoints/user-profile.md`
- OpenAPI spec: `docs/api/openapi/users.yaml`

---

## Task 4: Items API (Priority: Must Have)

**Objective**: Design item management endpoints.

### Endpoints Required
```
GET    /api/v1/items               - List items (with filtering)
POST   /api/v1/items               - Create new item
GET    /api/v1/items/{id}          - Get item details
PUT    /api/v1/items/{id}          - Update item
DELETE /api/v1/items/{id}          - Delete item
PATCH  /api/v1/items/{id}/status   - Update item status
POST   /api/v1/items/{id}/images   - Upload item images
DELETE /api/v1/items/{id}/images/{imageId} - Remove image
GET    /api/v1/items/my            - Get current user's items
GET    /api/v1/items/available     - Get available items from friends
```

### Query Parameters
```
?category={categoryId}
?status={status}
?ownerId={userId}
?circleId={circleId}
?search={keyword}
?page={page}
?pageSize={size}
?sortBy={field}
&sortOrder={asc|desc}
```

### Deliverables
- Output: `docs/api/endpoints/items.md`
- OpenAPI spec: `docs/api/openapi/items.yaml`

---

## Task 5: Categories API (Priority: Must Have)

**Objective**: Design item category endpoints.

### Endpoints Required
```
GET    /api/v1/categories          - List all categories
GET    /api/v1/categories/{id}     - Get category details
GET    /api/v1/categories/{id}/items - Get items in category
GET    /api/v1/categories/tree     - Get category hierarchy
```

### Deliverables
- Output: `docs/api/endpoints/categories.md`
- OpenAPI spec: `docs/api/openapi/categories.yaml`

---

## Task 6: Friend Circles API (Priority: Must Have)

**Objective**: Design friend circle management endpoints.

### Endpoints Required
```
GET    /api/v1/circles             - List user's circles
POST   /api/v1/circles             - Create new circle
GET    /api/v1/circles/{id}        - Get circle details
PUT    /api/v1/circles/{id}        - Update circle
DELETE /api/v1/circles/{id}        - Delete circle
GET    /api/v1/circles/{id}/members - List circle members
POST   /api/v1/circles/{id}/invite - Invite user to circle
POST   /api/v1/circles/{id}/join   - Join circle (accept invite)
POST   /api/v1/circles/{id}/leave  - Leave circle
DELETE /api/v1/circles/{id}/members/{userId} - Remove member
GET    /api/v1/circles/{id}/items  - Get items shared with circle
```

### Deliverables
- Output: `docs/api/endpoints/circles.md`
- OpenAPI spec: `docs/api/openapi/circles.yaml`

---

## Task 7: Borrow Requests API (Priority: Must Have)

**Objective**: Design borrowing workflow endpoints.

### Endpoints Required
```
GET    /api/v1/borrow-requests           - List user's requests
POST   /api/v1/borrow-requests           - Create borrow request
GET    /api/v1/borrow-requests/{id}      - Get request details
PATCH  /api/v1/borrow-requests/{id}      - Update request status
DELETE /api/v1/borrow-requests/{id}      - Cancel request
GET    /api/v1/borrow-requests/incoming  - Requests for my items
GET    /api/v1/borrow-requests/outgoing  - My requests to others
POST   /api/v1/borrow-requests/{id}/approve - Approve request
POST   /api/v1/borrow-requests/{id}/decline - Decline request
POST   /api/v1/borrow-requests/{id}/return  - Mark as returned
POST   /api/v1/borrow-requests/{id}/rate    - Rate the experience
```

### Request Status Transitions
```
Pending → Approved → Active → Returned
       ↘ Declined
       ↘ Cancelled
Active → Overdue
```

### Deliverables
- Output: `docs/api/endpoints/borrow-requests.md`
- OpenAPI spec: `docs/api/openapi/borrow-requests.yaml`

---

## Task 8: Notifications API (Priority: Should Have)

**Objective**: Design notification system endpoints.

### Endpoints Required
```
GET    /api/v1/notifications        - List user's notifications
GET    /api/v1/notifications/unread - Get unread count
PATCH  /api/v1/notifications/{id}/read - Mark as read
POST   /api/v1/notifications/read-all - Mark all as read
DELETE /api/v1/notifications/{id}   - Delete notification
GET    /api/v1/notifications/preferences - Get preferences
PUT    /api/v1/notifications/preferences - Update preferences
```

### Deliverables
- Output: `docs/api/endpoints/notifications.md`
- OpenAPI spec: `docs/api/openapi/notifications.yaml`

---

## Task 9: Search API (Priority: Should Have)

**Objective**: Design search and discovery endpoints.

### Endpoints Required
```
GET    /api/v1/search/items         - Search items
GET    /api/v1/search/users         - Search users
GET    /api/v1/search/circles       - Search circles
GET    /api/v1/search/suggestions   - Get search suggestions
```

### Search Parameters
```
?q={query}
?filters[category]={id}
?filters[status]={status}
?filters[circle]={circleId}
```

### Deliverables
- Output: `docs/api/endpoints/search.md`
- OpenAPI spec: `docs/api/openapi/search.yaml`

---

## Task 10: OpenAPI Specification (Priority: Must Have)

**Objective**: Create complete OpenAPI 3.0 specification.

### Requirements
- [ ] Combined OpenAPI spec file
- [ ] Shared component schemas
- [ ] Security schemes definition
- [ ] Example requests/responses
- [ ] Error response schemas

### Common Response Schemas
```yaml
ApiResponse:
  success: boolean
  data: object
  message: string

PaginatedResponse:
  items: array
  page: integer
  pageSize: integer
  totalPages: integer
  totalCount: integer

ErrorResponse:
  success: false
  error:
    code: string
    message: string
    details: array
```

### Deliverables
- Output: `docs/api/openapi/openapi.yaml`

---

## Output Locations

All API documentation should be saved to:
```
docs/
├── api/
│   ├── architecture.md
│   ├── endpoints/
│   │   ├── authentication.md
│   │   ├── user-profile.md
│   │   ├── items.md
│   │   ├── categories.md
│   │   ├── circles.md
│   │   ├── borrow-requests.md
│   │   ├── notifications.md
│   │   └── search.md
│   └── openapi/
│       ├── openapi.yaml (combined)
│       ├── auth.yaml
│       ├── users.yaml
│       ├── items.yaml
│       ├── categories.yaml
│       ├── circles.yaml
│       ├── borrow-requests.yaml
│       ├── notifications.yaml
│       └── search.yaml
```
