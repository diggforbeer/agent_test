# Security Engineer Tasks

## Overview
As the Security Engineer, design the authentication, authorization, and security architecture for the Friend Share platform.

---

## Task 1: Authentication Architecture (Priority: Must Have)

**Objective**: Design the complete authentication system.

### Requirements
- [ ] JWT-based authentication design
- [ ] Access token and refresh token strategy
- [ ] Token expiration policies
- [ ] Secure token storage (client-side)
- [ ] ASP.NET Core Identity integration

### Token Strategy
```
Access Token:
- Short-lived (15-30 minutes)
- Contains user claims
- Used for API authorization

Refresh Token:
- Long-lived (7-30 days)
- Stored securely in database
- One-time use with rotation
```

### Deliverables
- Output: `docs/security/authentication.md`

---

## Task 2: Authorization & Permissions (Priority: Must Have)

**Objective**: Design the authorization and permission model.

### Permission Areas
- [ ] Item management (own items only)
- [ ] Circle management (admin vs member)
- [ ] Borrow request handling
- [ ] User profile access (public vs private)

### Role-Based Access Control
```
Roles:
- User (default)
- CircleAdmin (per circle)
- SystemAdmin (platform-wide)

Resource-Based Authorization:
- Item ownership verification
- Circle membership verification
- Request participant verification
```

### Deliverables
- Output: `docs/security/authorization.md`
- Policy definitions for ASP.NET Core

---

## Task 3: Input Validation & Sanitization (Priority: Must Have)

**Objective**: Define input validation and sanitization strategies.

### Areas to Cover
- [ ] Request body validation
- [ ] Query parameter validation
- [ ] File upload validation (images)
- [ ] XSS prevention
- [ ] SQL injection prevention
- [ ] Path traversal prevention

### Validation Rules
```csharp
// Example validation requirements
Email: RFC 5322 compliant
Password: Min 8 chars, uppercase, lowercase, number, special
Username: 3-30 chars, alphanumeric + underscore
Item Title: 3-100 chars, sanitized HTML
Item Description: Max 2000 chars, markdown allowed
```

### Deliverables
- Output: `docs/security/input-validation.md`

---

## Task 4: Password Security (Priority: Must Have)

**Objective**: Design password handling and policies.

### Requirements
- [ ] Password complexity requirements
- [ ] Password hashing (ASP.NET Core Identity)
- [ ] Password reset flow security
- [ ] Account lockout policy
- [ ] Breach detection integration (optional)

### Password Policy
```
Minimum Length: 8 characters
Requirements:
- At least 1 uppercase letter
- At least 1 lowercase letter
- At least 1 digit
- At least 1 special character

Account Lockout:
- 5 failed attempts
- 15 minute lockout
- Progressive lockout on repeated failures
```

### Deliverables
- Output: `docs/security/password-policy.md`

---

## Task 5: API Security (Priority: Must Have)

**Objective**: Define API security measures.

### Requirements
- [ ] Rate limiting strategy
- [ ] Request throttling
- [ ] CORS configuration
- [ ] API key management (future)
- [ ] Request size limits

### Rate Limiting
```
Endpoints:
- Authentication: 5 requests/minute
- Password reset: 3 requests/hour
- General API: 100 requests/minute
- Search: 30 requests/minute
- File upload: 10 requests/minute
```

### CORS Configuration
```
Allowed Origins: [configured domains]
Allowed Methods: GET, POST, PUT, PATCH, DELETE
Allowed Headers: Authorization, Content-Type
Credentials: true
Max Age: 86400
```

### Deliverables
- Output: `docs/security/api-security.md`

---

## Task 6: Data Protection (Priority: Must Have)

**Objective**: Design data protection and privacy measures.

### Requirements
- [ ] PII identification and handling
- [ ] Data encryption (at rest and in transit)
- [ ] Data retention policies
- [ ] GDPR compliance considerations
- [ ] Data export/deletion capabilities

### PII Fields
```
Encrypted at Rest:
- Email address
- Phone number
- Physical location

Anonymization on Delete:
- User profile
- Item listings
- Borrow history
```

### Deliverables
- Output: `docs/security/data-protection.md`

---

## Task 7: File Upload Security (Priority: Must Have)

**Objective**: Design secure file upload handling.

### Requirements
- [ ] Allowed file types (images only)
- [ ] File size limits
- [ ] Malware scanning (if applicable)
- [ ] Secure storage location
- [ ] Image processing security

### File Upload Policy
```
Allowed Types:
- image/jpeg
- image/png
- image/webp
- image/gif

Max File Size: 5MB per image
Max Files per Item: 5
Storage: Azure Blob / AWS S3 with restricted access
Processing: Server-side resize, strip metadata
```

### Deliverables
- Output: `docs/security/file-upload.md`

---

## Task 8: Security Headers & HTTPS (Priority: Must Have)

**Objective**: Configure security headers and transport security.

### Required Headers
```
Strict-Transport-Security: max-age=31536000; includeSubDomains
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Content-Security-Policy: [defined policy]
Referrer-Policy: strict-origin-when-cross-origin
```

### HTTPS Configuration
- [ ] TLS 1.2+ only
- [ ] Strong cipher suites
- [ ] Certificate management
- [ ] HTTP to HTTPS redirect

### Deliverables
- Output: `docs/security/headers-https.md`

---

## Task 9: Audit Logging (Priority: Should Have)

**Objective**: Design security audit logging.

### Events to Log
- [ ] Authentication events (login, logout, failed)
- [ ] Authorization failures
- [ ] Password changes
- [ ] Sensitive data access
- [ ] Admin actions
- [ ] Rate limit violations

### Log Format
```json
{
  "timestamp": "ISO8601",
  "eventType": "string",
  "userId": "guid",
  "ipAddress": "string",
  "userAgent": "string",
  "resource": "string",
  "action": "string",
  "result": "success|failure",
  "details": {}
}
```

### Deliverables
- Output: `docs/security/audit-logging.md`

---

## Task 10: Security Checklist & Guidelines (Priority: Should Have)

**Objective**: Create security checklists for development team.

### Checklists Required
- [ ] Code review security checklist
- [ ] Deployment security checklist
- [ ] Dependency security (vulnerability scanning)
- [ ] Security testing requirements

### Deliverables
- Output: `docs/security/security-checklist.md`
- Output: `docs/security/secure-coding-guidelines.md`

---

## Output Locations

All security documentation should be saved to:
```
docs/
├── security/
│   ├── authentication.md
│   ├── authorization.md
│   ├── input-validation.md
│   ├── password-policy.md
│   ├── api-security.md
│   ├── data-protection.md
│   ├── file-upload.md
│   ├── headers-https.md
│   ├── audit-logging.md
│   ├── security-checklist.md
│   └── secure-coding-guidelines.md
```
