# DevOps Engineer Tasks

## Overview
As the DevOps Engineer, configure Docker containers, CI/CD pipelines, and deployment infrastructure for the Friend Share platform.

---

## Task 1: Docker Development Environment (Priority: Must Have)

**Objective**: Create Docker configuration for local development.

### Files Required
```
docker/
├── Dockerfile.dev          - Development Dockerfile
├── docker-compose.yml      - Development compose
└── docker-compose.override.yml - Local overrides
```

### Dockerfile.dev Requirements
- [ ] .NET 8 SDK base image
- [ ] Hot reload support
- [ ] Volume mounts for source code
- [ ] Debug configuration
- [ ] Development certificates

### docker-compose.yml Services
- [ ] api - .NET Web API
- [ ] web - Frontend (if separate)
- [ ] db - SQL Server or PostgreSQL
- [ ] cache - Redis (optional)
- [ ] seq - Logging (optional)

### Example docker-compose.yml Structure
```yaml
version: '3.8'
services:
  api:
    build:
      context: .
      dockerfile: docker/Dockerfile.dev
    ports:
      - "5000:5000"
      - "5001:5001"
    volumes:
      - ./src:/app/src
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    depends_on:
      - db

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${DB_PASSWORD}
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql

volumes:
  sqldata:
```

### Deliverables
- Output: `docker/` directory with all config files
- Output: `docs/devops/local-development.md`

---

## Task 2: Production Docker Configuration (Priority: Must Have)

**Objective**: Create optimized production Docker configuration.

### Files Required
```
docker/
├── Dockerfile              - Production multi-stage Dockerfile
└── docker-compose.prod.yml - Production compose
```

### Production Dockerfile Requirements
- [ ] Multi-stage build (build → runtime)
- [ ] Non-root user
- [ ] Health checks
- [ ] Optimized layer caching
- [ ] Minimal runtime image

### Example Production Dockerfile
```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.sln .
COPY src/FriendShare.Api/*.csproj ./src/FriendShare.Api/
# ... restore, build, publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
USER app
HEALTHCHECK CMD curl --fail http://localhost:80/health || exit 1
ENTRYPOINT ["dotnet", "FriendShare.Api.dll"]
```

### Deliverables
- Output: `docker/Dockerfile`
- Output: `docker/docker-compose.prod.yml`
- Output: `docs/devops/production-deployment.md`

---

## Task 3: GitHub Actions CI Pipeline (Priority: Must Have)

**Objective**: Create continuous integration workflow.

### Workflow Requirements
- [ ] Trigger on PR and push to main
- [ ] Build all projects
- [ ] Run unit tests
- [ ] Run integration tests
- [ ] Code coverage reporting
- [ ] Linting/code analysis

### Workflow File Structure
```yaml
name: CI

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4.1.7
      - uses: actions/setup-dotnet@v4.0.1
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build --verbosity normal
```

### Deliverables
- Output: `.github/workflows/ci.yml`
- Output: `docs/devops/ci-pipeline.md`

---

## Task 4: GitHub Actions CD Pipeline (Priority: Should Have)

**Objective**: Create continuous deployment workflow.

### Workflow Requirements
- [ ] Trigger on release/tag
- [ ] Build Docker images
- [ ] Push to container registry
- [ ] Deploy to staging environment
- [ ] Deploy to production (manual approval)

### Deployment Strategies
- [ ] Blue-green deployment option
- [ ] Rolling update option
- [ ] Rollback capability

### Deliverables
- Output: `.github/workflows/cd.yml`
- Output: `docs/devops/cd-pipeline.md`

---

## Task 5: Environment Configuration (Priority: Must Have)

**Objective**: Set up environment-specific configurations.

### Environment Files
```
├── .env.example           - Template for local .env
├── .env.development       - Development defaults
├── .env.staging           - Staging configuration
└── .env.production        - Production configuration (template only)
```

### Configuration Categories
- [ ] Database connection strings
- [ ] JWT secrets (placeholder)
- [ ] External service URLs
- [ ] Feature flags
- [ ] Logging levels

### ASP.NET Core Configuration
```
src/FriendShare.Api/
├── appsettings.json
├── appsettings.Development.json
├── appsettings.Staging.json
└── appsettings.Production.json
```

### Deliverables
- Environment files created
- Output: `docs/devops/environment-configuration.md`

---

## Task 6: Database Migration Automation (Priority: Should Have)

**Objective**: Automate database migrations in CI/CD.

### Requirements
- [ ] Migration script in GitHub Actions
- [ ] Database backup before migration
- [ ] Rollback procedure
- [ ] Migration idempotency checks

