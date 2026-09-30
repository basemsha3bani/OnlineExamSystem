# Repository and framework maintenance

The solution targets .NET 8. Install a current .NET 8 SDK (and the .NET 8 Hosting Bundle on IIS). Entity Framework packages are aligned to 8.0.31. AutoMapper 15.1.3 replaces the custom IModelMapper implementations.

## Context lifetime

Startup registers DbConext as scoped. ContextGateway<T> receives that context by constructor injection; it has no static database or transaction state. Repository operations map incoming models onto tracked entities and save once per aggregate. Gateways in one request share the context, while different requests receive different contexts.

The examiner attempt service uses an injected IDbContextFactory<DbConext> for short-lived operations. The background worker opens a dependency-injection scope and evaluation creates a separate context per attempt. Gateways do not dispose the injected context; dependency injection owns it. Explicit transactions are disposed by their coordinating operation.

## Mapping

RepositoryMappingProfile contains maps for difficulty levels, study subjects, questions, answer options, exams, sections, rules, and users. Its configuration is validated when the mapper is initialized. Update operations reconcile existing child records by ID rather than replacing tracked collections. User output maps do not return password hashes.

AutoMapper 15 uses license configuration. Supply the appropriate license through the AUTOMAPPER_LICENSE_KEY environment variable; do not commit a key. See https://docs.automapper.io/en/stable/License-configuration.html for the vendor's licensing configuration. Missing-license messages are not suppressed by this application.

## Exam details

The controller name is corrected to ExamSectionsController. The Add Section button loads a partial form, and a delegated jQuery submit handler saves it and replaces the section list. Invalid submissions return HTTP 422 with the editable form; other AJAX failures display an error. Form POSTs require an antiforgery token.

## Deployment and verification

This maintenance refactor adds no database schema changes. The earlier ExaminerAttempts migration still needs applying where it has not already been deployed. Historical migration/schema drift documented in examiner-module.md remains unchanged.

The newer SQL Server client enables encryption by default. Use a trusted SQL Server certificate for deployment. Configure any local development certificate exception explicitly in local settings rather than weakening production connection defaults. Existing SQL Server databases must also support the SQL features used by EF Core 8 (including OPENJSON for parameterized collections).

The local tool manifest pins EF CLI tools to version 8.0.31. Run:

```powershell
dotnet tool restore
dotnet build OnlineExamSystem.sln
dotnet run --project tests/ExaminerFlowChecks/ExaminerFlowChecks.csproj
```

Integration checks use a uniquely named, disposable LocalDB database and exercise both the examiner flow and the refactored repositories. They also start a local HTTP host to verify the section form and submission routes.
