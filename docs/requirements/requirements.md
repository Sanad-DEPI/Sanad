# SANAD (SACMP) — Software Requirements Specification

## Document status and source basis

This specification consolidates the detailed SANAD requirements supplied with
the project and the decisions recorded in the repository. It defines the
intended V1 behavior; it is **not** an implementation-status report. The
authentication foundation and `/api/me` reference feature exist, but this
document does not mark any business requirement complete merely because the
foundation exists.

The original specification's requirement identifiers are retained where
available. Its two supplied text versions differ on several scoring formulas;
those differences are recorded as **UNRESOLVED — REQUIRES DECISION** below.
The canonical entity model is
[`../architecture/sanad_erd.mermaid`](../architecture/sanad_erd.mermaid).
The older ERD embedded in the supplied text is not copied here because it
conflicts with the approved project ERD and Identity decisions.

The labels in this document mean:

- **Confirmed project decision**: an architectural, ownership, or scope
  decision recorded by the project.
- **Requirement from original specification**: intended V1 behavior retained
  from the supplied requirements.
- **UNRESOLVED — REQUIRES DECISION**: conflicting or missing business behavior
  that must be decided before it is implemented.
- **Implementation note**: a cross-reference to the project architecture; it
  does not add a business requirement.

## 1. Introduction and purpose

SANAD (Smart Aid & Charity Management Platform, SACMP) supports charitable
donations and aid distribution. It is intended to help staff register family
needs, verify supporting evidence, assess need consistently, allocate
donations, authorize and execute assistance, communicate confirmed outcomes,
and report on activity while protecting beneficiary identities.

This document is for the project team, developers, testers, instructors, and
charity stakeholders. It records confirmed requirements, decisions, and
unresolved questions so that implementation does not silently invent business
rules.

The original specification identifies three problems the system is intended to
address: duplicate aid across charities, subjective manual need decisions, and
fraud risks from unverified documents.

## 2. Scope, system overview, and constraints

### 2.1 V1 scope

The original specification identifies the following as core V1 capabilities:

- Family registration, identity validation, case management, and lifecycle.
- Document collection and verification.
- Needs assessment, scoring, and a waiting-time priority factor.
- Specific-need valuation and funding.
- Monetary donations and payment-gateway integration.
- Fund allocation and a three-step payout approval and execution workflow.
- Email and SMS notifications after confirmed events.
- PDF receipts and operational, financial, and audit reports.
- Background processing for notifications, reports, payment expiry, scoring,
  allocation, and reminders.

The confirmed project decision is that **core business functionality,
including payment integration, email, and SMS, is required for V1**. The
selection of providers and unresolved business rules remain open; provider
uncertainty does not make these capabilities optional.

### 2.2 Out of scope

- Stock matching, inventory tracking, and donation of physical goods. The
  charity purchases items using allocated funds.
- Donor browsing, searching, or selecting beneficiary cases.
- A microservices architecture or separate databases for the modules.
- Assuming a particular hosting provider.

### 2.3 Confirmed technology and architecture decisions

- Backend: .NET 10 LTS and ASP.NET Core Web API, using Clean Architecture.
- Frontend: React, TypeScript, and Vite.
- Persistence: SQL Server with EF Core.
- Authentication: ASP.NET Core Identity and JWT.
- Deployment unit: one application organized into modules; not microservices.
- Repository: one monorepo containing the backend and frontend applications.
- The backend uses one shared SQL Server database, one `AppDbContext`, one
  coordinated EF Core migration history, and one shared model snapshot.
- Hosting-provider availability is not a project constraint.

**Implementation note:** the architectural dependency rules and development
workflow are defined in [`../../backend/AGENTS.md`](../../backend/AGENTS.md)
and [`../architecture/foundation.md`](../architecture/foundation.md). This
requirements document defines expected behavior and does not replace those
implementation instructions.

### 2.4 Team and module ownership

Five full-stack developers each own the backend and frontend implementation,
tests, and integration for their assigned module. Ownership is unchanged from
[`../team/README.md`](../team/README.md) and the individual assignment files.
The modules share one application and database; ownership does not imply
separate services or databases.

| Developer | Module | Owned business entities |
| --- | --- | --- |
| 1 | Identity & Case Management | `FAMILY`, `FAMILYMEMBER`, `CASE`, `CASESURVEY`, `CASESTATUSHISTORY` |
| 2 | Document Verification & Assessment | `DOCUMENT`, `SCORINGRULESET`, `SCORERESULT`, `SPECIFICNEED` |
| 3 | Donations & Payments | `DONOR`, `DONATIONCATEGORY`, `DONATION`, `PAYMENTTRANSACTION` |
| 4 | Fund Allocation & Payouts | `DONATIONALLOCATION`, `LEDGERENTRY`, `PAYOUT`, `PAYOUTAPPROVAL` |
| 5 | Notifications, Reporting & Audit | `NOTIFICATIONOUTBOX`, `REPORT`, `AUDITLOG` |

ASP.NET Core Identity remains the shared owner of authentication users, roles,
and user-role relationships. No module may create duplicate business versions
of `USER`, `ROLE`, or `USERROLE`. Cross-module references must use the agreed
contracts and identifiers in
[`../integration/module-contracts.md`](../integration/module-contracts.md).

### 2.5 Language and text

- **Confirmed project decision:** Arabic must be supported. Arabic text must
  render correctly in right-to-left layouts and remain intact through the
  frontend, APIs, database, notifications, and reports.
- Names, addresses, comments, report content, and other approved user text must
  preserve Arabic/Unicode data without corruption.
- The original specification asks for Arabic and English UI and bilingual
  notification templates. Earlier project decisions say English and additional
  language support are not approved.

**UNRESOLVED — REQUIRES DECISION:** confirm whether English support from the
original specification remains in V1. Arabic is required regardless. Do not
silently remove the original English requirement or treat it as approved while
this conflict remains.

### 2.6 Regulatory and delivery constraints

- The original specification targets alignment with Egypt's Personal Data
  Protection Law (Law 151/2020). The project must obtain appropriate legal
  review; this document is not a legal compliance assessment.
- The delivery team has five developers and approximately ten days.
- No hosting-provider availability assumption is imposed. The original uptime
  target remains listed under non-functional requirements and needs feasibility
  confirmation.

## 3. Definitions and terminology

| Term | Meaning |
| --- | --- |
| Case | A family's aid request, identified in operational workflows by an anonymous Case ID. |
| Main Admin | The SuperAdmin role for staff administration. This role has no automatic access to beneficiary identity or business data. |
| RBAC | Role-based access control. |
| IPC | Income per capita after rent; its exact formula is unresolved in Section 5.4. |
| Payout | A proposed or confirmed release of funds for a case, subject to the approval workflow. |
| Proof | A document or other evidence submitted in support of a case. |
| Priority score | The case score after applying the waiting-time factor. |
| Identity user ID | The string key owned by ASP.NET Core Identity. |
| Business entity ID | A GUID key for a business entity, as specified by the approved ERD. |

