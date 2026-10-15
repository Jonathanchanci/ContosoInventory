<!--
Sync Impact Report
- Version change: 0.0.0 -> 1.0.0
- Modified principles: N/A -> I. Architecture-First, II. Contract Integrity, III. Security & Authorization, IV. Verification & TDD, V. Simplicity & Maintainability
- Added sections: Additional Constraints, Development Workflow
- Removed sections: None
- Deferred items: TODO(RATIFICATION_DATE): original ratification date was not recorded in the repo; confirm before final approval.
-->

# ContosoInventory Constitution

## Core Principles

### I. Architecture-First
The application architecture is a layered system: the API server owns business logic, controllers remain HTTP adapters, the service layer owns validation and orchestration, and the client consumes typed DTO contracts instead of directly binding database entities. Any change that alters a data flow or responsibility boundary MUST preserve this separation.

This rule preserves maintainability in a sample application that intentionally demonstrates clear boundaries. When a server feature is added or changed, the implementation MUST live in the server service layer and the corresponding client contract and UI flow MUST stay in sync.

### II. Contract Integrity
All API requests and responses MUST use shared DTOs from the shared project, and the server MUST never expose ORM entities directly. If a contract changes, the server, shared DTO definitions, and any consuming client code MUST be updated in the same change set.

This rule prevents drift between the front end and the back end. Contract changes are a product decision and MUST be reviewed as a compatibility decision, not as an isolated convenience change.

### III. Security & Authorization
Authentication and authorization are mandatory safeguards for any operation that reads or modifies inventory data. The application MUST preserve role-based access checks, especially the requirement that administrative actions remain restricted to the Admin role while general reads remain available to authenticated users.

This rule exists because data integrity and least privilege are non-negotiable in any production-facing application. Cookie-based identity flows and role checks are part of the system contract and must not be weakened casually.

### IV. Verification & TDD
All code changes MUST be validated with the smallest relevant build or test command before completion is claimed. When bug fixes or behavioral changes are introduced, the project MUST prefer a failing test or a focused reproduction before implementing the fix.

This rule keeps the project honest and prevents undocumented regressions. The repository already expects .NET validation through the solution build, and changes that affect contracts or business logic MUST be checked against that baseline.

### V. Simplicity & Maintainability
The codebase MUST favor clear, explicit workflows over unnecessary abstractions, framework-heavy patterns, or duplicated logic. Public classes, interfaces, and methods MUST use naming conventions consistent with the repo style, and complex business rules MUST be documented in code only where the intent is not obvious.

This rule keeps the sample app understandable to new contributors. Simplicity is not a preference; it is the default standard for this repository.

## Additional Constraints

The project is a .NET 8 application composed of a Blazor WebAssembly client, an ASP.NET Core Web API server, and a shared DTO project. It uses SQLite with EF Core and ASP.NET Core Identity, and it is intentionally designed as a learning starter app with straightforward patterns rather than enterprise complexity.

The following constraints are mandatory:

- Use the service layer for business logic and keep controllers focused on HTTP concerns and authorization.
- Use repository-style data access patterns and dependency injection for all service dependencies.
- Preserve case-insensitive duplicate validation for category operations.
- Return appropriate HTTP status codes and structured logging for exceptions and errors.
- Keep identifiers consistent with the repository conventions: PascalCase for public members, camelCase for locals, and an I prefix for interfaces.

## Development Workflow

The repository workflow MUST be explicit, reviewable, and contract-aware:

- Add or modify API contracts and the matching shared DTOs together.
- Update the corresponding client service and any UI consumers when server behavior changes.
- Keep changes small and aligned with the existing patterns in the project rather than introducing unrelated abstractions.
- Review authorization boundaries before merging features that create, update, or delete inventory data.
- Validate with the solution build before closing a change and record the result in the review process.

## Governance

This Constitution governs all design and implementation work in this repository. It supersedes ad hoc practices and informal conventions when a conflict exists. Any change to project structure, API boundaries, security rules, or delivery workflow MUST be documented here before the corresponding work proceeds.

Amendments MUST follow this process:

1. Document the proposed change to the Constitution.
2. Explain the governance rationale and the impacted project principles.
3. Update the version number according to the versioning rule below.
4. Review the amendment against current repository standards and security expectations.

Versioning policy:

- MAJOR: Backward-incompatible governance changes or principle removals/redefinitions.
- MINOR: Added or materially expanded principles or sections.
- PATCH: Clarifications, wording improvements, or non-semantic fixes.

Compliance review expectation:

- All pull requests and implementation work MUST be checked against the Constitution before merge.
- If a change conflicts with a principle, the conflict MUST be resolved or explicitly justified before the work is accepted.
- This repository MUST maintain a clear distinction between governance rules and implementation convenience.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): original ratification date was not recorded in the repository; confirm before final approval. | **Last Amended**: 2026-10-13
