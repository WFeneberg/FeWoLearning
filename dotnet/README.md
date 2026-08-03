# .NET / C# Track

Test-driven C# exercises for .NET 10, using **xUnit**. This is the deepest track:
beyond language katas it grows into the frameworks you architect with — WPF,
Avalonia, Uno, and Blazor (see the advanced/expert tiers in
[`catalog.md`](catalog.md)).

## Project layout

The `.slnx` solution contains two projects:

- `exercises/FeWoLearning.Exercises.csproj` — class library holding the **stubs**
  you implement. Stubs `throw new NotImplementedException()` so the solution
  always compiles and only the unfinished exercise's tests fail.
- `tests/FeWoLearning.Exercises.Tests.csproj` — the xUnit tests.

Reference implementations live under `solutions/<tier>/` and are **not** part of
the build (they share type names with the stubs by design). To check a solution,
copy it over the matching stub, or diff against it.

> Namespaces are fixed per tier (`FeWoLearning.Exercises.Beginner`, `.Intermediate`,
> `.Advanced`, `.Expert`) and do **not** follow the `NN-tier` folder names, since
> C# identifiers cannot start with a digit.

## Commands

| Action                       | Command                                                             |
|------------------------------|---------------------------------------------------------------------|
| Run all tests                | `dotnet test`                                                       |
| Run one class's tests        | `dotnet test --filter FullyQualifiedName~Ex001_FizzBuzz`            |
| Run one tier                 | `dotnet test --filter FullyQualifiedName~Tests.Beginner`           |
| Build only                   | `dotnet build`                                                     |

See [`catalog.md`](catalog.md) — the 100-row progress ledger. This track is **complete: 100 / 100**.
