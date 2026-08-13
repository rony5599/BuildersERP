# BuilderERP

An ASP.NET Core MVC ERP system for construction and real estate companies, covering the full project lifecycle — from land acquisition and sales through construction, contractor/labor management, and post-handover facility management.

## Architecture

BuilderERP follows a Clean Architecture layering:

```
src/
  BuilderERP.Domain          Entities, enums, repository interfaces (no dependencies)
  BuilderERP.Application     CQRS use cases (MediatR), DTOs, validators, mappings
  BuilderERP.Infrastructure  EF Core DbContext, migrations, repositories, identity, authorization
  BuilderERP.Shared          Cross-cutting: permission constants, middleware, attributes
  BuilderERP.Web             ASP.NET Core MVC UI: controllers, views, Swagger, entry point
tests/
  BuilderERP.UnitTests
  BuilderERP.IntegrationTests
```

Dependency direction flows inward: `Web` → `Infrastructure`/`Application` → `Domain`. `Domain` has no project references.

### Layer details

- **Domain** — ~95 POCO entities (e.g. `Project`, `LegalCase`, `FlatHandover`) and ~75 enums, plus `IRepository`/`IUnitOfWork` abstractions.
- **Application** — one `Features/<Module>` folder per business module containing MediatR commands/queries and handlers, FluentValidation validators, AutoMapper profiles, and shared DTOs/`PagedResult`. Reporting lives under `Features/Reports` (report specs + Excel/PDF export via ClosedXML and QuestPDF).
- **Infrastructure** — `Persistence/AppDbContext.cs` (EF Core, SQL Server), `Persistence/Migrations/`, generic repository + unit-of-work implementations, ASP.NET Core Identity setup, and a custom claims/permission-based authorization provider.
- **Shared** — `PermissionAuthorizeAttribute`, `PermissionNames`/`RoleNames` constants, and global exception-handling middleware.
- **Web** — ~100 controllers (mostly one per module) with matching Razor views, Bootstrap/jQuery front end (no SPA framework), Swagger for API exploration, and Serilog logging.

## Modules

The application covers the end-to-end construction/real-estate ERP lifecycle:

| Area | Examples |
|---|---|
| Land | Registrations, Mutations, Documents |
| CRM | Leads, Inquiries, FollowUps, Customers |
| Sales | Sale Agreements, Quotations, Installment Plans |
| Property | Buildings, Towers, Floors, Property Units, Parking Slots |
| Procurement | Purchase Orders/Requisitions/Returns, RFQs, Vendor Quotations, Rate Contracts |
| Inventory | Stocks, Stock Adjustments/Issues/Returns/Transfers, Warehouses |
| Construction / Project Management | Projects, Milestones, WBS Tasks, Daily Progress, BOQ Items, Budget Lines |
| Engineering | Drawings, Drawing Approvals/Revisions |
| Contractor Management | Contractors, Contractor Ledgers, Running Bills |
| Labor | Workers, Attendance, Overtime, Salaries |
| Equipment | Equipment, Rentals, Fuel Logs, Operator Assignments |
| Quality | Quality Checklists, NCRs, Material Inspections, Test Reports |
| Safety | Safety Audits/Inspections/Trainings, Incident Reports, PPE Tracking, Risk Assessments |
| Document Management | Documents, Document Versions |
| Legal | Legal Agreements, Legal Cases, Legal Notices |
| After Handover | Flat Handovers, Maintenance Requests, Warranties, Defect Records, Snag Items |
| Facility Management | Apartment Maintenance, Common Area Bookings, Security Incidents, Visitor Logs, Service Tickets, Utility Bills |
| Dashboard & Analytics | Cross-module dashboards |
| Reporting | Ad-hoc reports with Excel/PDF export |

## Tech Stack

- **.NET 8.0** across all projects
- **ASP.NET Core MVC** (Razor views, Bootstrap, jQuery)
- **EF Core 8** with **SQL Server**
- **ASP.NET Core Identity** with a custom permission/claims-based authorization system
- **MediatR** (CQRS), **AutoMapper**, **FluentValidation**
- **ClosedXML** (Excel export), **QuestPDF** (PDF export)
- **Serilog** (logging), **Swashbuckle/Swagger** (API docs)

## Authentication & Authorization

Uses ASP.NET Core Identity (`ApplicationUser`/`ApplicationRole`) with a custom permission-based authorization layer:

- `PermissionAuthorizeAttribute` (Shared) guards controllers/actions by permission name.
- `PermissionPolicyProvider` / `PermissionAuthorizationHandler` / `PermissionClaimsTransformation` (Infrastructure) resolve permissions dynamically at runtime.
- Roles and permissions are seeded via `Infrastructure/Identity/SeedData.cs`.
- Password policy: minimum 8 characters, no non-alphanumeric requirement, lockout after 5 failed attempts (15 minutes), unique email required.
- `AccountController`, `RolesController`, and `UsersController` (Web) handle login and role/user administration.

## Database

- DbContext: `src/BuilderERP.Infrastructure/Persistence/AppDbContext.cs`
- Provider: SQL Server, with `AuditSaveChangesInterceptor` populating audit fields automatically
- Migrations: `src/BuilderERP.Infrastructure/Persistence/Migrations/`
- Connection string key: `ConnectionStrings:DefaultConnection` in `src/BuilderERP.Web/appsettings.json`

> **Note:** `appsettings.json` currently contains a development connection string with embedded credentials. Consider moving this to `appsettings.Development.json`, environment variables, or [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) before this repository becomes public or is shared more broadly.

### Applying migrations

```powershell
dotnet ef database update --project src/BuilderERP.Infrastructure --startup-project src/BuilderERP.Web
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (local or remote) reachable via the connection string in `appsettings.json`

### Run locally

```powershell
dotnet restore
dotnet ef database update --project src/BuilderERP.Infrastructure --startup-project src/BuilderERP.Web
dotnet run --project src/BuilderERP.Web
```

The app launches (via `launchSettings.json`) on `https://localhost:7153` / `http://localhost:5249` in the `Development` environment. Swagger UI is available under `/swagger` in development.

### Run with Docker

```powershell
docker build -t builderERP .
docker run -p 8080:8080 -p 8081:8081 builderERP
```

The image is a multi-stage build (`aspnet:8.0` runtime, `sdk:8.0` build) that publishes and runs `BuilderERP.Web.dll`. A SQL Server instance must be provisioned and reachable separately — there is currently no bundled `docker-compose.yml`.

## Tests

```powershell
dotnet test
```

- `tests/BuilderERP.UnitTests` — validator and unit-level tests
- `tests/BuilderERP.IntegrationTests` — integration tests

## Solution Layout

```
BuilderERP.sln
src/
  BuilderERP.Domain/
  BuilderERP.Application/
  BuilderERP.Infrastructure/
  BuilderERP.Shared/
  BuilderERP.Web/
tests/
  BuilderERP.UnitTests/
  BuilderERP.IntegrationTests/
Dockerfile
```
