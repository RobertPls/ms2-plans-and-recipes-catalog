---
name: unit-test-generator
description: Generates unit tests (xUnit + Moq) for the ms2-plans-and-recipes-catalog microservice, reading and applying the rules defined in ai/project/unit-testing-rules.md. Use it when asked to generate, create or write unit tests for one source file (an entity, Value Object, factory or CQRS handler) or for an entire folder of the project. Also when extending Domain or Application coverage. It ONLY creates tests, it NEVER runs them. Language: EN.
---

# Unit Test Generator

Generates unit tests for the **ms2-plans-and-recipes-catalog** project, strictly following the
governance rules of the harness. **This skill ONLY creates test files. It does NOT build, run or
validate them.**

## Mandatory flow

### 1. Read the rules (always)

Before writing any test, **read the current rules**:

- Rules: `ai/project/unit-testing-rules.md`
- Test project structure and checklist: `ai/project/README.md`

These paths are relative to the repository root. Always apply the **latest** version of those
rules. If they do not exist, stop and tell the user before continuing.

### 2. Understand the source code under test

- Identify the **layer** of the target file/folder:
  - `Domain/` -> entities, Value Objects, Factories, business rules.
  - `Application/` -> Handlers (Commands/Queries), Domain Event Handlers, utilities.
- Read the class and its dependencies (constructor, interfaces, method parameters).
- Map the **public methods** and the **business rules/exceptions** they throw
  (`BussinessRuleValidationException`, etc.).

### 3. Determine the target

Expect/ask for one of these inputs:

- **A single file**: generate the test class for that single source class.
- **An entire folder**: generate one test class per source class in that folder, following the
  mirror tree of the project structure.

### 4. Location and naming (according to the rules)

- Single test project: `Test/Test.csproj` (`RootNamespace` = `Catalog.Test`).
- Mirror location by layer:
  - `Test/Domain/{Aggregate}/...`
  - `Test/Application/{Aggregate}/...`
- Namespace: `Catalog.Tests.Domain` or `Catalog.Tests.Application` (the `Shared` layer is
  **not** taken into account).
- File: `{ClassUnderTest}Tests.cs` (e.g. `CrearPlanHandler` -> `CrearPlanHandlerTests.cs`).
- Method: English format `{Action}_{Scenario}` in three parts `Action_State_ExpectedResult`.

## Writing rules you MUST follow

### General
- **AAA** pattern with explicit `//Arrange`, `//Act`, `//Assert` comments.
- Use `[Theory]` + `[InlineData]` (or `[MemberData]`) **always** when the same method is tested
  with different data or error codes. If 3+ scenarios differ only in data, collapse into a single
  `[Theory]`. Never `[Theory]` without data.
- One assertion concern per test.
- No dependency on order, time, exact GUID, culture, network or disk.
- No `try/catch` in tests; use `Assert.Throws<T>()`.

### Mocking (Moq) - ALL dependencies
- Mock **ALL** constructor dependencies of the system under test (repositories, factories,
  `IUnitOfWork`, `ILogger`, buses) and **verify interactions** with `Verify(...)` + `Times`.
- `Times.Once` on success; `Times.Never` on failure.
- Mock prefixes: `_repoMock`, `_factoryMock`, `_unitOfWorkMock`, `_loggerMock` (or local when
  inside the test).
- **Do not** mock the system under test or the Value Objects.

### Rules per layer
- **Domain** (D1-D9): instantiate the aggregate/VO directly (no mock); test every exception with
  `Assert.Throws` and its message; test invariants after mutations and events.
- **Application** (A1-A11): test the `Result` envelope (`IsSuccess`, `Value`, `Error`); do not
  test domain inside handlers; one test per scenario.
- **Factories** (F1-F3): mock repositories when the factory depends on them; test
  type/state/quantity; verify domain events.
- Coverage targets: Domain 90%, Application 80%, Value Objects 95%.

## Deliverables

- Write the `*Tests.cs` files in the correct mirror location.
- Do NOT create placeholders (`Class1.cs`, `UnitTest1.cs`).
- Do NOT run `dotnet test` or any build/execution command. Creation only.
- When done, report the list of created test files and mention that the checklist in
  `ai/project/README.md` applies before a PR.

## Before finishing

- Verify against the checklist in `ai/project/README.md` (structure + pre-PR checklist).
- Do not execute tests; if the user wants to run them, tell them to run
  `dotnet test Test/Test.csproj` themselves.