## 4. User roles, permissions, and privacy

The system has exactly eight application roles. `Beneficiary` and `Donor` are
public roles; `SuperAdmin`, `Manager`, `DocumentReviewer`, `FieldOfficer`,
`PayoutAuditor`, and `Treasurer` are staff/administration roles. Guest donors
do not need an account or application role.

| Capability / data view | SuperAdmin | Manager | Payout Auditor | Treasurer | Document Reviewer | Field Officer | Beneficiary | Donor |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Manage staff accounts and staff roles | Yes | No | No | No | No | No | No | No |
| View family identity (name, National ID, address, phone) | No automatic access | Unresolved; denied | Unresolved; denied | Unresolved; denied | Only as necessary for identity validation and document review | No | Own account only; family workflow not implemented | No |
| View case information | No automatic access | Unresolved; denied | Unresolved; denied | Unresolved; denied | Only as necessary for review | Needs and valuation information only; no identity or identity documents | No case access until approved workflow exists | No case information |
| Assess conditions and value needs | No | Unresolved | Unresolved | Unresolved | No | Yes, without identity data | No | No |
| Verify identity and review identity documents | No | Unresolved; denied | Unresolved; denied | Unresolved; denied | Yes, only as necessary for identity validation and submitted-document review | No | No | No |
| Authorize payout | No automatic access | Unresolved | No | No | No | No | No | No |
| Audit payout file | No | No | Yes, duties only; data fields unresolved | No | No | No | No | No |
| Execute payout | No | No | No | Yes, duties only; data fields unresolved | No | No | No | No |
| Donate | No | No | No | No | No | No | No | Yes; guests may donate without an account |
| Generate reports | No automatic access | Unresolved | Unresolved | Unresolved | No | No | No | Own donation receipts only |

### 4.1 Authorization and privacy requirements

- **FR-ROLE-1:** The system shall support exactly the eight roles listed above.
- **FR-ROLE-2:** Each protected API operation shall enforce the required role
  or policy on the server. Unauthorized access shall not be granted; requests
  from authenticated users without permission shall return HTTP 403.
- **FR-ROLE-3:** SuperAdmin may manage staff accounts and staff roles, but
  receives no automatic access to beneficiary cases, identity documents,
  donations, allocations, or payouts.
- **FR-ROLE-4:** Manager, Payout Auditor, and Treasurer shall not receive
  unresolved identity-data or business-data permissions. Access remains denied
  until the authorization matrix is approved.
- **FR-ROLE-5:** Field Officers shall assess financial/living conditions and
  value needs without accessing beneficiary identity, National IDs, or
  identity documents. Donors shall not see case information.
- **FR-ROLE-6:** Any future staff role-assignment changes shall be audit-logged.
- **FR-ROLE-7:** Role-assignment changes shall be audit-logged and take effect
  on the next token refresh, subject to the approved token lifecycle.
- **FR-ROLE-8:** The Document Reviewer may access identity information and
  identity documents only as necessary for identity validation and review.
- **FR-ROLE-9:** The Payout Auditor and Treasurer may perform their respective
  payout duties, but their accessible data fields remain unresolved and denied.
- **FR-ROLE-10:** Each role shall receive only the information needed for its
  decision. Documents are evidence; they do not grant access to the identity
  profile. Donors shall never browse, search, or select cases.
- **FR-ROLE-11:** No user may perform more than one approval step for the same
  payout. A Manager may not execute a transfer.
- **FR-ROLE-12:** Public registration shall assign only one of `Donor` or
  `Beneficiary`. It shall not accept arbitrary role names or create staff
  accounts. Users cannot change their own account type; an authorized
  administrative process is required.

The family is responsible for entering and submitting its own registration
information. The Field Officer assesses financial and living conditions,
evaluates needs, and provides valuations without access to family identity,
National IDs, or identity documents.

Beneficiary registration does not approve a family for assistance. Family
ownership, registration, and case workflows remain out of scope for the
authentication foundation. Guest donations do not require registration or a
Donor role.

**UNRESOLVED — REQUIRES DECISION:** finalize Manager, Payout Auditor, and
Treasurer data-access permissions and the staff account-management API/audit
contract before implementing those operations.

**UNRESOLVED — REQUIRES DECISION:** the permission matrix does not define every
read, update, correction, export, and deletion permission. Approve a complete
authorization matrix before implementing those operations.

## 5. Functional requirements

Requirements below are retained from the supplied specification unless marked
unresolved. Their presence does not indicate that they have been implemented.

### 5.1 Authentication, accounts, and National ID

#### Authentication and accounts

- **FR-AUTH-1:** The system shall authenticate users through ASP.NET Core
  Identity and issue short-lived JWT access tokens with refresh-token rotation.
  The original target access-token lifetime is 15 minutes.
- **FR-AUTH-2:** Passwords shall be stored using secure password hashing.
  Accounts shall be locked after five failed login attempts.
- **FR-AUTH-3:** Donors shall verify their email address during registration.
  Phone verification by OTP is optional in the original specification.
- **FR-AUTH-4:** Staff accounts shall be created and managed only through an
  authorized administrative process; the approved role for staff
  administration is SuperAdmin.
- **FR-AUTH-5:** Authentication, login, and OTP endpoints shall be protected
  against repeated attempts as specified by NFR-1.
- **FR-AUTH-6:** Public registration shall accept only `Donor` or
  `Beneficiary`; registration shall not create a family record or approve
  eligibility for assistance.
- **FR-AUTH-7:** A public account shall have no more than one public role.
  Public users cannot change their own account type. Guest donations do not
  require an account.

Donor email delivery/verification is not implemented in the foundation. Donor
accounts remain unable to sign in until an approved email-delivery and
verification workflow is added.

ASP.NET Core Identity is the source of truth for users, roles, and
authentication. These requirements do not authorize a second authentication
system or duplicate `USER`, `ROLE`, or `USERROLE` tables.

#### Egyptian National ID

- **FR-ID-1:** The system shall validate that an Egyptian National ID contains
  exactly 14 digits and validate its format.
- **FR-ID-2:** Validation shall extract the century, birth date, governorate
  code, and gender from the encoded information according to the officially
  applicable Egyptian National ID structure, where applicable. The original
  specification states odd gender digit = male and even gender digit = female.
