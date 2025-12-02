# Friend Share - Item Sharing Platform

A .NET web application that enables users to share items with their friends circles. Built to run in Docker, this platform allows users to list personal items they're willing to lend, create friend groups, and manage borrowing requests within their trusted network.

## 🆕 Recent Updates

### Authentication Pages Implementation (Latest)

✅ **NEW:** Complete authentication system with homepage and user-facing pages
- Landing page with hero section and feature highlights
- Login, Registration, and Password Reset pages
- Session-based authentication with JWT tokens
- Responsive design with Bootstrap 5
- Password strength indicator and form validation

**📋 Setup Required:** Before running the web application, you need to create the Auth views directory:

```bash
# Option 1: Python (cross-platform)
python3 setup-auth-views.py

# Option 2: Bash (Unix/Linux/Mac)
chmod +x create-auth-views.sh && ./create-auth-views.sh
```

📚 **Documentation:**
- `FINAL_SETUP_GUIDE.md` - Complete setup and testing guide
- `IMPLEMENTATION_SUMMARY.md` - What was implemented
- `SETUP_AUTH_VIEWS.md` - Quick reference

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

| Variable | Description | Required |
|----------|-------------|----------|
| `POSTGRES_USER` | PostgreSQL username | Yes - change from placeholder |
| `POSTGRES_PASSWORD` | PostgreSQL password (min 16 characters recommended) | Yes - change from placeholder |
| `POSTGRES_DB` | PostgreSQL database name | No (defaults to friendshare) |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET environment | No (defaults to Development) |

> **⚠️ Security Note**: The `.env.example` file contains placeholder values. You MUST create a `.env` file with your own secure credentials before starting the application.

### Health Checks

All services include health check endpoints:
- API: http://localhost:5000/health
- Web: http://localhost:5001/health

## Key Features (Roadmap)

See [MVP_EPICS.md](MVP_EPICS.md) for detailed epic specifications and acceptance criteria.

### MVP Features (Prioritized)

| Priority | Feature | Status |
|----------|---------|--------|
| Must Have | User authentication and profile management | 📋 Planned |
| Must Have | Item listing with categories and images | 📋 Planned |
| Must Have | Friend circle creation and management | 📋 Planned |
| Should Have | Borrow request workflow | 📋 Planned |
| Should Have | Item availability tracking | 📋 Planned |
| Could Have | Notifications for requests and returns | 📋 Planned |
| Could Have | Search and filtering | 📋 Planned |
| Could Have | Mobile-responsive design | 📋 Planned |

### Implementation Order
1. User Authentication (foundation)
2. Friend Circle Management (enables sharing boundaries)
3. Item Management (core listing functionality)
4. Borrow Request Workflow (core value proposition)
5. Item Availability Tracking
6. Notifications
7. Search and Filtering
8. Responsive Design

## Security Considerations

### Development Environment Security

This project implements security best practices for Docker development environments:

- **Port Binding**: All ports are bound to `127.0.0.1` (localhost only) to prevent external network access
- **Network Isolation**: Separate Docker networks isolate frontend and backend services
- **Non-root Users**: Production containers run as non-root users (`appuser`)
- **Resource Limits**: CPU and memory limits prevent resource exhaustion attacks
- **Read-only Filesystem**: Production containers use read-only root filesystems where possible
- **Capability Dropping**: Unnecessary Linux capabilities are dropped from containers

### Secret Management

⚠️ **Important Security Guidelines**:

1. **Never commit `.env` files** - The `.env` file contains secrets and is excluded from git via `.gitignore`
2. **Use `.env.example` as a template** - Copy it to `.env` and replace ALL placeholder values
3. **Use strong passwords** - Minimum 16 characters with mixed case, numbers, and symbols
4. **Rotate secrets regularly** - Change database passwords periodically
5. **Never share secrets via chat, email, or version control**

```bash
# Create your local environment file
cp .env.example .env

# Edit .env with your secure values
# POSTGRES_USER=your_unique_username
# POSTGRES_PASSWORD=your_strong_password_min_16_chars
```

### Docker Image Security

- **Pinned Versions**: Base images use specific version tags (e.g., `dotnet:8.0`), not `latest`
- **Official Images**: Only official Microsoft and PostgreSQL images are used
- **Alpine Variants**: PostgreSQL uses Alpine Linux for smaller attack surface
- **Multi-stage Builds**: Production images only contain runtime dependencies

### Regular Security Maintenance

```bash
# Update base images weekly
docker-compose pull

# Scan for vulnerable NuGet packages monthly
dotnet list package --vulnerable

# Scan Docker images for vulnerabilities (requires Trivy)
docker run --rm aquasec/trivy image friendshare-api:latest
docker run --rm aquasec/trivy image friendshare-web:latest
```

### Production Considerations

When deploying to production:

1. **Use HTTPS** - Configure TLS termination at the reverse proxy
2. **External Secrets** - Use a secrets manager (Azure Key Vault, AWS Secrets Manager, etc.)
3. **Network Security** - Database should never be exposed externally
4. **Image Scanning** - Integrate vulnerability scanning in CI/CD pipeline
5. **Audit Logging** - Enable and monitor container and application logs

### Reporting Security Issues

If you discover a security vulnerability, please report it privately:

- **Do not** open a public GitHub issue for security vulnerabilities
- Contact the maintainers directly with details of the vulnerability
- Allow reasonable time for a fix before public disclosure

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details