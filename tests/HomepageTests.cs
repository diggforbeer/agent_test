using Microsoft.Playwright;
using Xunit;

namespace FriendShare.E2ETests;

[Trait("Category", "E2E")]
public class HomepageTests : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;
    
    private const string BaseUrl = "http://localhost:5001";

    public async Task InitializeAsync()
    {
        // Install Playwright browsers if needed (happens automatically on first run)
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        if (_page != null) await _page.CloseAsync();
        if (_context != null) await _context.CloseAsync();
        if (_browser != null) await _browser.CloseAsync();
        _playwright?.Dispose();
    }

    [Fact]
    public async Task Homepage_ShouldLoad_WithCorrectTitle()
    {
        // Arrange & Act
        await _page!.GotoAsync(BaseUrl);

        // Assert
        var title = await _page.TitleAsync();
        Assert.Equal("Welcome to FriendShare - FriendShare", title);
    }

    [Fact]
    public async Task Homepage_ShouldDisplay_HeroSection()
    {
        // Arrange & Act
        await _page!.GotoAsync(BaseUrl);

        // Assert - Check hero heading
        var heroHeading = await _page.Locator("h1.display-3").TextContentAsync();
        Assert.Contains("Share Items with Friends", heroHeading);

        // Assert - Check hero description
        var heroDescription = await _page.Locator(".hero-section .lead").TextContentAsync();
        Assert.Contains("Borrow what you need, lend what you have", heroDescription);
    }

    [Fact]
    public async Task Homepage_ShouldDisplay_GetStartedButton()
    {
        // Arrange & Act
        await _page!.GotoAsync(BaseUrl);

        // Assert
        var getStartedButton = _page.Locator("text=Get Started");
        await Assertions.Expect(getStartedButton).ToBeVisibleAsync();
        
        // Verify button links to registration
        var href = await getStartedButton.GetAttributeAsync("href");
        Assert.Contains("Register", href);
    }

    [Fact]
    public async Task Homepage_ShouldDisplay_LoginButton()
    {
        // Arrange & Act
        await _page!.GotoAsync(BaseUrl);

        // Assert
        var loginButton = _page.Locator("text=Login").First;
        await Assertions.Expect(loginButton).ToBeVisibleAsync();
        
        // Verify button links to login page
        var href = await loginButton.GetAttributeAsync("href");
        Assert.Contains("Login", href);
    }

    [Fact]
    public async Task Homepage_ShouldDisplay_ThreeFeatureCards()
    {
        // Arrange & Act
        await _page!.GotoAsync(BaseUrl);

        // Assert - Check all three feature cards are present
        var featureCards = _page.Locator(".features-section .feature-card");
        await Assertions.Expect(featureCards).ToHaveCountAsync(3);

        // Assert - Check feature titles
        var browseItemsFeature = _page.Locator("text=Browse Items");
        await Assertions.Expect(browseItemsFeature).ToBeVisibleAsync();

        var requestToBorrowFeature = _page.Locator("text=Request to Borrow");
        await Assertions.Expect(requestToBorrowFeature).ToBeVisibleAsync();

        var trackLendingFeature = _page.Locator("text=Track Lending");
        await Assertions.Expect(trackLendingFeature).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Homepage_ShouldDisplay_HowItWorksSection()
    {
        // Arrange & Act
        await _page!.GotoAsync(BaseUrl);

        // Assert - Check How It Works heading
        var howItWorksHeading = _page.Locator("text=How It Works");
        await Assertions.Expect(howItWorksHeading).ToBeVisibleAsync();

        // Assert - Check all four steps are present
        var step1 = _page.Locator("text=Create Your Account");
        await Assertions.Expect(step1).ToBeVisibleAsync();

        var step2 = _page.Locator("text=Add Friends");
        await Assertions.Expect(step2).ToBeVisibleAsync();

        var step3 = _page.Locator("text=List Your Items");
        await Assertions.Expect(step3).ToBeVisibleAsync();

        var step4 = _page.Locator(".how-it-works-section").Locator("text=Start Sharing");
        await Assertions.Expect(step4).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Homepage_ShouldDisplay_CallToActionSection()
    {
        // Arrange & Act
        await _page!.GotoAsync(BaseUrl);

        // Assert - Check CTA heading
        var ctaHeading = _page.Locator(".cta-section h2");
        var ctaText = await ctaHeading.TextContentAsync();
        Assert.Contains("Ready to start sharing", ctaText);

        // Assert - Check Sign Up button in CTA
        var signUpButton = _page.Locator(".cta-section").Locator("text=Sign Up Now");
        await Assertions.Expect(signUpButton).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Homepage_NavigationButtons_ShouldWork()
    {
        // Arrange
        await _page!.GotoAsync(BaseUrl);

        // Act - Click Get Started button
        await _page.Locator("text=Get Started").First.ClickAsync();

        // Assert - Should navigate to registration page
        await _page.WaitForURLAsync("**/Auth/Register");
        Assert.Contains("/Auth/Register", _page.Url);
    }
}