- **FR-ID-3:** The system shall calculate age from the extracted birth date.
- **FR-ID-4:** A National ID shall be unique across all families. A person
  shall not be registered in two families, including in different household
  roles. A duplicate shall be rejected. Identity verification shall be
  performed by the Document Reviewer.

**UNRESOLVED — REQUIRES DECISION:** the exact structure/check-digit rules,
valid century and date ranges, governorate-code list, and behavior for invalid
or future birth dates are not specified. Confirm these rules before implementing
the structural National ID parser.

### 5.2 Family registration and case management

- **FR-CASE-1:** A family shall enter and submit its own registration
  information through the platform, subject to National ID validation and
  privacy requirements. The Field Officer shall assess financial and living
  conditions and value needs without access to family identity data.
- **FR-CASE-2:** A case shall be associated with its family through an
  internal identifier and shall use an anonymous Case ID in operational
  workflows.
- **FR-CASE-3:** The system shall support the case states `Draft`,
  `PendingReview`, `Verified`, and `Rejected`, as named by the approved ERD.
  The original workflow describes `Draft -> Pending Review -> Verified /
  Rejected`.
- **FR-CASE-4:** A case submission and each change of case state shall record
  the relevant actor, time, previous state, next state, and reason where
  required. Case status history shall be immutable.
- **FR-CASE-5:** A case shall not be scored for publication or receive aid
  until it is `Verified`.
- **FR-CASE-6:** A case survey shall capture the assessment inputs listed in
  the approved ERD, including monthly income and rent, family size, housing
  type, provider status, children under six, chronic illness count, disability
  severity, medical expenditure, school-age children, children in school, and
  home condition.

**UNRESOLVED — REQUIRES DECISION:** the exact case-state transition graph,
correction and resubmission path after rejection, conditions for reopening a
case, and the point at which the one-to-one survey must exist are not defined.

**UNRESOLVED — REQUIRES DECISION:** Module 1 owns `CASE` and
`CASESTATUSHISTORY`; Module 2 owns `DOCUMENT` and document verification. The
original specification assigns case verification transitions to the Document
Reviewer. The modules must agree on who owns and performs those transitions,
the API contract, and the audit event before implementation. This does not
change the agreed module ownership.

### 5.3 Document verification

- **FR-DOC-1:** Authorized applicants or staff shall be able to submit
  supporting proofs for a case: National ID, birth certificates, income
  proof, medical reports, and rent or housing proof.
- **FR-DOC-2:** The original proof workflow shall support `Draft`,
  `Pending Review`, `Verified`, and `Rejected` states.
- **FR-DOC-3:** Only the Document Reviewer shall verify or reject proofs or
  the case as agreed in the cross-module contract. Rejection shall require a
  reason.
- **FR-DOC-4:** The reviewer queue shall identify cases by anonymous Case ID
  and display only evidence needed for review. It shall not display the
  identity profile, contact details, address, or full family composition.
- **FR-DOC-5:** Every state change shall record who changed it, when, the
  previous and new state, and the reason where applicable.
- **FR-DOC-6:** A case shall not be scored for publication or receive aid
  until its required verification is complete.
- **FR-DOC-7:** Uploaded files shall be PDF, JPG, or PNG and no larger than
  5 MB each. Files shall be stored outside the web root.
- **FR-DOC-8:** Document review shall be evidence-only. Names printed on a
  proof are part of the evidence and shall not be searchable, copied into
  system identity fields, or included in exports through reviewer workflows.
- **FR-DOC-9:** Every reviewer or auditor document view shall be logged. The
  displayed document shall carry a watermark identifying the viewer by
  username.

**UNRESOLVED — REQUIRES DECISION:** the source does not define document
replacement, resubmission, retention, permitted file counts, malware scanning,
or whether verification status belongs to an individual document, the case, or
both. Do not assume the case-state flow fully defines each document's lifecycle.

### 5.4 Needs assessment and scoring

The scoring engine shall produce a need score from 0 to 100, with higher scores
representing greater need. It shall calculate component scores `F` (Financial),
`S` (Social), `H` (Health), and `E` (Education & Living), then apply a
waiting-time factor to produce `PriorityScore`.

#### Master base score

The supplied specification gives:

`BaseScore = wF * F + wS * S + wH * H + wE * E`

with source defaults `wF = 0.35`, `wS = 0.25`, `wH = 0.25`, and `wE = 0.15`.
Weights are configurable, must each be within `[0, 1]`, and must sum to `1.00`.

#### Financial score (F; source default weight 35%)

- `F_income = 100 * max(0, 1 - IPC / T)`, where `T` is the configurable
  poverty threshold per person. The score shall be zero when `IPC >= T`.
- `F_housing`: owned = 0, rented = 60, no stable housing = 100.
- `F = 0.80 * F_income + 0.20 * F_housing`.

The two supplied requirement texts give conflicting IPC expressions:

- One gives `IPC = (MonthlyIncome - MonthlyRent) + FamilySize`.
- The other gives `IPC = (MonthlyIncome - MonthlyRent) / FamilySize` and notes
  that the source PDF may show `+`.

**UNRESOLVED — REQUIRES DECISION:** do not silently select or implement one
IPC expression.

#### Social score (S; source default weight 25%)

- Provider status `P` (maximum 50): widow = 50, orphan household = 50,
  disabled provider = 45, unemployed = 40, daily worker = 35, other = 0.
- Family-size factor `Z` (maximum 25): five points for each family member
  above two, capped at 25.
- Children-under-six factor `C` (maximum 25): eight points per child, capped
  at 25.
- `S = min(100, P + Z + C)`.

#### Health score (H; source default weight 25%)

- Chronic illness factor `I`: 25 points per case, capped at 40 total.
- Disability factor `D`: none = 0, mild = 15, moderate = 25, severe = 35.
- `H = min(100, I + D + M)`.

The two supplied requirement texts give conflicting medical-burden expressions:

- One gives `M = 25 * min(1, MonthlyMedicalSpend + MonthlyIncome)`.
- The other gives `M = 25 * min(1, MonthlyMedicalSpend / MonthlyIncome)` and
  notes that the source PDF may show `+`.

**UNRESOLVED — REQUIRES DECISION:** do not silently select or implement one
medical-burden expression.

#### Education and living score (E; source default weight 15%)

- Home condition: poor = 100, fair = 50, good = 0.
- `E = 0.50 * Edu + 0.50 * Home`.
- The source says `Edu = 0` when there are no school-age children.

The two supplied requirement texts give conflicting education expressions:

- One gives `Edu = 100 * (1 - ChildrenInSchool + SchoolAgeChildren)`.
- The other gives
  `Edu = 100 * (1 - ChildrenInSchool / SchoolAgeChildren)` and notes that the
  source PDF may contain a `-` in the formula.

**UNRESOLVED — REQUIRES DECISION:** do not silently select or implement one
education expression.

