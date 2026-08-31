# Khet-360 ERP — Complete Rock-Solid ERP Master Specification

**Repository:** `Dvanskot/Khet-360-ERP`
**Branch:** `main`
**Authority:** This is the **single normative Khet-360 ERP document**. Product requirements, tenant architecture, domain functionality, security, integrations, UX, offline behaviour, testing and delivery requirements belong here unless explicitly superseded by an ADR in the repository.

---

## How to use this specification

This document is both the product contract and the delivery guide. A feature is not complete
because its screen exists: it must have an owner, state, next action, authorization rule,
financial effect where applicable, audit trail and recovery path. Build the productivity
foundation before adding more module screens.

The recommended implementation order is:

1. **Make work visible** with a unified work queue, ownership and exception states.
2. **Remove repeated effort** with templates, defaults, bulk actions and linked records.
3. **Guide the workflow** with checklists, next-best actions and safe automation.
4. **Measure the outcome** with operational productivity KPIs and user feedback.

---

## 1. Executive vision

Khet-360 is a production-grade, multi-tenant, multi-branch ERP designed to run an entire service-led business from customer acquisition through operational delivery, financial settlement, people management and long-term customer relationships. Its initial domain strength is funeral services, but the architecture is deliberately modular and reusable for related operational businesses.

Khet-360 must stand out because it is not merely a collection of screens. It must be a **single connected operating system for the business**: every important event has an owner, state, next action, financial consequence, audit trail and relationship to the rest of the business.

### Consumer promise

> **One business truth. One connected operating model. One exceptional experience.**

The product must be:

- rock-solid and financially trustworthy;
- secure and tenant-isolated;
- fast and intuitive;
- operationally intelligent;
- auditable and explainable;
- resilient to connectivity problems;
- configurable without becoming chaotic;
- self-service for subscription, billing and support;
- extensible without breaking existing tenants.

---

## 2. Product principles

1. **Business outcome over screen count.** Every feature must reduce work, risk, delay or uncertainty.
2. **One source of truth.** A business fact has one authoritative owner.
3. **Tenant isolation by architecture.** Each tenant has its own business database.
4. **Platform before tenant.** Tenant entitlement is validated before tenant business access.
5. **Domain rules are authoritative.** Controllers, UI and integrations cannot bypass domain invariants.
6. **Financial correctness is non-negotiable.** Posted accounting history is immutable and balanced.
7. **Critical actions are explainable.** State, actor, time, reason, approval and financial effect are discoverable.
8. **Offline is controlled.** Offline capability is permitted only where risk and conflict policy allow it.
9. **No silent destructive behaviour.** Downgrades, synchronization, migrations and lifecycle operations must never silently destroy business data.
10. **Least privilege everywhere.** Tenant users, platform staff, integrations and services receive only required access.
11. **Automation must be observable.** Background work has retries, idempotency, dead-letter handling and auditability.
12. **Configuration belongs to the owner.** Tenant business configuration remains tenant-owned; platform configuration remains platform-owned.

---

# 3. System boundaries and tenancy

Khet-360 utilizes a **"Shared Application, Isolated Data"** pattern. A single deployment of the frontend and backend serves all tenants, while each tenant's data is physically isolated.

## 3.1 Tenant Routing & Resolution
Tenant identification is driven by the URL subdomain (e.g., `tenanta.khet360.co.za`).
1. **Subdomain Resolution**: The system extracts the tenant slug from the host header.
2. **Platform Validation**: The resolver queries the Platform Control DB to verify the tenant's existence and status.
3. **Dynamic Configuration**: Upon validation, the system resolves the tenant's specific SQL Server connection string and external secrets (WhatsApp, Payment Gateway, etc.).
4. **Context Injection**: The resolved tenant context is injected into the request execution pipeline for the duration of the request.

## 3.2 Platform Control Plane
The Khet-360 platform database owns platform concerns:

- tenant registry;
- tenant lifecycle and status;
- subscription plans and pricing;
- subscriptions and entitlements;
- subscription invoices and platform payments;
- tenant database registry and provisioning metadata;
- platform users and platform roles;
- platform support tickets;
- platform billing and account service;
- platform audit;
- platform health and operational metadata.

It does **not** become a shared tenant ERP database.

## 3.3 Tenant Business Plane
Every tenant receives a dedicated database. It owns all tenant business truth:

- organisation and branches;
- tenant users, roles and permissions;
- customer/family relationships;
- CRM;
- funeral cases and service arrangements;
- policies, premiums and claims;
- repatriation and custody;
- mortuary;
- fleet;
- inventory and procurement;
- sales and POS;
- catering;
- memorials;
- HR, employees, leave and payroll;
- operational finance and accounting;
- workflows and approvals;
- documents and business communications;
- notifications;
- reporting/read models;
- tenant audit;
- offline outbox/inbox and synchronization state;
- tenant-specific integration configuration.

## 3.4 Secrets Plane
Actual secrets are stored in a secure secrets manager. Databases contain only references where needed.

Secret classes include:

- tenant database credentials;
- tenant Netcash credentials;
- tenant WhatsApp credentials;
- tenant email credentials;
- tenant storage/integration secrets;
- platform subscription-payment credentials.

Secrets are never placed in JWTs, URLs, browser storage or ordinary application logs.

---

# 4. Tenant authentication, routing and authorization

## 4.1 Mandatory login sequence

A tenant user MUST follow this sequence:

```text
Login / SSO request
  -> Extract tenant slug from URL (e.g. tenanta.khet360.co.za)
  -> establish trusted tenant identity
  -> query Platform Control DB
  -> tenant exists?
  -> tenant status permits access?
  -> subscription permits access?
  -> required feature entitlement permits access?
  -> resolve registered tenant database
  -> retrieve database credential securely
  -> connect to tenant database
  -> authenticate tenant user
  -> verify tenant user active
  -> load tenant roles/permissions
  -> load branch assignments
  -> load approval authority / segregation of duties
  -> create trusted execution context (Bind JWT to TenantId)
  -> authorize requested operation
  -> execute domain command/query
```

