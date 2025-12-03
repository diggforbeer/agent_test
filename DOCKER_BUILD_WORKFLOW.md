# Docker Build and Push Workflow

## Overview

This GitHub Action automatically builds and pushes Docker images to GitHub Container Registry (ghcr.io) when changes are merged to the `main` branch.

## Workflow Details

**File**: `.github/workflows/docker-build-push.yml`

### Triggers

The workflow runs on:
- **Push to main branch** - Automatically triggered when code is merged
- **Manual dispatch** - Can be triggered manually via GitHub Actions UI

The workflow only runs when relevant files change:
- `src/**` - Application source code
- `docker/Dockerfile.api` - API Dockerfile
- `docker/Dockerfile.web` - Web Dockerfile
- `FriendShare.sln` - Solution file
- `.github/workflows/docker-build-push.yml` - Workflow file itself

### Built Images

Two Docker images are built and pushed:

1. **API Image**: `ghcr.io/diggforbeer/agent_test-api`
   - Built from: `docker/Dockerfile.api`
   - Contains: ASP.NET Core Web API

2. **Web Image**: `ghcr.io/diggforbeer/agent_test-web`
   - Built from: `docker/Dockerfile.web`
   - Contains: ASP.NET Core MVC Frontend

### Image Tags

Each image is tagged with:
- `latest` - Always points to the most recent main branch build
- `main` - Branch name tag
- `main-<git-sha>` - Branch name with commit SHA for traceability

### Image Labels

Images follow the [OCI Image Spec](https://github.com/opencontainers/image-spec/blob/main/annotations.md) with standard labels:

- `org.opencontainers.image.title` - Human-readable title
- `org.opencontainers.image.description` - Description of the image
- `org.opencontainers.image.vendor` - Organization/vendor name
- `org.opencontainers.image.licenses` - License identifier
- `org.opencontainers.image.source` - Source repository URL (auto-added)
- `org.opencontainers.image.version` - Version/tag (auto-added)
- `org.opencontainers.image.created` - Build timestamp (auto-added)
- `org.opencontainers.image.revision` - Git commit SHA (auto-added)

## Setup Requirements

### Permissions

The workflow requires the following permissions (already configured):
- `contents: read` - Read repository contents
- `packages: write` - Push to GitHub Container Registry
- `id-token: write` - OIDC token for secure authentication

### Secrets

No additional secrets are required! The workflow uses the built-in `GITHUB_TOKEN` which is automatically provided by GitHub Actions.

### Registry Access

Images are pushed to GitHub Container Registry (ghcr.io), which is:
- Free for public repositories
- Automatically configured for the repository
- Accessible at: https://github.com/orgs/diggforbeer/packages

## Using the Images

### Pull Images

```bash
# Pull latest API image
docker pull ghcr.io/diggforbeer/agent_test-api:latest

# Pull latest Web image
docker pull ghcr.io/diggforbeer/agent_test-web:latest

# Pull specific version by commit SHA
docker pull ghcr.io/diggforbeer/agent_test-api:main-abc123
```

### Run Containers

```bash
# Run API container
docker run -d -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="your-connection-string" \
  ghcr.io/diggforbeer/agent_test-api:latest

# Run Web container
docker run -d -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ApiBaseUrl="https://your-api-url" \
  ghcr.io/diggforbeer/agent_test-web:latest
```

### Use in Docker Compose

Update your `docker-compose.prod.yml` to use the published images:

```yaml
version: '3.8'

services:
  api:
    image: ghcr.io/diggforbeer/agent_test-api:latest
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=${DATABASE_URL}
    depends_on:
      - db

  web:
    image: ghcr.io/diggforbeer/agent_test-web:latest
    ports:
      - "5001:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ApiBaseUrl=http://api:8080
    depends_on:
      - api

  db:
    image: postgres:16-alpine
    # ... rest of db config
```

## Workflow Features

### Build Cache

The workflow uses GitHub Actions cache to speed up builds:
- Docker layer caching is enabled
- Subsequent builds reuse unchanged layers
- Significantly faster build times after the first build

### Multi-Architecture Support

Currently builds for:
- `linux/amd64` - Standard x86_64 architecture

Can be extended to support ARM architectures if needed.

### Build Summary

After each successful build, a summary is generated showing:
- Image tags created
- Pull commands for easy access
- Available in the GitHub Actions run summary

## Monitoring

### View Workflow Runs

1. Go to the repository on GitHub
2. Click "Actions" tab
3. Select "Docker Build and Push" workflow
4. View run history and logs

### View Published Images

1. Go to repository on GitHub
2. Click "Packages" in the right sidebar
3. View `agent_test-api` and `agent_test-web` packages
4. See image tags, labels, and download statistics

## Troubleshooting

### Build Failures

If the build fails, check:
1. **Dockerfile syntax** - Ensure Dockerfiles are valid
2. **Build context** - All required files are in the repository
3. **Dependencies** - NuGet packages can be restored
4. **Workflow logs** - Detailed error messages in Actions logs

### Push Failures

If push fails, check:
1. **Permissions** - Workflow has `packages: write` permission
2. **GITHUB_TOKEN** - Token has not expired (automatically refreshed)
3. **Registry status** - GitHub Container Registry is operational

### Image Pull Failures

If you can't pull images, ensure:
1. **Image exists** - Workflow has run successfully at least once
2. **Authentication** - You're logged in to ghcr.io if pulling private images
3. **Image name** - Using correct image name and tag

```bash
# Login to GitHub Container Registry
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin
```

## Maintenance

### Updating the Workflow

To modify the workflow:
1. Edit `.github/workflows/docker-build-push.yml`
2. Test changes on a feature branch
3. Merge to main when validated

### Adding More Images

To build additional Docker images:
1. Add a new `Extract metadata` step
2. Add a new `Build and push` step
3. Update the build summary step

### Changing Tags or Labels

Modify the `docker/metadata-action` configuration in the relevant steps:
- `tags:` section controls image tags
- `labels:` section controls OCI labels

## Best Practices

1. **Semantic Versioning**: Consider adding semantic version tags for releases
2. **Security Scanning**: Add vulnerability scanning for production images
3. **Image Signing**: Consider signing images for supply chain security
4. **Retention Policy**: Configure package retention to manage storage
5. **Multi-stage Builds**: Dockerfiles already use multi-stage builds for smaller images

## Related Documentation

- [GitHub Container Registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry)
- [Docker Metadata Action](https://github.com/docker/metadata-action)
- [Docker Build Push Action](https://github.com/docker/build-push-action)
- [OCI Image Spec](https://github.com/opencontainers/image-spec/blob/main/annotations.md)
