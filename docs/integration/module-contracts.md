# SANAD Module Contracts

This document is the coordination point for the five planned business modules. It
defines logical ownership inside one backend application and one shared database.
It does not create modules, entities, migrations, or business rules.

## Current implementation status

The repository currently contains only the authentication foundation and the
`CurrentUser` reference feature. The five business modules and their tables are
not implemented. The ERD at
[`../architecture/sanad_erd.mermaid`](../architecture/sanad_erd.mermaid) is now
the source for the entity, attribute, key, relationship, and status information
recorded below. These are design facts from the ERD, not implemented application
facts.

## Logical ownership

| Module | Owner scope | Entities |
| --- | --- | --- |
| 1. Identity & Case Management | Family registration, family members, case creation, identity validation, and case lifecycle | `FAMILY`, `FAMILYMEMBER`, `CASE`, `CASESURVEY`, `CASESTATUSHISTORY` |
| 2. Document Verification & Assessment | Document management, verification, assessment, scoring, priority, and specific needs | `DOCUMENT`, `SCORINGRULESET`, `SCORERESULT`, `SPECIFICNEED` |
| 3. Donations & Payments | Donor business information, donations, payment integration, and payment confirmation | `DONOR`, `DONATIONCATEGORY`, `DONATION`, `PAYMENTTRANSACTION` |
| 4. Fund Allocation & Payouts | Fund allocation, financial records, payout authorization, review, and execution | `DONATIONALLOCATION`, `LEDGERENTRY`, `PAYOUT`, `PAYOUTAPPROVAL` |
| 5. Notifications, Reporting & Audit | Notifications, delivery retries, reports, and audit records | `NOTIFICATIONOUTBOX`, `REPORT`, `AUDITLOG` |

These ownership assignments cover every business entity in the ERD. `USER`,
`ROLE`, and `USERROLE` are shared authentication infrastructure owned by the
existing Identity foundation, not duplicate business-module entities. These are
ownership boundaries within the same application, not separate applications,
services, or databases.

## ERD-verified entities and attributes

The following inventory preserves the ERD names, keys, foreign keys, and
declared attributes. `nullable` is recorded where the ERD explicitly marks a
foreign key as nullable; absence of that marker means the ERD declares the
relationship as required.