**The platform gate is mandatory. A normal tenant business database connection must not be established before platform tenant validation succeeds.**

The client can never provide or select a database name, server, connection string or credential.

## 4.2 Responsibility split

| Concern | Owner |
|---|---|
| Tenant exists | Platform |
| Tenant active/suspended | Platform |
| Subscription | Platform |
| Commercial entitlements | Platform |
| Tenant DB routing | Platform |
| Platform support/billing/account | Platform |
| Tenant users | Tenant DB |
| Tenant roles/permissions | Tenant DB |
| Branch assignments | Tenant DB |
| Approval authority | Tenant DB |
| Business authorization | Tenant/domain |
| Business transactions | Tenant DB |

Effective authorization is the intersection of platform access, subscription entitlement, tenant-user status, role, permission, branch scope, approval authority, segregation of duties and domain state.

## 4.3 Suspended tenant

Suspension blocks ordinary business transactions but permits a restricted platform experience for subscription recovery, billing, support, account servicing and approved export/recovery operations.

Tenant data is retained according to contractual and legal retention policies.

---

# 5. Tenant organisation and administration

Each tenant can configure:

- legal entity and trading names;
- registration and tax details;
- addresses and contacts;
- currencies and tax settings;
- fiscal year and accounting periods;
- branches and branch hierarchy;
- departments;
- cost centres;
- document numbering;
- invoice/receipt numbering;
- operational calendars;
- business hours and holidays;
- approval policies;
- service catalogues;
- pricing;
- notification preferences;
- branding;
- workflow rules;
- integration settings;
- data retention settings;
- role and permission policies.

Branch management supports branch status, address, contacts, operating hours, managers, cost centres, stock locations, tills, service areas and user assignments.

## 5.1 Productivity operating model

Khet-360 must provide one consistent way to turn business events into owned work. Every
actionable item, regardless of module, uses the same work model:

```text
Source record -> Work item -> Owner/team -> Due time/SLA -> Next action -> Outcome
```

A work item contains at least:

- source entity and deep link;
- branch and tenant scope;
- current state and allowed transitions;
- owner, team and escalation owner;
- priority, due time and SLA status;
- next recommended action;
- dependencies/blockers;
- completion reason and audit history.

The application shell provides:

- **My Work**: the signed-in user's due, overdue, blocked and recently assigned items;
- **Team Queue**: unassigned and team-owned work with safe claim/reassign actions.
- **Exceptions**: one queue for SLA breaches, missing information, conflicts, failed
  integrations and items awaiting approval.
- **Quick Capture**: create a lead, task, customer note or case draft without leaving the
  current workflow;
- persistent tenant, branch and date context so users do not repeatedly re-select scope.

### 5.1.1 SLA-Driven Intelligent Routing
The Team Queue is augmented by an intelligent routing engine:
- **SLA Templates**: Define target response and resolution times per case type.
- **Automated Routing**: High-priority "Exceptions" are automatically routed to the most available qualified manager based on real-time load and skill.
- **Escalation**: If a target time is breached, the system automatically escalates the work item to the next authority level.

### 5.1.2 Power-User Interface (Command Palette)
The application shell includes a global **Action-Oriented Command Palette** (`Ctrl+K`):
- **Fuzzy Search**: Search for navigation targets and direct actions using `fuse.js`.
- **Direct Mutation**: Allow power users to execute commands (e.g., `/create-lead [Name]`, `/assign-driver [ID]`) without leaving their current context.

Completing a work item should offer the next valid action, such as scheduling a follow-up,
requesting a missing document, allocating a payment or assigning the next task. Users must
be able to complete a task and create its follow-up in one transaction where the domain
allows it. No module may create a private task system that is invisible to My Work.

---

# 6. Customer / Family 360 and CRM

Khet-360 maintains a persistent customer relationship graph rather than treating every transaction as a disconnected record.

## 6.1 Customer management

Capabilities include:

- customer onboarding;
- individuals and organisations;
- contact details;
- identification and verification metadata;
- customer status;
- communication preferences;
- consent;
- addresses;
- notes and activities;
- documents;
- accounts and balances;
- quotations;
- orders;
- invoices;
- payments;
- policies;
- claims;
- funeral cases;
- memorial orders;
- support history where appropriate.

## 6.2 Family relationship graph

Relationships may include:

```text
Family
 ├── Customer / Payer
 ├── Spouse / Partner
 ├── Parent / Child
 ├── Next of Kin
 ├── Deceased
 ├── Policy Member
 ├── Beneficiary
 ├── Funeral Case
 └── Memorial
```

Relationship changes are auditable and do not overwrite historical relationships that were valid at the time of a transaction.

## 6.3 CRM

CRM includes leads, opportunities, activities, follow-ups, communication history, campaigns, customer segmentation, tasks, reminders, quotations, conversion tracking and service history.

The dashboard must surface next-best actions such as overdue follow-up, unpaid invoice, missing claim document, upcoming funeral milestone or unresolved customer issue.

Repeated CRM work must be accelerated with:

- reusable lead, customer and communication templates;
- automatic deduplication suggestions before creating a customer;
- one-click logging of a call, visit, message or outcome;
- follow-up sequences with explicit consent, pause and opt-out controls;
- a customer timeline that links every interaction, quote, case, invoice and support issue;
- bulk assignment and bulk status changes with a preview and audit record.

