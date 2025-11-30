# Database Architect Tasks

## Overview
As the Database Architect, design the database schema, Entity Framework Core models, and data access patterns for the Friend Share platform.

---

## Task 1: Entity Relationship Diagram (Priority: Must Have)

**Objective**: Design the complete ERD for the platform.

### Core Entities Required
- [ ] User
- [ ] UserProfile
- [ ] Item
- [ ] ItemCategory
- [ ] ItemImage
- [ ] FriendCircle
- [ ] CircleMembership
- [ ] BorrowRequest
- [ ] Notification

### Deliverables
- [ ] ERD diagram (Mermaid or PlantUML format)
- [ ] Entity descriptions
- [ ] Relationship documentation

### Output Location
- `docs/database/erd.md`

---

## Task 2: User & Profile Schema (Priority: Must Have)

**Objective**: Design the user and profile data model.

### Tables/Entities
```
User
├── Id (GUID)
├── Email (unique, indexed)
├── PasswordHash
├── EmailConfirmed
├── CreatedAt
├── UpdatedAt
└── IsActive

UserProfile
├── Id (GUID)
├── UserId (FK)
├── DisplayName
├── Bio
├── AvatarUrl
├── Location
├── PhoneNumber
└── NotificationPreferences (JSON)
```

### Requirements
- [ ] ASP.NET Core Identity integration design
- [ ] Index recommendations
- [ ] Soft delete strategy

### Deliverables
- Entity class definition
- Migration script example
- Output: `docs/database/schemas/user-profile.md`

---

## Task 3: Item & Category Schema (Priority: Must Have)

**Objective**: Design the item management data model.

### Tables/Entities
```
Item
├── Id (GUID)
├── OwnerId (FK to User)
├── Title
├── Description
├── CategoryId (FK)
├── Condition (enum)
├── Status (enum: Available, Borrowed, Unavailable)
├── CreatedAt
├── UpdatedAt
└── IsDeleted (soft delete)

ItemCategory
├── Id (int)
├── Name
├── Description
├── ParentCategoryId (self-referential, nullable)
└── IconUrl

ItemImage
├── Id (GUID)
├── ItemId (FK)
├── ImageUrl
├── IsPrimary
├── SortOrder
└── CreatedAt
```

### Requirements
- [ ] Full-text search indexing strategy
- [ ] Category hierarchy support
- [ ] Item status enumeration values

### Deliverables
- Output: `docs/database/schemas/item-category.md`

---

## Task 4: Friend Circle Schema (Priority: Must Have)

**Objective**: Design the friend circle and membership data model.

### Tables/Entities
```
FriendCircle
├── Id (GUID)
├── Name
├── Description
├── CreatedById (FK to User)
├── CreatedAt
├── UpdatedAt
└── IsActive

CircleMembership
├── Id (GUID)
├── CircleId (FK)
├── UserId (FK)
├── Role (enum: Admin, Member)
├── Status (enum: Pending, Active, Declined, Left)
├── JoinedAt
└── InvitedById (FK to User)

ItemCircleVisibility
├── Id (GUID)
├── ItemId (FK)
├── CircleId (FK)
└── CreatedAt
```

### Requirements
- [ ] Many-to-many relationship handling
- [ ] Circle membership status workflow
- [ ] Item visibility per circle

### Deliverables
- Output: `docs/database/schemas/friend-circle.md`

---

## Task 5: Borrowing Workflow Schema (Priority: Must Have)

**Objective**: Design the borrowing request and tracking data model.

### Tables/Entities
```
BorrowRequest
├── Id (GUID)
├── ItemId (FK)
├── BorrowerId (FK to User)
├── OwnerId (FK to User)
├── Status (enum)
├── RequestedAt
├── ResponseAt
├── StartDate
├── ExpectedReturnDate
├── ActualReturnDate
├── Message
└── ResponseMessage

BorrowingHistory
├── Id (GUID)
├── BorrowRequestId (FK)
├── ItemId (FK)
├── BorrowerId (FK)
├── OwnerId (FK)
├── StartDate
├── ReturnDate
├── Rating
├── Review
└── CreatedAt
```

### Status Enum Values
- Pending
- Approved
- Declined
- Active (item currently borrowed)
- Returned
- Cancelled
- Overdue

### Deliverables
- Output: `docs/database/schemas/borrowing.md`

---

## Task 6: Notification Schema (Priority: Should Have)

**Objective**: Design the notification system data model.

### Tables/Entities
```
Notification
├── Id (GUID)
├── UserId (FK)
├── Type (enum)
├── Title
├── Message
├── RelatedEntityType
├── RelatedEntityId
├── IsRead
├── CreatedAt
└── ReadAt

NotificationPreference
├── Id (GUID)
├── UserId (FK)
├── NotificationType
├── EmailEnabled
├── PushEnabled
└── InAppEnabled
```

### Deliverables
- Output: `docs/database/schemas/notification.md`

---

## Task 7: Entity Framework Core Configuration (Priority: Must Have)

**Objective**: Create EF Core entity configurations and DbContext.

### Deliverables
- [ ] DbContext design with DbSets
- [ ] Entity configurations (Fluent API)
- [ ] Relationship configurations
- [ ] Index configurations
- [ ] Value conversions for enums
- [ ] Audit field handling (CreatedAt, UpdatedAt)

### Output Location
- `docs/database/ef-core-configuration.md`

---

## Task 8: Database Seeding Strategy (Priority: Should Have)

**Objective**: Design data seeding for development and testing.

### Seed Data Required
- [ ] Default categories
- [ ] Test users
- [ ] Sample items
- [ ] Sample circles
- [ ] Sample borrow requests

### Deliverables
- Output: `docs/database/seeding-strategy.md`

---

## Task 9: Query Optimization Recommendations (Priority: Should Have)

**Objective**: Document query patterns and optimization strategies.

### Areas to Cover
- [ ] Common query patterns
- [ ] Index recommendations
- [ ] N+1 query prevention
- [ ] Pagination strategies
- [ ] Include/projection best practices

### Deliverables
- Output: `docs/database/query-optimization.md`

---

## Task 10: Migration Strategy (Priority: Should Have)

**Objective**: Define database migration and versioning strategy.

### Topics to Cover
- [ ] Migration naming conventions
- [ ] Rollback procedures
- [ ] Production migration checklist
- [ ] Data migration patterns
- [ ] Blue-green deployment considerations

### Deliverables
- Output: `docs/database/migration-strategy.md`

---

## Output Locations

All database documentation should be saved to:
```
docs/
├── database/
│   ├── erd.md
│   ├── ef-core-configuration.md
│   ├── seeding-strategy.md
│   ├── query-optimization.md
│   ├── migration-strategy.md
│   └── schemas/
│       ├── user-profile.md
│       ├── item-category.md
│       ├── friend-circle.md
│       ├── borrowing.md
│       └── notification.md
```