| Entity | Primary key | Foreign keys | Other attributes and constraints |
| --- | --- | --- | --- |
| `USER` | `Id` (`string`, Identity) | — | `Email` (`string`, unique), `PhoneNumber`, `FullName`, `IsActive`, `CreatedAt` |
| `ROLE` | `Id` (`string`, Identity) | — | `Name` (`string`, unique; approved roles: `SuperAdmin`, `Manager`, `PayoutAuditor`, `Treasurer`, `DocumentReviewer`, `FieldOfficer`, `Donor`, `Beneficiary`) |
| `USERROLE` | `UserId` + `RoleId` (`string`) | `UserId` → `USER`, `RoleId` → `ROLE` | Composite PK |
| `DONOR` | `Id` (`guid`) | `UserId` → `USER` (`string`, unique), `DeletedById` → `USER` (`string`, nullable) | `PreferredChannel`, `OptOutNonEssential`, `IsDeleted`, `DeletedAt` (nullable) |
| `FAMILY` | `Id` (`guid`) | `PrimaryBeneficiaryUserId` → `USER` (`string`, required, unique); `DeletedById` → `USER` (`string`, nullable) | `HeadFullNameEnc`, `AddressEnc`, `PhoneEnc`, `Governorate`, `CreatedAt`, `IsDeleted`, `DeletedAt` (nullable); exactly one primary Beneficiary account |
| `FAMILYMEMBER` | `Id` (`guid`) | `FamilyId` → `FAMILY`, `DeletedById` → `USER` (`string`, nullable) | `FullNameEnc`, `NationalIdEnc`, `NationalIdHash` (unique), `BirthDate`, `Gender`, `Relation`, `IsProvider`, `IsDeleted`, `DeletedAt` (nullable) |
| `CASE` | `Id` (`guid`) | `FamilyId` → `FAMILY`, `CreatedByOfficerId` → `USER` (`string`), `DeletedById` → `USER` (`string`, nullable) | `Status` (four ERD values), `SubmittedAt`, `VerifiedAt`, `PausedDays`, `IsDeleted`, `DeletedAt` (nullable) |
| `CASESURVEY` | `CaseId` (`guid`) | `CaseId` → `CASE` | `MonthlyIncome`, `MonthlyRent`, `FamilySize`, `HousingType` (three ERD values), `ProviderStatus`, `ChildrenUnder6`, `ChronicIllnessCount`, `DisabilitySeverity`, `MonthlyMedicalSpend`, `SchoolAgeChildren`, `ChildrenInSchool`, `HomeCondition` |
| `DOCUMENT` | `Id` (`guid`) | `CaseId` → `CASE`, `UploadedById` → `USER` (`string`), `DeletedById` → `USER` (`string`, nullable) | `Type` (five ERD values), `FilePath`, `FileHash`, `UploadedAt`, `IsDeleted`, `DeletedAt` (nullable) |
| `CASESTATUSHISTORY` | `Id` (`guid`) | `CaseId` → `CASE`, `ReviewerId` → `USER` (`string`) | `FromStatus`, `ToStatus`, `Reason`, `ChangedAt`; immutable historical record |
| `SCORINGRULESET` | `Id` (`guid`) | `CreatedById` → `USER` (`string`) | `Version` (unique), `WeightFinancial`, `WeightSocial`, `WeightHealth`, `WeightEducation`, `PovertyThreshold`, `WaitLambda`, `WaitMaxDays`, `PointTablesJson`, `IsActive`, `CreatedAt`; deactivate instead of delete |
| `SCORERESULT` | `Id` (`guid`) | `CaseId` → `CASE`, `RuleSetId` → `SCORINGRULESET` | `FinancialScore`, `SocialScore`, `HealthScore`, `EducationScore`, `BaseScore`, `WaitDays`, `WaitFactor`, `PriorityScore`, `PriorityBand`, `CalculatedAt` |
| `SPECIFICNEED` | `Id` (`guid`) | `CaseId` → `CASE`, `ValuedById` → `USER` (`string`), `DeletedById` → `USER` (`string`, nullable) | `Description`, `Category`, `EstimatedPrice`, `FundedAmount`, `Status` (four ERD values), `CreatedAt`, `IsDeleted`, `DeletedAt` (nullable) |
| `DONATIONCATEGORY` | `Id` (`guid`) | — | `Name` (`string`, unique; `Education` or `Health`), `IsActive`; general fund is not a category |
| `DONATION` | `Id` (`guid`) | `DonorId` → `DONOR` (nullable), `CategoryId` → `DONATIONCATEGORY` (nullable) | `GuestReceiptEmail` (nullable; required when `DonorId` is null), `Amount`, `FeeAmount`, `Currency`, `Status` (four ERD values), `CreatedAt`, `ConfirmedAt`; financial record, never physically deleted |
| `PAYMENTTRANSACTION` | `Id` (`guid`) | `DonationId` → `DONATION` | `Gateway`, `GatewayRef` (unique), `Status`, `PayloadHash`, `WebhookReceivedAt`; financial record, never physically deleted |
| `DONATIONALLOCATION` | `Id` (`guid`) | `DonationId` → `DONATION`, `CaseId` → `CASE` (required), `NeedId` → `SPECIFICNEED` (nullable) | `Amount`, `AllocatedAt`; financial record, never physically deleted |
| `LEDGERENTRY` | `Id` (`guid`) | `DonationId` → `DONATION` (nullable), `AllocationId` → `DONATIONALLOCATION` (nullable), `PayoutId` → `PAYOUT` (nullable) | `EntryType` (five ERD values), `Amount`, `CreatedAt`; financial record, never physically deleted |
| `PAYOUT` | `Id` (`guid`) | `CaseId` → `CASE` | `Amount`, `Status` (six ERD values), `Method` (three ERD values), `Reference`, `CreatedAt`, `ExecutedAt`; financial record, never physically deleted |
| `PAYOUTAPPROVAL` | `Id` (`guid`) | `PayoutId` → `PAYOUT`, `ActorId` → `USER` (`string`) | `Step` (three ERD values), `Decision` (two ERD values), `Comment`, `DecidedAt`; immutable historical approval |
| `NOTIFICATIONOUTBOX` | `Id` (`guid`) | `UserId` → `USER` (`string`, required), `DonationId` → `DONATION` (nullable), `PayoutId` → `PAYOUT` (nullable), `CaseId` → `CASE` (nullable), `SpecificNeedId` → `SPECIFICNEED` (nullable) | `Type` (five ERD values), `Channel` (two ERD values), `Subject`, `Body`, `Status` (three ERD values), `Attempts`, `CreatedAt`, `SentAt`; preserve delivery status/history |
| `REPORT` | `Id` (`guid`) | `GeneratedById` → `USER` (`string`), `DeletedById` → `USER` (`string`, nullable) | `ReportType`, `PeriodStart`, `PeriodEnd`, `FilePath`, `FileHash`, `CreatedAt`, `IsDeleted`, `DeletedAt` (nullable) |
| `AUDITLOG` | `Id` (`guid`) | `UserId` → `USER` (`string`) | `Action`, `EntityName`, `EntityId`, `OldValue`, `NewValue`, `At`; append-only immutable record |