## 6.4 The Family Experience Portal
To remove friction from document chasing, Khet-360 provides a secure, external-facing portal for the Next-of-Kin:
- **Self-Service Upload**: Families can upload required death certificates and ID documents directly.
- **Progress Timeline**: A read-only view of the funeral service milestones.
- **Payments**: Integrated portal payments for balances.
- **Security**: Access is restricted to the specific family/case via a secure invitation token.

---

# 7. Funeral Operations 360

Funeral management is a connected workflow, not a single record.

## 7.1 Funeral case

A case can contain:

- case number;
- deceased details;
- next of kin;
- payer;
- policy and claim references;
- death notification;
- cause/death documentation metadata;
- service package;
- venue;
- date/time milestones;
- burial/cremation information;
- transport;
- mortuary;
- repatriation;
- catering;
- flowers/merchandise;
- memorial product;
- assigned staff;
- tasks;
- approvals;
- documents;
- communications;
- charges;
- payments;
- accounting entries;
- customer feedback.

## 7.2 Funeral workflow

```text
Enquiry
 -> Case Opened
 -> Verification
 -> Arrangement
 -> Policy/Claim Check
 -> Repatriation/Mortuary if required
 -> Service Planning
 -> Resource Assignment
 -> Service Delivery
 -> Burial/Cremation
 -> Settlement
 -> Memorial Follow-up
 -> Case Closure
```

Every milestone is timestamped and attributable.

## 7.3 Service arrangement

Users can build packages from configurable services/products, quantities, prices, discounts, taxes, optional items and approvals. Changes after confirmation must preserve an audit trail and financial history.

### 7.3.1 Guided Arrangement Wizards
To reduce errors and training time, arrangements are handled via dynamic, branch-specific wizards:
- **Adaptive Steppers**: The wizard adapts fields based on the selected service package.
- **Atomic Saves**: Data is saved as a "Draft" at each step to prevent loss on refresh.
- **Step-Level Validation**: "Next" buttons are disabled until `FluentValidation` rules for the current step are satisfied.

## 7.4 Funeral command centre

Provide operational dashboards for:

- today's services;
- upcoming services;
- delayed cases;
- missing documents;
- unassigned tasks;
- vehicle/resource conflicts;
- mortuary status;
- outstanding payments;
- claims awaiting action;
- SLA breaches;
- high-risk exceptions.

### 7.4.1 Single-Screen Deal Boards
The command centre utilizes "Deal Boards" to minimize page loads:
- **Kanban Layout**: Visual representation of cases moving through the workflow.
- **Slide-Over Details**: Clicking a case opens a detail drawer (notes, docs, history) without leaving the board.
- **Real-time Sync**: Use SignalR to push board state changes across all active users instantly.

The command centre must also accelerate execution, not only report status. It provides:

- case templates for common service types and branch-specific checklists;
- package templates that create the required operational tasks, documents and approvals;
- a single readiness indicator showing missing information, unresolved conflicts and
  blocked dependencies;
- drag-free assignment tools for staff, vehicles, venues and mortuary capacity;
- conflict detection before confirmation for time, resource, branch and custody clashes;
- bulk actions for safe, same-state work such as assigning, reminding and requesting
  documents;
- a customer-facing progress summary generated from the authoritative case state.

Templates are versioned. Existing cases retain the version used at creation, while
administrators can publish a new version for future cases.

---

# 8. Policy administration

Policy functionality includes:

- products/plans;
- policy creation;
- policy numbers;
- members;
- beneficiaries;
- dependants;
- premium schedules;
- payment status;
- waiting periods;
- exclusions;
- cover limits;
- amendments;
- reinstatements;
- cancellations;
- lapses;
- policy documents;
- claims linkage;
- customer communication.

Policy state transitions are controlled and auditable. A claim cannot bypass policy eligibility and configured approval rules.

---

# 9. Claims management

Claims support:

1. claim initiation;
2. member/policy verification;
3. required-document checklist;
4. document receipt;
5. validation;
6. assessment;
7. reserve/financial impact where applicable;
8. approval/rejection;
9. payment instruction;
10. reconciliation;
11. customer communication;
12. closure and audit.

Claim states must be explicit and invalid transitions rejected.

High-risk claim approval remains online-only and requires appropriate authority.

---

# 10. Repatriation, mortuary and custody

## 10.1 Repatriation

Support:

- collection requests;
- origin/destination;
- route planning;
- dispatch;
- vehicle and driver assignment;
- collection confirmation;
- custody handover;
- border/document requirements where applicable;
- arrival;
- final handover;
- exception management;
- cost tracking.

## 10.2 Mortuary

Support:

- admission;
- identity verification;
- location/slot assignment;
- custody records;
- movement history;
- release authorization;
- release confirmation;
- capacity dashboard;
- environmental/operational checks;
- audit history.

Custody history is append-only. Release requires the configured authority and cannot be silently reversed.

---

# 11. Fleet management

Fleet capabilities include:

- vehicle register;
- vehicle types;
- ownership/lease metadata;
- registration/licensing metadata;
- driver register;
- driver authorization;
- inspections;
- maintenance schedules;
- maintenance work orders;
- fuel tracking;
- mileage/odometer;
- dispatch;
- trip management;
- vehicle availability;
- accident/incident records;
- document expiry alerts;
- branch assignment;
- cost reporting.

Dispatch validates vehicle state, driver eligibility, branch scope and conflicting assignments.

---

# 12. Inventory and procurement

## 12.1 Inventory

Support:

- item master;
- SKU/barcode;
- categories;
- units of measure;
- variants;
- images/thumbnails;
- suppliers;
- warehouses;
- branch stock locations;
- minimum/maximum levels;
- reorder points;
- stock on hand;
- reserved stock;
- available stock;
- stock movements;
- adjustments;
- transfers;
- stock counts;
- consumption;
- damaged/expired stock;
- batch/serial tracking where required;
- valuation;
- inventory reports.

