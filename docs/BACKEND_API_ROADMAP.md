# Backend REST API coverage

The backend API is organized around platform control-plane resources and tenant business resources.

## Platform resources

- tenants
- subscription plans
- subscriptions
- invoices
- platform payments
- tenant database registrations
- platform audit
- support tickets
- platform users and roles

## Tenant resources

- organisation, branches, departments and cost centres
- tenant users, roles, permissions and approval authorities
- customers, contacts, families, leads, opportunities and CRM activities
- funeral cases, milestones and service arrangements
- policy products, policies, members, premium schedules and claims
- repatriation, mortuary admissions and custody records
- fleet, vehicles and assignments
- suppliers, stock locations, inventory and purchase orders
- sales orders, POS transactions and catering orders
- memorial products and orders
- employees, leave requests, payroll runs and payslips
- chart of accounts, journals, invoices, payments and allocations
- workflow definitions, instances and approval requests
- documents, communications and notifications
- work items and offline sync messages
- integrations and tenant audit

State transitions must be implemented as explicit commands where domain invariants apply. Generic CRUD must not be used to bypass workflow, approval, accounting or custody rules.
