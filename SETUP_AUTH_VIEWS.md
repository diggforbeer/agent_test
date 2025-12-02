# Setup Required: Auth Views

⚠️ **IMPORTANT**: Before running the web application, you must create the Auth views directory structure.

## Why Manual Setup is Needed

The development environment's file creation tools cannot create parent directories automatically. The Auth view files have complete content embedded in the setup scripts and just need the directory structure created.

## Quick Setup (Choose One)

### Option 1: Python Script (Recommended - Cross-Platform)

```bash
python3 setup-auth-views.py
```

or on Windows:

```bash
python setup-auth-views.py
```

### Option 2: Bash Script (Unix/Linux/Mac)

```bash
chmod +x create-auth-views.sh && ./create-auth-views.sh
```

Both scripts will:
1. Create `src/FriendShare.Web/Views/Auth/` directory
2. Create all authentication view files with embedded content
3. Verify the setup

## Alternative: Manual Creation

If neither script works, create the directory manually and run either script:

```bash
mkdir -p src/FriendShare.Web/Views/Auth
```

Then run either setup script above.

## What Gets Created

- `src/FriendShare.Web/Views/Auth/Login.cshtml`
- `src/FriendShare.Web/Views/Auth/Register.cshtml`
- `src/FriendShare.Web/Views/Auth/ForgotPassword.cshtml`
- `src/FriendShare.Web/Views/Auth/ConfirmEmail.cshtml`

## After Setup

1. Build: `dotnet build`
2. Run API: `cd src/FriendShare.Api && dotnet run`
3. Run Web: `cd src/FriendShare.Web && dotnet run`

See `AUTH_VIEWS_CREATION_GUIDE.md` and `PR_SUMMARY.md` for complete details.