#### Waiting-time factor, priority score, and rule versions

- `D` is the number of days since submission for review. The waiting clock is
  described as pausing while a case is rejected and awaiting corrections.
- `W = 1 + lambda * (min(D, D_max) / D_max)`.
- Source defaults are `lambda = 0.30` and `D_max = 60` days.
- `PriorityScore = min(100, BaseScore * W)`.
- Priority bands are Critical 80–100, High 60–79, Medium 40–59, and Low 0–39.
- **FR-SC-1:** Scores shall be computed automatically from verified inputs
  and recomputed when applicable verified inputs change.
- **FR-SC-2:** Every result shall retain its component breakdown, inputs used,
  BaseScore, waiting days, W, PriorityScore, and rule version.
- **FR-SC-3:** The original specification requires the Main Admin to configure
  weights, threshold, point values, waiting factor, and maximum wait without
  redeployment.
- **FR-SC-4:** Scoring shall be deterministic and independently testable, with
  documented cases for each priority band.
- **FR-SC-5:** Users shall not manually override a score. Corrections shall be
  made through input or rule changes and audited.
- **FR-SC-6:** The Main Admin shall manage scoring parameters through an
  administrative workflow.
- **FR-SC-7:** The system shall reject a rule set with weights outside `[0, 1]`
  or whose sum is not `1.00`.
- **FR-SC-8:** Each saved rule set shall create a new version. Only one version
  shall be active at a time; historical results shall retain the version used.
  The Main Admin may choose whether open cases are recalculated after a rule
  change.
- **FR-SC-9:** A daily job shall update waiting-time-dependent priority.
  BaseScore changes only when applicable case inputs or scoring rules change.
- **FR-SC-10:** Each result shall persist BaseScore, waiting days, W, and
  PriorityScore.

**UNRESOLVED — REQUIRES DECISION:** approve final weights, `T`, point values,
`lambda`, and `D_max`; formula variants above; rounding and precision; priority
band boundaries for non-integer scores; behavior for zero family size or zero
income; behavior for negative income after rent; handling of missing inputs;
and tie-breaking between equal priority scores. The source uses `D` for both
disability and waiting days; use of distinct notation must be agreed without
changing the underlying meaning.

### 5.5 Specific needs and valuation

- **FR-NEED-1:** A family may request a specific need in addition to general
  financial assistance.
- **FR-NEED-2:** The Valuation/Field Officer shall assess and set a fair price
  for each specific need.
- **FR-NEED-3:** Specific needs shall be funded from monetary donations
  through the allocation process; the charity, not the donor, purchases the
  item.
- **FR-NEED-4:** The system shall track the funded amount against the approved
  estimated price and close a need when fully covered.
- **FR-NEED-5:** The system shall prevent overfunding or redirect excess
  allocation to the general fund.

The approved ERD records need status values `Open`, `Funded`, `Purchased`, and
`Closed`. The rules governing status transitions, price changes, reopening,
and excess-fund handling remain unresolved.

### 5.6 Donations and payment integration

- **FR-PAY-1:** Donors shall be able to make online monetary donations using a
  payment gateway that supports the approved channels.
- **FR-PAY-2:** Raw card data shall not pass through or be stored on SANAD
  servers. Use the gateway's hosted checkout or iframe and retain the
  transaction reference and status.
- **FR-PAY-3:** The intended payment flow is: create a Pending donation, send
  the donor to the gateway, receive the gateway webhook, verify its HMAC
  signature, then mark the payment Succeeded or Failed.
- **FR-PAY-4:** Webhook processing shall be idempotent. Replaying a gateway
  transaction reference shall not credit a donation more than once.
- **FR-PAY-5:** Donors may contribute to a general fund or an approved
  donation category, such as Education or Health. Donors shall not choose or
  see a beneficiary case.
- **FR-PAY-6:** An operating fee may be configurable. If enabled, it shall be
  shown transparently before payment.
- **FR-PAY-7:** Failed or abandoned payments shall expire after 30 minutes.
  Refunds shall be supported and recorded.
- **FR-PAY-8:** Every donation, fee, allocation, payout, and refund movement
  shall be recorded in the immutable financial ledger within the approved
  transaction boundary.
- **FR-PAY-9:** Payouts shall follow the three-step workflow in Section 5.8.
- **FR-PAY-10:** Donors shall be able to download a PDF receipt for each
  successful payment.
- **FR-PAY-11:** Payment and payout state changes shall follow the approved
  state machines; a returned payout shall include the Auditor's reason.
- **FR-PAY-12:** Before approving a payout, the Payout Auditor shall check
  verified documents, the existence of a score, the approved need price or
  ceiling, and the absence of a duplicate payout for the same period.
- **FR-PAY-13:** Only an audit-approved payout may be executed by the Treasurer.
  Its amount shall be locked after audit; changing it restarts the approval
  chain.
- **FR-PAY-14:** Each payout approval step shall record actor, decision,
  comment, and timestamp.

The original proposal names Paymob or Fawry for Egyptian payments and Stripe
as an optional international provider. These are candidates, not a selected
provider. Payment integration remains required for V1.

**UNRESOLVED — REQUIRES DECISION:** choose the production gateway(s), supported
payment methods, refund and reconciliation behavior, currency and amount rules,
fee activation and calculation, webhook retry/error responses, and the
definition of a duplicate payout period.

### 5.7 Fund allocation and financial integrity

- **FR-ALL-1:** Donors shall contribute to the general fund or an approved
  category; they shall never browse, search, or select cases.
- **FR-ALL-2:** Confirmed donations shall be allocated to eligible verified
  cases and needs in descending PriorityScore order under the approved
  allocation policy.
- **FR-ALL-3:** Each allocation shall link the donation to a case and,
  optionally, a specific need so impact can be reported without revealing
  beneficiary identity.
- **FR-ALL-4:** Allocation shall be transactional and shall never exceed the
  remaining approved need amount.
- **FR-ALL-5:** Stock and item-donation tracking are out of scope.

The approved contract additionally requires `DONATIONALLOCATION.CaseId` and
requires an associated `SPECIFICNEED` to belong to that same case.
`LEDGERENTRY` shall reference exactly one of Donation, Allocation, or Payout,
with the `EntryType` mapping in
[`../integration/module-contracts.md`](../integration/module-contracts.md).
These integrity requirements do not define accounting semantics or amount
signs.

**UNRESOLVED — REQUIRES DECISION:** define category-to-fund eligibility,
allocation scheduling and ordering beyond PriorityScore, transaction and
concurrency behavior, treatment of unallocated balances, and how refunds or
overfunding affect previously recorded allocations and ledger entries.

### 5.8 Payout authorization, approval, and execution

