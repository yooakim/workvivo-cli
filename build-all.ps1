# Build script for creating self-contained executables for all platforms

Write-Host "Building Workvivo CLI for all platforms..." -ForegroundColor Green

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path "dist") {
    Remove-Item -Path "dist" -Recurse -Force
}
New-Item -ItemType Directory -Path "dist" -Force | Out-Null

# Function to build for a specific platform
function Build-Platform {
    param(
        [string]$Rid,
        [string]$PlatformName
    )

    Write-Host ""
    Write-Host "Building for $PlatformName ($Rid)..." -ForegroundColor Cyan
    dotnet publish src/WorkvivoCli.csproj `
        -c Release `
        -r $Rid `
        --self-contained `
        -o "dist/$PlatformName"

    Write-Host "✓ Build complete: dist/$PlatformName" -ForegroundColor Green
}

# Build for all platforms
Build-Platform -Rid "win-x64" -PlatformName "windows-x64"
Build-Platform -Rid "osx-x64" -PlatformName "macos-x64"
Build-Platform -Rid "osx-arm64" -PlatformName "macos-arm64"
Build-Platform -Rid "linux-x64" -PlatformName "linux-x64"
Build-Platform -Rid "linux-arm64" -PlatformName "linux-arm64"

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "All builds complete!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Executables are in the dist/ directory:" -ForegroundColor Yellow
Get-ChildItem -Path "dist\*\wv.exe", "dist\*\wv" -ErrorAction SilentlyContinue |
    Select-Object Directory, Name, @{Name="Size (MB)";Expression={[math]::Round($_.Length/1MB, 2)}} |
    Format-Table -AutoSize