The approved identity foundation recognizes exactly eight application roles:
`Beneficiary`, `Donor`, `SuperAdmin`, `Manager`, `DocumentReviewer`,
`FieldOfficer`, `PayoutAuditor`, and `Treasurer`, matching
[`AppRoles.cs`](../../backend/src/SANAD.Domain/Constants/AppRoles.cs).

The approved ERD now links `FAMILY.PrimaryBeneficiaryUserId` directly to the
existing ASP.NET Identity `USER.Id` using a required, unique string foreign
key. Each Beneficiary account owns exactly one family, and each family has
exactly one primary Beneficiary account. The linked Identity user must have the
Beneficiary role. This does not add a duplicate user entity or change the
responsibilities or ownership of `FAMILY` and `FAMILYMEMBER`.
The unique foreign key ensures each Identity user can be linked to at most one
family; the Beneficiary account-creation workflow must atomically create/link
exactly one family for every Beneficiary account, since the FK alone cannot
enforce that reverse participation constraint.

## Identity ownership

ASP.NET Core Identity owns authentication users, roles, and user-role
relationships. The current implementation uses `ApplicationUser : IdentityUser`,
`IdentityRole`, and one `AppDbContext`; the existing migration creates the
standard tables.

The current Identity foundation seeds the eight approved roles, provisions an
initial SuperAdmin from secure configuration, and optionally creates a
development-only Manager account. The Manager seed is not the production
provisioning path. Future registration and authorization work must follow the
requirements document. Do not treat role membership as permission to expose
beneficiary or financial data.

The approved ERD supports the Beneficiary-to-family and guest-donation
workflows. Family and donation business entities, migrations, and workflows
remain unimplemented; the authentication foundation does not create family or
donation records.

| ERD concept | Existing Identity table |
| --- | --- |
| `USER` | `AspNetUsers` |
| `ROLE` | `AspNetRoles` |
| `USERROLE` | `AspNetUserRoles` |

Do not create duplicate authentication tables or a second authentication system.
`DONOR` remains a business entity and may be linked to the existing Identity user
ID; it is not an alias for `AspNetUsers`. Other modules must reference the
existing Identity user ID when an operation needs an associated user.

The ERD declares `USER ||--o| DONOR`: a user has zero or one donor record, and
each donor belongs to exactly one user; `DONOR.UserId` is unique. The ERD also
adds donor attributes not present in the current Identity model. The mapping
differences are recorded below and require an approved persistence decision.

The approved Identity decision is to keep the existing ASP.NET Core Identity
implementation. `USER.Id`, `ROLE.Id`, `USERROLE.UserId`, `USERROLE.RoleId`, and
every foreign key to `USER` are therefore `string`; business entity primary keys
remain `guid`. Do not create duplicate authentication tables or change Identity
to GUID keys.

