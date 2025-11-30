# Developer Tasks

## Overview
As the Senior .NET Developer, implement the core application architecture, business logic, and infrastructure for the Friend Share platform.

---

## Task 1: Solution Structure Setup (Priority: Must Have)

**Objective**: Create the .NET solution with Clean Architecture.

### Projects to Create
```
FriendShare.sln
├── src/
│   ├── FriendShare.Api/           - Web API (Controllers, Middleware)
│   ├── FriendShare.Web/           - Web Frontend (MVC/Blazor)
│   ├── FriendShare.Core/          - Domain (Entities, Interfaces, Enums)
│   ├── FriendShare.Application/   - Business Logic (Services, DTOs, Validators)
│   └── FriendShare.Infrastructure/ - Data Access (EF Core, Repositories)
└── tests/
    ├── FriendShare.UnitTests/
    ├── FriendShare.IntegrationTests/
    └── FriendShare.E2ETests/
```

### Configuration Required
- [ ] .NET 8 SDK configuration
- [ ] Project references setup
- [ ] NuGet package configuration
- [ ] Global.json with SDK version

### Deliverables
- Solution and project files created
- README updated with project structure

---

## Task 2: Domain Entities (Priority: Must Have)

**Objective**: Implement domain entities in FriendShare.Core.

### Entities to Create
```csharp
// Core Entities
- User.cs
- UserProfile.cs
- Item.cs
- ItemCategory.cs
- ItemImage.cs
- FriendCircle.cs
- CircleMembership.cs
- BorrowRequest.cs
- Notification.cs
```

### Base Entity
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### Enums to Create
- ItemCondition
- ItemStatus
- BorrowRequestStatus
- MembershipRole
- MembershipStatus
- NotificationType

### Deliverables
- All entity classes in `src/FriendShare.Core/Entities/`
- Enums in `src/FriendShare.Core/Enums/`

---

## Task 3: Repository Interfaces (Priority: Must Have)

**Objective**: Define repository interfaces in FriendShare.Core.

### Interfaces Required
```csharp
- IRepository<T> (generic base)
- IUserRepository
- IItemRepository
- ICircleRepository
- IBorrowRequestRepository
- INotificationRepository
```

### Generic Repository Interface
```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    IQueryable<T> Query();
}
```

### Deliverables
- Interface files in `src/FriendShare.Core/Interfaces/`

---

## Task 4: Entity Framework Configuration (Priority: Must Have)

**Objective**: Implement EF Core DbContext and configurations.

### Requirements
- [ ] ApplicationDbContext with all DbSets
- [ ] Entity configurations (Fluent API)
- [ ] Identity integration configuration
- [ ] Audit field handling (CreatedAt, UpdatedAt)
- [ ] Soft delete configuration

### DbContext Setup
```csharp
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemCategory> Categories { get; set; }
    public DbSet<FriendCircle> Circles { get; set; }
    // ... etc
}
```

### Deliverables
- DbContext in `src/FriendShare.Infrastructure/Data/`
- Configurations in `src/FriendShare.Infrastructure/Data/Configurations/`

---

## Task 5: Repository Implementations (Priority: Must Have)

**Objective**: Implement repository pattern in Infrastructure.

### Repositories to Implement
- [ ] GenericRepository<T>
- [ ] UserRepository
- [ ] ItemRepository
- [ ] CircleRepository
- [ ] BorrowRequestRepository
- [ ] NotificationRepository

### Deliverables
- Repository implementations in `src/FriendShare.Infrastructure/Repositories/`
- Unit of Work pattern (optional but recommended)

---

## Task 6: Application Services (Priority: Must Have)

**Objective**: Implement business logic services.

### Services to Create
```csharp
- IUserService / UserService
- IItemService / ItemService
- ICircleService / CircleService
- IBorrowService / BorrowService
- INotificationService / NotificationService
- IFileStorageService / FileStorageService
```