Stock movement history is authoritative. Available stock must be derived consistently from movements, reservations and adjustments.

## 12.2 Stock transfer

A transfer wizard supports:

```text
Draft
 -> Requested
 -> Approved
 -> Picked
 -> In Transit
 -> Received
 -> Reconciled
```

Source and destination branches must be shown by name, not opaque identifiers. Variances require explicit resolution.

## 12.3 Procurement

Support supplier register, requests, quotations, purchase orders, approvals, goods received, supplier invoices, returns, supplier payments and purchasing reports.

### 12.3.1 Vendor Collaboration Hub
To integrate the supply chain, Khet-360 provides a secure, limited-access interface for external suppliers:
- **Task Confirmation**: Vendors can mark delivered services (e.g., catering delivered) as "Complete".
- **Direct Invoicing**: Vendors can upload invoices directly against a Purchase Order.
- **Identity**: Vendors are authenticated via secure invitation tokens and linked to the tenant they serve.

---

# 13. Sales, quotations and POS

## 13.1 Sales

Support:

- leads/opportunities;
- quotations;
- configurable price lists;
- discounts;
- sales orders;
- fulfilment;
- invoices;
- receipts;
- returns/credit notes;
- customer balances;
- sales reports.

## 13.2 POS

POS supports tills, cashier sessions, barcode scanning, item search, customer selection, cart, discounts, taxes, payments, receipts, refunds subject to authority, cash-up and reconciliation.

Every POS transaction has a server-side idempotency key/client transaction identifier. Duplicate submissions must not create duplicate financial transactions.

Offline POS is permitted only within configured risk limits and requires durable local outbox and later reconciliation.

---

# 14. Catering

Catering management includes:

- menus;
- recipes/items;
- packages;
- bookings;
- service date/time;
- venue;
- headcount;
- dietary requirements;
- customer/order linkage;
- inventory consumption;
- staff assignment;
- preparation tasks;
- delivery/setup;
- completion;
- costing;
- billing.

A catering booking can be linked directly to a funeral case and service schedule.

---

# 15. Memorial management

Memorial functionality includes:

- memorial product catalogue;
- quotation/order;
- design brief;
- artwork/design versioning;
- customer approval;
- production;
- quality control;
- delivery;
- installation;
- cemetery/grave metadata;
- maintenance/follow-up;
- photographs/documents;
- customer communication.

## 15.1 In-House Manufacturing & Production
For providers who manufacture tombstones in-house, Khet-360 provides a full production shop floor management system:

- **Raw Material Tracking**: Integration with Inventory to track stone slabs, polishing compounds, and engraving materials.
- **Production Lifecycle**: A detailed manufacturing state machine:
  ```text
  Order Confirmed -> Slab Selection -> Cutting/Shaping -> Polishing -> Engraving -> Finishing -> Quality Check -> Ready for Delivery
  ```
- **Shop Floor Management**:
  - Assignment of specific artisans/craftsmen to production stages.
  - Time-tracking per stage for cost analysis.
  - Production queue management to prevent bottlenecks.
- **Quality Gates**: Mandatory sign-offs after Engraving and Finishing before the item is marked "Ready".

## 15.2 Installation Workflow
Tombstone installation is treated as a field operation:
- **Installation Scheduling**: Integration with the Fleet module to assign vehicles and crews.
- **Site Readiness Check**: Documentation and verification of cemetery plot readiness.
- **Installation Log**: Record of installation date, crew, and photos of the completed work.
- **Customer Sign-off**: Digital confirmation of installation quality.

## 15.3 Memorial Lifecycle
The general memorial lifecycle is:

```text
Order
 -> Design
 -> Customer Approval
 -> Production (In-house or Outsourced)
 -> Quality Check
 -> Delivery
 -> Installation
 -> Historical Record
```

Customer approval and design versions are retained.

---

# 16. HR, employee management, leave and payroll

## 16.1 Employee management

Support employee profiles, employment status, contracts, departments, positions, branch assignments, reporting lines, qualifications, documents, emergency contacts and lifecycle events.

## 16.2 Leave

Support:

- leave types;
- accrual policies;
- opening balances;
- employee balances;
- applications;
- approval workflow;
- cancellation of pending applications;
- calendars;
- overlapping-leave checks;
- public holidays;
- adjustments;
- audit history;
- reporting.

Leave balances are initialized according to employee type and configured policy; balances are never silently overwritten.

## 16.3 Payroll

Payroll supports employee pay profiles, earnings, deductions, statutory/configurable deductions, allowances, overtime inputs, leave inputs, payroll periods, review, approval, payslips, payroll reports and accounting integration.

Payroll runs are versioned and auditable. A finalized run is not edited in place; corrections use controlled adjustment/reversal processes.

---

# 17. Finance and accounting

Finance is the tenant's financial source of truth.

## 17.1 Core accounting

Support:

- chart of accounts;
- fiscal periods;
- journals;
- journal lines;
- general ledger;
- accounts receivable;
- accounts payable;
- customer statements;
- supplier statements;
- invoices;
- credit notes;
- receipts;
- payments;
- allocations;
- bank accounts;
- reconciliation;
- tax/VAT configuration;
- cost centres;
- branches;
- budgets;
- financial reporting.

Every posted journal satisfies:

```text
Total Debits = Total Credits
```

Posted financial history is immutable. Corrections use explicit reversals/adjustments.

## 17.2 Payment lifecycle

```text
Payment Initiated
 -> Provider Interaction
 -> Provider Verification
 -> Payment Recorded
 -> Allocation
 -> Settlement
 -> Accounting
 -> Reconciliation
```

Payment operations are idempotent and replay-safe.

## 17.3 Netcash

Netcash is the initial financial operations provider. Provider-specific code is isolated behind application abstractions/adapters.