The remaining conceptual differences from the current implementation are:

- ERD `USER` includes `FullName`, `IsActive`, and `CreatedAt`; the current
  `ApplicationUser` does not define these custom properties.
- ERD `USER.Email` is unique; ASP.NET Core Identity currently enforces unique
  emails through `RequireUniqueEmail`, but the physical Identity schema also
  contains the standard username, claim, login, token, and concurrency columns.
- ERD `ROLE.Name` lists the eight recognized application roles; the current implementation
  seeds those names through `AppRoles`, while Identity also owns role claims.
- The ERD models only `USER`, `ROLE`, and `USERROLE`; the current Identity
  migration additionally contains `AspNetRoleClaims`, `AspNetUserClaims`,
  `AspNetUserLogins`, and `AspNetUserTokens`.

No authentication implementation should be changed in response to the
additional conceptual fields without an approved decision. Identity key
consistency is resolved and must be preserved.

## Shared references and dependencies

The ERD verifies these cross-module references:

- Module 1 owns `CASE`; Modules 2 and 4 reference it through surveys,
  documents, scoring, needs, allocations, and payouts.
- Module 2 owns `SPECIFICNEED`; Module 4 references it through
  `DONATIONALLOCATION.NeedId`.
- Module 3 owns `DONATION`; Module 4 references it through
  `DONATIONALLOCATION.DonationId` and `LEDGERENTRY.DonationId`.
- Module 3 owns `DONOR`; `DONATION` depends on it and the Identity foundation
  owns the linked `USER` when `DONATION.DonorId` is non-null. Guest donations
  have no `DONOR` relationship.
- `FAMILY.PrimaryBeneficiaryUserId` references the existing Identity `USER`
  with a required, unique string foreign key. It is restricted to users with
  the Beneficiary role; each family has exactly one primary Beneficiary, and
  each Beneficiary account owns exactly one family.
- Module 3's `DONATION` is referenced by Module 5's nullable
  `NOTIFICATIONOUTBOX.DonationId`.
- Identity `USER` is referenced by all modules through creator, reviewer,
  uploader, actor, recipient, and generator foreign keys.
- Module 4's `PAYOUT` is referenced by `PAYOUTAPPROVAL` and nullable
  `LEDGERENTRY.PayoutId`; Module 5's `NOTIFICATIONOUTBOX.PayoutId` is also
  nullable and is required for `PayoutImpact` notifications.
- Module 5's `AUDITLOG` has no typed foreign key to the audited entity; it stores
  `EntityName` and `EntityId` as generic values.

The owning developer implements and configures an entity. Other developers use
the agreed entity and application/API contract; they must not create a duplicate
entity or independently change its structure. A cross-module reference does not
automatically require an HTTP/service call: the agreed design is one backend
application with a shared SQL Server database. The owning module still controls
its behavior and contract.

When a feature crosses a boundary:

1. Identify the owner and the required reference or contract change before
   editing.
2. Agree on the identifier, nullability, lifecycle, authorization, and response
   shape with the owner.
3. Keep business rules in Domain/Application and expose only the API contract
   needed by the consumer.
4. Include both owners in review for shared contracts, schema changes,
   authorization, sensitive data, or money movement.

### ERD relationship cardinalities

The ERD declares the following relationship cardinalities. In each bullet, the
parent-side count is followed by the child-side count:

- `USER` → `USERROLE`: zero-or-many; `ROLE` → `USERROLE`: zero-or-many.
- `USER` → `DONOR`: zero-or-one; each `DONOR` has exactly one `USER`.
- `USER` → `FAMILY`: zero-or-one family for each Identity user; each `FAMILY`
  has exactly one primary Beneficiary user. The relationship applies only to
  `USER` accounts with the Beneficiary role, and the unique FK prevents one
  Beneficiary account from owning multiple families.