### Service Example Structure
```csharp
public interface IItemService
{
    Task<ItemDto> CreateItemAsync(CreateItemRequest request, Guid userId);
    Task<ItemDto?> GetItemAsync(Guid id);
    Task<PaginatedResult<ItemDto>> GetItemsAsync(ItemFilterRequest filter);
    Task<ItemDto> UpdateItemAsync(Guid id, UpdateItemRequest request, Guid userId);
    Task DeleteItemAsync(Guid id, Guid userId);
    Task<IEnumerable<ItemDto>> GetAvailableItemsForUserAsync(Guid userId);
}
```

### Deliverables
- Service interfaces in `src/FriendShare.Application/Interfaces/`
- Service implementations in `src/FriendShare.Application/Services/`

---

## Task 7: DTOs and Mapping (Priority: Must Have)

**Objective**: Create DTOs and AutoMapper profiles.

### DTO Categories
- [ ] Request DTOs (CreateItemRequest, UpdateItemRequest, etc.)
- [ ] Response DTOs (ItemDto, UserDto, etc.)
- [ ] Filter DTOs (ItemFilterRequest, etc.)

### AutoMapper Configuration
- [ ] Entity to DTO mappings
- [ ] Request to Entity mappings
- [ ] Custom type converters

### Deliverables
- DTOs in `src/FriendShare.Application/DTOs/`
- Mapping profiles in `src/FriendShare.Application/Mappings/`

---

## Task 8: API Controllers (Priority: Must Have)

**Objective**: Implement REST API controllers.

### Controllers to Create
- [ ] AuthController
- [ ] UsersController
- [ ] ItemsController
- [ ] CategoriesController
- [ ] CirclesController
- [ ] BorrowRequestsController
- [ ] NotificationsController

### Controller Structure
```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;
    
    // Endpoints...
}
```

### Deliverables
- Controllers in `src/FriendShare.Api/Controllers/`

---

## Task 9: Authentication Implementation (Priority: Must Have)

**Objective**: Implement JWT authentication with ASP.NET Core Identity.

### Requirements
- [ ] ASP.NET Core Identity setup
- [ ] JWT token generation
- [ ] Refresh token handling
- [ ] Authentication middleware
- [ ] Authorization policies

### Configuration
```csharp
// JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            // ...
        };
    });
```

### Deliverables
- Auth service implementation
- JWT middleware configuration
- Identity configuration

---

## Task 10: Dependency Injection Configuration (Priority: Must Have)

**Objective**: Configure DI container with all services.

### Requirements
- [ ] Service registration extensions
- [ ] Repository registrations
- [ ] AutoMapper registration
- [ ] Logging configuration
- [ ] Options pattern for configuration

### Extension Methods
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services);
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config);
}
```

### Deliverables
- DI configuration in `src/FriendShare.Api/Extensions/`
- Application layer extensions
- Infrastructure layer extensions

---

## Task 11: Middleware & Exception Handling (Priority: Should Have)

**Objective**: Implement global middleware.

### Middleware to Create
- [ ] Global exception handler
- [ ] Request logging
- [ ] Request validation
- [ ] Performance monitoring

### Exception Handling
```csharp
public class GlobalExceptionHandlerMiddleware
{
    // Convert exceptions to ApiResponse format
    // Log exceptions appropriately
    // Return proper HTTP status codes
}
```

### Deliverables
- Middleware in `src/FriendShare.Api/Middleware/`

---

## Task 12: Database Migrations (Priority: Must Have)

**Objective**: Create initial database migrations.

### Requirements
- [ ] Initial migration with all entities
- [ ] Seed data migration
- [ ] Index migrations

### Commands
```bash
# Create migration
dotnet ef migrations add InitialCreate -p src/FriendShare.Infrastructure -s src/FriendShare.Api

# Apply migration
dotnet ef database update -p src/FriendShare.Infrastructure -s src/FriendShare.Api
```

### Deliverables
- Migration files in `src/FriendShare.Infrastructure/Migrations/`

---

## Output Locations

Core implementation files:
```
src/
├── FriendShare.Api/
│   ├── Controllers/
│   ├── Extensions/
│   ├── Middleware/
│   └── Program.cs
├── FriendShare.Core/
│   ├── Entities/
│   ├── Enums/
│   └── Interfaces/
├── FriendShare.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Mappings/
│   └── Services/
└── FriendShare.Infrastructure/
    ├── Data/
    ├── Repositories/
    └── Migrations/
```