Payout methods listed in the source are Bank, Wallet, and Cash. The intended
approval sequence is:

1. Manager authorizes a payout: `Requested -> Authorized`.
2. Payout Auditor approves it (`AuditApproved`) or returns it to the Manager
   with a reason (`Returned`).
3. Treasurer executes only an audit-approved payout, records the transfer
   reference, and marks it `Executed`.

The approved ERD also lists `Failed` as a payout status. The three roles must
remain separate for each payout: no user may perform multiple approval steps,
and a Manager cannot execute a transfer. The approved amount is locked after
audit; any approved amount change restarts the chain.

**UNRESOLVED — REQUIRES DECISION:** decide whether Manager authorization is
required for every payout or only above a threshold; define status transitions
for failed, returned, retried, or cancelled payouts; define how cash handover
is evidenced; define when a payout is considered executed; and confirm the
meaning of the source's "duplicate payout in the same period" rule.

### 5.9 Notifications

- **FR-NOT-1:** Send notifications only after the triggering event is
  confirmed.
- **FR-NOT-2:** Source events include successful donations, case verification
  or rejection, payout approval or return, payout execution, a specific need
  becoming fully covered, and monthly reminders.
- **FR-NOT-3:** Email and SMS are required channels for V1. Donors may select
  a preferred channel subject to the approved channel policy.
- **FR-NOT-4:** Donor impact messages shall describe effects in general terms
  and shall never disclose family-identifying details.
- **FR-NOT-5:** Use a notification outbox processed in the background, with
  retries and exponential backoff. Failures shall be logged and visible to
  the Manager.
- **FR-NOT-6:** The original specification calls for editable Arabic and
  English templates; English support remains subject to Section 2.5.
- **FR-NOT-7:** Each notification shall record its status (`Queued`, `Sent`,
  or `Failed`), timestamp, and channel.
- **FR-NOT-8:** Users may opt out of non-essential messages. Transactional
  receipts shall still be sent.
- **FR-NOT-9:** After gateway confirmation, the donor shall receive an email
  receipt and an impact message.
- **FR-NOT-10:** If the donation has already been allocated, the impact
  message shall describe the actual effect in general terms. Otherwise, it
  shall provide an overall impact summary, such as families supported that
  month, total funds distributed, or spending by category.
- **FR-NOT-11:** When allocated funds are later paid out or spent, send a
  follow-up message about the general impact without family-identifying
  details.

The approved notification contract adds the exact type/reference mappings for
`DonationImpact`, `PayoutImpact`, `CaseStatus`, `SpecificNeedCovered`, and
`MonthlyReminder`. Exactly the reference required by the type shall be set;
unrelated references shall be null. `MonthlyReminder` has no business
reference.

**UNRESOLVED — REQUIRES DECISION:** select email and SMS providers, define
which event uses which channel, template-editing permissions, retry limits,
backoff schedule, opt-out exceptions, and notification retention/archival.
The original says the donor immediately receives an email while also allowing
channel preference; reconcile those requirements.

### 5.10 PDF reporting

- **FR-REP-1:** The system shall generate PDF reports. The original proposal
  names QuestPDF or iTextSharp; no library is selected by this requirement.
- **FR-REP-2:** Required report types are:
  1. Monthly distribution report.
  2. Donation and financial summary.
  3. Donor receipt.
  4. Anonymized case scoring summary.
  5. Payout approval trail.
  6. Audit-log extract.
- **FR-REP-3:** Reports shall include a title, date range, generator name,
  report ID, page numbers, and official footer.
- **FR-REP-4:** Reports for non-Manager roles shall apply the privacy
  restrictions in Section 4.
- **FR-REP-5:** Monthly reports shall be generated automatically, stored, and
  have download history.
- **FR-REP-6:** Reports shall support approved date-range, campaign, and status
  filters and Arabic RTL rendering. English rendering remains unresolved as
  described in Section 2.5.
- **FR-REP-7:** A generated report shall be stored with a hash for tamper
  evidence.

**UNRESOLVED — REQUIRES DECISION:** define report access by role, campaign
filter semantics, storage and download retention, regeneration behavior, and
whether an audit-log extract may contain sensitive values.

## 6. Business rules and formulas

This section gathers rules that govern more than one endpoint or module. A
listed rule remains subject to any explicit unresolved marker above.

1. Identity and case data shall be protected server-side, not only hidden in
   the user interface.
2. A donor contributes money to a fund or category and never selects or
   browses a case.
3. An unverified case is ineligible for scoring for publication and aid.
4. Manual score overrides are prohibited. Score changes arise from approved
   input corrections or a new scoring-rule version and must be audited.
5. Allocation must be atomic, must not overfund a need, and must enforce that
   a targeted need belongs to the allocation's case.
6. Financial movements are recorded in an immutable ledger. The original
   specification requires transactions but does not define accounting signs,
   balancing rules, or all rollback behavior.
7. Payout approval is separated among Manager, Payout Auditor, and Treasurer.
   The amount is locked after audit; a changed amount restarts approval.
8. A single user may not act in more than one approval step of the same payout.
9. Notification and report content must not expose family identity to roles
   that are not permitted to see it.
10. Ordinary soft deletion is selective. Financial, audit, status-history, and
    approval history must not be physically deleted through normal operations.
11. Scoring formulas and weights shall be versioned. Historical results retain
    the rule version used to calculate them.
12. The three contradictory formula pairs in Section 5.4 remain unresolved;
    no implementation may choose a version without approval.

## 7. Data and integrity requirements

The approved entity inventory, attributes, and cardinalities are maintained in
the canonical [SANAD ERD](../architecture/sanad_erd.mermaid) and
[module contracts](../integration/module-contracts.md). The following
requirements explain the relevant data behavior without duplicating the full
ERD.

### 7.1 Original data requirements

- **DR-1:** Foreign keys and indexes shall enforce approved relationships and
  support lookups, including unique National ID hash and indexes for case and
  status fields.
- **DR-2:** User-role relationships shall use the composite relationship
  defined by ASP.NET Core Identity. The project shall not create a duplicate
  `USERROLE` table or replace Identity's schema.
- **DR-3:** Family identity data shall be stored separately from case workflow
  data and linked only by an internal identifier. Manager access is for
  management purposes; Document Reviewer access is limited to identity
  validation and necessary document review. Field Officers and Donors shall not
  access family identity.
- **DR-4:** National IDs shall be encrypted at rest using AES-256-GCM.
  Duplicate detection shall use HMAC-SHA-256 with a dedicated secret key
  separate from the encryption key. Store the ciphertext and HMAC hash, and
  enforce a unique database index on `NationalIdHash`. Keys shall be supplied
  through .NET User Secrets or environment variables during development and a
  secure secret-management service in production. Keys shall never be
  committed to Git, stored in the database, exposed to the frontend, or logged.
  Decryption shall occur only in the backend after authorization. Encryption
  keys shall support rotation; HMAC-key rotation shall regenerate hashes for
  existing records.