- `USER` → `AUDITLOG`, `NOTIFICATIONOUTBOX`, `REPORT`, `CASE`,
  `CASESTATUSHISTORY`, `DOCUMENT`, `SCORINGRULESET`, `SPECIFICNEED`, and
  `PAYOUTAPPROVAL`: zero-or-many; each child has exactly one referenced user.
- `FAMILY` → `FAMILYMEMBER`: one-or-many; `FAMILYMEMBER` has exactly one
  family. `FAMILY` → `CASE` is zero-or-many; each case has exactly one family.
- `CASE` → `CASESURVEY`: exactly-one-to-exactly-one.
- `CASE` → `DOCUMENT`, `CASESTATUSHISTORY`, `SCORERESULT`, `SPECIFICNEED`,
  `DONATIONALLOCATION`, and `PAYOUT`: zero-or-many; each child has exactly one
  case.
- `SCORINGRULESET` → `SCORERESULT`: zero-or-many; each result has exactly one
  rule set.
- `DONOR` → `DONATION`: zero-or-many donations per donor; each donation has
  zero-or-one donor (`DonorId` nullable). A non-null `DonorId` references an
  authenticated Donor; null means guest donation.
- `DONATIONCATEGORY` → `DONATION`: zero-or-many donations per category; each
  donation has zero-or-one category (`CategoryId` nullable). Null means the
  general fund; a non-null category is an approved category (`Education` or
  `Health`). Guest donations must have null `CategoryId`.
- `DONATION` → `PAYMENTTRANSACTION` and `DONATIONALLOCATION`: zero-or-many;
  each child has exactly one donation. `DONATIONALLOCATION` → `CASE` is
  required, while its `SPECIFICNEED` reference is zero-or-one per allocation.
- `LEDGERENTRY` has zero-or-one reference to each of `DONATION`,
  `DONATIONALLOCATION`, and `PAYOUT`; the integrity rule below requires exactly
  one of those references.
- `PAYOUT` → `PAYOUTAPPROVAL`: one-or-many; each approval has exactly one
  payout.
- `NOTIFICATIONOUTBOX` has zero-or-one reference to each of `DONATION`,
  `PAYOUT`, `CASE`, and `SPECIFICNEED`; `USER` remains required as recipient.

### Allocation integrity

`DONATIONALLOCATION.CaseId` is required. `NeedId` is nullable so an allocation
may support a case generally or target a specific need. When `NeedId` is
populated, application validation and the database relationship must ensure the
referenced `SPECIFICNEED.CaseId` equals the allocation's `CaseId`. This
same-case rule is not expressible by a simple single-column foreign key.

### Ledger integrity

`LEDGERENTRY.DonationId`, `AllocationId`, and `PayoutId` are all nullable, but
exactly one must be populated. The database must enforce a CHECK constraint
equivalent to:

```sql
(
    CASE WHEN DonationId IS NOT NULL THEN 1 ELSE 0 END
  + CASE WHEN AllocationId IS NOT NULL THEN 1 ELSE 0 END
  + CASE WHEN PayoutId IS NOT NULL THEN 1 ELSE 0 END
) = 1
```

Application validation must also enforce the `EntryType` mapping:

| `EntryType` | Required reference |
| --- | --- |
| `Donation` | `DonationId` |
| `Fee` | `DonationId` |
| `Refund` | `DonationId` |
| `Allocation` | `AllocationId` |
| `Payout` | `PayoutId` |

No amount signs, accounting behavior, or transaction rules are inferred here.

### Donation account and fund constraints

`DONATION.DonorId` is nullable: when populated, it references an authenticated
Donor account; when null, the donation is a guest donation. A guest donation
must carry a `GuestReceiptEmail`, which is null for authenticated donations.
`CategoryId` is nullable and null denotes the general fund; guest donations
must have null `CategoryId`. A non-null category is restricted to the approved
`Education` and `Health` categories. The general fund is not a
`DONATIONCATEGORY`. Enforce the conditional donor/email/category combinations
as a single invariant: guest (`DonorId` null) requires receipt email and null
category; authenticated (`DonorId` non-null) requires null guest receipt email
and may select a category or the general fund. No placeholder Donor account is
created for guests. Donations remain financial records and retain payment
transactions, confirmations, receipts, and ledger tracking regardless of donor
type.

