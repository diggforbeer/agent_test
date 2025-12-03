# Friend Share

Item sharing platform for friend circles. Built with .NET 8, ASP.NET Core, PostgreSQL, and Docker.

## Quick Start

```bash
# Clone and setup
git clone <repository-url>
cd agent_test/docker

# Create environment file
cat > .env << EOF
POSTGRES_USER=friendshare_user
POSTGRES_PASSWORD=dev_password_123
POSTGRES_DB=friendshare_db
EOF

# Start all services
docker compose up -d

# Access applications
# Web:     http://localhost:5001
# API:     http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

## Docker Compose

**Start services:**
```bash
docker compose up -d              # Run in background
docker compose up                 # Run with logs
docker compose up --build         # Rebuild and start
```

**View logs:**
```bash
docker compose logs -f            # All services
docker compose logs -f web        # Web only
docker compose logs -f api        # API only
```

**Stop services:**
```bash
docker compose down               # Stop containers
docker compose down -v            # Stop and remove data
docker compose restart api        # Restart one service
```

**Health checks:**
```bash
docker compose ps                 # View status
curl http://localhost:5000/swagger   # Test API
curl http://localhost:5001           # Test Web
```

## Tech Stack

- .NET 8 / ASP.NET Core
- PostgreSQL 16
- ASP.NET Core Identity + JWT
- Docker Compose
- Clean Architecture

## Project Structure

```
src/
├── FriendShare.Api/              # REST API
├── FriendShare.Web/              # Web UI (MVC)
├── FriendShare.Core/             # Domain models
├── FriendShare.Application/      # Business logic
└── FriendShare.Infrastructure/   # Data access

docker/
├── docker-compose.yml            # Dev environment
├── Dockerfile.api.dev            # API (hot reload)
└── Dockerfile.web.dev            # Web (hot reload)
```

## Local Development

```bash
dotnet restore
dotnet build
dotnet test

# Setup database migrations (first time only)
dotnet tool install --global dotnet-ef
./migrations.sh create InitialCreate
./migrations.sh update

# Run API
dotnet run --project src/FriendShare.Api

# Run Web (separate terminal)
dotnet run --project src/FriendShare.Web
```

## Database Migrations

The project uses Entity Framework Core migrations for database schema management.

**Quick commands:**
```bash
./migrations.sh create <MigrationName>   # Create new migration
./migrations.sh update                   # Apply migrations
./migrations.sh list                     # List all migrations
```

**Alternative: Use GitHub Actions workflow**
- Go to Actions tab → "Create Database Migration" → Run workflow
- Enter migration name and the workflow will create and commit the files

See [MIGRATIONS.md](MIGRATIONS.md) for detailed documentation.

## Environment Variables

Required in `docker/.env`:
- `POSTGRES_USER` - Database username
- `POSTGRES_PASSWORD` - Database password  
- `POSTGRES_DB` - Database name

## Features

See [MVP_EPICS.md](MVP_EPICS.md) for detailed specifications.

- User authentication & profiles
- Item listing & categorization
- Friend circle management
- Borrow request workflow
- Availability tracking
- Search & filtering

## License

MIT License - see [LICENSE](LICENSE)