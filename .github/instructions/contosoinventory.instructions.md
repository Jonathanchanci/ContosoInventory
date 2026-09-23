# ContosoInventory instructions

## Scope

This repository is a .NET 8 sample app with a Blazor WebAssembly client, ASP.NET Core Web API, and shared DTO layer. Follow the repo's existing patterns when making changes.

## Architecture

- Server project: `ContosoInventory/ContosoInventory.Server/`
- Client project: `ContosoInventory/ContosoInventory.Client/`
- Shared DTO project: `ContosoInventory/ContosoInventory.Shared/`

Use the service layer for business logic and keep controllers focused on HTTP concerns and authorization.

## Build and validation

Run from the repo root:

```bash
dotnet build ContosoInventory/ContosoInventory.sln
```

Keeping validation lightweight is preferred when the change is narrow; do not claim a fix is complete without a build or relevant verification result.

## Coding conventions

- Preserve ASP.NET Core controller attribute usage and authorization roles.
- Use DTOs for API contracts instead of exposing entity models directly.
- Keep duplicate-name checks case-insensitive when working with category logic.
- Use the existing style for logging and error handling.
- When adding a new API operation, update the corresponding client service and any UI consumers in the same patch.

## References

- [README.md](../../README.md)
- [AGENTS.md](../../AGENTS.md)
