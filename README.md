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
- **Frontend**: ASP.NET Core MVC
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
src/
├── FriendShare.Api/              # Web API project
├── FriendShare.Web/              # Web frontend (MVC)
├── FriendShare.Core/             # Domain models, interfaces
├── FriendShare.Application/      # Business logic, services
└── FriendShare.Infrastructure/   # Data access, external services

tests/
├── FriendShare.UnitTests/
├── FriendShare.IntegrationTests/
└── FriendShare.E2ETests/

docker/
├── Dockerfile.api                # Production API Dockerfile
├── Dockerfile.api.dev            # Development API Dockerfile (hot reload)
├── Dockerfile.web                # Production Web Dockerfile
├── Dockerfile.web.dev            # Development Web Dockerfile (hot reload)
├── docker-compose.yml            # Development orchestration
└── docker-compose.prod.yml       # Production orchestration
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker Desktop
- Git

### Running with Docker (Recommended)

The fastest way to get started is using Docker Compose for development:

```bash
# 1. Clone the repository
git clone <repository-url>
cd agent_test

# 2. Copy the environment file and customize if needed
cp .env.example .env

# 3. Build and run all services (from docker directory)
cd docker
docker-compose up --build

# Or run in detached mode
docker-compose up -d --build
```

Once running, access the applications at:
- **Web Frontend**: http://localhost:5001
- **API**: http://localhost:5000
- **Swagger UI** (API docs): http://localhost:5000/swagger
- **Database**: localhost:5432 (PostgreSQL)

#### Docker Commands Reference

```bash
# Build and start all services
cd docker
docker-compose up --build

# Start in background (detached mode)
docker-compose up -d

# View logs
docker-compose logs -f

# View logs for specific service
docker-compose logs -f api

# Stop all services
docker-compose down

# Stop and remove volumes (reset database)
docker-compose down -v

# Rebuild a specific service
docker-compose up --build api
```

#### Hot Reload Development

The development Docker configuration supports hot reload:
- Code changes in `src/` directories are automatically detected
- The application rebuilds and restarts automatically
- No need to restart containers for code changes

### Local Development (Without Docker)

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the API
dotnet run --project src/FriendShare.Api

# Run the Web frontend (in another terminal)
dotnet run --project src/FriendShare.Web
```

### Environment Variables

Copy `.env.example` to `.env` and configure:

| Variable | Description | Default |
|----------|-------------|---------|
| `POSTGRES_USER` | PostgreSQL username | friendshare |
| `POSTGRES_PASSWORD` | PostgreSQL password | DevPassword123! |
| `POSTGRES_DB` | PostgreSQL database name | friendshare |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET environment | Development |

### Health Checks

All services include health check endpoints:
- API: http://localhost:5000/health
- Web: http://localhost:5001/health

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