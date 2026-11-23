# ADR-001: Use .NET 10 LTS with pinned compatible packages

Status: Accepted
Date: 2026-11-23

## Context

The team agreed on .NET 10 (LTS) for the backend and a short delivery timeline. The environment also contained an older .NET 9 SDK, and NuGet "latest" package resolution selected .NET 11-era packages that are incompatible with `net10.0`.

## Problem

Which framework and package baseline should the foundation use so the solution builds reproducibly without introducing unsupported runtime assumptions?

## Options

- Option A: .NET 10 LTS with explicitly pinned compatible package versions and the classic `SANAD.sln` format.
- Option B: .NET 11 with the newest packages.
- Option C: .NET 8 with older packages.

## Decision

Use .NET 10 LTS. Keep EF Core, Identity, JWT bearer, `Mvc.Testing`, and EF InMemory packages at `10.0.15`, pin `Swashbuckle.AspNetCore` to `9.0.6`, and pin `System.IdentityModel.Tokens.Jwt` to `8.23.0`. Use the classic `.sln` format rather than the newer `.slnx` default.

.NET 8 was rejected because it is out of support as of 2026-11-10. .NET 11 was rejected because the team explicitly chose an LTS baseline. Swashbuckle `10.x` was rejected because its Microsoft.OpenApi 2 API is incompatible with the classic OpenAPI security-scheme configuration used by the API.

## Consequences

- The solution builds with a .NET 10 SDK and is reproducible from committed package versions.
- Developers with only an older system SDK must install/select .NET 10 explicitly.
- Package pins must be reviewed deliberately when the team upgrades the target framework.
- No hosting-provider or availability assumption is introduced by this decision.

## Follow-up

If the team later approves .NET 11 or a different Swagger client generator, record a new ADR and update all package references together.
