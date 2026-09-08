# Khet-360 Frontend Applications

A modern, real-time ERP system for funeral service businesses built with Vue 3, TypeScript, and Vite.

## 📋 Overview

This repository contains the frontend applications for Khet-360, a production-grade, multi-tenant ERP designed for service-led businesses (initial focus: funeral services). The system functions as a connected operating system where every business event has an owner, state, next action, financial consequence, and audit trail.

## 🏗️ Architecture

The frontend consists of five specialized applications:

1. **Tenant-ERP** (`@khet360/tenant-erp`) - Core ERP application for funeral home operations
2. **Tenant-Mobile** (`@khet360/tenant-mobile`) - Mobile field workforce application with offline capabilities
3. **Family-Portal** (`@khet360/family-portal`) - Family-facing case status portal
4. **Vendor-Hub** (`@khet360/vendor-hub`) - Vendor product/inventory management system
5. **Public-Site** (`@khet360/public-site`) - Public information and marketing website

All applications share common UI components and API clients through workspace packages.

## 🛠️ Technology Stack

- **Framework**: Vue 3 (Composition API)
- **Language**: TypeScript
- **Build Tool**: Vite
- **State Management**: Pinia
- **Routing**: Vue Router
- **HTTP Client**: Axios
- **Real-time Communication**: SignalR (@microsoft/signalr)
- **UI Components**: Custom component library (@khet360/ui-shared)
- **API Client**: Shared API client (@khet360/api-client)
- **Offline Storage**: Dexie.js (for tenant-mobile)
- **Formatting**: Prettier, ESLint
- **Type Checking**: TypeScript

## 📦 Installation & Setup

### Prerequisites

- Node.js >= 18.0.0
- npm >= 9.0.0
- Git

### Backend Requirements

Ensure the Khet-360 backend is running and accessible at:
- API Base URL: `http://localhost:5000` (or configured via environment variables)
- SignalR Hub: `http://localhost:5000/hubs/notifications`

### Installation Steps

1. **Clone the repository**:
   ```bash
   git clone https://github.com/Dvanskot/Khet-360.git
   cd Khet-360
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Environment Configuration**:
   Create `.env` files in each application directory based on the `.env.example` files:
   ```bash
   # Example for tenant-erp
   cp src/frontend/apps/tenant-erp/.env.example src/frontend/apps/tenant-erp/.env
   ```

   Typical environment variables:
   ```
   VITE_API_BASE_URL=http://localhost:5000/api
   VITE_SIGNALR_HUB_URL=http://localhost:5000/hubs/notifications
   ```

4. **Install Playwright (for testing)**:
   ```bash
   npx playwright install
   ```

## 🚀 Running the Applications

### Development Mode

To run all applications in development mode:

```bash
# Start all applications concurrently
npm run dev:all
```

Or start individual applications:

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

### Production Build

To build all applications for production:

```bash
npm run build:all
```

Or build individual applications:

```bash
npm run build:tenant-erp
npm run build:tenant-mobile
npm run build:family-portal
npm run build:vendor-hub
npm run build:public-site
```

### Preview Production Build

To preview the production build locally:

```bash
npm run preview:all
```

Or preview individual applications:

```bash
npm run preview:tenant-erp
npm run preview:tenant-mobile
npm run preview:family-portal
npm run preview:vendor-hub
npm run preview:public-site
```

## 🧪 Testing

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
```

### End-to-End Tests

```bash
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
```

## 🔧 Configuration

### Environment Variables

Each application supports the following environment variables:

| Variable | Description | Default |
|----------|-------------|---------|
| `VITE_API_BASE_URL` | Base URL for API requests | `http://localhost:5000/api` |
| `VITE_SIGNALR_HUB_URL` | URL for SignalR hub connection | `http://localhost:5000/hubs/notifications` |
| `VITE_APP_TITLE` | Application title shown in browser tab | Application-specific |
| `VITE_ENABLE_OFFLINE` | Enable offline capabilities (tenant-mobile only) | `true` |

### Browser Support

- Chrome >= 100
- Firefox >= 100
- Safari >= 14
- Edge >= 100

## 📁 Project Structure

```
src/
├── frontend/
│   ├── apps/
│   │   ├── tenant-erp/          # Core ERP application
│   │   ├── tenant-mobile/       # Mobile field workforce app
│   │   ├── family-portal/       # Family-facing portal
│   │   ├── vendor-hub/          # Vendor management system
│   │   └── public-site/         # Public marketing website
│   ├── packages/
│   │   ├── api-client/          # Shared API client
│   │   └── ui-shared/           # Shared UI components
│   └── shared/                  # Shared utilities and types
```

## 🎨 Design System

The frontend applications follow a modern design system with:

- **Color Scheme**: Professional blues and grays with accent colors for status indicators
- **Typography**: Clean, readable fonts with proper hierarchy
- **Spacing**: Consistent 8px grid system
- **Border Radius**: 8px for cards, 4px for inputs and buttons
- **Shadows**: Subtle elevation for depth
- **Transitions**: Smooth 0.2s-0.3s transitions for all interactive elements
- **Animations**: Micro-interactions for buttons, cards, and form elements
- **Dark Mode**: CSS variables prepared for dark mode implementation
- **Responsive Design**: Mobile-first approach with breakpoints at 640px, 768px, 1024px, and 1280px

## 🌟 Key Features

### Real-time Capabilities
- SignalR-based real-time updates across all applications
- Automatic reconnection with exponential backoff
- Live data synchronization without manual refresh
- Connection status indicators in all applications

### Authentication & Security
- JWT-based authentication with automatic token refresh
- Secure token storage (HttpOnly cookies where possible)
- Role-based access control
- Protected routes and API endpoints

### Notification System
- Centralized notification service
- Real-time notifications via SignalR
- UI components for displaying notifications
- Notification history and read/unread status

### Offline Capabilities (Tenant-Mobile)
- Dexie.js for local storage
- Automatic synchronization when back online
- Conflict resolution strategies
- Queue-based offline operations

### Accessibility
- Semantic HTML structure
- Proper ARIA labels and roles
- Keyboard navigable interfaces
- Sufficient color contrast
- Focus management

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## 🙏 Acknowledgments

- Vue.js team for the amazing framework
- Vite team for the lightning-fast build tool
- TypeScript team for the type-safe JavaScript
- The SignalR team for real-time communication capabilities
- All open-source contributors whose work makes this possible

## 📞 Support

For support, please open an issue in the GitHub repository or contact the development team directly.

---

*Built with ❤️ for funeral service professionals who deserve the best technology to serve their communities.*