# Requirements

Status: **skeleton**. Only information already established in the project context is recorded here. Detailed requirements are pending and must not be invented.

## Functional requirements
_To be defined._

- The platform interface and approved user-facing workflows must support the
  Arabic language.
- Arabic text must render correctly with right-to-left layout across pages,
  forms, tables, navigation, validation, notifications, reports, and status
  information.
- Backend APIs and database text handling must preserve Arabic/Unicode content
  without mojibake or loss.

Established scope areas (from project context, not yet specified in detail):
- Family registration and case management
- Identity validation
- Document management and verification
- Needs assessment and scoring
- Donations and payment processing
- Fund allocation
- Payout authorization and execution
- Notifications
- Reporting
- Audit trails

## Non-functional requirements
_To be defined._

Established priorities: security, privacy, separation of duties, financial consistency, maintainability.

## Business rules
_To be defined._

## Constraints
- Backend: .NET 10 (LTS) Web API with Clean/Onion Architecture; frontend: React; database: SQL Server.
- Monorepo; backend and frontend are separate applications.
- Team of five developers; approximately 10 days.
- No hosting provider assumptions.

## Edge cases
_To be defined._

## Acceptance criteria
_To be defined per feature._

## Open questions
_Pending input from stakeholders; nothing below is a decided requirement._
- Detailed behavior and acceptance criteria for each scope area.
- Authorization roles and separation-of-duties rules.
- Payment/payout provider and reconciliation process.
- Document storage and verification approach.
- Reporting and audit retention expectations.
