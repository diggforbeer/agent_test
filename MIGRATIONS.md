# Database Migrations Setup

## Quick Start

### Create Initial Migration

```bash
cd /home/runner/work/agent_test/agent_test
dotnet ef migrations add InitialCreate \
  --project src/FriendShare.Infrastructure \
  --startup-project src/FriendShare.Api
```

### Apply Migration to Database

```bash
dotnet ef database update \
  --project src/FriendShare.Infrastructure \
  --startup-project src/FriendShare.Api
```

## Prerequisites

1. Install EF Core tools:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

2. Configure connection string in `src/FriendShare.Api/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=friendshare;Username=your_user;Password=your_password"
     }
   }
   ```

## Database Schema

The initial migration creates ASP.NET Core Identity tables with custom ApplicationUser properties:

### Tables Created
- AspNetUsers (with custom fields: FirstName, LastName, Bio, PhotoUrl, CreatedAt, UpdatedAt, IsActive, RefreshToken, RefreshTokenExpiryTime)
- AspNetRoles
- AspNetUserRoles
- AspNetUserClaims
- AspNetRoleClaims
- AspNetUserLogins
- AspNetUserTokens

### Indexes
- IX_AspNetUsers_RefreshToken
- EmailIndex on NormalizedEmail
- UserNameIndex on NormalizedUserName (unique)
- RoleNameIndex on NormalizedName (unique)

## Common Commands

### List Migrations
```bash
dotnet ef migrations list --project src/FriendShare.Infrastructure --startup-project src/FriendShare.Api
```

### Remove Last Migration
```bash
dotnet ef migrations remove --project src/FriendShare.Infrastructure --startup-project src/FriendShare.Api
```

### Generate SQL Script
```bash
dotnet ef migrations script --project src/FriendShare.Infrastructure --startup-project src/FriendShare.Api --output migrations.sql
```

## Package Requirements

The following package has been added to FriendShare.Infrastructure:
- Microsoft.EntityFrameworkCore.Design 8.0.11

This package provides the design-time tools needed for creating and managing migrations.