Tenant Netcash configuration belongs to the tenant. Platform subscription-payment Netcash configuration belongs to the platform secret namespace. These credentials must never be mixed.

Supported platform/tenant operations may include Pay Now, payment status, notifications, inbound events, NIF/debit-order capabilities where commercially and technically configured.

Provider callbacks/webhooks must be authenticated, validated, idempotent, correlated and audited.

---

# 18. Workflow, approvals, SLA and automation

Khet-360 needs a general workflow engine capable of:

- state machines;
- configurable workflow definitions;
- workflow versioning;
- tasks;
- assignments;
- approvals;
- rejection;
- escalation;
- timers;
- SLA targets;
- reminders;
- retries;
- dead-letter handling;
- replay;
- event triggers;
- conditional rules;
- audit history.

Critical actions must support approval limits and segregation of duties.

Examples:

```text
Purchase Order -> Approval -> Release
Claim -> Assessment -> Approval -> Settlement
Leave -> Manager Approval -> HR Review
Refund -> Supervisor Approval -> Finance Posting
Mortuary Release -> Authorized Release -> Handover
```

No workflow may bypass domain validation.

## 18.1 Automation guardrails

Automation is used to remove repetitive work, not to hide decisions. Each automated action
must declare its trigger, actor (`system`), rule/version, affected records, notification
behaviour and rollback or retry path.

Automation must:

- be idempotent and safe to replay;
- create visible work items when it cannot finish automatically;
- respect branch scope, business hours, quiet hours and user notification preferences;
- never auto-approve a high-risk, financial or custody action;
- expose a human-readable explanation in the activity history;
- support pause, disable and test/simulation modes for administrators;
- record failures in an actionable exception queue rather than silently retrying forever.

High-volume repeated operations should support a dry-run preview before execution, including
the number of records, skipped records and expected financial or customer impact.

---

# 19. Documents and records

Document management supports:

- secure metadata;
- document categories;
- versioning;
- ownership;
- linkage to business entities;
- upload/download authorization;
- retention;
- archival;
- access audit;
- virus/content validation where infrastructure supports it;
- signed/approved document status.

Binary storage is separate from relational business data where appropriate; the tenant retains ownership and isolation of its documents.

---

# 20. Communications and integrations

Tenant-configurable integrations include:

### WhatsApp

- provider configuration;
- sender/account references;
- message templates;
- outbound messages;
- delivery status;
- inbound messages where supported;
- conversation linkage;
- consent and opt-out;
- retry and failure handling.

### Email

- provider configuration;
- sender profiles;
- templates;
- notifications;
- delivery status;
- retry;
- audit.

### Notifications

Email, WhatsApp, SMS/push/portal channels are abstracted so providers can change without rewriting domain logic.

Tenant integration configuration remains tenant-owned; credentials are held by the secure secrets manager.

---

# 21. Reporting and management intelligence

Dashboards should include:

### Executive

- revenue;
- profitability;
- cash position;
- receivables;
- sales pipeline;
- operational volume;
- service performance;
- branch performance.

### Funeral operations

- active cases;
- cases by stage;
- services today;
- delayed milestones;
- SLA performance;
- mortuary occupancy;
- repatriation status;
- fleet utilization.

### Finance

- income statement;
- balance sheet;
- cash flow;
- AR aging;
- AP aging;
- bank reconciliation;
- payment exceptions;
- tax reports.

### Inventory

- stock on hand;
- low stock;
- stock valuation;
- transfers;
- adjustments;
- consumption;
- shrinkage.

### HR

- headcount;
- leave balances;
- absenteeism;
- payroll summary;
- employee lifecycle.

Reports must use governed read models and must not compromise transactional consistency.

---

# 22. Support Centre

Every tenant receives a Khet-360 Support Centre.

## Support categories

### Support

Technical issues, application behaviour, integrations, performance, offline/synchronization, user assistance and incidents.

### Billing

Invoices, subscription payments, payment failures, credits, refunds, renewals, pricing and upgrade/downgrade questions.

### Account

Company details, account ownership, administrator changes, verification, account lifecycle, data export and closure.

## Ticket lifecycle

```text
Draft
 -> Submitted
 -> Triaged
 -> Assigned
 -> In Progress
 -> Waiting for Customer
 -> Waiting for Platform
 -> Resolved
 -> Closed
```

Tenant users can create, view, reply to and close/reopen tickets subject to permissions.

Tickets support priority, SLA, attachments, comments, internal notes, team assignment, assignee, status history and resolution.

Platform agents receive minimum necessary tenant context. Support access is not a blanket permission to browse tenant ERP data.

---

# 23. Subscription, billing and tenant self-service

Khet-360 provides first-class platform subscription management.

## 23.1 Plan catalogue

A plan may define:

- price;
- billing frequency;
- currency;
- included modules;
- user limits;
- branch limits;
- storage limits;
- transaction limits;
- feature entitlements;
- support tier;
- usage limits;
- add-ons.

## 23.2 Tenant subscription

Tenants can:

- view current plan;
- compare plans;
- view entitlements;
- view invoices;
- make subscription payments;
- view receipts;
- retry eligible failed payments;
- upgrade;
- downgrade;
- schedule changes;
- preview prorated charges/credits;
- review subscription history;
- contact Billing.

## 23.3 Upgrade/downgrade safety

Downgrades must evaluate limits and dependencies before confirmation. Khet-360 must never silently delete branches, users, transactions, documents or other business data because of a plan change.

Example:

```text
Professional -> Starter
Current active branches: 17
Starter limit: 5

Result:
Explain conflict
Allow remediation or scheduled downgrade
Never silently delete 12 branches
```

## 23.4 Platform subscription payments

Subscription payments are platform transactions, not tenant operational accounting transactions.

