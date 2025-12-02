using Microsoft.AspNetCore.Mvc;
using FriendShare.Web.Models;
using FriendShare.Web.Services;
using FriendShare.Core.DTOs;

namespace FriendShare.Web.Controllers;

/// <summary>
/// Controller for authentication pages.
/// </summary>
public class AuthController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ApiClient apiClient, ILogger<AuthController> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    /// Displays the login page.
    /// </summary>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Handles login form submission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new LoginRequest
        {
            Email = model.Email,
            Password = model.Password
        };

        var response = await _apiClient.LoginAsync(request);

        if (!response.Success)
        {
            ModelState.AddModelError(string.Empty, response.Message ?? "Login failed");
            return View(model);
        }

        // Store token in session
        if (response.Token != null)
        {
            HttpContext.Session.SetString("JwtToken", response.Token);
        }

        if (response.RefreshToken != null)
        {
            HttpContext.Session.SetString("RefreshToken", response.RefreshToken);
        }

        if (response.User != null)
        {
            HttpContext.Session.SetString("UserName", response.User.UserName);
            HttpContext.Session.SetString("UserId", response.User.Id);
        }

        // Redirect to return URL or dashboard
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Displays the registration page.
    /// </summary>
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    /// <summary>
    /// Handles registration form submission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new RegisterRequest
        {
            UserName = model.UserName,
            Email = model.Email,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        var response = await _apiClient.RegisterAsync(request);

        if (!response.Success)
        {
            ModelState.AddModelError(string.Empty, response.Message ?? "Registration failed");
            return View(model);
        }

        TempData["SuccessMessage"] = "Registration successful! Please check your email to confirm your account.";
        return RedirectToAction(nameof(Login));
    }

    /// <summary>
    /// Displays the forgot password page.
    /// </summary>
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    /// <summary>
    /// Handles forgot password form submission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new ForgotPasswordRequest
        {
            Email = model.Email
        };

        var response = await _apiClient.ForgotPasswordAsync(request);

        TempData["SuccessMessage"] = "If your email is registered, you will receive a password reset link shortly.";
        return RedirectToAction(nameof(Login));
    }

    /// <summary>
    /// Handles email confirmation.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            ViewBag.ErrorMessage = "Invalid email confirmation link.";
            return View();
        }

        var request = new ConfirmEmailRequest
        {
            UserId = userId,
            Token = token
        };

        var response = await _apiClient.ConfirmEmailAsync(request);

        if (response.Success)
        {
            ViewBag.SuccessMessage = "Email confirmed successfully! You can now log in.";
        }
        else
        {
            ViewBag.ErrorMessage = response.Message ?? "Email confirmation failed.";
        }

        return View();
    }

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
