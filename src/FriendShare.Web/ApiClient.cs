using System.Net.Http.Json;
using FriendShare.Core.DTOs;

namespace FriendShare.Web.Services;

/// <summary>
/// Service for making HTTP requests to the FriendShare API.
/// </summary>
public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            
            return result ?? new AuthResponse 
            { 
                Success = false, 
                Message = "Failed to parse response" 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return new AuthResponse 
            { 
                Success = false, 
                Message = $"Registration failed: {ex.Message}" 
            };
        }
    }

    /// <summary>
    /// Logs in a user.
    /// </summary>
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            
            return result ?? new AuthResponse 
            { 
                Success = false, 
                Message = "Failed to parse response" 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return new AuthResponse 
            { 
                Success = false, 
                Message = $"Login failed: {ex.Message}" 
            };
        }
    }

    /// <summary>
    /// Sends a forgot password request.
    /// </summary>
    public async Task<AuthResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/forgot-password", request);
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            
            return result ?? new AuthResponse 
            { 
                Success = false, 
                Message = "Failed to parse response" 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password");
            return new AuthResponse 
            { 
                Success = false, 
                Message = $"Forgot password failed: {ex.Message}" 
            };
        }
    }

    /// <summary>
    /// Confirms a user's email.
    /// </summary>
    public async Task<AuthResponse> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/confirm-email", request);
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            
            return result ?? new AuthResponse 
            { 
                Success = false, 
                Message = "Failed to parse response" 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email confirmation");
            return new AuthResponse 
            { 
                Success = false, 
                Message = $"Email confirmation failed: {ex.Message}" 
            };
        }
    }
}