Lifecycle:

```text
Invoice Issued
 -> Payment Initiated
 -> Payment Submitted
 -> Provider Confirmation
 -> Payment Verified
 -> Payment Allocated
 -> Invoice Paid
 -> Entitlements Updated
 -> Receipt Issued
 -> Reconciled
```

All payment references are idempotent. Provider callbacks are replay-safe.

---

# 24. Subscription lifecycle and access enforcement

```text
Trial
 -> Provisioning
 -> Active
 -> PaymentDue
 -> GracePeriod
 -> Suspended
 -> ReadOnly (policy dependent)
 -> CancellationPending
 -> Terminated
 -> Archived
```

The platform is authoritative for this lifecycle.

Suspension does not delete tenant data.

---

# 25. Offline-first and synchronization

Offline capability is explicit per operation.

## Offline-safe examples

- selected POS operations;
- drafts;
- selected operational checklists;
- selected data capture tasks.

## Online-only examples

- payment authorization/verification;
- claim approval;
- privileged mortuary release;
- debit-order activation;
- other high-risk operations requiring authoritative state.

Offline commands carry:

```text
ClientTransactionId
DeviceId
UserId
TenantId
BranchId
CreatedAtUtc
CommandType
Payload
SchemaVersion
```

Synchronization requires:

- durable outbox;
- inbox/idempotency handling;
- retries;
- conflict classification;
- deterministic resolution;
- user-visible conflicts where required;
- audit;
- dead-letter handling;
- replay.

Financial, custody and other critical histories must never use blind last-write-wins.

---

# 26. UX and application shell

The web application should present a coherent ERP experience with:

- left navigation grouped by business area;
- role/permission-aware menu items;
- responsive dashboards;
- entity workbenches;
- cards for visual stock/customer summaries where useful;
- tables for transactional history;
- modal forms for quick creation where appropriate;
- labels above inputs;
- real-time Zod validation on the frontend;
- FluentValidation on the server;
- clear error messages;
- toast notifications for asynchronous success/failure;
- confirmation dialogs for destructive/financial actions;
- consistent loading states;
- offline/outbox status visibility;
- conflict-resolution UI;
- receipt printing;
- barcode scanning support;
- accessible keyboard navigation where practical.

Primary save/confirmation actions use the Khet-360 ERP design token `--erp-teal` / `#08AFAF`.

The interface should make **next action, state, owner and exception** immediately visible.

## 26.1 Productivity interaction requirements

To reduce context switching and duplicate data entry, the shell must include:

- global search across permitted customers, cases, policies, tasks, documents and payments;
- a command palette for navigation and frequent actions;
- recent records and recently used actions;
- saved table views with filters, columns, sort order and branch scope;
- keyboard shortcuts for search, save, cancel, next record and open My Work;
- bulk actions with permission checks, selection counts, validation preview and per-record
  results;
- autosaved drafts with clear draft/posted status and recovery after a refresh or reconnect;
- sensible defaults from branch, customer history and templates, always editable and
  displayed before confirmation;
- inline editing only where the domain permits it; financial and historical records remain
  explicitly controlled;
- progressive disclosure so routine users see the common path while advanced options remain
  available;
- accessible focus order, visible focus indicators, readable contrast, screen-reader labels,
  keyboard alternatives to drag interactions and a responsive layout for field work.

Every list should make the primary action available from the row, preserve the user's
filters after returning from a detail page and show why a record is blocked. Loading,
empty, offline, conflict and permission-denied states must each explain what the user can
do next.

---

# 27. Security model

Security is layered:

```text
Identity
 -> Platform tenant gate
 -> Subscription entitlement
 -> Tenant authentication
 -> Tenant RBAC
 -> Branch scope
 -> Approval authority
 -> Domain authorization
 -> Validation
 -> Transaction
 -> Audit
```

Requirements:

- no client-controlled tenant routing;
- no cross-tenant database access;
- no secrets in tokens/logs/client storage;
- encrypted transport;
- encryption at rest;
- least privilege;
- secure password/identity handling;
- secure document access;
- sensitive-data redaction;
- immutable critical audit events;
- segregation of duties;
- rate limiting and abuse controls;
- secure webhook validation;
- idempotency for payment and financial commands;
- platform staff access is purpose-bound and audited.

**Identity binding**: All JWTs must be cryptographically bound to a specific `TenantId`. Any request where the resolved tenant (from subdomain) does not match the JWT's `TenantId` must be rejected.

---

# 28. Audit and observability

Every critical operation records, where applicable:

```text
TenantId
UserId
BranchId
CorrelationId
CommandId
EntityId
Action
PreviousState
NewState
Reason
Approval
TimestampUtc
Source
```

Observability includes:

- structured logs;
- metrics;
- traces;
- health checks;
- readiness checks;
- dependency health;
- background-job status;
- queue depth;
- synchronization failures;
- payment exceptions;
- database health;
- tenant provisioning status;
- security events.

Logs must never leak secrets or unnecessary sensitive personal/business information.

---

# 29. Data lifecycle, backup and disaster recovery

Because each tenant has a dedicated business database, Khet-360 must support:

- per-tenant backups;
- point-in-time recovery where infrastructure supports it;
- per-tenant restore;
- tenant export;
- tenant archival;
- controlled tenant termination;
- database health monitoring;
- migration tracking;
- disaster-recovery exercises.

The platform database also requires high availability, backup, recovery and monitoring because it is the tenant-routing and subscription authority.

---

# 30. Tenant provisioning

Provisioning must be automated, idempotent and auditable:

```text
Create Tenant
 -> Create Subscription
 -> Provision Tenant DB
 -> Create DB Principal
 -> Create Secret Reference
 -> Apply Tenant Migrations
 -> Seed Organisation
 -> Seed Roles/Permissions
 -> Create Tenant Administrator Identity Binding
 -> Seed Configuration
 -> Health Check
 -> Mark Active
```

