#!/usr/bin/env bash
# Build script for creating self-contained executables for all platforms

set -e

echo "Building Workvivo CLI for all platforms..."

# Clean previous builds
echo "Cleaning previous builds..."
rm -rf dist
mkdir -p dist

# Function to build for a specific platform
build_platform() {
    local rid=$1
    local platform_name=$2

    echo ""
    echo "Building for $platform_name ($rid)..."
    dotnet publish src/WorkvivoCli.csproj \
        -c Release \
        -r $rid \
        --self-contained \
        -o "dist/$platform_name"

    echo "✓ Build complete: dist/$platform_name"
}

# Build for all platforms
build_platform "win-x64" "windows-x64"
build_platform "osx-x64" "macos-x64"
build_platform "osx-arm64" "macos-arm64"
build_platform "linux-x64" "linux-x64"
build_platform "linux-arm64" "linux-arm64"

echo ""
echo "========================================="
echo "All builds complete!"
echo "========================================="
echo ""
echo "Executables are in the dist/ directory:"
ls -lh dist/*/wv* | grep -v ".pdb"
