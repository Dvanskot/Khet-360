#!/bin/bash

# Khet-360 Frontend Validation Script
# Ensures all frontend applications are properly configured

set -e

echo "🔍 Validating Khet-360 Frontend Applications"
echo "=========================================="

# Check Node.js version
node_version=$(node --version)
echo "✅ Node.js version: $node_version"

# Check if pnpm is available
if ! command -v pnpm &> /dev/null; then
    echo "❌ pnpm not found! Please install pnpm:"
    echo "   npm install -g pnpm"
    exit 1
fi
pnpm_version=$(pnpm --version)
echo("✅ pnpm version: $pnpm_version")

# Check project root
if [ ! -f "package.json" ]; then
    echo("❌ package.json not found in current directory")
    exit 1
fi
echo("✅ Project root verified")

# Check applications directory
if [ ! -d "apps" ]; then
    echo("❌ apps directory not found")
    exit 1
fi
echo("✅ Applications directory verified")

# Check each application
APPS=("tenant-erp" "tenant-mobile" "family-portal" "vendor-hub" "public-site")
for app in "${APPS[@]}"; do
    if [ ! -d "apps/$app" ]; then
        echo("❌ $app directory not found")
        exit 1
    fi
    
    if [ ! -f "apps/$app/package.json" ]; then
        echo("❌ $app/package.json not found")
        exit 1
    fi
    
    # Check for correct package name format
    if ! grep -q '"@khet360/'"$app"'"' "apps/$app/package.json"; then
        echo("❌ $app has incorrect package name format")
        exit 1
    fi
    
    echo("✅ Application $app directory verified")
done

# Check packages directory
if [ ! -d "packages" ]; then
    echo("❌ packages directory not found")
    exit 1
fi
echo("✅ Packages directory verified")

# Check shared packages
PACKAGES=("api-client" "ui-shared")
for pkg in "${PACKAGES[@]}"; do
    if [ ! -d "packages/$pkg" ]; then
        echo("❌ $pkg directory not found")
        exit 1
    fi
    
    if [ ! -f "packages/$pkg/package.json" ]; then
        echo("❌ $pkg/package.json not found")
        exit 1
    fi
    
    echo("✅ Shared package $pkg verified")
done

# Try to run a quick build test to verify workspace functionality
echo("")
echo("🧪 Testing workspace functionality...")
if pnpm ls > /dev/null 2>&1; then
    echo("✅ Workspace recognition working")
else
    echo("❌ Workspace recognition failed")
    echo("💡 Try running: pnpm install")
    exit 1
fi

echo("")
echo("🎉 All validations passed!")
echo("✅ Project structure is correct")
echo("✅ All applications are configured")
echo("✅ Workspaces protocol properly configured")
echo("")
echo("Next steps:")
echo("1. Run: pnpm install")
echo("2. Run: pnpm run dev:all to start all applications")
echo("3. Visit http://localhost:5173 for Tenant-ERP")