Failure at any step must be recoverable without duplicate tenants/databases.

The ordinary web request path must not perform uncontrolled schema migrations.

---

# 31. Architecture and technology baseline

Khet-360 uses a **Shared Application, Isolated Data** deployment model. A single deployment of the API and Frontend serves all tenants, with physical database isolation per tenant.

## 31.1 Backend
- .NET 8;
- Clean Architecture;
- modular monolith with bounded contexts;
- EF Core;
- dependency inversion;
- FluentValidation;
- transactional outbox/inbox;
- provider adapters;
- secure configuration;
- structured logging;
- health/readiness endpoints;
- resilient background processing.

## 31.2 Frontend
- Vue 3;
- TypeScript;
- Vite;
- Axios;
- Zod;
- offline-capable architecture;
- role-aware routing and navigation.

## 31.3 OSS Infrastructure Stack
To ensure high performance with zero licensing costs, the following tools are utilized:
- **Caching**: Redis (OSS) for session state and SLA timers.
- **Messaging**: RabbitMQ (OSS) for async routing and notifications.
- **Object Storage**: MinIO (OSS) for S3-compatible tenant document storage.
- **Observability**: Prometheus & Grafana (OSS) for system and SLA health.
- **Network/SSL**: Caddy (OSS) for automatic HTTPS and routing.
- **Orchestration**: Docker & Docker Compose.
- **CI/CD**: GitHub Actions.

## 31.4 Testing
- xUnit;
- FluentAssertions;
- Moq;
- integration tests;
- database-backed integration testing;
- API tests;
- end-to-end tests for critical flows;
- tenant isolation tests;
- subscription enforcement tests;
- payment idempotency tests;
- offline synchronization tests;
- accounting invariant tests;
- authorization/segregation-of-duties tests.

All code must compile cleanly. Avoid reserved language keywords as class names, namespaces or identifiers where this can cause ambiguity or compilation problems.

---

# 32. API and application behaviour

APIs must:

- use consistent HTTP semantics;
- validate input server-side;
- return stable error contracts;
- include correlation identifiers;
- enforce tenant context server-side;
- enforce authorization before business execution;
- use idempotency for retried commands where appropriate;
- avoid leaking implementation/database details;
- support pagination/filtering/sorting safely;
- avoid over-fetching sensitive data;
- provide explicit status transitions for stateful entities.

Controllers are thin. Application handlers orchestrate use cases. Domain rules remain in the domain/application boundary appropriate to the rule.

---

# 33. Business state-machine requirements

Critical entities must have explicit valid transitions.

Examples:

### Funeral case

```text
Draft -> Open -> Arranging -> Confirmed -> InService -> Completed -> Closed
```

### Claim

```text
Draft -> Submitted -> Verified -> Assessed -> Approved/Rejected -> Settled -> Closed
```

### Purchase order

```text
Draft -> Submitted -> Approved -> Released -> PartiallyReceived -> Received -> Closed
```

### Stock transfer

```text
Draft -> Requested -> Approved -> Picked -> InTransit -> Received -> Reconciled
```

### Support ticket

```text
Draft -> Submitted -> Triaged -> Assigned -> InProgress -> Waiting -> Resolved -> Closed
```

### Subscription

```text
Trial -> Active -> PaymentDue -> GracePeriod -> Suspended -> ReadOnly -> Terminated -> Archived
```

Invalid transitions must be rejected and audited.

---

# 34. Cross-module business integration

The power of Khet-360 is the connection between modules.

Examples:

### Funeral $\rightarrow$ Finance
Funeral package confirmation creates/updates billable items; payment updates receivables; settlement posts accounting entries.

### Funeral $\rightarrow$ Inventory
Selected services can reserve/consume inventory.

### Funeral $\rightarrow$ Fleet
Transport requirements create dispatch tasks and vehicle assignments.

### Funeral $\rightarrow$ Mortuary
Admission and release are tied to case/custody state.

### Funeral $\rightarrow$ Catering
Service schedule can create catering booking/headcount requirements.

### Funeral $\rightarrow$ Memorial
Completed service can trigger memorial follow-up.

### Policy $\rightarrow$ Claims $\rightarrow$ Funeral $\rightarrow$ Finance
Eligibility flows into claim assessment, service authorization and settlement.

### HR $\rightarrow$ Payroll $\rightarrow$ Finance
Approved payroll feeds accounting while preserving payroll authority and audit.

### Inventory $\rightarrow$ Procurement $\rightarrow$ Finance
Reorder signals can create purchasing workflows; goods receipt and supplier invoices feed payables.

### POS $\rightarrow$ Inventory $\rightarrow$ Finance
Completed POS reduces/reserves stock as configured and creates the appropriate financial records.

### Subscription $\rightarrow$ Platform Access
Platform payment confirmation changes subscription state and therefore tenant entitlements.

---

# 35. Reporting, KPIs and operational excellence

Khet-360 should expose actionable KPIs, not decorative dashboards.

Every major dashboard should answer:

1. What happened?
2. What needs attention?
3. Who owns it?
4. What is overdue?
5. What financial impact exists?
6. What should happen next?

A management dashboard should therefore combine metrics with drill-downs, exceptions and next-best actions.

## 35.1 Productivity scorecard

Khet-360 should measure whether it is reducing work, risk and delay. The initial scorecard
should include:

- median time from enquiry to case creation;
- median time from case creation to arrangement confirmation;
- percentage of work items completed on time;
- overdue and blocked work by team, branch and reason;
- first-response time for leads and support tickets;
- percentage of cases using a template or checklist;
- duplicate customer detection rate and confirmed duplicate reduction;
- percentage of records completed without re-keying data from another Khet-360 module;
- automation success, exception and manual-override rates;
- offline command queue age, replay success and conflict resolution time;
- user adoption of My Work, saved views and bulk actions.

