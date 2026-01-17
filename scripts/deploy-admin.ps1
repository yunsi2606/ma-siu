# Deploy Admin Dashboard to Vercel
# Requires: npm i -g vercel

param(
    [switch]$Production,
    [switch]$Preview
)

$ErrorActionPreference = "Stop"

Write-Host "=== Ma Siu Admin - Vercel Deployment ===" -ForegroundColor Cyan

# Navigate to admin app
$adminPath = Join-Path $PSScriptRoot "..\src\Apps\app-admin"
Set-Location $adminPath

# Check if vercel is installed
if (-not (Get-Command vercel -ErrorAction SilentlyContinue)) {
    Write-Host "Vercel CLI not found. Installing..." -ForegroundColor Yellow
    npm install -g vercel
}

# Build the app first
Write-Host "Building Next.js app..." -ForegroundColor Yellow
npm run build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""

if ($Production) {
    Write-Host "Deploying to PRODUCTION..." -ForegroundColor Red
    vercel --prod
} elseif ($Preview) {
    Write-Host "Deploying to Preview..." -ForegroundColor Yellow
    vercel
} else {
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\deploy-admin.ps1 -Preview      # Deploy to preview URL"
    Write-Host "  .\deploy-admin.ps1 -Production   # Deploy to production"
    Write-Host ""
    Write-Host "First time setup:" -ForegroundColor Cyan
    Write-Host "  1. Run 'vercel login' to authenticate"
    Write-Host "  2. Run 'vercel link' in app-admin folder to link project"
    Write-Host ""
}

Write-Host ""
Write-Host "Done!" -ForegroundColor Green
