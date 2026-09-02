# Khet-360 Golden Path Scenarios

These scenarios represent the critical business lifecycles that must be validated to ensure the ERP is production-ready.

## 1. The Ultimate Funeral Lifecycle (Core Business Path)
**Goal**: Verify the flow from a potential lead to the final financial closure of a funeral.
- **Lead Intake**: Create a `Lead` $\to$ Qualify.
- **Conversion**: Convert `Lead` $\to$ `Customer` & `Opportunity`.
- **Case Initiation**: Open a `FuneralCase` $\to$ Assign Branch.
- **Arrangement**: Create `ServiceArrangement` $\to$ Define `ArrangementItems`.
- **Production**: Create `ProductionOrder` (Memorial) $\to$ Shop Floor Progress $\to$ Quality Check $\to$ Ready for Delivery.
- **Financials**: Generate `Invoice` $\to$ Record `Payment` $\to$ Verify `FinancialTransaction` balances.
- **Closure**: Mark Case as `Closed` $\to$ Archive.

## 2. HR & Payroll Lifecycle (People Path)
**Goal**: Verify employee onboarding to monthly pay.
- **Onboarding**: Create `Employee` $\to$ Assign `Department`, `Position`, and `Branch`.
- **Compensation**: Create `PayProfile` $\to$ Assign `PayItems`.
- **Execution**: Create `PayrollRun` $\to$ Calculate $\to$ Finalize.
- **Output**: Generate `Payslip` $\to$ Verify Net Pay calculation.

## 3. Insurance Claim Lifecycle (Finance Path)
**Goal**: Verify the recovery of funds from insurance providers.
- **Policy**: Link `Customer` to `InsurancePolicy`.
- **Claim**: Submit `InsuranceClaim` $\to$ Attach Evidence.
- **Processing**: Verify Claim $\to$ Approve.
- **Payout**: Record `ClaimPayment` $\to$ Apply to `FuneralCase` balance $\to$ Verify GL entries.

## 4. Operational Excellence (Productivity Path)
**Goal**: Verify that the system drives action and maintains SLAs.
- **Work Assignment**: Case milestone triggers a `WorkItem`.
- **Escalation**: `WorkItem` exceeds SLA $\to$ Escalation worker triggers alert.
- **Resolution**: Employee completes `WorkItem` $\to$ State Sync updates Dashboard.
