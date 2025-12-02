# Testing Guide for FriendShare

This document provides a comprehensive guide for testing the FriendShare application at all levels.

## Table of Contents

1. [Test Structure](#test-structure)
2. [Quick Start](#quick-start)
3. [Unit Tests](#unit-tests)
4. [Integration Tests](#integration-tests)
5. [E2E Tests with Playwright](#e2e-tests-with-playwright)
6. [CI/CD Testing](#cicd-testing)
7. [Troubleshooting](#troubleshooting)

## Test Structure

The project follows a three-tier testing approach:

```
tests/
├── FriendShare.UnitTests/              # Fast, isolated tests for business logic
│   ├── Services/
│   ├── Validators/
│   └── Domain/
├── FriendShare.IntegrationTests/       # API and database integration tests
│   ├── Api/
│   ├── Data/
│   └── Fixtures/
└── FriendShare.E2ETests/               # End-to-end browser automation tests
    ├── FriendShare.E2ETests.csproj
    └── HomepageTests.cs
```

## Quick Start

### Running All Tests

```bash
# From repository root
dotnet test
```

### Running Specific Test Categories

```bash
# Unit tests only (fast)
dotnet test --filter "Category=Unit"

# Integration tests only
dotnet test --filter "Category=Integration"

# E2E tests only (requires web app running)
dotnet test --filter "Category=E2E"
```

## Unit Tests

Unit tests validate business logic in isolation using mocks for external dependencies.

### Framework & Tools
- **Test Framework**: xUnit
- **Mocking**: Moq
- **Assertions**: xUnit + FluentAssertions (recommended)

### Running Unit Tests

```bash
dotnet test tests/FriendShare.UnitTests/FriendShare.UnitTests.csproj
```

### Example Unit Test

```csharp
[Fact]
[Trait("Category", "Unit")]
public async Task ServiceMethod_WithValidInput_ReturnsExpectedResult()
{
    // Arrange
    var mockRepository = new Mock<IRepository>();
    mockRepository.Setup(r => r.GetAsync(It.IsAny<int>()))
                  .ReturnsAsync(new Entity { Id = 1 });
    var service = new Service(mockRepository.Object);

    // Act
    var result = await service.ProcessAsync(1);

    // Assert
    Assert.NotNull(result);
    mockRepository.Verify(r => r.GetAsync(1), Times.Once);
}
```

## Integration Tests

Integration tests validate API endpoints and database interactions using WebApplicationFactory.

### Framework & Tools
- **Test Framework**: xUnit
- **Web Testing**: WebApplicationFactory
- **Database**: In-memory or test database

### Running Integration Tests

```bash
dotnet test tests/FriendShare.IntegrationTests/FriendShare.IntegrationTests.csproj
```

## E2E Tests with Playwright

End-to-end tests validate complete user workflows using browser automation.

### Framework & Tools
- **Test Framework**: xUnit
- **Browser Automation**: Microsoft.Playwright
- **Browser**: Chromium (headless)

### First-Time Setup

```bash
# 1. Install Playwright CLI globally
dotnet tool install --global Microsoft.Playwright.CLI

# 2. Install Chromium browser
playwright install chromium

# 3. Install system dependencies for Chromium
playwright install-deps chromium
```

### Running E2E Tests

**Step 1: Start the Web Application**

```bash
# From docker directory
cd docker
docker-compose up -d web

# Wait for application to be healthy
curl http://localhost:5001/health
```

**Step 2: Run the Tests**

```bash
# From repository root
dotnet test tests/FriendShare.E2ETests.csproj

# With verbose output
dotnet test tests/FriendShare.E2ETests.csproj --verbosity normal

# Run specific test
dotnet test tests/FriendShare.E2ETests.csproj --filter "FullyQualifiedName~Homepage_ShouldLoad_WithCorrectTitle"
```

**Step 3: Stop the Web Application**

```bash
cd docker
docker-compose down
```

### What the E2E Tests Cover

The homepage tests validate:

✅ **Page Load**
- Page loads successfully
- Correct title is displayed

✅ **Hero Section**
- Hero heading is visible
- Hero description is displayed
- Get Started and Login buttons are present

✅ **Features Section**
- Three feature cards are displayed
- Feature icons and descriptions are visible
- All features (Browse, Request, Track) are present

✅ **How It Works Section**
- Section heading is visible
- All four steps are displayed
- Step descriptions are clear

✅ **Call-to-Action Section**
- CTA heading is visible
- Sign Up button is present

✅ **Navigation**
- Get Started button navigates to registration
- Login button navigates to login page

### E2E Test Structure

```csharp
[Trait("Category", "E2E")]
public class HomepageTests : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    
    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new()
        {
            Headless = true
        });
        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    [Fact]
    public async Task YourTest_Scenario_ExpectedResult()
    {
        // Arrange
        await _page!.GotoAsync("http://localhost:5001");

        // Act
        var element = _page.Locator("css-selector");

        // Assert
        await Assertions.Expect(element).ToBeVisibleAsync();
    }
}
```

## CI/CD Testing

### GitHub Actions Workflows

The project includes multiple test workflows:

#### 1. **Playwright E2E Tests** (`.github/workflows/playwright-tests.yml`)

**Triggers:**
- Pull requests that modify web frontend code
- Pushes to main branch
- Manual workflow dispatch

**Steps:**
1. Checkout code
2. Setup .NET 8
3. Restore and build solution
4. Start Docker services (database + web)
5. Install Playwright browsers
6. Run E2E tests
7. Upload test results as artifacts
8. Cleanup Docker services

**Test Results:**
- Test results are uploaded as TRX files
- Artifacts retained for 30 days
- Web logs available on failure

#### 2. **Docker Environment Tests** (`.github/workflows/docker-tests.yml`)

Validates that the Docker Compose setup works correctly.

### Viewing Test Results in CI

1. Go to the GitHub Actions tab
2. Select the workflow run
3. View test results in the summary
4. Download artifacts for detailed TRX reports

## Troubleshooting

### E2E Tests Fail with Connection Errors

**Problem:** Tests cannot connect to `http://localhost:5001`

**Solutions:**
```bash
# 1. Check if web app is running
curl http://localhost:5001/health

# 2. Check Docker services
cd docker
docker-compose ps

# 3. View web app logs
docker-compose logs web

# 4. Restart web service
docker-compose restart web
```

### Playwright Browser Not Found

**Problem:** `Browser executable not found`

**Solution:**
```bash
# Reinstall Playwright browsers
playwright install chromium
playwright install-deps chromium

# If that fails, reinstall Playwright CLI
dotnet tool uninstall --global Microsoft.Playwright.CLI
dotnet tool install --global Microsoft.Playwright.CLI
playwright install chromium
```

### Tests Time Out

**Problem:** Tests hang or timeout

**Solutions:**
1. **Increase timeout in test:**
   ```csharp
   await _page.GotoAsync(BaseUrl, new() { Timeout = 60000 }); // 60 seconds
   ```

2. **Check if page is loading:**
   ```bash
   # Open browser to check manually
   curl -I http://localhost:5001
   ```

3. **Run tests in headed mode (locally):**
   ```csharp
   _browser = await _playwright.Chromium.LaunchAsync(new()
   {
       Headless = false,  // See browser
       SlowMo = 50        // Slow down actions
   });
   ```

### Build Errors in CI

**Problem:** Build fails in GitHub Actions

**Solutions:**
1. Check .NET version matches (`net8.0`)
2. Ensure all NuGet packages are compatible
3. Review build logs for specific errors

### Test Results Not Showing in CI

**Problem:** Test results not visible in GitHub Actions

**Solution:**
- Results are uploaded as artifacts
- Click "Summary" tab in workflow run
- Download "playwright-test-results" artifact
- Open `.trx` file with Visual Studio or text editor

## Best Practices

### Writing E2E Tests

1. **Keep tests independent** - Each test should set up its own state
2. **Use descriptive names** - `Homepage_ShouldDisplay_LoginButton` is better than `Test1`
3. **Test user flows** - Not just element presence
4. **Use Playwright assertions** - Better error messages than xUnit asserts
5. **Clean up after tests** - Dispose browser resources properly

### Test Maintenance

1. **Run tests locally before pushing** - Catch issues early
2. **Update tests when UI changes** - Keep tests in sync with code
3. **Review failed CI tests promptly** - Don't let tests stay red
4. **Add tests for new features** - Maintain test coverage

## Additional Resources

- [xUnit Documentation](https://xunit.net/)
- [Playwright for .NET Documentation](https://playwright.dev/dotnet/)
- [Microsoft Testing Documentation](https://learn.microsoft.com/en-us/dotnet/core/testing/)
- [E2E Test Examples](tests/HomepageTests.cs)
- [E2E Test README](tests/E2E_README.md)

## Getting Help

If you encounter issues not covered in this guide:

1. Check the [GitHub Issues](https://github.com/diggforbeer/agent_test/issues)
2. Review the workflow logs in GitHub Actions
3. Check Docker logs: `docker-compose logs web`
4. Create a new issue with detailed error information
