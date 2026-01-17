# Start Development Environment
# Starts infrastructure services and opens dev URLs

param(
    [switch]$InfraOnly,
    [switch]$All
)

$ErrorActionPreference = "Stop"

Write-Host "=== Ma Siu Development Environment ===" -ForegroundColor Cyan

$projectRoot = Split-Path -Parent $PSScriptRoot
if ($projectRoot -eq "") { $projectRoot = "." }
Set-Location $projectRoot

if ($InfraOnly) {
    Write-Host "Starting infrastructure only (MongoDB, Redis, RabbitMQ, MinIO)..." -ForegroundColor Yellow
    docker compose up -d mongodb redis rabbitmq minio
    
    # Wait for services
    Start-Sleep -Seconds 5
    
    Write-Host ""
    Write-Host "Infrastructure ready!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Connection strings:" -ForegroundColor Cyan
    Write-Host "  MongoDB: mongodb://admin:masiu_dev_2026@localhost:27017" -ForegroundColor White
    Write-Host "  Redis:   localhost:6379,password=masiu_redis_2026" -ForegroundColor White
    Write-Host "  RabbitMQ: amqp://admin:masiu_rmq_2026@localhost:5672/masiu" -ForegroundColor White
    Write-Host "  MinIO:   http://localhost:9000 (admin/masiu_minio_2026)" -ForegroundColor White
    Write-Host ""
    Write-Host "Management UIs:" -ForegroundColor Cyan
    Write-Host "  RabbitMQ: http://localhost:15672" -ForegroundColor White
    Write-Host "  MinIO:    http://localhost:9001" -ForegroundColor White
    
} elseif ($All) {
    Write-Host "Starting ALL services..." -ForegroundColor Yellow
    docker compose up -d
    
    Start-Sleep -Seconds 10
    
    Write-Host ""
    Write-Host "All services started!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Application URLs:" -ForegroundColor Cyan
    Write-Host "  API Gateway: http://localhost:5300" -ForegroundColor White
    Write-Host "  Admin:       http://localhost:3000" -ForegroundColor White
    
} else {
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\dev-start.ps1 -InfraOnly   # Start only infrastructure (DB, cache, queue)"
    Write-Host "  .\dev-start.ps1 -All         # Start all services including apps"
    Write-Host ""
    Write-Host "Recommended workflow:" -ForegroundColor Cyan
    Write-Host "  1. .\dev-start.ps1 -InfraOnly"
    Write-Host "  2. Run .NET services from IDE (F5)"
    Write-Host "  3. cd src/Apps/app-admin && npm run dev"
    Write-Host "  4. cd src/Apps/App.Client && flutter run"
    Write-Host ""
}
