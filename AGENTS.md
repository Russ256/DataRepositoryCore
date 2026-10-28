# Repository Guidelines

## Project Structure & Module Organization

- `DataRepositoryCore.Abstractions/` contains entity and repository interfaces.
- `DataRepositoryCore/` implements EF Core repositories, the data context, and dependency injection registration.
- `DataRepositoryCore.Tests/` contains TUnit tests using EF Core's in-memory provider. Add tests alongside `RepositoryTests.cs` or in appropriately named test classes.
- `Directory.Build.props` holds shared framework, nullable, packaging, and compiler settings. The solution is `DataRepositoryCore.sln`.

## Build, Test, and Development Commands

The solution targets `net11.0` and currently uses an EF Core 11 release candidate; install a compatible .NET SDK.

- `dotnet restore DataRepositoryCore.sln` restores dependencies.
- `dotnet build DataRepositoryCore.sln` builds the projects; library builds also generate NuGet packages.
- `dotnet test DataRepositoryCore.sln` builds and runs the test suite.

This repository is a library, so there is no standalone application to run. Consumers supply an EF Core context and register repositories with `AddDataRepositories()`.

## Coding Style & Naming Conventions

- Use four spaces for indentation, file-scoped namespaces, and the existing C# style.
- Use PascalCase for types, public members, and methods; prefix interfaces with `I`; use camelCase for parameters and local variables. 
- Preserve nullable annotations and pass cancellation tokens through asynchronous operations. 
- Warnings are treated as errors. No repository-specific formatter or lint configuration is currently defined.
- Do not use `var`; use explicit types for clarity.

## Testing Guidelines

- Use TUnit `[Test]` methods and descriptive PascalCase test names describing the behavior under test. 
- Use a unique in-memory database per test to keep tests isolated. 
- Cover observable repository behavior, including tracking differences between read and write repositories, and run `dotnet test DataRepositoryCore.sln` before submitting changes. 
- No coverage threshold is configured. 
- The in-memory provider does not establish behavior specific to a relational database.
- Use the AAA (Arrange, Act, Assert) pattern for test structure.

## Commit & Pull Request Guidelines

- Recent commits use short, informal, imperative or descriptive subjects, such as `fix the logger warnings` and `Set AsTracking on update repo`. 
- Keep each subject focused on the change. 
- Pull requests should explain the behavior changed, link any relevant issue, and report the build and tests run. 
- Include screenshots only when they help explain a visible change.
