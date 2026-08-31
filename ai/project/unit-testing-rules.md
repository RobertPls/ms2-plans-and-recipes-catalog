# Unit Testing Rules

> Governance document for the unit testing harness of the microservice
> **ms2-plans-and-recipes-catalog**. Defines how unit tests are written, organized, named and
> executed. All new code must comply with these rules.
>
> Adapted to the DDD + Clean Architecture context of this project (Domain: Alimento,
> Receta, PlanAlimentario; Application: CQRS/MediatR handlers).

---

## 1. Objective and scope

Unit tests verify the **behavior** of a code unit in an **isolated**, fast and deterministic way.
In this architecture two layers are prioritized:

| Layer | What is tested | Isolation |
|-------|----------------|-----------|
| `Domain` | Entities, Value Objects, Factories, business rules | No external dependencies. Instantiated directly. |
| `Application` | Handlers (Commands/Queries), Domain Event Handlers, utilities | ALL dependencies are mocked and verified with **Moq**. |

> **Not** in scope of unit tests: real database access, real HTTP, containers, messaging.
> That corresponds to integration tests.

---

## 2. Stack and tools (harness)

| Tool | Purpose | Reference version |
|------|---------|-------------------|
| **xUnit** | Test framework (`[Fact]`, `[Theory]`) | 2.9.3 |
| **xunit.runner.visualstudio** | Adapter for `dotnet test` | 3.0.2 |
| **Microsoft.NET.Test.Sdk** | Test SDK | 17.13.0 |
| **Moq** | Dependency mocking | 4.20.72 |

Target framework: **net10.0**. The test project must set `<IsTestProject>true</IsTestProject>`
and never be packed (`<IsPackable>false</IsPackable>`).

> **Do not** add infrastructure dependencies (EF Core, PostgreSQL, HttpClient) to the unit test
> project. Keep it lightweight and fast to compile.

---

## 3. Namespace and class conventions

> The test project folder structure is documented in
> [`README.md`](README.md).

- Test project `RootNamespace`: `Catalog.Test`.
- Namespace of each test file uses the `.Tests` suffix + the layer (the `Shared` layer is
  **not** taken into account):

```
Catalog.Tests.Domain
Catalog.Tests.Application
```

- **Class name** = name of the class under test + `Tests` suffix.
  Example: `CrearPlanHandler` -> `CrearPlanHandlerTests`.
- The test class is `public`, with no unnecessary inheritance.
- **Delete** any placeholder file (`Class1.cs`, `UnitTest1.cs`) once real tests exist.

---

## 4. Test method naming conventions

The business code uses **Spanish** for identifiers (`Alimento`, `Receta`, `PlanAlimentario`);
test method names are written in **English**.

Mandatory format (AAA, 3 parts separated by `_`):

```
<Action>_<State/Input>_<ExpectedResult>
```

Correct examples:

```
Handle_ValidCommand_ReturnsSuccessAndVerifies()     (was: Handle_CommandValido_RetornaSuccessYVerifica)
AgregarIngrediente_DuplicateName_ThrowsException()  (keeps the Spanish method name AgregarIngrediente)
AgregarIngrediente_Duplicated_ThrowsException()
Handle_PlanNotFound_ReturnsFailure()
CalcularInfoNutricional_WithIngredients_CalculatesTotals()
```

- `Action`: verb in infinitive/imperative describing what is invoked.
- `State/Input`: the condition or data being exercised.
- `ExpectedResult`: the behavior verified (return, exception, interaction, state).

---

## 5. AAA pattern (Arrange-Act-Assert)

Every unit test follows **AAA** in order, visually separating the phases with
**explicit comments** `//Arrange`, `//Act`, `//Assert`:

```csharp
// Arrange
var command = new CrearPlanCommand { Nombre = "Plan Saludable", DuracionTipo = "QUINCENAL", ComidasPorDia = 3 };
_factoryMock.Setup(f => f.Create(...)).Returns(plan);

// Act
var result = await _handler.Handle(command, CancellationToken.None);

// Assert
Assert.True(result.IsSuccess);
_repoMock.Verify(r => r.CreateAsync(It.IsAny<PlanAlimentario>()), Times.Once);
```