Each KPI must show its definition, period, data freshness, denominator, owner and drill-down
records. Targets are configurable per tenant; the platform must not present a score without
explaining how it was calculated. Usage analytics must be aggregated and permission-aware,
with no unnecessary capture of sensitive customer content.

---

# 36. Platform support access to tenant data

Platform support, billing and account personnel must not receive unrestricted tenant database credentials.

Where support needs business context, Khet-360 uses:

- purpose-bound access;
- least-privilege service operations;
- audited support impersonation where explicitly authorized;
- masked/summarized diagnostic data;
- secure references to tenant records;
- explicit tenant consent/authorization where policy requires it.

Cross-tenant access is prohibited by default.

---

# 37. Commercial and operational tiers

The architecture supports plan differentiation without changing tenant data ownership.

A plan may control:

- modules;
- users;
- branches;
- storage;
- transaction volume;
- support level;
- advanced reporting;
- automation;
- integration availability;
- enterprise capabilities.

An enterprise tenant may receive dedicated infrastructure while retaining the same logical application contracts.

---

# 38. Data ownership and portability

Tenant business data belongs to the tenant subject to contractual terms and applicable law.

Khet-360 must provide controlled export capabilities for appropriate tenant data and must support orderly migration/termination without holding data hostage.

Exports must be authenticated, authorized, logged and protected.

---

# 39. Production readiness gates

A release is not production-ready until all applicable gates pass:

### Tenancy
- platform-first tenant validation;
- tenant DB routing cannot be spoofed;
- cross-tenant access tests pass;
- tenant credentials never reach clients;
- suspended tenant cannot execute normal business mutations.

### Security
- authorization tests pass;
- branch isolation tests pass;
- secrets are protected;
- platform-agent boundaries pass;
- audit events exist for critical actions.

### Finance
- debit equals credit;
- payment commands are idempotent;
- callbacks are replay-safe;
- allocation limits are enforced;
- posted entries are immutable;
- reconciliation exceptions are explicit.

### Subscription
- upgrades are safe;
- downgrades do not silently destroy data;
- billing payments are idempotent;
- subscription state drives entitlements;
- suspension/reactivation is tested.

### Offline
- outbox survives restart;
- replay is idempotent;
- conflicts are deterministic;
- high-risk operations remain online-only.

### Operations
- backup and restore are proven;
- migrations are tested;
- health/readiness checks work;
- monitoring and alerts work;
- integration failures are recoverable.

---

# 40. Delivery roadmap

## Phase 1 — Platform foundation
- platform control database;
- tenant registry;
- tenant lifecycle;
- subscription plans;
- entitlement engine;
- tenant DB registry;
- secure secret references;
- platform-first tenant resolver;
- tenant DB connection resolver;
- provisioning;
- platform authentication/roles;
- platform audit.

## Phase 2 — Tenant identity and core ERP
- tenant authentication;
- tenant RBAC;
- branch scope;
- organisation configuration;
- Customer/Family 360;
- CRM;
- funeral cases;
- operational tasks;
- productivity foundation: My Work, Team Queue, Exceptions, global search, saved views,
  quick capture and reusable templates;
- initial finance/accounting.

## Phase 3 — Internal Operations
- **SLA-Driven Intelligent Routing**;
- **Redis Caching**;
- **RabbitMQ** (async task exchange);
- policy;
- claims;
- repatriation;
- mortuary;
- fleet;
- service arrangements;
- catering;
- memorials;
- operational command centre;
- guided case checklists, readiness indicators, resource conflict detection and bulk-safe
  assignment/document actions.

## Phase 4 — External Ecosystem
- **The Family Experience Portal**;
- **Vendor Collaboration Hub**;
- **MinIO** (S3-compatible object storage);
- integrated payment and document uploads.

## Phase 5 — Power-User UX
- **Action-Oriented Command Palette**;
- **Single-Screen Deal Boards**;
- **Guided Arrangement Wizards**;
- advanced search and real-time SignalR updates.

## Phase 6 — Monitoring and Intelligence
- **Prometheus & Grafana** (SLA and system health dashboards);
- productivity scorecard;
- user-configurable dashboards and
- continuous improvement feedback.

## Phase 7 — Resilience and scale
- offline outbox/inbox;
- synchronization;
- conflict resolution;
- per-tenant backup/restore;
- tenant migration tooling;
- enterprise isolation tiers;
- advanced reporting/intelligence.

---

# 41. Definition of a rock-solid Khet-360 ERP

Khet-360 stands out when consumers can trust it with the business's most important work.

It must:

1. Protect every tenant as a separate business system.
2. Validate tenant status and entitlement before tenant business access.
3. Keep tenant branches, users, configuration and business truth inside the tenant boundary.
4. Provide a complete ERP rather than isolated modules.
5. Connect operational, customer, people and financial workflows.
6. Make next actions and exceptions obvious.
7. Make financial records correct and auditable.
8. Make critical operational state explainable.
9. Provide safe offline capability where appropriate.
10. Give tenants direct control over subscription, billing and support.
11. Provide transparent upgrade and downgrade journeys.
12. Never silently destroy tenant data.
13. Give platform teams strong support tooling without unrestricted tenant access.
14. Remain maintainable through explicit architecture and automated tests.
15. Be resilient under retries, duplicate requests, provider callbacks, network failures and partial outages.

## Final architectural rule

> **The Khet-360 platform decides whether a tenant may enter. The tenant database decides what that tenant's people may do. The domain decides whether the business action is valid. The accounting system decides the financial truth. The audit trail explains what happened.**
