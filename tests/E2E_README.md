# Playwright E2E Tests

This directory contains end-to-end tests for the FriendShare application using Microsoft.Playwright.

## Test Coverage

The homepage tests (`HomepageTests.cs`) validate:

- ✅ Page loads with correct title
- ✅ Hero section displays properly
- ✅ Get Started and Login buttons are visible and functional
- ✅ Three feature cards are displayed (Browse Items, Request to Borrow, Track Lending)
- ✅ "How It Works" section with all four steps
- ✅ Call-to-action section
- ✅ Navigation buttons work correctly

## Running Tests Locally

### Prerequisites

1. .NET 8 SDK installed
2. Docker and Docker Compose (for running the web application)
3. Playwright browsers installed

### Setup

1. Install Playwright browsers (first time only):
   ```bash
   dotnet tool install --global Microsoft.Playwright.CLI
   playwright install chromium
   playwright install-deps chromium
   ```

2. Start the web application:
   ```bash
   # From the docker directory
   cd docker
   docker-compose up -d web
   ```

3. Wait for the application to be healthy:
   ```bash
   curl http://localhost:5001/health
   ```

### Running the Tests

From the repository root:

```bash
# Run all E2E tests
dotnet test tests/FriendShare.E2ETests.csproj

# Run with detailed output
dotnet test tests/FriendShare.E2ETests.csproj --verbosity normal

# Run specific test
dotnet test tests/FriendShare.E2ETests.csproj --filter "FullyQualifiedName~Homepage_ShouldLoad_WithCorrectTitle"
```

### Troubleshooting

**Tests fail with connection errors:**
- Ensure the web application is running on `http://localhost:5001`
- Check that the health endpoint returns 200: `curl http://localhost:5001/health`

**Playwright browser not found:**
- Run `playwright install chromium` to install the browser
- Run `playwright install-deps` to install system dependencies

**Tests fail with timeout:**
- Increase the timeout in the test configuration
- Check if the application is slow to start

## CI/CD

These tests run automatically on:
- Pull requests that modify web frontend code
- Pushes to the main branch
- Manual workflow dispatch

The GitHub Actions workflow:
1. Builds the .NET solution
2. Starts the database and web services using Docker Compose
3. Installs Playwright browsers
4. Runs the E2E tests
5. Uploads test results as artifacts

See `.github/workflows/playwright-tests.yml` for the complete workflow configuration.

## Writing New Tests

Follow the existing test structure in `HomepageTests.cs`:

```csharp
[Fact]
[Trait("Category", "E2E")]
public async Task YourTest_Scenario_ExpectedResult()
{
    // Arrange
    await _page!.GotoAsync(BaseUrl);

    // Act
    // Perform actions

    // Assert
    // Verify expected results
}
```

Use Playwright's locator API for finding elements:
- `_page.Locator("text=Button Text")` - Find by text
- `_page.Locator(".css-class")` - Find by CSS selector
- `_page.Locator("#element-id")` - Find by ID

Use Playwright assertions for better error messages:
- `await Assertions.Expect(locator).ToBeVisibleAsync()`
- `await Assertions.Expect(locator).ToHaveCountAsync(3)`
- `await Assertions.Expect(locator).ToHaveTextAsync("Expected text")`
