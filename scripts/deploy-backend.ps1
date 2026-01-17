# Deploy Backend Services to Docker
# Run this script from the project root directory

param(
    [switch]$Build,
    [switch]$Up,
    [switch]$Down,
    [switch]$Logs,
    [string]$Service
)

$ErrorActionPreference = "Stop"

Write-Host "=== Ma Siu Backend Deployment ===" -ForegroundColor Cyan

# Navigate to project root
$projectRoot = Split-Path -Parent $PSScriptRoot
if ($projectRoot -eq "") { $projectRoot = "." }
Set-Location $projectRoot

if ($Down) {
    Write-Host "Stopping all services..." -ForegroundColor Yellow
    docker compose down
    Write-Host "All services stopped!" -ForegroundColor Green
    exit 0
}

if ($Build) {
    Write-Host "Building Docker images..." -ForegroundColor Yellow
    if ($Service) {
        docker compose build $Service
    } else {
        docker compose build
    }
    Write-Host "Build complete!" -ForegroundColor Green
}

if ($Up) {
    Write-Host "Starting services..." -ForegroundColor Yellow
    if ($Service) {
        docker compose up -d $Service
    } else {
        docker compose up -d
    }
    Write-Host ""
    Write-Host "Services started!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Service URLs:" -ForegroundColor Cyan
    Write-Host "  API Gateway:    http://localhost:5300" -ForegroundColor White
    Write-Host "  Admin Dashboard: http://localhost:3000" -ForegroundColor White
    Write-Host "  MongoDB:        localhost:27017" -ForegroundColor White
    Write-Host "  Redis:          localhost:6379" -ForegroundColor White
    Write-Host "  RabbitMQ:       http://localhost:15672 (admin/masiu_rmq_2026)" -ForegroundColor White
    Write-Host "  MinIO Console:  http://localhost:9001 (admin/masiu_minio_2026)" -ForegroundColor White
}

if ($Logs) {
    if ($Service) {
        docker compose logs -f $Service
    } else {
        docker compose logs -f
    }
}

if (-not $Build -and -not $Up -and -not $Down -and -not $Logs) {
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\deploy-backend.ps1 -Build           # Build all images"
    Write-Host "  .\deploy-backend.ps1 -Up              # Start all services"
    Write-Host "  .\deploy-backend.ps1 -Build -Up       # Build and start"
    Write-Host "  .\deploy-backend.ps1 -Down            # Stop all services"
    Write-Host "  .\deploy-backend.ps1 -Logs            # View logs"
    Write-Host "  .\deploy-backend.ps1 -Up -Service api-gateway  # Start specific service"
    Write-Host ""
}
