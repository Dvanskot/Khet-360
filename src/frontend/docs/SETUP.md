# Khet-360 Frontend Applications - Setup and Deployment Guide

## Table of Contents
1. [Overview](#overview)
2. [System Requirements](#system-requirements)
3. [Installation](#installation)
4. [Configuration](#configuration)
5. [Running Applications](#running-applications)
6. [Building for Production](#building-for-production)
7. [Testing](#testing)
8. [Troubleshooting](#troubleshooting)
9. [Architecture Overview](#architecture-overview)
10. [API Endpoints](#api-endpoints)
11. [Real-time Features](#real-time-features)
12. [Security Considerations](#security-considerations)

## Overview

Khet-360 is a production-grade, multi-tenant ERP system designed specifically for funeral service businesses. The frontend consists of five specialized applications built with Vue 3, TypeScript, and Vite, providing a modern, responsive user experience with real-time capabilities.

## System Requirements

### Development Environment
- **Node.js**: >= 18.0.0 (LTS recommended)
- **npm**: >= 9.0.0
- **Git**: >= 2.30.0
- **Supported OS**: Windows 10+, macOS 12+, Ubuntu 20.04+

### Browser Support
- Chrome: >= 100
- Firefox: >= 100
- Safari: >= 14
- Edge: >= 100

### Backend Requirements
- Khet-360 Backend Server (ASP.NET Core 8.0)
- Microsoft SQL Server 2019+ or Azure SQL
- Redis (for SignalR backplane in production)
- SMTP Server (for email notifications)

## Installation

### Step 1: Clone the Repository
```bash
git clone https://github.com/Dvanskot/Khet-360.git
cd Khet-360
```

### Step 2: Install Dependencies
```bash
# Install all dependencies
npm install

# Verify installation
npm list
```

### Step 3: Configure Environment Variables
Create `.env` files in each application directory:

```bash
# Tenant-ERP
cp src/frontend/apps/tenant-erp/.env.example src/frontend/apps/tenant-erp/.env

# Tenant-Mobile
cp src/frontend/apps/tenant-mobile/.env.example src/frontend/apps/tenant-mobile/.env

# Family-Portal
cp src/frontend/apps/family-portal/.env.example src/frontend/apps/family-portal/.env

# Vendor-Hub
cp src/frontend/apps/vendor-hub/.env.example src/frontend/apps/vendor-hub/.env

# Public-Site
cp src/frontend/apps/public-site/.env.example src/frontend/apps/public-site/.env
```

### Step 4: Configure Environment Variables
Edit each `.env` file to match your backend configuration:

```
# Backend API Configuration
VITE_API_BASE_URL=http://your-backend-domain.com/api
VITE_SIGNALR_HUB_URL=http://your-backend-domain.com/hubs/notifications

# Application Specific Settings
VITE_APP_TITLE=Khet-360 Funeral ERP
VITE_ENABLE_OFFLINE=true  # Only for tenant-mobile
```

## Configuration

### Environment Variables Reference

| Variable | Description | Example Value |
|----------|-------------|---------------|
| `VITE_API_BASE_URL` | Base URL for all API requests | `https://api.khet360.com/api` |
| `VITE_SIGNALR_HUB_URL` | SignalR hub connection URL | `https://api.khet360.com/hubs/notifications` |
| `VITE_APP_TITLE` | Application title (browser tab) | `Khet-360 Funeral Services` |
| `VITE_ENABLE_OFFLINE` | Enable offline capabilities | `true` (tenant-mobile only) |
| `VITE_APP_VERSION` | Application version display | `1.0.0` |
| `VITE_FEATURE_FLAGS` | Comma-separated feature flags | `new_ui,advanced_reports` |

### Backend URL Formats

For local development:
```
VITE_API_BASE_URL=http://localhost:5000/api
VITE_SIGNALR_HUB_URL=http://localhost:5000/hubs/notifications
```

For production with HTTPS:
```
VITE_API_BASE_URL=https://api.khet360.com/api
VITE_SIGNALR_HUB_URL=https://api.khet360.com/hubs/notifications
```

For subdomain deployment:
```
VITE_API_BASE_URL=https://erp.khet360.com/api
VITE_SIGNALR_HUB_URL=https://erp.khet360.com/hubs/notifications
```

## Running Applications

### Development Mode

#### Start All Applications
```bash
npm run dev:all
```
This will start all five applications on different ports:
- Tenant-ERP: http://localhost:5173
- Tenant-Mobile: http://localhost:5174
- Family-Portal: http://localhost:5175
- Vendor-Hub: http://localhost:5176
- Public-Site: http://localhost:5177

#### Start Individual Applications
```bash
# Tenant-ERP (Core ERP)
npm run dev:tenant-erp

# Tenant-Mobile (Field Workforce)
npm run dev:tenant-mobile

# Family-Portal (Family Facing)
npm run dev:family-portal

# Vendor-Hub (Vendor Management)
npm run dev:vendor-hub

# Public-Site (Marketing Website)
npm run dev:public-site
```

### Production Mode

#### Build All Applications
```bash
npm run build:all
```

#### Build Individual Applications
```bash
npm run build:tenant-erp
npm run build:tenant-mobile
npm run build:family-portal
npm run build:vendor-hub
npm run build:public-site
```

#### Preview Production Build
```bash
npm run preview:all
```

#### Serve Production Build
After building, serve the `dist` folders with any static file server:
```bash
# Example using serve package
npm install -g serve
serve -s src/frontend/apps/tenant-erp/dist
```

## Testing

### Unit Tests
```bash
# Run all unit tests
npm run test

# Run tests for specific application
npm run test:tenant-erp
npm run test:tenant-mobile
npm run test:family-portal
npm run test:vendor-hub
npm run test:public-site

# Watch mode
npm run test:watch
```

### End-to-End Tests
```bash
# Install Playwright browsers (first time only)
npx playwright install

# Run all E2E tests
npm run test:e2e

# Run E2E tests for specific application
npm run test:e2e:tenant-erp
npm run test:e2e:tenant-mobile
npm run test:e2e:family-portal
npm run test:e2e:vendor-hub
npm run test:e2e:public-site
```

### Test Coverage
```bash
npm run test:coverage
# View coverage report: open coverage/index.html
```

## Building for Production

### Optimization Flags
The production build includes:
- JavaScript minification (esbuild)
- CSS minification
- HTML minification
- Asset fingerprinting for cache busting
- Tree shaking to remove unused code
- Code splitting for lazy loading

### Customizing Build
To customize the build, edit the `vite.config.ts` files in each application directory.

### Docker Deployment
For containerized deployment, use the provided Dockerfiles:
```bash
# Build tenant-erp Docker image
docker build -t khet360/tenant-erp -f src/frontend/apps/tenant-erp/Dockerfile .

# Run the container
docker run -p 80:80 khet360/tenant-erp
```

## Architecture Overview

### Frontend Applications
Each application is a standalone Vue 3 application with its own:
- Routing (Vue Router)
- State Management (Pinia)
- API Communication (Axios + custom services)
- Real-time Communication (SignalR)
- UI Components (Shared component library)

### Shared Packages
1. **@khet360/api-client** - Axios-based API client with interceptors
2. **@khet360/ui-shared** - Shared UI components (buttons, inputs, modals, etc.)

### Communication Patterns
- **REST APIs**: For CRUD operations
- **SignalR**: For real-time updates
- **Local Storage**: For user preferences and tokens
- **IndexedDB** (via Dexie): For offline data (tenant-mobile)

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh-token` - Token refresh
- `POST /api/auth/logout` - User logout

### Work Items
- `GET /api/work-items` - List work items
- `GET /api/work-items/{id}` - Get work item
- `POST /api/work-items` - Create work item
- `PUT /api/work-items/{id}` - Update work item
- `DELETE /api/work-items/{id}` - Delete work item
- `POST /api/work-items/{id}/complete` - Complete work item

### Leads
- `GET /api/leads` - List leads
- `GET /api/leads/{id}` - Get lead
- `POST /api/leads` - Create lead
- `PUT /api/leads/{id}` - Update lead
- `DELETE /api/leads/{id}` - Delete lead
- `POST /api/leads/{id}/convert` - Convert lead to case

### Products (Vendor Hub)
- `GET /api/vendor/products` - List products
- `GET /api/vendor/products/{id}` - Get product
- `POST /api/vendor/products` - Create product
- `PUT /api/vendor/products/{id}` - Update product
- `DELETE /api/vendor/products/{id}` - Delete product
- `GET /api/vendor/inventory` - Get inventory levels

### Family Portal
- `GET /api/family-portal/case/{caseId}/timeline` - Get case timeline
- `POST /api/family-portal/case/{caseId}/documents` - Upload document
- `POST /api/family-portal/case/{caseId}/payment` - Process payment

### Public Site
- `GET /api/public/statistics` - Get site statistics
- `GET /api/public/testimonials` - Get testimonials
- `GET /api/public/services` - Get services
- `POST /api/public/contact` - Submit contact form

## Real-time Features

### SignalR Hubs
All applications connect to the `/hubs/notifications` hub for real-time updates.

### Event Types
#### Work Item Events
- `WorkItemCreated` - New work item created
- `WorkItemUpdated` - Work item updated
- `WorkItemDeleted` - Work item deleted
- `WorkItemCompleted` - Work item marked as complete

#### Lead Events
- `LeadCreated` - New lead created
- `LeadUpdated` - Lead updated
- `LeadDeleted` - Lead deleted
- `LeadConverted` - Lead converted to case

#### Product Events
- `ProductCreated` - New product added
- `ProductUpdated` - Product updated
- `ProductDeleted` - Product removed
- `ProductOrdered` - Product ordered (stock decreased)
- `LowStockAlert` - Product stock below reorder level

#### Case Events (Family Portal)
- `CaseTimelineUpdated` - Case timeline updated
- `DocumentUploaded` - Document uploaded to case
- `PaymentMade` - Payment processed for case

#### General Events
- `StatisticsUpdated` - Site statistics updated
- `NewTestimonialAdded` - New testimonial added
- `ServiceUpdated` - Service information updated
- `NewContactSubmission` - New contact form submitted

### Connection Management
- Automatic reconnection with exponential backoff
- Heartbeat monitoring to detect disconnections
- Graceful handling of network interruptions
- Token refresh for maintaining authenticated connections

## Security Considerations

### Authentication
- JWT tokens stored in secure, HttpOnly cookies
- Automatic token renewal before expiration
- Short-lived access tokens (15 minutes)
- Refresh token rotation to prevent replay attacks

### Authorization
- Role-based access control (RBAC)
- Permission-based UI rendering
- Server-side authorization for all API endpoints
- Route guards for client-side navigation protection

### Data Protection
- HTTPS enforced in production
- Input validation and sanitization on client and server
- Output encoding to prevent XSS attacks
- CSRF protection for state-changing operations
- Rate limiting to prevent abuse

### Secure Communication
- SignalR connections use HTTPS/WSS in production
- JWT tokens passed via Authorization header
- Sensitive data never stored in localStorage
- Session timeout after inactivity

## Performance Optimization

### Loading Strategies
- Code splitting for lazy-loaded routes
- Prefetching of critical resources
- Efficient asset caching with fingerprints
- Critical CSS extraction

### Rendering Performance
- Virtual scrolling for large lists (where applicable)
- RequestAnimationFrame for animations
- CSS transform and opacity for GPU-accelerated animations
- Will-change property for expected animations

### Bundle Optimization
- Tree shaking to remove unused code
- Side effect elimination
- Module concatenation (scope hoisting)
- Common chunk extraction

## Monitoring and Logging

### Frontend Monitoring
- Error boundaries for graceful error handling
- Performance monitoring with Vue DevTools
- Custom analytics for user interactions
- Real-time connection status monitoring

### Logging
- Structured logging for debugging
- Error reporting to backend services
- User action tracking for analytics
- Performance metrics collection

## Troubleshooting

### Common Issues

#### 1. "Failed to load resource" errors
- Check that backend server is running
- Verify CORS configuration on backend
- Confirm API base URL in .env file
- Check network tab in browser dev tools

#### 2. SignalR connection failures
- Verify SignalR hub URL is correct
- Check that SignalR service is running on backend
- Confirm network connectivity to SignalR endpoint
- Check browser console for detailed error messages

#### 3. Authentication issues
- Clear browser cookies and localStorage
- Verify backend authentication service is running
- Check token expiration times
- Confirm username/password correctness

#### 4. Stale data display
- Hard refresh (Ctrl+F5 or Cmd+Shift+R)
- Clear browser cache for the domain
- Check SignalR connection status indicator
- Verify background sync is working (mobile)

### Diagnostic Commands
```bash
# Check Node.js version
node --version

# Check npm version
npm --version

# Check git version
git --version

# Verify port availability
npx kill-port 5173 5174 5175 5176 5177

# Test backend connectivity
curl -I http://localhost:5000/api/health
```

## Performance Benchmarks

### Target Metrics
- **First Contentful Paint**: < 1.5s
- **Time to Interactive**: < 3.0s
- **SignalR Connection Time**: < 1.0s
- **Initial Load Size**: < 2.0MB (gzipped)
- **Frame Rate**: 60fps for animations

### Optimization Techniques
- Route-based code splitting
- Image optimization and lazy loading
- Critical CSS inlining
- Font loading optimization
- Third-party script deferral

## Accessibility Compliance

### WCAG 2.1 AA
- Color contrast ratio >= 4.5:1 for normal text
- Color contrast ratio >= 3:1 for large text
- Keyboard navigable interface
- ARIA labels for interactive elements
- Focus visible indicators
- Skip navigation links
- Form label associations
- Error identification and suggestions

### Testing Tools
- axe-core automated testing
- Manual keyboard navigation testing
- Screen reader testing (NVDA, JAWS, VoiceOver)
- Color contrast analysis tools

## Internationalization

### Current Support
- English (en-US) - Primary language
- Date/time formatting based on user locale
- Number formatting based on user locale
- Currency formatting (ZAR, USD, EUR)

### Future Plans
- Full i18n framework integration
- Language selection dropdown
- Right-to-left (RTL) layout support
- Localized date/time formats
- Region-specific formatting

## Backup and Recovery

### Frontend Assets
- Version control via Git
- CDN caching with cache busting
- Fallback to previous versions
- Rollback capability via deployment scripts

### User Data
- Backend database backups
- Transaction log shipping
- Point-in-time recovery
- Disaster recovery site

## Legal and Compliance

### Data Protection
- POPIA (South Africa) compliant
- GDPR ready for EU expansion
- HIPAA considerations for health-related data
- PCI DSS for payment processing

### Accessibility Laws
- ADA Title III (US)
- Equality Act 2010 (UK)
- EN 301 549 (EU)

## Contact and Support

### Documentation
- This document: SETUP.md
- API Documentation: docs/api.md
- Development Guide: docs/development.md
- Deployment Guide: docs/deployment.md

### Support Channels
- GitHub Issues: https://github.com/Dvanskot/Khet-360/issues
- Email: support@khet360.com
- Phone: +27 11 123 4567 (Business hours)
- Community Forum: forum.khet360.com

### Emergency Support
- 24/7 critical support for production issues
- Dedicated account managers for enterprise clients
- Service Level Agreements (SLAs) available

---

*Last updated: September 8, 2026*
*Version: 1.0.0*
*© 2026 Khet-360. All rights reserved.*