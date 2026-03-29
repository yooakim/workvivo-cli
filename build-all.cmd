@echo off
REM Build script for creating self-contained executables for all platforms

echo Building Workvivo CLI for all platforms...

REM Clean previous builds
echo Cleaning previous builds...
if exist dist rmdir /s /q dist
mkdir dist

echo.
echo Building for Windows x64 (win-x64)...
dotnet publish src\\workvivo-cli\\WorkvivoCli.csproj -c Release -r win-x64 --self-contained -o dist\windows-x64
if %errorlevel% neq 0 exit /b %errorlevel%
echo Build complete: dist\windows-x64

echo.
echo Building for macOS Intel (osx-x64)...
dotnet publish src\\workvivo-cli\\WorkvivoCli.csproj -c Release -r osx-x64 --self-contained -o dist\macos-x64
if %errorlevel% neq 0 exit /b %errorlevel%
echo Build complete: dist\macos-x64

echo.
echo Building for macOS ARM (osx-arm64)...
dotnet publish src\\workvivo-cli\\WorkvivoCli.csproj -c Release -r osx-arm64 --self-contained -o dist\macos-arm64
if %errorlevel% neq 0 exit /b %errorlevel%
echo Build complete: dist\macos-arm64

echo.
echo Building for Linux x64 (linux-x64)...
dotnet publish src\\workvivo-cli\\WorkvivoCli.csproj -c Release -r linux-x64 --self-contained -o dist\linux-x64
if %errorlevel% neq 0 exit /b %errorlevel%
echo Build complete: dist\linux-x64

echo.
echo Building for Linux ARM (linux-arm64)...
dotnet publish src\\workvivo-cli\\WorkvivoCli.csproj -c Release -r linux-arm64 --self-contained -o dist\linux-arm64
if %errorlevel% neq 0 exit /b %errorlevel%
echo Build complete: dist\linux-arm64

echo.
echo =========================================
echo All builds complete!
echo =========================================
echo.
echo Executables are in the dist\ directory
dir /s /b dist\wv.exe dist\wv 2>nul

exit /b 0
