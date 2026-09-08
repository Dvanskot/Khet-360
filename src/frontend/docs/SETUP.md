# Khet-360 Frontend Setup Guide

## Prerequisites
- Node.js (v18 or higher)
- pnpm (v8 or higher) - **Required for workspace support**

## Installation

**IMPORTANT: Use pnpm, not npm, due to workspace dependencies**

```bash
# Install dependencies using pnpm
pnpm install
```

## Available Scripts

### Development
```bash
# Start all applications in parallel
pnpm run dev:all

# Start individual applications
pnpm run dev:tenant-erp
pnpm run dev:tenant-mobile
pnpm run dev:family-portal
pnpm run dev:vendor-hub
pnpm run dev:public-site
```

### Build
```bash
# Build all applications
pnpm run build:all

# Build individual applications
pnpm run build:tenant-erp
pnpm run build:tenant-mobile
pnpm run build:family-portal
pnpm run build:vendor-hub
pnpm run build:public-site
```

### Application URLs
When running with `dev:all`:
- Tenant-ERP: http://localhost:5173
- Tenant-Mobile: http://localhost:5174
- Family-Portal: http://localhost:5175
- Vendor-Hub: http://localhost:5176
- Public-Site: http://localhost:5177

## Troubleshooting

### "Unsupported URL Type workspace:" Error
This occurs when using `npm install` instead of `pnpm install`. The workspace protocol (`workspace:*`) is only supported by pnpm.

**Solution:** Always use `pnpm install` for this project.

### "No projects matched the filters" Error
This occurs when dependencies haven't been installed yet or when using npm instead of pnpm.

**Solution:**
1. Ensure you're using pnpm: `pnpm --version`
2. Install dependencies: `pnpm install`
3. Then run your desired script: `pnpm run dev:all`

## Quick Start
```bash
pnpm install
pnpm run dev:all
```
