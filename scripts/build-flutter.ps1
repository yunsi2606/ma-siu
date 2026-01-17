# Build Flutter App
# Builds APK or IPA for release

param(
    [switch]$Apk,
    [switch]$Appbundle,
    [switch]$Ios,
    [string]$ApiUrl = "https://api.masiu.vn"
)

$ErrorActionPreference = "Stop"

Write-Host "=== Ma Siu Flutter App Build ===" -ForegroundColor Cyan

# Navigate to Flutter app
$flutterPath = Join-Path $PSScriptRoot "..\src\Apps\App.Client"
Set-Location $flutterPath

# Clean previous build
Write-Host "Cleaning previous build..." -ForegroundColor Yellow
flutter clean
flutter pub get

$dartDefine = "--dart-define=API_BASE_URL=$ApiUrl"

if ($Apk) {
    Write-Host "Building APK with API_BASE_URL=$ApiUrl..." -ForegroundColor Yellow
    flutter build apk --release $dartDefine
    
    $apkPath = "build\app\outputs\flutter-apk\app-release.apk"
    if (Test-Path $apkPath) {
        Write-Host ""
        Write-Host "APK built successfully!" -ForegroundColor Green
        Write-Host "Location: $apkPath" -ForegroundColor Cyan
        
        # Copy to project root for easy access
        $destPath = Join-Path $PSScriptRoot "..\MaSiu-release.apk"
        Copy-Item $apkPath $destPath -Force
        Write-Host "Copied to: $destPath" -ForegroundColor Cyan
    }
}

if ($Appbundle) {
    Write-Host "Building App Bundle with API_BASE_URL=$ApiUrl..." -ForegroundColor Yellow
    flutter build appbundle --release $dartDefine
    
    $aabPath = "build\app\outputs\bundle\release\app-release.aab"
    if (Test-Path $aabPath) {
        Write-Host ""
        Write-Host "App Bundle built successfully!" -ForegroundColor Green
        Write-Host "Location: $aabPath" -ForegroundColor Cyan
    }
}

if ($Ios) {
    Write-Host "Building iOS (requires macOS)..." -ForegroundColor Yellow
    flutter build ios --release $dartDefine
    Write-Host "iOS build complete. Open Xcode to archive and upload." -ForegroundColor Green
}

if (-not $Apk -and -not $Appbundle -and -not $Ios) {
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\build-flutter.ps1 -Apk                           # Build release APK"
    Write-Host "  .\build-flutter.ps1 -Appbundle                     # Build App Bundle for Play Store"
    Write-Host "  .\build-flutter.ps1 -Ios                           # Build iOS (macOS only)"
    Write-Host "  .\build-flutter.ps1 -Apk -ApiUrl 'https://xxx'     # Build with custom API URL"
    Write-Host ""
    Write-Host "For development with local backend:" -ForegroundColor Cyan
    Write-Host "  flutter run --dart-define=API_BASE_URL=http://192.168.x.x:5300"
    Write-Host ""
}
