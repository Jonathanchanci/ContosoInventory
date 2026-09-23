# AGENTS.md

## Project overview

This repository contains a .NET 8 sample application for managing Contoso inventory.

- `ContosoInventory/ContosoInventory.Server/`: ASP.NET Core Web API, Entity Framework Core, SQLite, and ASP.NET Core Identity
- `ContosoInventory/ContosoInventory.Client/`: Blazor WebAssembly frontend
- `ContosoInventory/ContosoInventory.Shared/`: DTOs shared across client and server
- `README.md`: primary project overview and setup steps

## Essential commands

Run from the repository root:

```bash
dotnet build ContosoInventory/ContosoInventory.sln
```

Start the API/application from the server project:

```bash
cd ContosoInventory/ContosoInventory.Server
dotnet run
```

If a new test project is added later, use:

```bash
dotnet test
```

## Architecture and conventions

- The server exposes a REST API through controllers under `Controllers/` and uses a service layer in `Services/`.
- Domain logic should live in the service layer, not directly in controllers.
- DTOs live in `ContosoInventory.Shared/DTOs` and are the contract boundary between server and client.
- Authentication is cookie-based and uses ASP.NET Core Identity with role-based access checks.
- The database is SQLite and initialized via `Data/DbInitializer.cs` when the app starts.
- The client is Blazor WebAssembly; UI pages call typed API services rather than directly hitting HTTP in components.

## Repo-specific patterns

- Prefer the existing service/controller pattern when adding new API features.
- Keep API requests and responses strongly typed via shared DTOs.
- For category-related operations, follow the current authorization pattern:
  - `GET` endpoints are usually accessible to authenticated users
  - create/update/delete actions require the `Admin` role
- Duplicate validation is case-insensitive in the category service and should be preserved when adding similar logic.
- When you add or modify a server API contract, update the matching client service and any consuming UI code in the same change.

## Files worth checking first

- [README.md](README.md)
- `ContosoInventory/ContosoInventory.Server/Program.cs`
- `ContosoInventory/ContosoInventory.Server/Controllers/CategoriesController.cs`
- `ContosoInventory/ContosoInventory.Server/Services/CategoryService.cs`
- `ContosoInventory/ContosoInventory.Client/Program.cs`

## Working guidance for AI agents

- Keep changes small and consistent with the current architecture.
- Prefer matching the style and patterns already in the codebase over introducing new abstractions.
- Validate with `dotnet build` after C# changes; if you add tests, run them.
- Do not remove existing security settings or authentication patterns without a clear reason.
- Prefer updating shared DTOs and corresponding service methods together so the client and API stay in sync.

## Notes

This project is intentionally a learning starter app. Maintain clarity and simplicity over framework-heavy patterns unless a feature truly requires them.
