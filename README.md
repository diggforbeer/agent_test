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
- **Database**: SQL Server or PostgreSQL
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

## Project Structure (Planned)

```
src/
├── FriendShare.Api/              # Web API project
├── FriendShare.Web/              # Web frontend (MVC/Blazor)
├── FriendShare.Core/             # Domain models, interfaces
├── FriendShare.Application/      # Business logic, services
└── FriendShare.Infrastructure/   # Data access, external services

tests/
├── FriendShare.UnitTests/
├── FriendShare.IntegrationTests/
└── FriendShare.E2ETests/

docker/
├── Dockerfile
├── Dockerfile.dev
├── docker-compose.yml
└── docker-compose.prod.yml
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker Desktop
- SQL Server (or PostgreSQL) - can use Docker image

### Running with Docker

```bash
# Build and run all services
docker-compose up --build

# Run in detached mode
docker-compose up -d

# Stop all services
docker-compose down
```

### Local Development

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the API
dotnet run --project src/FriendShare.Api
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