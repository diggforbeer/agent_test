# Setup Required: Auth Views

⚠️ **IMPORTANT**: Before running the web application, you must create the Auth views directory structure.

## Quick Setup

Run from the repository root:

```bash
chmod +x create-auth-views.sh && ./create-auth-views.sh
```

This script will:
1. Create `src/FriendShare.Web/Views/Auth/` directory
2. Copy all authentication view files from `/tmp/` to the Auth directory
3. Verify the setup

## Alternative: Manual Setup

If the script doesn't work, manually create the directory and copy files:

```bash
mkdir -p src/FriendShare.Web/Views/Auth
cp /tmp/Login.cshtml src/FriendShare.Web/Views/Auth/
cp /tmp/Register.cshtml src/FriendShare.Web/Views/Auth/
cp /tmp/ForgotPassword.cshtml src/FriendShare.Web/Views/Auth/
cp /tmp/ConfirmEmail.cshtml src/FriendShare.Web/Views/Auth/
```

## After Setup

1. Build: `dotnet build`
2. Run API: `cd src/FriendShare.Api && dotnet run`
3. Run Web: `cd src/FriendShare.Web && dotnet run`

See `AUTH_VIEWS_CREATION_GUIDE.md` and `PR_SUMMARY.md` for complete details.