- **DR-5:** Monetary values shall use decimal precision equivalent to
  `decimal(18,2)`. EGP is the currency in the original specification; the
  final currency policy remains unresolved.

### 7.2 Ownership, identifiers, and cardinality decisions

- **DR-6:** Module 1 owns family, family-member, case, case-survey, and
  case-status-history data.
- **DR-7:** Module 2 owns document, scoring-rule-set, scoring-result, and
  specific-need data.
- **DR-8:** Module 3 owns donor, donation-category, donation, and payment-
  transaction data.
- **DR-9:** Module 4 owns donation-allocation, ledger-entry, payout, and
  payout-approval data.
- **DR-10:** Module 5 owns notification-outbox, report, and audit-log data.
- **DR-11:** ASP.NET Core Identity is the sole source of authentication users,
  roles, and user-role relationships. `DONOR` is a business record linked to
  an existing Identity user; it is not an authentication table.
- **DR-12:** Identity IDs remain strings. Business entity primary keys remain
  GUIDs. Every business foreign key that references an Identity user uses a
  string.
- **DR-13:** A National ID is a unique identifying attribute, not a business
  primary key. Business entities use the GUID keys in the approved ERD.
- **DR-14:** The case survey is one-to-one with its case in the approved ERD.
  A family has one or more family members; a family may have zero or more
  cases. Follow the remaining cardinalities in the approved ERD.

### 7.3 History, ledger, allocation, and notification integrity

- **DR-15:** Case status history and payout approvals are immutable historical
  records. Audit records are generic, append-only records using controlled
  entity names and identifiers; they shall not add a foreign key for every
  business entity.
- **DR-16:** A ledger entry shall have exactly one populated source reference:
  Donation, Allocation, or Payout. `EntryType` must match that reference:
  `Donation`, `Fee`, and `Refund` use Donation; `Allocation` uses Allocation;
  `Payout` uses Payout.
- **DR-17:** An allocation requires a case reference. If it references a
  specific need, that need must belong to the same case.
- **DR-18:** A notification must have a required Identity recipient and
  exactly the business reference required by its type. All unrelated
  references must be null; a monthly reminder has no business reference.
- **DR-19:** Notification delivery history shall be preserved. No additional
  delivery-attempt entity is specified by the approved module contract.
- **DR-20:** For entities marked soft-deletable in the approved ERD,
  `IsDeleted = true` requires a deletion timestamp, and `IsDeleted = false`
  requires a null deletion timestamp. A deleting user ID may be null for
  system-initiated deletion.
- **DR-21:** Soft deletion applies selectively as defined by the approved ERD.
  Financial records, audit records, case-status history, payout approvals,
  case surveys, and other entities without soft-delete fields shall not be
  physically deleted through normal operations. No cascade physical deletion
  shall erase related history.

**UNRESOLVED — REQUIRES DECISION:** define National ID canonicalization before
HMAC calculation, key identifiers and rotation procedures, retention/erasure
rules, and handling of a deleted member's unique hash. National ID structural
validation details remain unresolved under FR-ID-2.

**UNRESOLVED — REQUIRES DECISION:** the ERD requires `AUDITLOG.UserId`, while
the module contract notes that system-generated audit events may have no user
and leaves nullability unresolved. Resolve this without changing the ERD
arbitrarily. Retention and privacy-erasure rules for audit values, files,
reports, notifications, and soft-deleted personal data also remain open.

## 8. Non-functional requirements

- **NFR-1 — Security:** All traffic shall use HTTPS/TLS. JWT access tokens
  shall have a 15-minute target lifetime and refresh tokens shall rotate.
  Apply OWASP Top 10 mitigations and rate-limit login and OTP endpoints.
- **NFR-2 — Privacy:** Family identity shall be available only to the
  Manager, enforced by server-side authorization and automated tests.
- **NFR-3 — Performance:** The original target is 95% of API calls under
  800 ms for 500 concurrent users, and PDF reports under 10 seconds for
  10,000 rows. Workload and measurement method require validation.
- **NFR-4 — Reliability:** The original targets are 99.5% uptime and daily
  backups. Payment and payout operations shall be atomic at approved
  transaction boundaries. Confirm operational feasibility and recovery
  objectives.
- **NFR-5 — Auditability:** Financial actions, role changes, and case-state
  changes shall be logged. Audit and historical records shall be immutable
  through normal operations.
- **NFR-6 — Maintainability:** The system shall follow the confirmed Clean
  Architecture decision. The original target is at least 70% unit-test
  coverage for Domain and scoring logic.
- **NFR-7 — Usability and accessibility:** The interface shall be responsive,
  accessible, and support Arabic RTL. The original source asks for Arabic and
  English; English remains unresolved under Section 2.5. WCAG 2.1 AA is a
  source target "where practical"; confirm the acceptance scope.
- **NFR-8 — Scalability:** API instances should be stateless. Background
  processing must support the scheduled and retry workloads in Section 9.
  This does not authorize a microservices architecture.
- **NFR-9 — Compliance:** The system shall be designed toward alignment with
  Egypt's Personal Data Protection Law (Law 151/2020), subject to legal
  review.
- **NFR-10 — Input validation:** All external input shall be validated and
  invalid requests rejected safely and consistently.

**Implementation note for NFR-10:** the original names FluentValidation. The
current backend instructions specify request DataAnnotations with API model
validation and re-checking application invariants. This requirement concerns
validation behavior; follow the current backend convention unless the project
approves a change.

**UNRESOLVED — REQUIRES DECISION:** confirm performance load profiles, uptime
measurement, backup and recovery targets, test-coverage measurement, WCAG
acceptance scope, and whether a worker may run as a separate process while
remaining part of the same modular application.

## 9. External integrations and interfaces

### 9.1 Payment

The system shall integrate with a payment gateway for donations and refunds.
The original names Paymob and Fawry as Egyptian-supported options and Stripe
as an optional international option. No production provider is selected.
Gateway-hosted checkout, HMAC-verified webhooks, idempotency, and no handling
of raw card data are required as described in Section 5.6.

### 9.2 Email and SMS

Email and SMS delivery are required for V1. The source names SMTP/SendGrid for
email and Twilio or a local Egyptian provider for SMS. No provider is selected.
Email verification and optional phone OTP are part of the original account
requirements. Arabic templates are required; English templates remain
unresolved.

### 9.3 Document storage and PDFs