### Notification integrity

`NOTIFICATIONOUTBOX.UserId` is required. The notification `Type` determines
which optional business reference is required:

| `Type` | Required reference |
| --- | --- |
| `DonationImpact` | `DonationId` |
| `PayoutImpact` | `PayoutId` |
| `CaseStatus` | `CaseId` |
| `SpecificNeedCovered` | `SpecificNeedId` |
| `MonthlyReminder` | No business entity reference |

Application validation must enforce these type-specific rules and reject
inconsistent combinations. Each notification must reference exactly the
business entity corresponding to its type, and all unrelated business reference
columns must be null. `MonthlyReminder` must have no business entity reference.
Notification records are preserved with their delivery status/history; retention
and archival policies will be defined separately. No delivery-attempt entity is
introduced by this contract.

### Audit log integrity

`AUDITLOG` intentionally keeps the generic `EntityName` and `EntityId` design;
it must not gain one foreign key per business entity. `EntityName` must use
stable, controlled entity-type identifiers. `EntityId` must support both GUID
business identifiers and string Identity identifiers. An index on
`(EntityName, EntityId, At)` is recommended. Audit records are append-only
through normal application operations, and `OldValue`/`NewValue` must not expose
sensitive personal information unnecessarily.
Audit events may be generated by a user or by the system. `UserId` nullability
remains unresolved until the requirements are reviewed; the ERD is not changed
to resolve that question.

### Soft deletion and historical preservation

The ERD applies ordinary soft deletion selectively using:

- `IsDeleted` (`bool`);
- `DeletedAt` (`datetime`, nullable);
- `DeletedById` (`string`, nullable) referencing Identity `USER`.

Soft deletion fields are applied to `DONOR`, `FAMILY`, `FAMILYMEMBER`, `CASE`,
`DOCUMENT`, `SPECIFICNEED`, and `REPORT`. They are not
applied to financial records, `CASESURVEY`, `CASESTATUSHISTORY`,
`SCORINGRULESET`, `DONATIONCATEGORY`, `PAYOUTAPPROVAL`, `AUDITLOG`, or Identity
tables. Scoring rules and donation categories use their existing `IsActive`
fields and are deactivated rather than deleted. Identity users use `IsActive`
when historical references exist.

Donations, allocations, payouts, approvals, payment transactions, and ledger
entries must not be physically deleted through normal application operations.
Audit logs and historical status/approval records remain immutable. Ordinary
soft-deleted records remain available to authorized historical/audit operations.
No cascade physical deletion is introduced across business relationships.
Soft deletion does not authorize retaining personal data indefinitely; applicable
retention and privacy requirements still apply.

For every remaining soft-deletable entity, `IsDeleted = true` requires a
non-null `DeletedAt`, while `IsDeleted = false` requires `DeletedAt = NULL`.
`DeletedById` remains nullable for system-initiated deletion.

## Database and migrations

There is one SQL Server database, one shared `AppDbContext`, and one coordinated
EF Core migration history, and one shared EF Core model snapshot. Developers
own their assigned entities, configurations, application logic, APIs, tests, and
feature-branch migrations when schema changes are required. The project lead
coordinates integration and reviews the shared migration history; this does not
make the project lead responsible for implementing another developer's
Infrastructure code.

Developers may create migrations independently on their feature branches. They
must keep migration and snapshot changes in their branch, apply migrations in
order to their own local SQL Server database, and never blindly overwrite or
accept another developer's model snapshot. They must not delete or modify
another developer's migrations without coordination. If migrations were created
from the same previous snapshot, reconcile them against the latest integrated
model with the project lead. Never merge conflicting migrations merely to make
a pull request pass.

To request a schema change, provide:

- owning module and entity/table;
- business reason and affected use cases;
- columns, types, nullability, keys, indexes, and relationships;
- data migration or backfill needs;
- authorization, privacy, and retention implications;
- compatibility/rollback considerations and tests.

