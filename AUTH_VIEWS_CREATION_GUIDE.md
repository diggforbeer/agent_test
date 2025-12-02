# Auth Views Setup Guide

The authentication views need to be created in the `src/FriendShare.Web/Views/Auth/` directory.

## Quick Setup (Recommended)

Run the provided shell script from the repository root:

```bash
chmod +x create-auth-views.sh
./create-auth-views.sh
```

This will automatically create the Auth directory and all view files.

## Manual Setup

If you prefer to create the files manually or if the script doesn't work, follow these steps:

### Step 1: Create the Auth Directory

```bash
mkdir -p src/FriendShare.Web/Views/Auth
```

### Step 2: Copy View Files

The view files have been created in `/tmp` directory. Copy them to the Auth views folder:

```bash
cp /tmp/Login.cshtml src/FriendShare.Web/Views/Auth/
cp /tmp/Register.cshtml src/FriendShare.Web/Views/Auth/
cp /tmp/ForgotPassword.cshtml src/FriendShare.Web/Views/Auth/
cp /tmp/ConfirmEmail.cshtml src/FriendShare.Web/Views/Auth/
```

## Verification

After creating the files, verify the structure:

```bash
ls -la src/FriendShare.Web/Views/Auth/
```

You should see:
- Login.cshtml
- Register.cshtml  
- ForgotPassword.cshtml
- ConfirmEmail.cshtml

## What's Been Implemented

✅ **Backend Integration**
- ApiClient service for HTTP requests to the API
- AuthController with actions for Login, Register, ForgotPassword, ConfirmEmail, and Logout
- ViewModels for all authentication forms
- Session management for JWT tokens

✅ **Frontend Pages**
- Enhanced homepage with hero section and feature highlights
- Login page with form validation
- Register page with password strength indicator
- Forgot password page
- Email confirmation page
- Responsive design with Bootstrap 5

✅ **UI Enhancements**
- Updated navigation bar with login/logout states
- Custom CSS styling for authentication pages
- JavaScript for password strength checking and form validation
- Loading spinner for async operations
- Auto-dismissing alerts

✅ **Configuration**
- Updated Program.cs with Session support and HttpClient
- API base URL configuration in appsettings.json
- Added System.Net.Http.Json NuGet package
- Added reference to FriendShare.Core project

## Next Steps

After creating the Auth views:

1. Build the solution to ensure everything compiles
2. Run the FriendShare.Api project (backend)
3. Run the FriendShare.Web project (frontend)
4. Test the authentication flow:
   - Register a new user
   - Check email confirmation (if email service is configured)
   - Login with credentials
   - Test forgot password
   - Verify session management

## Troubleshooting

If you encounter issues:

1. **Views not found**: Ensure the Auth directory exists at `src/FriendShare.Web/Views/Auth/`
2. **Compilation errors**: Make sure FriendShare.Core project is referenced
3. **API connection errors**: Verify the API base URL in appsettings.json matches your API endpoint
4. **Session not working**: Ensure `app.UseSession()` is called before `app.UseAuthorization()` in Program.cs


## 1. Views/Auth/Login.cshtml

```cshtml
@model FriendShare.Web.Models.LoginViewModel

@{
    ViewData["Title"] = "Login";
}

<div class="container">
    <div class="row justify-content-center">
        <div class="col-md-6 col-lg-5">
            <div class="card shadow-sm mt-5">
                <div class="card-body p-5">
                    <h2 class="card-title text-center mb-4">Login</h2>

                    @if (TempData["SuccessMessage"] != null)
                    {
                        <div class="alert alert-success alert-dismissible fade show" role="alert">
                            @TempData["SuccessMessage"]
                            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                        </div>
                    }

                    <form asp-action="Login" method="post">
                        <div asp-validation-summary="ModelOnly" class="text-danger mb-3"></div>

                        <div class="mb-3">
                            <label asp-for="Email" class="form-label"></label>
                            <input asp-for="Email" class="form-control" placeholder="your@email.com" />
                            <span asp-validation-for="Email" class="text-danger"></span>
                        </div>

                        <div class="mb-3">
                            <label asp-for="Password" class="form-label"></label>
                            <input asp-for="Password" class="form-control" placeholder="Enter your password" />
                            <span asp-validation-for="Password" class="text-danger"></span>
                        </div>

                        <div class="mb-3 form-check">
                            <input asp-for="RememberMe" class="form-check-input" />
                            <label asp-for="RememberMe" class="form-check-label"></label>
                        </div>

                        <div class="d-grid mb-3">
                            <button type="submit" class="btn btn-primary btn-lg">Login</button>
                        </div>

                        <div class="text-center">
                            <a asp-action="ForgotPassword" class="text-decoration-none">Forgot your password?</a>
                        </div>
                    </form>

                    <hr class="my-4">

                    <div class="text-center">
                        <p class="mb-0">Don't have an account? <a asp-action="Register" class="text-decoration-none fw-semibold">Sign Up</a></p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

## 2. Views/Auth/Register.cshtml

```cshtml
@model FriendShare.Web.Models.RegisterViewModel

@{
    ViewData["Title"] = "Register";
}

