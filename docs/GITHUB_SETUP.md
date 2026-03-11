# Adding Hotel Booking Solution to GitHub

## Quick Start - Choose Your Method

### ⭐ Method 1: Visual Studio GUI (Recommended)
Best for: Visual Studio users who prefer GUI

### 🔧 Method 2: PowerShell Script
Best for: Quick automated setup

### 💻 Method 3: Manual Commands
Best for: Full control and understanding

---

## Method 1: Using Visual Studio (Easiest) ⭐

### Prerequisites
- Visual Studio 2022
- GitHub account
- Solution open in Visual Studio

### Steps

#### 1. Open Git Changes Window
- Go to **View** → **Git Changes**
- Or press `Ctrl+0, Ctrl+G`
- Or click the **Git Changes** tab at bottom

#### 2. Create Git Repository
- If not initialized, click **Create Git Repository**
- Dialog opens with options:

#### 3. Configure Repository
```
┌─────────────────────────────────────────┐
│ Create a Git repository                 │
├─────────────────────────────────────────┤
│ ☑ Local repository only                │
│ ☐ Push to a new remote                 │
│                                         │
│ Choose: ☑ Push to a new remote         │
│                                         │
│ Account: [Select your GitHub account]  │
│ Owner: [Your username]                  │
│ Repository name: HotelBooking           │
│                                         │
│ ☑ Private repository                   │
│                                         │
│ [Create and Push]                       │
└─────────────────────────────────────────┘
```

#### 4. Select GitHub Account
If you haven't signed in:
1. Click **Add an account**
2. Select **GitHub**
3. Click **Sign in with browser**
4. Authorize Visual Studio in browser
5. Return to Visual Studio

#### 5. Fill Repository Details
- **Owner:** Your GitHub username
- **Repository name:** `HotelBooking`
- **Description:** (Optional) `Comprehensive hotel booking platform with .NET 8`
- **Visibility:** Choose Public or Private
- ✅ Check: "Add a README" (if you want)
- ✅ Check: "Add .gitignore" (already have one)

#### 6. Create and Push
- Click **Create and Push**
- Visual Studio will:
  1. Initialize local Git repository
  2. Create initial commit
  3. Create GitHub repository
  4. Push code to GitHub

#### 7. Verify
- Open browser to `https://github.com/YOUR_USERNAME/HotelBooking`
- Your code should be there! 🎉

---

## Method 2: Using PowerShell Script 🔧

### Steps