Proofs shall be restricted to PDF/JPG/PNG, limited to 5 MB each, and stored
outside the web root. PDF generation is required; the source names QuestPDF
and iTextSharp as alternatives but selects neither. Storage provider, retention,
and PDF implementation are not specified by this requirements document.

### 9.4 Database and proposed API groups

SQL Server is the confirmed database. The source proposes API groups:
`/api/auth`, `/api/cases`, `/api/documents`, `/api/scoring`, `/api/needs`,
`/api/donations`, `/api/payments/webhook`, `/api/payouts`,
`/api/allocations`, `/api/funds`, `/api/notifications`, `/api/reports`, and
`/api/admin`.

These are indicative groups from the original specification, not approved
endpoint paths or payload contracts. Exact API contracts must follow approved
use cases and module ownership.

## 10. Background jobs and automated processes

- **BJ-1:** Process queued notifications through the outbox, retry failures
  with exponential backoff, record delivery status, and make failures visible
  to the Manager.
- **BJ-2:** Generate and store monthly reports, with download history.
- **BJ-3:** Expire failed or abandoned pending payments after 30 minutes.
- **BJ-4:** Expire stale draft cases according to a policy that remains to be
  decided.
- **BJ-5:** Recalculate waiting days, W, and PriorityScore daily. BaseScore
  shall not be recalculated by elapsed time alone.
- **BJ-6:** Run allocation over confirmed donations and eligible open
  case/need balances according to the approved allocation policy.
- **BJ-7:** Send monthly payout reminders and perform the proposed monthly
  payout run only as permitted by the approved payout workflow.

**UNRESOLVED — REQUIRES DECISION:** define schedules, time zones, job
idempotency, retry limits, stale-draft duration, job overlap/locking, operational
monitoring, and the relationship between a monthly payout run and individual
Manager authorization. The hosting provider is not constrained.

## 11. Use cases and workflows

These use cases preserve the source workflows. They do not settle the
unresolved rules or API shapes listed elsewhere.

### UC-1 — Donate

1. Donor chooses an amount and general fund or approved donation category; no
   case is displayed or selectable.
2. The system creates a pending donation and presents the approved gateway
   checkout.
3. The gateway reports the result by webhook.
4. The system verifies the signature and processes the transaction
   idempotently.
5. On confirmed success, record the donation/ledger effects and queue a receipt
   and privacy-safe impact message.

### UC-2 — Register and verify a case

1. Authorized staff enter family and member information.
2. The system validates National IDs and rejects duplicates.
3. Staff create a case using an anonymous Case ID and submit required proofs.
4. The Document Reviewer reviews only the allowed evidence and verifies or
   rejects with a reason.
5. The system records status history. The case is ineligible for scoring and
   aid until verified.

The role visibility and Module 1/Module 2 ownership conflict in this workflow
must be resolved before implementation.

### UC-3 — Score a case

1. The system loads verified case inputs and the active scoring-rule version.
2. It calculates component scores, BaseScore, waiting-time factor, and
   PriorityScore.
3. It stores the breakdown, inputs, and rule version for auditability.
4. The case is ordered for allocation according to the approved priority
   policy and remains invisible to donors.

Execution is blocked until the conflicting formulas and related scoring
decisions are approved.

### UC-4 — Request and fund a specific need

1. A family requests an item.
2. A Valuation/Field Officer records its approved description and price.
3. Confirmed donations are allocated without exceeding the remaining need
   amount.
4. The charity purchases the item; the system tracks funding and closes the
   need when fully covered.

### UC-5 — Authorize and execute a payout

1. The Manager authorizes an amount for an anonymous Case ID.
2. The Payout Auditor checks the evidence, score, approved amount/ceiling, and
   duplicate-payout rule; the Auditor approves or returns it with a reason.
3. The Treasurer executes only an audit-approved payout and records its
   transfer reference.
4. Each step is recorded, and a privacy-safe impact notification may be queued.

### UC-6 — Generate a monthly report

1. A scheduled job generates the approved monthly PDF.
2. The report includes required metadata and role-appropriate anonymization.
3. An authorized user downloads it; generation and download history are
   retained according to the approved retention policy.

## 12. Edge cases and failure handling

The system shall handle the following cases without bypassing privacy,
authorization, or financial integrity. Where the source does not define the
outcome, the behavior is unresolved rather than implied.

- Invalid or duplicate National ID; impossible date or unknown governorate
  code.
- A Field Officer attempting data entry that would expose identity contrary
  to the approved role policy.
- Missing, invalid, rejected, or replaced proof; rejection without a reason;
  unauthorized review or document access.
- Scoring with zero family size, zero income, negative income after rent,
  missing survey values, or no school-age children.
- Waiting time at 0, 30, 60, and 90 days; a case rejected while corrections
  are pending; a case resubmitted after rejection.
- Concurrent allocation attempts, a need with no remaining balance, an
  allocation targeting another case's need, and excess funding.
- Replayed, invalidly signed, delayed, failed, or abandoned payment webhooks;
  refund after allocation; gateway timeout.
- Payout attempted out of order, by a user who already performed another
  approval step, with a changed amount, or for a duplicate period.
- Notification-provider failure, repeated delivery, user opt-out, and a
  transaction receipt that must still be sent.
- PDF generation failure, large report volumes, sensitive fields in an export,
  and an expired or unauthorized download.
- Attempts to physically delete financial or historical records.

**UNRESOLVED — REQUIRES DECISION:** define retry exhaustion and recovery for
payments, notifications, jobs, and report generation; user-facing errors and
remediation for each failure; duplicate-payout period; and zero/missing-input
scoring behavior. Do not infer financial rollback or accounting behavior.

## 13. Acceptance criteria

The following acceptance criteria are retained from the original specification.
They are not evidence that the feature is already implemented.

- **AC-01:** A valid 14-digit Egyptian National ID yields the approved birth
  date, gender, and governorate; duplicate IDs across families are rejected.
- **AC-02:** A case cannot be scored for publication or receive aid before it
  is Verified.
- **AC-03:** For approved scoring formulas and inputs, the engine returns the
  documented component breakdown and BaseScore. This criterion is blocked
  until the formula discrepancies in Section 5.4 are resolved.
- **AC-04:** For `D = 0, 30, 60, 90`, the source expects `W = 1.00, 1.15,
  1.30, 1.30` using `lambda = 0.30` and `D_max = 60`.
- **AC-05:** A scoring-rule set with weights not summing to 1.00 or outside
  `[0,1]` is rejected. A valid save creates a new version, and historical
  results retain their original version.
- **AC-06:** A non-Manager token never receives family identity fields from an
  API response or PDF.
- **AC-07:** A Donor token receives HTTP 403 from case endpoints and cannot
  browse, search, or select cases.