<div class="container">
    <div class="row justify-content-center">
        <div class="col-md-6 col-lg-5">
            <div class="card shadow-sm mt-5">
                <div class="card-body p-5">
                    <h2 class="card-title text-center mb-4">Create Account</h2>

                    <form asp-action="Register" method="post">
                        <div asp-validation-summary="ModelOnly" class="text-danger mb-3"></div>

                        <div class="mb-3">
                            <label asp-for="UserName" class="form-label"></label>
                            <input asp-for="UserName" class="form-control" placeholder="Choose a username" />
                            <span asp-validation-for="UserName" class="text-danger"></span>
                        </div>

                        <div class="mb-3">
                            <label asp-for="Email" class="form-label"></label>
                            <input asp-for="Email" class="form-control" placeholder="your@email.com" />
                            <span asp-validation-for="Email" class="text-danger"></span>
                        </div>

                        <div class="mb-3">
                            <label asp-for="Password" class="form-label"></label>
                            <input asp-for="Password" class="form-control" placeholder="Create a strong password" />
                            <span asp-validation-for="Password" class="text-danger"></span>
                            <ul class="password-requirements small mt-2 mb-0 ps-3">
                                <li>At least 8 characters long</li>
                                <li>Contains uppercase letter (A-Z)</li>
                                <li>Contains lowercase letter (a-z)</li>
                                <li>Contains number (0-9)</li>
                                <li>Contains special character (!@@#$%^&*)</li>
                            </ul>
                        </div>

                        <div class="mb-3">
                            <label asp-for="ConfirmPassword" class="form-label"></label>
                            <input asp-for="ConfirmPassword" class="form-control" placeholder="Re-enter password" />
                            <span asp-validation-for="ConfirmPassword" class="text-danger"></span>
                        </div>

                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label asp-for="FirstName" class="form-label"></label>
                                <input asp-for="FirstName" class="form-control" placeholder="(Optional)" />
                                <span asp-validation-for="FirstName" class="text-danger"></span>
                            </div>

                            <div class="col-md-6 mb-3">
                                <label asp-for="LastName" class="form-label"></label>
                                <input asp-for="LastName" class="form-control" placeholder="(Optional)" />
                                <span asp-validation-for="LastName" class="text-danger"></span>
                            </div>
                        </div>

                        <div class="d-grid mb-3">
                            <button type="submit" class="btn btn-primary btn-lg">Create Account</button>
                        </div>
                    </form>

                    <hr class="my-4">

                    <div class="text-center">
                        <p class="mb-0">Already have an account? <a asp-action="Login" class="text-decoration-none fw-semibold">Login</a></p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

## 3. Views/Auth/ForgotPassword.cshtml

```cshtml
@model FriendShare.Web.Models.ForgotPasswordViewModel

@{
    ViewData["Title"] = "Forgot Password";
}

<div class="container">
    <div class="row justify-content-center">
        <div class="col-md-6 col-lg-5">
            <div class="card shadow-sm mt-5">
                <div class="card-body p-5">
                    <h2 class="card-title text-center mb-4">Forgot Password</h2>
                    <p class="text-muted text-center mb-4">
                        Enter your email address and we'll send you a link to reset your password.
                    </p>

                    <form asp-action="ForgotPassword" method="post">
                        <div asp-validation-summary="ModelOnly" class="text-danger mb-3"></div>

                        <div class="mb-3">
                            <label asp-for="Email" class="form-label"></label>
                            <input asp-for="Email" class="form-control" placeholder="your@email.com" />
                            <span asp-validation-for="Email" class="text-danger"></span>
                        </div>

                        <div class="d-grid mb-3">
                            <button type="submit" class="btn btn-primary btn-lg">Send Reset Link</button>
                        </div>

                        <div class="text-center">
                            <a asp-action="Login" class="text-decoration-none">Back to Login</a>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

## 4. Views/Auth/ConfirmEmail.cshtml

```cshtml
@{
    ViewData["Title"] = "Email Confirmation";
}

<div class="container">
    <div class="row justify-content-center">
        <div class="col-md-6 col-lg-5">
            <div class="card shadow-sm mt-5">
                <div class="card-body p-5 text-center">
                    @if (ViewBag.SuccessMessage != null)
                    {
                        <div class="text-success mb-3">
                            <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" fill="currentColor" class="bi bi-check-circle" viewBox="0 0 16 16">
                                <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14m0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16"/>
                                <path d="m10.97 4.97-.02.022-3.473 4.425-2.093-2.094a.75.75 0 0 0-1.06 1.06L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-1.071-1.05"/>
                            </svg>
                        </div>
                        <h2 class="text-success mb-3">Success!</h2>
                        <p class="mb-4">@ViewBag.SuccessMessage</p>
                        <a asp-action="Login" class="btn btn-primary">Go to Login</a>
                    }
                    else if (ViewBag.ErrorMessage != null)
                    {
                        <div class="text-danger mb-3">
                            <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" fill="currentColor" class="bi bi-x-circle" viewBox="0 0 16 16">
                                <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14m0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16"/>
                                <path d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708"/>
                            </svg>
                        </div>
                        <h2 class="text-danger mb-3">Error</h2>
                        <p class="mb-4">@ViewBag.ErrorMessage</p>
                        <a asp-action="Login" class="btn btn-outline-primary">Back to Login</a>
                    }
                    else
                    {
                        <div class="spinner-border text-primary mb-3" role="status">
                            <span class="visually-hidden">Loading...</span>
                        </div>
                        <p>Confirming your email...</p>
                    }
                </div>
            </div>
        </div>
    </div>
</div>
```

## Instructions

1. Create a new folder: `Views/Auth`
2. Create the four files listed above with their respective content
3. Ensure the Auth folder is at the same level as the Home and Shared folders

All view files are ready to use and follow ASP.NET Core MVC conventions.