Rules:
- **Arrange**: prepare mocks, inputs, prior state.
- **Act**: a single action under test (ideally a single line/call).
- **Assert**: verify the result (`Result` envelope) and the mock interactions.
- A multiple-call `Act` must be justified or refactored; the goal is **one** action per test.

---

## 6. Test attributes and types

- `[Fact]`: unique, deterministic test. No order or external data dependency.
- `[Theory]` + `[InlineData]` (or `[MemberData]`): **use it ALWAYS when the same method is
  tested with different data or error codes.** If 3+ scenarios differ only in data,
  collapse them into a single `[Theory]`.
  - Examples: range validations, valid/invalid durations, portions, empty names.
- **Forbidden** `[Theory]` without `[InlineData]`/`[MemberData]`: every data-driven test must
  have data.
- No shared state between tests via mutable `static`. Each test is independent.
- No explicit cleanup required (no persistent state in unit tests).

---

## 7. Mocking with Moq

- **Mock ALL constructor dependencies** of the system under test: repositories, factories,
  `IUnitOfWork`, `ILogger`, event buses, etc. No real dependency.
- Create mocks as `readonly` fields initialized in the test class constructor (pattern already
  used in the project), or as instance helpers.
- Prefix with `_`: `_repoMock`, `_factoryMock`, `_unitOfWorkMock`, `_loggerMock`.
- The SUT (`_handler`, `_sut`) is also exposed as a `readonly` field.
- **Verify interactions with `Verify(...)`, not just the mock existence:**
  - `Verify(x => x.CreateAsync(It.Is<PlanAlimentario>(p => p.Nombre.Value == "Plan")), Times.Once)`
  - `Times.Once` for expected operations; `Times.Never` for what must NOT happen on failures.
- **Do not** mock the system under test or the Value Objects (structs). Only their collaborators.
- Prefer `It.IsAny<T>()` and `It.Is<T>(...)` to validate arguments when the value matters.
- Use `MockBehavior.Loose` unless strict behavior is intentional and documented.

---

## 8. Domain tests (entities, VOs, factories)

**Location**: `Test/Domain/{Aggregate}/`

**What to test:**
- Aggregate construction and initial state.
- Happy path and alternative paths (invalid input, edge cases) of **every public method**.
- **Every exception** thrown (`BussinessRuleValidationException`, etc.).
- **Value Object** equality and arithmetic.
- State transitions and invariants after mutations
  (e.g. `AgregarIngrediente` -> increases `Ingredientes`).
- That **domain events** are queued correctly after a mutation.

**Rules:**

| # | Rule |
|---|------|
| D1 | One test class per aggregate or Value Object. File: `{Aggregate}Tests.cs` |
| D2 | Naming: `{Method}_{Scenario}` — following the English format of section 4 |
| D3 | AAA pattern with explicit comments |
| D4 | **Do NOT** mock the aggregate under test. The domain is pure logic: instantiate it directly |
| D5 | **Do NOT** mock Value Objects (`RecipeName`, `Porcion`, `DuracionPlan`). Test them as-is |
| D6 | Mock **only** external dependencies injected via constructor or parameters (e.g. `Func<Guid, Alimento>` in `CalcularInfoNutricionalTotal`) |
| D7 | Every exception is tested with `Assert.Throws<T>()` **and**, when present, the message assertion |
| D8 | Test invariants after mutations: if `AgregarIngrediente(...)` -> assert `Ingredientes`, duplicates, event |
| D9 | **One assertion concern per test.** If computing nutritional info AND updating stock, use two tests |

**Example — single test with `[Fact]`:**

```csharp
[Fact]
public void CalcularInfoNutricional_WithIngredients_CalculatesTotals()
{
    // Arrange
    var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
    var alimento = new Alimento(new AlimentoName("Manzana"), new InfoNutricional(1, 52, 0.3m, 14, 0.2m));
    receta.AgregarIngrediente(alimento.Id, new Porcion(2));

    // Act
    var info = receta.CalcularInfoNutricionalTotal(id => alimento);

    // Assert
    Assert.Equal(104m, info.Calorias);
    Assert.Single(receta.Ingredientes);
}
```

**Example — same method, different data with `[Theory]`:**

