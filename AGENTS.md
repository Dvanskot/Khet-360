# AGENTS.md — Khet-360

## Stack & Architecture
- .NET 10, clean architecture: `Domain` → `Application` → `Infrastructure` → `Api`
- Multi-tenant ERP: one **Platform** SQL Server DB + one isolated **Tenant** DB per tenant
- Background workers: RabbitMQ consumers, outbox/inbox, SLA escalation, backups, low-stock alerts

## Key Commands
```bash
# Build + test everything
dotnet build
dotnet test

# Single project
dotnet test src/Khet360.Tests/Khet360.Tests.csproj

# Format (required before merge)
dotnet format
```

## Critical Conventions
- **Tenant DB naming**: `KhetLinQ_<tenant-slug>` — never accept raw connection strings from client requests
- **Dual JWT**: `PlatformJwt` (admin) and `TenantJwt` (business ops); tenant resolver middleware runs **before** auth
- **C# style**: 4-space indent, file-scoped namespaces, braces on own lines, explicit accessibility, one statement per line
- **Validation**: All request DTOs require FluentValidation validators (global filter in `Program.cs`)
- **Tests**: xUnit + FluentAssertions + Moq; integration tests spin up in-memory tenant/platform DBs with mocked services

## Infrastructure
- **Local dev**: SQL Server, Redis, RabbitMQ, MinIO via `docker-compose.yml`
- **Env**: `.env` file (git-ignored) supplies infra creds; `appsettings.json` has default local connection strings
- **Reverse proxy**: Caddy (`Caddyfile`) routes `/api/*` → backend `:8080`, frontend → `:3000`
- **No CI/CD** or frontend exists yet

## Quirks & Gotchas
- `app.UseHttpMetrics()` must precede `app.UseMetricServer()` in `Program.cs` for HTTP metrics to be collected
- `ProductivityScorecardService` and `IntelligenceService` query an external Prometheus instance configured via `Prometheus:Url`
- `double.TryParse` in both `QueryPrometheus` methods uses `CultureInfo.InvariantCulture` — Prometheus always returns `.`-delimited floats
- `TenantDbContext` is resolved via `TenantDbContextFactory` per-request; do not register it as a singleton
- Background workers are registered in `Program.cs` — adding a new worker requires DI registration there
