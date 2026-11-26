# Developer 3 — Donations & Payments

## 1. Purpose and responsibilities

Own donor business information, donation categories, donations, payment
transactions, gateway integration boundaries, and payment confirmation. Payment
provider, reconciliation, and detailed financial requirements remain pending.

Role boundary: authenticated Donors may view only their own donation history,
payment status, receipts, category, and anonymized impact updates; they may
never browse or select families. Guest donations go to the general fund only,
have no account association or category, and retain payment confirmation and
receipt records. Do not expose beneficiary identities or case details.
`DONATION.DonorId` is nullable for guest donations; `CategoryId` is also null
for guests and denotes the general fund. `GuestReceiptEmail` is required for
guest donations. Do not create a placeholder Donor account.

## 2. Entity ownership and ERD relationships

Own exactly:

- `DONOR`
- `DONATIONCATEGORY`
- `DONATION`
- `PAYMENTTRANSACTION`

`DONOR` has a unique string Identity `UserId`. `DONATION.DonorId` is nullable:
non-null identifies an authenticated Donor; null identifies a guest
donation. `DONATION.CategoryId` is nullable: null denotes the general fund;
non-null references an approved category. A guest donation has null
`DonorId`/`CategoryId` and a required `GuestReceiptEmail`. `PAYMENTTRANSACTION`
references every donation. Module 4 consumes `DONATION` through
`DONATIONALLOCATION`; Module 5 may reference it for `DonationImpact`
notifications. Financial records must not be physically deleted through normal
operations. Follow the ERD for statuses, uniqueness, keys, and selective soft
deletion.

## 3. Complete user-facing workflows

The approved requirements establish donor information, donation categories,
donations, payment processing, and payment confirmation. The ERD establishes
gateway/reference and webhook fields, but not a provider, retry policy,
reconciliation process, amount rules, currency policy, or state transitions.
Do not invent them.

Deliver complete vertical donation and payment workflows: backend
functionality, APIs, validation, authorization, persistence, frontend
interfaces, payment interaction boundaries, error handling, and tests must work
together. Do not deliver isolated APIs or frontend mockups. If an external
integration cannot be completed, document the limitation and request approval;
do not silently simulate it.

## 4. Backend deliverables and API surface

Implement only the required Domain entities/rules, Application use cases,
services, DTOs and validation, Infrastructure configurations and persistence,
API endpoints and authorization, and focused backend tests. No business-module
endpoint paths are approved. Payment and donation endpoints must be defined
only after requirements and provider decisions are approved. Follow existing
DTO, validation, authorization, response, error, and logging conventions; never
log payment payloads, tokens, or sensitive personal data.

## 5. Frontend deliverables

Implement the approved donor, category, donation, and payment pages and routes,
module-specific components and forms, Axios API integration, payment
interaction UI only for approved provider behavior, role-aware access, and
loading, empty, error, success, and validation states. Use Arabic text and
RTL layout and preserve Unicode donor and payment-description data. Reuse existing layouts,
navigation, auth context, shared components, and types. Frontend checks are UX
only; backend authorization is authoritative.

## 6. Shared frontend integration and dependencies

Use the existing ASP.NET Core Identity user as the donor account reference;
`DONOR` is not an authentication table. Coordinate `DONATION` identifiers and
status contracts with Developer 4 and notification/audit events with Developer
5. Do not implement allocation, payout, ledger, or audit entities.

Consume other modules through agreed APIs and identifiers. Coordinate shared
frontend changes with the project lead and use actual APIs for final
integration. Do not duplicate allocation, payout, ledger, audit, or Identity
logic in the frontend or backend.

## 7. Required backend and frontend tests

Test approved donor/category/donation validation, unique constraints, payment
transaction relationships, authorization, webhook/payment confirmation
behavior, privacy-safe logging, financial immutability, and cross-module
donation references. Add frontend component/integration tests for approved
pages, forms, payment states, routes, API failures, and role-aware UX. Apply
migrations to local SQL Server and run both test suites after integrating
`develop`.

## 8. Mandatory backend implementation order

Follow **Analyze → Domain → Application → Infrastructure → API → Testing →
Review** and the shared [backend instructions](../../../backend/AGENTS.md).
Create only the layers required by approved requirements.

## 9. Git and EF Core migration workflow

Work on `feature/*`, create and test required EF migrations on that branch,
apply them sequentially to local SQL Server, merge the latest `develop` before
opening the PR, and reconcile migration/snapshot conflicts with the project
lead. Target the PR at `develop`; never push directly to `develop` or `main`.

## 10. Definition of Done

- Owned entities/configurations match the approved ERD and key decisions.
- Payment/provider assumptions are approved or clearly unresolved.
- Required backend and frontend donation/payment workflows are integrated end
  to end.
- Frontend uses shared conventions and actual agreed APIs.
- Financial records are protected from normal physical deletion.
- Cross-module donation contracts are coordinated with Developers 4 and 5.
- Relevant tests and local migration/schema verification pass after `develop`.
- PR includes database, security, privacy, API, and testing impacts.

## 11. Unresolved decisions

Payment gateway, reconciliation, idempotency, refund behavior, currency and
amount rules, authorization, retention, and API endpoint shapes require
approval.
