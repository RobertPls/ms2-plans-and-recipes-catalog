# Testing Conventions — ms2-plans-and-recipes-catalog

This README complements the rules document [`unit-testing-rules.md`](unit-testing-rules.md).
It documents the **testing project structure** and the **pre-PR review checklist**. The detailed
rules (naming, AAA, mocking, coverage, etc.) live in the `.md` file.

---

## Testing project structure

There is **a single testing project** by default: `Test/Test.csproj`.

Inside `Test/`, the folder structure **mirrors the layer** and the aggregate under test:

```
Test/
├── Domain/                                # Entities, Value Objects, Factories
│   ├── Alimento/
│   │   ├── AlimentoFactoryTests.cs
│   │   └── AlimentoNameTests.cs
│   ├── Receta/
│   │   ├── RecetaTests.cs                 # entity (add/remove ingredient, nutritional info)
│   │   └── RecetaFactoryTests.cs
│   └── PlanAlimentario/
│       ├── PlanAlimentarioFactoryTests.cs
│       └── DuracionPlanTests.cs           # Value Object
├── Application/                           # Handlers (Commands/Queries) and DomainEventHandlers
│   ├── Alimento/
│   │   ├── CrearAlimentoHandlerTests.cs
│   │   └── ActualizarAlimentoHandlerTests.cs
│   ├── Receta/
│   │   ├── CrearRecetaHandlerTests.cs
│   │   └── AgregarIngredienteHandlerTests.cs
│   └── PlanAlimentario/
│       └── CrearPlanHandlerTests.cs
```

**Mirror rule**: one test file per relevant production class.
Example: `Application/.../CrearPlan/CrearPlanHandler.cs` →
`Test/Application/PlanAlimentario/CrearPlanHandlerTests.cs`.

Namespace names (the `Shared` layer is **not** taken into account):

```
Catalog.Tests.Domain
Catalog.Tests.Application
```

**Class name** = class under test name + `Tests` suffix.
Example: `CrearPlanHandler` → `CrearPlanHandlerTests`.

---

## Pre-PR checklist

Before approving a new test, verify:

- [ ] The new code has its corresponding unit tests.
- [ ] All tests pass locally (`dotnet test`).
- [ ] No `Class1.cs`/`UnitTest1.cs` placeholder remains.
- [ ] Names follow `Action_State_Result` (English format from the rules doc).
- [ ] AAA comments (`//Arrange`, `//Act`, `//Assert`) are present in every test.
- [ ] `[Theory]` is used whenever the scenarios differ only in data (3+ cases).
- [ ] **All** dependencies are mocked (repositories, factories, unit of work, loggers, buses).
- [ ] Mocks **verify interactions** (`Verify` + `Times`), not only their existence.
- [ ] Commits are verified with `Times.Once` on success and `Times.Never` on failure.
- [ ] No mutable shared state between tests.
- [ ] No infrastructure tests inside the unit tests.