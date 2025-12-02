# Friend Share - Item Sharing Platform

A .NET web application that enables users to share items with their friends circles. Built to run in Docker, this platform allows users to list personal items they're willing to lend, create friend groups, and manage borrowing requests within their trusted network.

## Project Overview

Friend Share is a social sharing platform where:
- **Users** can create accounts and authenticate securely
- **Item Owners** can list items they're willing to share with friends
- **Friend Circles** allow users to create groups of trusted friends
- **Borrowers** can discover available items from their connections
- **Requests** can be made to borrow items, with owner approval workflow

## Tech Stack

- **Backend**: .NET 8 / ASP.NET Core
- **Frontend**: ASP.NET Core MVC or Blazor
- **Database**: PostgreSQL
- **Authentication**: ASP.NET Core Identity
- **Containerization**: Docker & Docker Compose
- **Architecture**: Clean Architecture

## Custom Agents

This repository includes specialized GitHub Copilot agents to assist with development:

| Agent | Description |
|-------|-------------|
| **[Product](/.github/agents/product.agent.md)** | Product Manager for requirements, user stories, and feature specifications |
| **[Developer](/.github/agents/developer.agent.md)** | Senior .NET Developer for implementation, architecture, and Docker setup |
| **[Test Automation](/.github/agents/test-automation.agent.md)** | QA Engineer for unit tests, integration tests, and E2E testing |
| **[DevOps](/.github/agents/devops.agent.md)** | DevOps Engineer for CI/CD, Docker configuration, and deployment |
| **[UI/UX Designer](/.github/agents/ui-designer.agent.md)** | Designer for frontend components, user experience, and accessibility |
| **[Security](/.github/agents/security.agent.md)** | Security Engineer for authentication, authorization, and secure coding |
| **[Database Architect](/.github/agents/database-architect.agent.md)** | DBA for schema design, Entity Framework Core, and query optimization |
| **[Code Reviewer](/.github/agents/code-reviewer.agent.md)** | Senior engineer for code review, best practices, and quality |
| **[API Designer](/.github/agents/api-designer.agent.md)** | API specialist for RESTful design, OpenAPI specs, and documentation |

## Project Structure

```
/
├── src/
│   ├── FriendShare.Api/              # Web API project
│   │   └── Dockerfile
│   ├── FriendShare.Web/              # Web frontend (MVC/Razor Pages)
│   │   └── Dockerfile
│   ├── FriendShare.Core/             # Domain models, interfaces
│   ├── FriendShare.Application/      # Business logic, services
│   └── FriendShare.Infrastructure/   # Data access, external services
├── tests/
│   ├── FriendShare.UnitTests/
│   ├── FriendShare.IntegrationTests/
│   └── FriendShare.E2ETests/
├── docker-compose.yml                # Production-like configuration
├── docker-compose.override.yml       # Development overrides (hot reload)
├── .dockerignore
├── .env.example
└── FriendShare.sln
```

## Getting Started with Docker

### Prerequisites

- Docker Desktop 4.0+ with Docker Compose v2
- Git
- (Optional) .NET 8 SDK for local development without Docker

### Quick Start

1. Clone the repository:
   ```bash
   git clone https://github.com/diggforbeer/agent_test.git
   cd agent_test
   ```

2. Copy `.env.example` to `.env` and update the passwords:
   ```bash
   cp .env.example .env
   # Edit .env with your preferred editor to set secure passwords
   ```

3. Start all services:
   ```bash
   docker-compose up --build
   ```

4. Access the application:
   - **Web Frontend**: http://localhost:5000
   - **API**: http://localhost:5001
   - **API Health Check**: http://localhost:5001/health

### Development Commands

```bash
# Start all services with hot reload (development mode)
docker-compose up

# Start all services in detached mode
docker-compose up -d

# Rebuild and start services
docker-compose up --build

# Stop all services
docker-compose down

# Stop and remove volumes (WARNING: deletes database data)
docker-compose down -v

# View logs for all services
docker-compose logs -f

# View logs for a specific service
docker-compose logs -f api

# Execute commands in running containers
docker-compose exec api dotnet ef migrations add [MigrationName]
docker-compose exec api dotnet ef database update

# Restart a specific service
docker-compose restart api
```

### Service Architecture

```
┌─────────────────┐
│   Web Frontend  │ (Port 5000)
│  (Razor Pages)  │
└────────┬────────┘
         │ HTTP
         ▼
┌─────────────────┐
│   Web API       │ (Port 5001)
│   (ASP.NET Core)│
└────────┬────────┘
         │ TCP
         ▼
┌─────────────────┐
│   PostgreSQL    │ (Port 5432)
│   (Database)    │
└─────────────────┘
```

### Troubleshooting

| Issue | Solution |
|-------|----------|
| Port conflicts | Edit port mappings in `docker-compose.yml` or `docker-compose.override.yml` |
| Permission issues | Ensure Docker daemon is running; on Linux, add user to docker group |
| Slow performance | Adjust Docker Desktop resource limits (CPU/Memory) |
| Database connection errors | Wait for health check; verify `.env` credentials match |
| Hot reload not working | Ensure `docker-compose.override.yml` is present; check volume mounts |

### Production Deployment

For production deployment, use only `docker-compose.yml` without the override file:

```bash
docker-compose -f docker-compose.yml up -d
```

## Local Development (Without Docker)

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the API
dotnet run --project src/FriendShare.Api

# Run the Web frontend
dotnet run --project src/FriendShare.Web
```

## Key Features (Roadmap)

- [ ] User authentication and profile management
- [ ] Item listing with categories and images
- [ ] Friend circle creation and management
- [ ] Borrow request workflow
- [ ] Item availability tracking
- [ ] Notifications for requests and returns
- [ ] Search and filtering
- [ ] Mobile-responsive design

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details