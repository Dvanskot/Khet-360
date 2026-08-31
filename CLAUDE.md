# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview
Khet-360 is a production-grade, multi-tenant ERP designed for service-led businesses (initial focus: funeral services). It functions as a connected operating system where every business event has an owner, state, next action, financial consequence, and audit trail.

## Architecture
The system uses a layered tenancy model to ensure strict isolation:

### 1. Platform Control Plane
- **Responsibility:** Tenant registry, lifecycle, subscription plans, entitlements, and database routing.
- **Authority:** The platform decides whether a tenant may enter.
- **Database:** A single shared platform database.

### 2. Tenant Business Plane
- **Responsibility:** All business truth (CRM, Cases, Finance, HR, Inventory).
- **Authority:** The tenant database decides what that tenant's people may do.
- **Database:** One isolated SQL Server database per tenant, named `KhetLinQ_<tenant-slug>`.

### 3. Secrets Plane
- **Responsibility:** Secure storage of credentials (DB, WhatsApp, Email, Netcash).
- **Constraint:** Secrets are never stored in JWTs, logs, or browser storage.

### Core Architectural Rules
- **Mandatory Platform Gate:** A tenant business database connection must NOT be established before platform tenant validation succeeds.
- **Domain Authority:** Domain rules are authoritative; controllers and UI cannot bypass domain invariants.
- **Financial Immutability:** Posted accounting history is immutable and must always balance (Total Debits = Total Credits).
- **Explicit State Transitions:** Critical entities (Funeral Cases, Claims, POs, Subscriptions) must use explicit valid transitions.

## Technology Stack
### Backend
- **Runtime:** .NET 8
- **Architecture:** Clean Architecture / Modular Monolith with bounded contexts.
- **Data Access:** EF Core
- **Validation:** FluentValidation
- **Testing:** xUnit, FluentAssertions, Moq
- **Formatting:** `dotnet format`

### Frontend
- **Framework:** Vue 3 (Composition API)
- **Language:** TypeScript
- **Build Tool:** Vite
- **HTTP Client:** Axios
- **Validation:** Zod

## Common Commands
### Backend
- Build: `dotnet build`
- Test: `dotnet test`
- Format: `dotnet format`

### Frontend
- Install: `npm install`
- Development: `npm run dev`
- Build: `npm run build`

## Development Guidelines
- **Indentation:** 4-space indentation for C#.
- **Namespaces:** Use file-scoped namespaces.
- **API Design:** Use thin controllers; application handlers orchestrate use cases.
- **Domain Logic:** Keep domain rules in the domain/application boundary.
- **Validation:** All request models must be validated with FluentValidation (server) and Zod (client).
- **Auditability:** Every critical operation must record TenantId, UserId, BranchId, CorrelationId, Action, PreviousState, NewState, Reason, and TimestampUtc.