The project lead reviews entity definitions against the approved ERD, EF Core
configurations, relationships, keys, indexes, constraints, migration
operations, snapshot consistency, compatibility with integrated migrations,
data integrity, security, privacy, tests, and Clean Architecture compliance.
The lead may approve, request changes, or ask the developer to reconcile or
regenerate a migration. After approval, the combined migration sequence must be
verified against the integrated solution before it is applied to the shared
development database. This task does not create migrations or modify a
database.

## API and naming conventions

Follow the existing foundation conventions: controllers are the HTTP boundary,
DTOs define API payloads, `ApiResponse<T>` is the standard response envelope
except for `/health`, validation is explicit, and backend authorization is
authoritative. Reuse the existing Axios client and API types on the frontend.
All approved user-facing module workflows must support Arabic text and RTL
layout. Frontend and backend contracts must preserve Unicode/Arabic text in
names, descriptions, comments, reports, notifications, and audit values that
contain approved text. Language selection and English or additional-language
support remain unresolved and must not be invented.

Use the established C# naming conventions for classes and properties. Exact
database table names, key types for future business entities, status values, and
endpoint shapes are unresolved and require an approved contract; do not infer
them from this planning document.

## Standard backend workflow

Every backend feature follows:

1. Analyze requirements, existing code, business rules, ownership, and
   dependencies.
2. Domain: add necessary entities and domain behavior only.
3. Application: add use cases, DTOs, interfaces, and validation.
4. Infrastructure: add EF Core configurations and infrastructure services only
   when needed.
5. API: add endpoints, controllers, and registrations.
6. Testing: add and run focused unit/integration tests.
7. Review: verify architecture, security, consistency, and build results.

Dependency direction is strict: Domain depends on nothing; Application depends
on Domain; Infrastructure depends on Application and Domain; API composes
Application and Infrastructure. Domain and Application never depend on API or
Infrastructure. Do not create classes in every layer automatically.

## Collaboration workflow

1. Receive a task within the assigned module.
2. Create a short-lived `feature/*` branch from the latest `develop`.
3. Read the requirements, ownership, this contract, and applicable instructions.
4. Implement using the established architecture.
5. Test locally and report exact results.
6. Merge the latest `develop` into the feature branch, resolve code/migration/
   snapshot conflicts, and rerun relevant tests.
7. Open a focused pull request against `develop`.
8. Coordinate shared contract and database changes with the owning developer and
   project lead.
9. Merge only after approval, then verify the integrated migration sequence and
   update the local branch/database.

### Pull request checklist

- [ ] Correct module ownership and no duplicate shared entities.
- [ ] Clean Architecture dependency rules are preserved.
- [ ] Relevant tests were added/run and results are reported.
- [ ] Database changes are documented and coordinated; no competing migration.
- [ ] Latest `develop` was merged into the feature branch and conflicts were
  resolved correctly.
- [ ] API contract changes are called out and reviewed.
- [ ] No unrelated files or sensitive data were included.

## Unresolved decisions

- Detailed requirements and acceptance criteria for every module are pending.
- The ERD's conceptual `USER` fields `FullName`, `IsActive`, and `CreatedAt`
  still require a decision about whether and how to extend `ApplicationUser`;
  the existing Identity implementation must remain unchanged until approved.
- Status values and field value lists are recorded from the ERD, but their
  allowed transitions and business behavior remain undefined.
- Retention periods and privacy erasure procedures for soft-deleted records,
  documents, notifications, reports, and audit values remain undefined.
- Notification retention and archival policy remains undefined.
- Audit actor nullability for system-generated events remains unresolved pending
  requirements review.
- The exact database implementation of the same-case allocation rule and the
  ledger CHECK constraint must be reviewed when the schema is implemented.
- Cross-module API shapes, document storage behavior, payment gateway behavior,
  and audit/report retention are not defined by the ERD.
- Compatibility with external instruction discovery mechanisms beyond the
  repository's documented Copilot, OpenCode, and Antigravity setup is not
  verified; existing configurations must remain intact.