```csharp
[Theory]
[InlineData(2, 104)]   // portion, expected calories
[InlineData(1, 52)]
[InlineData(0.5, 26)]
public void CalcularInfoNutricional_DifferentPortions_CalculatesCalories(decimal portion, decimal expected)
{
    // Arrange
    var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
    var alimento = new Alimento(new AlimentoName("Manzana"), new InfoNutricional(1, 52, 0.3m, 14, 0.2m));
    receta.AgregarIngrediente(alimento.Id, new Porcion(portion));

    // Act
    var info = receta.CalcularInfoNutricionalTotal(id => alimento);

    // Assert
    Assert.Equal(expected, info.Calorias);
}
```

**Example — exception validation with `[Theory]`:**

```csharp
[Theory]
[InlineData(0)]
[InlineData(-1)]
[InlineData(-100)]
public void AgregarIngrediente_NonPositiveQuantity_ThrowsException(decimal quantity)
{
    // Arrange
    var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");

    // Act + Assert
    Assert.Throws<BussinessRuleValidationException>(() =>
        receta.AgregarIngrediente(Guid.NewGuid(), new Porcion(quantity)));
}
```

---

## 9. Application tests (Handlers)

**Location**: `Test/Application/{Aggregate}/{Feature}/`

**What to test:**
- The handler returns `Result.Success` with the correct value on valid input.
- The handler returns `Result.Failure` with the correct error on invalid input.
- Repository methods are called with the **correct arguments**.
- `IUnitOfWork.Commit()` is called **exactly once** on success.
- The handler **does NOT** call `Commit()` on failure.
- Dependency interactions are **verified** (mock `Verify`).

**Rules:**

| # | Rule |
|---|------|
| A1 | One test class per handler. File: `{HandlerName}Tests.cs` |
| A2 | Naming: `{Method}_{Scenario}` — e.g. `Handle_PlanNotFound_ReturnsFailure` |
| A3 | AAA pattern with explicit comments |
| A4 | **Mock ALL** handler constructor dependencies: repositories, `IUnitOfWork`, factories, `ILogger`, buses |
| A5 | Use `Mock<T>` for interfaces (`IPlanAlimentarioRepository`, `IUnitOfWork`, `IPlanAlimentarioFactory`, `ILogger<CrearPlanHandler>`) |
| A6 | Verify repository calls with `It.Is<T>(predicate)` to validate arguments |
| A7 | `Times.Once` to verify commit on success, `Times.Never` on failure |
| A8 | Test the `Result` envelope: `result.IsSuccess`, `result.Value`, `result.Error` |
| A9 | **Do NOT** test domain logic inside handler tests. It is already covered in domain tests |
| A10 | One test per scenario: happy path, each failure path, edge cases |
| A11 | Use `[Theory]`/`[InlineData]` when testing the same handler with different data or error codes. If 3+ scenarios differ only in data, collapse into one `[Theory]` |

**Example — handler with `[Fact]`:**

```csharp
[Fact]
public async Task Handle_ValidCommand_ReturnsSuccessAndVerifiesInteractions()
{
    // Arrange
    var repoMock = new Mock<IPlanAlimentarioRepository>();
    var factoryMock = new Mock<IPlanAlimentarioFactory>();
    var uowMock = new Mock<IUnitOfWork>();
    var loggerMock = new Mock<ILogger<CrearPlanHandler>>();
    var handler = new CrearPlanHandler(repoMock.Object, factoryMock.Object, uowMock.Object, loggerMock.Object);

    var duracion = new DuracionPlan(TipoDuracion.QUINCENAL);
    var plan = new PlanAlimentarioFactory().Create("Plan Saludable", duracion, 3);
    factoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<DuracionPlan>(), It.IsAny<int>()))
        .Returns(plan!);

    var command = new CrearPlanCommand { Nombre = "Plan Saludable", DuracionTipo = "QUINCENAL", ComidasPorDia = 3 };

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(plan.Id, result.Value);
    repoMock.Verify(x => x.CreateAsync(It.Is<PlanAlimentario>(p => p.Nombre.Value == "Plan Saludable")), Times.Once);
    uowMock.Verify(x => x.Commit(), Times.Once);
}
```

**Example — handler validation with `[Theory]`:**

