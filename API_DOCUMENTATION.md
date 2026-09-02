# Khet-360 API Documentation

## 🚀 Overview
Khet-360 is a multi-tenant ERP designed for service-led businesses. The API is structured into two planes:
1. **Platform Control Plane**: Manages tenants, subscriptions, and global routing.
2. **Tenant Business Plane**: Handles the core business logic for a specific tenant.

### Authentication
The system uses dual-scheme JWT authentication:
- **PlatformJwt**: For administrative actions across the platform.
- **TenantJwt**: For business operations within a tenant context.

### Tenancy
Requests to the Tenant Plane must include a tenant identifier (typically via subdomain or a custom header) which is resolved by the `TenantResolverMiddleware`.

---

## 🛠️ API Reference

### 1. Platform Administration
| Controller | Endpoint | Description |
| :--- | :--- | :--- |
| `TenantAdminController` | `/api/platform/tenant-admin/*` | Manage tenant lifecycles, subscriptions, and entitlements. |
| `TenantController` | `/api/tenant/*` | Tenant-specific platform settings. |

### 2. Core CRM & Family Management
| Controller | Endpoint | Description |
| :--- | :--- | :--- |
| `CustomersController` | `/api/customers/*` | Manage individual and organizational customers. |
| `RelationshipsController` | `/api/relationships/*` | Manage family and business relationships. |
| `LeadsController` | `/api/leads/*` | Lead intake and qualification. |
| `OpportunitiesController` | `/api/opportunities/*` | Pipeline management for potential cases. |
| `ActivitiesController` | `/api/activities/*` | Task and interaction tracking. |

### 3. Funeral Case & Operational Workflow
| Controller | Endpoint | Description |
| :--- | :--- | :--- |
| `FuneralCasesController` | `/api/funeral-cases/*` | Core case management and milestone tracking. |
| `ServiceArrangementsController` | `/api/service-arrangements/*` | Detailed planning of funeral services. |
| `MortuaryController` | `/api/mortuary/*` | Slot management and mortuary operations. |
| `RepatriationController` | `/api/repatriation/*` | International transfer and repatriation logic. |

### 4. Logistics & Production
| Controller | Endpoint | Description |
| :--- | :--- | :--- |
| `FleetController` | `/api/fleet/*` | Vehicle tracking, maintenance, and trip assignments. |
| `ProductionController` | `/api/production/*` | Memorial manufacturing shop-floor management. |
| `InstallationController` | `/api/installation/*` | Field operations and sign-off workflows. |

### 5. Finance & Payments
| Controller | Endpoint | Description |
| :--- | :--- | :--- |
| `PaymentController` | `/api/payment/*` | Transaction processing and gateway integration. |
| `PaymentConfigurationController` | `/api/payment-config/*` | Tenant-specific gateway settings. |
| `FinanceVerificationController` | `/api/finance-verification/*` | Invariant checks (Debits = Credits). |
| `PoliciesController` | `/api/policies/*` | Insurance policy management. |
| `ClaimsController` | `/api/claims/*` | Claim processing and payout tracking. |

### 6. HR & People Management
| Controller | Endpoint | Description |
| :--- | :--- | :--- |
| `EmployeesController` | `/api/employees/*` | Employee profiles and contracts. |
| `LeaveController` | `/api/leave/*` | Leave applications and balances. |
| `PayrollController` | `/api/payroll/*` | Pay profiles and payroll run execution. |

### 7. Productivity & Intelligence
| Controller | Endpoint | Description |
| :--- | :--- | :--- |
| `DashboardController` | `/api/dashboard/*` | Widget-based operational overviews. |
| `ProductivityScorecardController` | `/api/productivity-scorecard/*` | KPI and performance metrics. |
| `IntelligenceController` | `/api/intelligence/*` | Advanced reporting and analytics. |
| `CommandPaletteController` | `/api/command-palette/*` | Quick-action system commands. |

### 8. System & Resilience
| Controller | Endpoint | Description |
| :--- | :--- | :--- |
| `BackupController` | `/api/backup/*` | Request and monitor tenant backups. |
| `MigrationController` | `/api/migration/*` | Manage tenant tier migrations. |
| `FeedbackController` | `/api/feedback/*` | Continuous improvement loop. |

---

## 🚦 Error Handling
The API uses standard HTTP status codes:
- `200 OK`: Request succeeded.
- `201 Created`: Resource created.
- `204 No Content`: Update successful.
- `400 Bad Request`: Validation failed (see response body for FluentValidation details).
- `401 Unauthorized`: Authentication missing or invalid.
- `403 Forbidden`: User lacks required permission (RBAC).
- `404 Not Found`: Resource not found.
- `500 Internal Server Error`: Unhandled exception.

## 📖 Developer Guide
### Request Pattern
All requests to the tenant plane must follow this pattern:
`https://{tenant-slug}.khet360.com/api/{controller}/{action}`

### Pagination
Collections are returned with a `PagedResult<T>` containing:
- `Items`: The list of results.
- `TotalCount`: Total items matching the query.
- `PageNumber` / `PageSize`: Current pagination state.