#### 1. Open PowerShell in Solution Directory
In Visual Studio:
- **View** → **Terminal** (Ctrl+`)
- Or open PowerShell and navigate to your solution folder:
  ```powershell
  cd C:\Work\Temp\005-final\HotelBooking
  ```

#### 2. Run the Script
```powershell
.\setup-github.ps1
```

#### 3. Follow the Prompts
The script will:
1. Check if Git is installed
2. Initialize Git repository
3. Configure Git user
4. Create initial commit
5. Guide you through GitHub setup

#### 4. Create GitHub Repository
When prompted, go to https://github.com/new and:
- Repository name: `HotelBooking`
- **DO NOT** initialize with README or .gitignore
- Click **Create repository**

#### 5. Complete Setup
Follow the script prompts to add remote and push.

---

## Method 3: Manual Commands 💻

### Step 1: Check Git Installation
```powershell
git --version
```

If not installed, download from https://git-scm.com/

### Step 2: Configure Git (First Time Only)
```powershell
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
```

### Step 3: Initialize Repository
```powershell
# Navigate to solution folder
cd C:\Work\Temp\005-final\HotelBooking

# Initialize Git
git init

# Check status
git status
```

### Step 4: Add Files to Git
```powershell
# Add all files
git add .

# Check what will be committed
git status
```

### Step 5: Create Initial Commit
```powershell
git commit -m "Initial commit: Hotel Booking System

- Clean layered architecture with .NET 8
- Repository and Unit of Work patterns
- Comprehensive test suite (34 tests)
- Architecture and interview documentation
- API with Swagger documentation
- Audit logging infrastructure"
```

### Step 6: Create GitHub Repository

#### Option A: Using GitHub CLI (if installed)
```powershell
gh repo create HotelBooking --public --source=. --remote=origin --push
```

#### Option B: Using Web Browser
1. Go to https://github.com/new
2. Fill in details:
   - **Repository name:** `HotelBooking`
   - **Description:** `Comprehensive hotel booking platform with .NET 8`
   - **Visibility:** Public or Private
   - ❌ **DO NOT** check "Add a README file"
   - ❌ **DO NOT** check "Add .gitignore"
   - ❌ **DO NOT** choose a license yet
3. Click **Create repository**

### Step 7: Connect Local to GitHub
After creating the repository, GitHub shows commands. Run them:

```powershell
# Add GitHub remote (replace YOUR_USERNAME)
git remote add origin https://github.com/YOUR_USERNAME/HotelBooking.git

# Verify remote was added
git remote -v

# Rename branch to main (if needed)
git branch -M main

# Push to GitHub
git push -u origin main
```

### Step 8: Verify
Open browser to:
```
https://github.com/YOUR_USERNAME/HotelBooking
```

You should see your code! 🎉

---

## What Gets Committed?

Based on your `.gitignore`, these files **WILL** be committed:
- ✅ Source code (`.cs` files)
- ✅ Project files (`.csproj`, `.sln`)
- ✅ Documentation (`.md` files)
- ✅ Configuration files (`appsettings.json`)

These files **WILL NOT** be committed:
- ❌ Build outputs (`bin/`, `obj/`)
- ❌ User settings (`.vs/`, `.vscode/`, `.idea/`)
- ❌ Database files (`.db`, `.sqlite`)
- ❌ Packages folder
- ❌ Log files

---

## Common Issues & Solutions

### Issue 1: "Git is not recognized"
**Solution:** Install Git from https://git-scm.com/ and restart Visual Studio

### Issue 2: "Permission denied (publickey)"
**Solution:** Use HTTPS instead of SSH:
```powershell
git remote set-url origin https://github.com/YOUR_USERNAME/HotelBooking.git
```

### Issue 3: "Repository already exists"
**Solution:** 
1. Delete the repository on GitHub
2. Or use a different repository name
3. Or force push (⚠️ careful):
```powershell
git push -u origin main --force
```

### Issue 4: "Authentication failed"
**Solution:** Use Personal Access Token (PAT):
1. Go to GitHub Settings → Developer settings → Personal access tokens
2. Generate new token (classic)
3. Select scopes: `repo`
4. Use token as password when prompted

### Issue 5: Large files not being committed
**Solution:** GitHub has a 100MB file limit. Check for large files:
```powershell
# Find large files
Get-ChildItem -Recurse | Where-Object {$_.Length -gt 50MB} | Select-Object FullName, @{Name="SizeMB";Expression={[math]::Round($_.Length / 1MB, 2)}}
```

---

## Post-Setup Tasks

### 1. Add Repository Description
On GitHub:
1. Click **⚙️ Settings**
2. Add description: "Comprehensive hotel booking platform built with .NET 8"
3. Add topics: `dotnet`, `csharp`, `aspnetcore`, `hotel-booking`, `efcore`

### 2. Add Repository Badges
Add to top of README.md:
```markdown
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-green)
![Build](https://img.shields.io/badge/build-passing-brightgreen)
```

### 3. Enable GitHub Actions (Optional)
Create `.github/workflows/dotnet.yml` for CI/CD

### 4. Protect Main Branch
On GitHub:
1. Settings → Branches
2. Add branch protection rule for `main`
3. Enable:
   - ✅ Require pull request reviews
   - ✅ Require status checks to pass
   - ✅ Require branches to be up to date

### 5. Add LICENSE File
```powershell
# Create MIT License
@"
MIT License

Copyright (c) $(Get-Date -Format yyyy) Your Name

Permission is hereby granted, free of charge, to any person obtaining a copy...
"@ | Out-File -FilePath LICENSE -Encoding UTF8

git add LICENSE
git commit -m "Add MIT license"
git push
```

---

## Useful Git Commands

### Check Status
```powershell
git status                    # See changes
git log --oneline            # See commit history
git remote -v                # See remote repositories
```

### Make Changes
```powershell
git add .                    # Stage all changes
git commit -m "message"      # Commit changes
git push                     # Push to GitHub
```

### Branching
```powershell
git branch feature-name      # Create branch
git checkout feature-name    # Switch to branch
git checkout -b feature-name # Create and switch
git merge feature-name       # Merge branch
```

### Undo Changes
```powershell
git restore file.cs          # Discard changes to file
git restore --staged file.cs # Unstage file
git reset --hard HEAD        # ⚠️ Discard all changes
git revert <commit-id>       # Revert a commit
```

### Update from GitHub
```powershell
git pull                     # Fetch and merge
git fetch                    # Fetch only
```

---

## Best Practices

### Commit Messages
Use clear, descriptive messages:
```
✅ Good:
git commit -m "Add booking validation logic"
git commit -m "Fix: Resolve null reference in RoomService"
git commit -m "Refactor: Extract payment processing to separate service"

❌ Bad:
git commit -m "fixed stuff"
git commit -m "WIP"
git commit -m "asdf"
```

### Commit Frequency
- Commit often (small, logical chunks)
- Each commit should be a complete, working change
- Don't commit broken code to main

### Branch Strategy
```
main (production-ready code)
  ├── develop (integration branch)
  │   ├── feature/booking-system
  │   ├── feature/payment-integration
  │   └── feature/loyalty-program
  └── hotfix/critical-bug
```

### .gitignore
Keep it updated:
- Never commit secrets/passwords
- Never commit large binary files
- Never commit build artifacts

---

## Troubleshooting

### View Ignored Files
```powershell
git status --ignored
```

### Force Add Ignored File (not recommended)
```powershell
git add -f path/to/file
```

### Remove File from Git (keep locally)
```powershell
git rm --cached path/to/file
```

### Clear Git Cache
```powershell
git rm -r --cached .
git add .
git commit -m "Clear cache and reapply .gitignore"
```

---

## Need Help?

- **Git Documentation:** https://git-scm.com/doc
- **GitHub Guides:** https://guides.github.com/
- **Visual Studio Git:** https://docs.microsoft.com/visualstudio/version-control/

---

## Summary

**Quickest Method:**
1. Visual Studio → Git Changes → Create Git Repository
2. Select GitHub
3. Fill in details
4. Click Create and Push
5. Done! ✅

**Most Control:**
1. `git init`
2. `git add .`
3. `git commit -m "Initial commit"`
4. Create repo on GitHub
5. `git remote add origin URL`
6. `git push -u origin main`

Choose the method that works best for you! 🚀