- **AC-08:** A Treasurer cannot execute before Auditor approval, and no user
  can perform two approval steps for the same payout.
- **AC-09:** Replaying a payment webhook does not credit a donation twice.
- **AC-10:** After confirmed donation, the donor receives the approved
  receipt/impact communication, which contains no family-identifying details.
- **AC-11:** The monthly PDF is generated automatically and is downloadable
  by an authorized user.

Acceptance tests for these criteria must also cover the privacy, integrity,
authorization, and cross-module contracts applicable to the feature.

## 14. Unresolved decisions and consistency register

All entries below are **UNRESOLVED — REQUIRES DECISION** unless a separate
confirmed decision is explicitly stated.

### 14.1 Conflicting source formulas

The two supplied requirement texts disagree on these expressions:

1. **IPC:** `(MonthlyIncome - MonthlyRent) + FamilySize` versus
   `(MonthlyIncome - MonthlyRent) / FamilySize`.
2. **Medical burden M:** `25 * min(1, MonthlyMedicalSpend + MonthlyIncome)`
   versus `25 * min(1, MonthlyMedicalSpend / MonthlyIncome)`.
3. **Education Edu:** `100 * (1 - ChildrenInSchool + SchoolAgeChildren)`
   versus `100 * (1 - ChildrenInSchool / SchoolAgeChildren)`.

The second text provides interpretations suggesting division, but those
interpretations are not an approved decision. Preserve the alternatives until
the source owner/stakeholders confirm the intended formulas. Do not implement
the mathematically plausible expression by assumption.

### 14.2 Roles, cases, and documents

- The identity-access responsibilities for Manager, Document Reviewer,
  Field Officer, Beneficiary, and Donor are stated in Sections 4 and 5. The
  Beneficiary-to-Family relationship and authorization to submit family data
  remain unresolved and out of scope.
- Finalize data-access permissions for Payout Auditor and Treasurer.
- Agree how Module 2's Document Reviewer changes Module 1-owned case status
  and how proof status differs from case status.
- Approve case correction/resubmission, reopening, survey timing, and all
  transition authorization.
- Complete the role/permission matrix for reads, updates, exports, and
  deletion.
- Define document replacement, storage, scanning, retention, and review
  lifecycle.

### 14.3 Scoring

- Approve `T`, point tables, final weights, `lambda`, `D_max`, rounding,
  precision, priority-band boundaries, and tie-breaking.
- Define behavior for zero income, zero family size, negative post-rent
  income, missing inputs, and the waiting clock during rejected/correction
  states.
- Disambiguate the source's use of `D` for disability and waiting days without
  changing the approved formula.
- Decide whether an active rule-set change recalculates open cases and which
  historical values remain immutable.

### 14.4 Identity, language, and data lifecycle

- Confirm English support from the original specification against the
  repository decision that English is unresolved.
- Confirm National ID structure, governorate codes, date validation, and any
  check-digit algorithm before implementing the parser. AES-256-GCM encryption
  and HMAC-SHA-256 duplicate detection with a separate key are approved;
  canonicalization and operational key-rotation procedures remain to be
  finalized.
- Define personal-data retention, erasure, soft-deleted record access, document
  retention, and audit-value redaction.
- Resolve whether system-generated audit events may have null `AUDITLOG.UserId`.

### 14.5 Payments, allocation, and payouts

- Select payment gateway(s), SMS/email providers, supported methods, and
  reconciliation process.
- Decide whether the operating fee is enabled, how it is calculated, and
  treatment of fees/refunds in the ledger.
- Define payment status transitions, webhook response/retry behavior, and
  refund processing.
- Define category-specific fund eligibility, allocation cadence, concurrency,
  rollback, overfund handling, and treatment of unallocated balances.
- Decide whether Manager approval is required for every payout or only above a
  threshold; define failed, cancelled, and retried payout behavior.
- Define accounting semantics, amount signs, cash evidence, and the meaning
  of a duplicate payout in the same period.

### 14.6 Notifications, reports, jobs, and non-functional targets

- Select email/SMS providers and define event-to-channel behavior, retry limits,
  backoff, template editing, opt-out exceptions, and retention.
- Reconcile mandatory immediate email receipts with donor channel preference.
- Define report access, filters, storage, retention, regeneration, and
  sensitive-data rules.
- Define job schedules, time zones, overlap prevention, retry exhaustion, and
  stale-draft expiry.
- Confirm performance and uptime measurement, backup/recovery targets,
  coverage measurement, WCAG acceptance scope, and worker deployment model.
- Confirm that proposed API group names and response contracts meet approved
  use cases; no business endpoint paths are currently approved by the module
  contracts.

### 14.7 Reconciliation with approved project documents

- **Resolved Identity key conflict:** the older supplied ERD assigns GUIDs to
  `USER`, `ROLE`, and their references. The confirmed project decision and
  approved ERD use ASP.NET Core Identity string keys and GUID business keys.
  This requirements document follows the confirmed decision; do not change the
  ERD or create duplicate Identity tables.
- **Canonical schema:** the older embedded ERD is not authoritative. The
  approved ERD and module contracts include current Identity references,
  selective soft deletion, typed notification references, and the ledger
  allocation reference. The canonical ERD remains unchanged by this document
  update.
- **Verification ownership:** the original assigns case verification to the
  Document Reviewer, while project ownership places `CASE` in Module 1 and
  `DOCUMENT` in Module 2. This is a coordination issue, not permission to
  redistribute module ownership.
- **Validation technology:** the original names FluentValidation; current
  backend instructions specify DataAnnotations and application-level invariant
  checks. The functional requirement is input validation; follow the confirmed
  implementation convention unless an ADR changes it.
- **English localization:** original requirements include English, while
  current project guidance leaves English and additional languages unresolved.
  Arabic remains confirmed; English must be decided explicitly.
- **V1 delivery:** core business features, payment, email, and SMS are required
  for V1, even though provider choices and multiple business rules remain open.
  Requirements are not to be reduced to the currently implemented foundation.

## 15. Traceability and supporting documents

- [Project overview](../project-overview.md)
- [Foundation architecture](../architecture/foundation.md)
- [Approved ERD](../architecture/sanad_erd.mermaid)
- [Module contracts](../integration/module-contracts.md)
- [Team assignments](../team/README.md) and
  [`assignments/`](../team/assignments/)
- [Backend instructions](../../backend/AGENTS.md)
- [ADR-001: .NET 10 LTS](../decisions/ADR-001-dotnet-10-lts.md)

The original supplied text contains `[cite: n]` markers but no accompanying
bibliography or source pages in the repository. Requirement identifiers and
the substantive requirement content are retained; the unresolved formula
discrepancies are recorded above rather than treated as verified citations.
