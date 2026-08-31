# KhetLinQ Backend Implementation

## Scope

The backend uses a platform control-plane database and one isolated SQL Server database per tenant.

## Tenant database naming

Tenant database names are derived exclusively from the validated tenant slug:

`KhetLinQ_<tenant-slug>`

The API must never accept a tenant database name or raw connection string from a tenant registration request.

## API rules

- Platform endpoints operate against the platform database.
- Tenant endpoints resolve the tenant before constructing a tenant `DbContext`.
- Tenant identifiers supplied by clients must be validated against the authenticated tenant context.
- State-changing domain operations should be exposed as explicit commands rather than unrestricted status updates.
- All request models must be validated with FluentValidation.

## Testing

The test suite uses xUnit, FluentAssertions and Moq. Tests should cover validation, domain invariants, tenant routing, provisioning, authorization and important state transitions.

## Formatting

C# code should use four-space indentation, file-scoped namespaces, braces on their own lines, explicit accessibility for public APIs, and one logical statement per line. Repository code should be formatted with `dotnet format` before merge.