```csharp
[Theory]
[InlineData("")]
[InlineData(null)]
[InlineData("   ")]
public async Task Handle_InvalidName_ReturnsFailureAndNoCommit(string? name)
{
    // Arrange
    var repoMock = new Mock<IPlanAlimentarioRepository>();
    var factoryMock = new Mock<IPlanAlimentarioFactory>();
    var uowMock = new Mock<IUnitOfWork>();
    var loggerMock = new Mock<ILogger<CrearPlanHandler>>();
    var handler = new CrearPlanHandler(repoMock.Object, factoryMock.Object, uowMock.Object, loggerMock.Object);

    var command = new CrearPlanCommand { Nombre = name, DuracionTipo = "QUINCENAL", ComidasPorDia = 3 };

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.False(result.IsSuccess);
    uowMock.Verify(x => x.Commit(), Times.Never);
}
```

---

## 10. Factory tests

**Location**: `Test/Domain/{Aggregate}/` or `Test/Application/{Aggregate}/`

**What to test:**
- The factory creates the aggregate with the correct type/initial state.
- Correctly generates derived collections (e.g. plan days according to duration).
- Applies quantities/data with correct values.
- Throws on invalid inputs (empty name, unsupported duration, etc.).

**Rules:**

| # | Rule |
|---|------|
| F1 | **Mock repositories** when the factory depends on them for lookups (`IAlimentoRepository`, etc.) |
| F2 | Test the created object's **type, state and quantity** (e.g. `DiasDelPlan.Count()` = 15/30 according to `TipoDuracion`) |
| F3 | Test that **domain events** are queued correctly after creation |

---

## 11. Isolation and determinism

- Each test must run **independently** and in **any order**.
- **Forbidden** to depend on: current time, `Guid.NewGuid()` with exact-value assertions,
  regional culture, unordered collection order, network or disk.
- If a known GUID is needed, fix it with a local variable and compare **by that variable**.
- Do not use `Thread.Sleep`, timers or waits.
- Do not catch exceptions with `try/catch` in tests: use `Assert.Throws<T>()`.

---

## 12. Quality and maintainability

- One responsibility per test; names that **document the contract**.
- Avoid "mirror tests" that only clone production code without value.
- Keep a reasonable number of tests per unit (quality > quantity).
- Tests are part of the code: same style, no unnecessary comments, consistent formatting.
- When modifying production, update/adjust the affected tests in the same commit.

---

## 13. Coverage targets

| Layer | Minimum coverage |
|-------|------------------|
| Domain (entities + guard clauses) | 90% |
| Application (handlers: success + failure) | 80% |
| Value Objects (arithmetic + equality) | 95% |

Every error branch (fail paths) must have at least one test.

Run with coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

---

## 14. Execution and verification

Run the whole suite:

```bash
dotnet test
```

Run a specific project:

```bash
dotnet test Test/Test.csproj
```

Filter by layer or class:

```bash
dotnet test --filter "FullyQualifiedName~Catalog.Tests.Domain"
dotnet test --filter "FullyQualifiedName~Catalog.Tests.Application"
dotnet test --filter "FullyQualifiedName~PlanAlimentarioFactoryTests"
```

---

## 15. Anti-patterns (DO NOT)

1. **DO NOT** test private methods directly. Test through the public API.
2. **DO NOT** write tests that depend on execution order. Each test must pass in isolation.
3. **DO NOT** use `Assert.True(result.Value == expected)`. Use `Assert.Equal(expected, result.Value)`.
4. **DO NOT** mock the system under test. Only mock its collaborators.
5. **DO NOT** catch exceptions with `try/catch` in tests. Use `Assert.Throws<T>()`.
6. **DO NOT** test infrastructure (EF Core, HTTP, DB) in unit tests.
7. **DO NOT** leave `Class1.cs` / `UnitTest1.cs`. Delete them once real tests exist.
8. **DO NOT** use `[Theory]` without `[InlineData]`/`[MemberData]`. Data-driven tests must have data.
9. **DO NOT** verify only the mock existence; always verify **interactions** (`Verify` + `Times`).

---

## 16. CI integration

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage

# Run a specific layer
dotnet test --filter "FullyQualifiedName~Catalog.Tests.Domain"
dotnet test --filter "FullyQualifiedName~Catalog.Tests.Application"
```

> See `.github/workflows` for the existing pipeline configuration.

---

## 17. Checklist before PR

> The full checklist is documented in [`README.md`](README.md).