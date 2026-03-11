# PowerShell Script to Initialize Git and Push to GitHub
# Run this from your solution root directory

Write-Host "🚀 Initializing Git Repository for Hotel Booking Solution" -ForegroundColor Cyan
Write-Host ""

# Check if git is installed
try {
    $gitVersion = git --version
    Write-Host "✅ Git is installed: $gitVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Git is not installed. Please install from https://git-scm.com/" -ForegroundColor Red
    exit 1
}

# Initialize git repository
if (Test-Path ".git") {
    Write-Host "⚠️  Git repository already initialized" -ForegroundColor Yellow
} else {
    Write-Host "📦 Initializing Git repository..." -ForegroundColor Cyan
    git init
    Write-Host "✅ Git repository initialized" -ForegroundColor Green
}

# Configure git user (if not already configured)
$userName = git config user.name
$userEmail = git config user.email

if (-not $userName) {
    $inputName = Read-Host "Enter your name for Git commits"
    git config user.name $inputName
}

if (-not $userEmail) {
    $inputEmail = Read-Host "Enter your email for Git commits"
    git config user.email $inputEmail
}

Write-Host ""
Write-Host "👤 Git user configured:" -ForegroundColor Cyan
Write-Host "   Name: $(git config user.name)"
Write-Host "   Email: $(git config user.email)"
Write-Host ""

# Add all files
Write-Host "📝 Adding files to Git..." -ForegroundColor Cyan
git add .

# Check git status
Write-Host ""
Write-Host "📊 Git Status:" -ForegroundColor Cyan
git status --short

# Create initial commit
Write-Host ""
Write-Host "💾 Creating initial commit..." -ForegroundColor Cyan
git commit -m "Initial commit: Hotel Booking System

- Clean layered architecture with .NET 8
- Repository and Unit of Work patterns
- Comprehensive test suite (34 tests)
- Architecture and interview documentation
- API with Swagger documentation
- Audit logging infrastructure
"

Write-Host "✅ Initial commit created" -ForegroundColor Green

# Instructions for GitHub
Write-Host ""
Write-Host "🌐 Next Steps - Create GitHub Repository:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Go to https://github.com/new" -ForegroundColor White
Write-Host "2. Repository name: HotelBooking" -ForegroundColor White
Write-Host "3. Description: Comprehensive hotel booking platform with .NET 8" -ForegroundColor White
Write-Host "4. Choose Public or Private" -ForegroundColor White
Write-Host "5. ❌ DO NOT initialize with README (you already have one)" -ForegroundColor Red
Write-Host "6. ❌ DO NOT add .gitignore (you already have one)" -ForegroundColor Red
Write-Host "7. Click 'Create repository'" -ForegroundColor White
Write-Host ""
Write-Host "After creating the repository, GitHub will show you commands." -ForegroundColor Yellow
Write-Host "Run these commands (replace YOUR_USERNAME):" -ForegroundColor Yellow
Write-Host ""
Write-Host "git remote add origin https://github.com/YOUR_USERNAME/HotelBooking.git" -ForegroundColor Cyan
Write-Host "git branch -M main" -ForegroundColor Cyan
Write-Host "git push -u origin main" -ForegroundColor Cyan
Write-Host ""

# Ask if user wants to add remote now
$addRemote = Read-Host "Do you want to add GitHub remote now? (y/n)"

if ($addRemote -eq 'y' -or $addRemote -eq 'Y') {
    $username = Read-Host "Enter your GitHub username"
    $repoName = Read-Host "Enter repository name (default: HotelBooking)"

    if ([string]::IsNullOrWhiteSpace($repoName)) {
        $repoName = "HotelBooking"
    }

    $remoteUrl = "https://github.com/$username/$repoName.git"

    Write-Host ""
    Write-Host "🔗 Adding remote origin: $remoteUrl" -ForegroundColor Cyan

    try {
        git remote add origin $remoteUrl
        Write-Host "✅ Remote origin added" -ForegroundColor Green

        # Rename branch to main
        Write-Host ""
        Write-Host "🔄 Renaming branch to main..." -ForegroundColor Cyan
        git branch -M main

        Write-Host ""
        Write-Host "⚠️  Make sure you've created the repository on GitHub first!" -ForegroundColor Yellow
        $push = Read-Host "Ready to push to GitHub? (y/n)"

        if ($push -eq 'y' -or $push -eq 'Y') {
            Write-Host ""
            Write-Host "🚀 Pushing to GitHub..." -ForegroundColor Cyan
            git push -u origin main
            Write-Host ""
            Write-Host "✅ Successfully pushed to GitHub!" -ForegroundColor Green
            Write-Host "🌐 Visit: https://github.com/$username/$repoName" -ForegroundColor Cyan
        }
    } catch {
        Write-Host "❌ Error adding remote: $_" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "✨ Done! Your repository is ready." -ForegroundColor Green