### Migration Workflow
```yaml
- name: Run Migrations
  run: |
    dotnet ef database update \
      --project src/FriendShare.Infrastructure \
      --startup-project src/FriendShare.Api \
      --connection "${{ secrets.DATABASE_CONNECTION }}"
```

### Deliverables
- Migration scripts/workflows
- Output: `docs/devops/database-migrations.md`

---

## Task 7: Secrets Management (Priority: Must Have)

**Objective**: Set up secure secrets management.

### GitHub Secrets Required
- [ ] DATABASE_CONNECTION
- [ ] JWT_SECRET
- [ ] CONTAINER_REGISTRY_USERNAME
- [ ] CONTAINER_REGISTRY_PASSWORD
- [ ] DEPLOYMENT_SSH_KEY (if needed)

### Documentation
- [ ] How to add/update secrets
- [ ] Secret rotation procedures
- [ ] Local development secrets handling

### Deliverables
- Output: `docs/devops/secrets-management.md`

---

## Task 8: Logging & Monitoring Setup (Priority: Should Have)

**Objective**: Configure logging and monitoring infrastructure.

### Logging Configuration
- [ ] Structured logging with Serilog
- [ ] Log levels per environment
- [ ] Log aggregation setup (Seq, ELK, etc.)
- [ ] Log retention policies

### Monitoring Options
- [ ] Health check endpoints
- [ ] Application metrics
- [ ] Infrastructure metrics
- [ ] Alerting configuration

### Health Checks
```csharp
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
```

### Deliverables
- Logging configuration
- Output: `docs/devops/logging-monitoring.md`

---

## Task 9: Container Registry Setup (Priority: Should Have)

**Objective**: Configure container registry for image storage.

### Options
- [ ] GitHub Container Registry (ghcr.io)
- [ ] Docker Hub
- [ ] Azure Container Registry
- [ ] AWS ECR

### Workflow Integration
```yaml
- name: Login to Container Registry
  uses: docker/login-action@v3
  with:
    registry: ghcr.io
    username: ${{ github.actor }}
    password: ${{ secrets.GITHUB_TOKEN }}

- name: Build and Push
  uses: docker/build-push-action@v5
  with:
    push: true
    tags: ghcr.io/${{ github.repository }}:${{ github.sha }}
```

### Deliverables
- Registry configuration
- Output: `docs/devops/container-registry.md`

---

## Task 10: Nginx/Reverse Proxy Configuration (Priority: Should Have)

**Objective**: Configure reverse proxy for production.

### Requirements
- [ ] SSL termination
- [ ] Static file serving
- [ ] Gzip compression
- [ ] Rate limiting
- [ ] Security headers

### nginx.conf Example
```nginx
server {
    listen 80;
    listen 443 ssl;
    server_name friendshare.example.com;

    ssl_certificate /etc/ssl/certs/cert.pem;
    ssl_certificate_key /etc/ssl/private/key.pem;

    location / {
        proxy_pass http://api:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
    }
}
```

### Deliverables
- Output: `docker/nginx/nginx.conf`
- Output: `docs/devops/reverse-proxy.md`

---

## Task 11: Backup & Disaster Recovery (Priority: Could Have)

**Objective**: Design backup and recovery procedures.

### Areas to Cover
- [ ] Database backup strategy
- [ ] Backup scheduling
- [ ] Backup storage location
- [ ] Recovery procedures
- [ ] RTO/RPO definitions

### Deliverables
- Output: `docs/devops/backup-recovery.md`

---

## Task 12: Infrastructure as Code (Priority: Could Have)

**Objective**: Create IaC templates for cloud deployment.

### Options
- [ ] Terraform for cloud resources
- [ ] Kubernetes manifests
- [ ] Docker Swarm configuration
- [ ] Azure ARM templates

### Deliverables
- Output: `infrastructure/` directory
- Output: `docs/devops/infrastructure-as-code.md`

---

## Output Locations

All DevOps configuration should be saved to:
```
.github/
└── workflows/
    ├── ci.yml
    └── cd.yml

docker/
├── Dockerfile
├── Dockerfile.dev
├── docker-compose.yml
├── docker-compose.prod.yml
├── docker-compose.override.yml
└── nginx/
    └── nginx.conf

infrastructure/  (optional)
├── terraform/
└── kubernetes/

docs/
└── devops/
    ├── local-development.md
    ├── production-deployment.md
    ├── ci-pipeline.md
    ├── cd-pipeline.md
    ├── environment-configuration.md
    ├── database-migrations.md
    ├── secrets-management.md
    ├── logging-monitoring.md
    ├── container-registry.md
    ├── reverse-proxy.md
    └── backup-recovery.md
```
