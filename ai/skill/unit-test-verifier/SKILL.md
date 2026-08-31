---
name: unit-test-verifier
description: Verifies that generated unit tests comply with the UNIT TESTING RULES defined in ai/project/unit-testing-rules.md. For a specific file or folder of test classes, it reads the rules, analyzes compliance, runs dotnet test, and reports pass/fail per rule. Use it after unit-test-generator or when reviewing test compliance. Language: EN.
---

# Unit Test Verifier

Verifies that unit tests in the **ms2-plans-and-recipes-catalog** project comply with the
governance rules. **This skill reads rules, analyzes test files, runs the test suite, and
reports compliance.** It is the companion to `unit-test-generator`.

## Mandatory flow

### 1. Read the rules (always)

Before any verification, **read the current rules**:

- Rules: `ai/project/unit-testing-rules.md`
- Checklist and structure: `ai/project/README.md`

These are the **single source of truth**. **Do NOT hardcode or duplicate any rule here** —
always derive every check from the latest content of these files. If they do not exist, stop
and tell the user.

### 2. Determine the target

Expect/ask for one of these inputs:

- **A single test file**: verify that specific `*Tests.cs` file.
- **A folder of tests**: verify all `*Tests.cs` files in that folder (recursive).
- **The entire Test project**: verify all `*Tests.cs` under `Test/`.

### 3. Static analysis — against the rules

For each test file in the target, read the rules document(s) and evaluate **every applicable
rule** against the file. Group checks by the sections in the rules doc:

- **File & structure** (namespaces, mirror paths, class naming, placeholders).
- **Method naming** (`Action_State_ExpectedResult`).
- **AAA pattern** (explicit `//Arrange`, `//Act`, `//Assert`).
- **Attributes** (`[Theory]` + `[InlineData]`/`[MemberData]`).
- **Mocking** (all deps mocked, `_` prefix, `Verify` + `Times`).
- **Domain rules** (D1-D9), **Application rules** (A1-A11), **Factory rules** (F1-F3).
- **Isolation/determinism** and **Anti-patterns** sections.

For each rule, report **PASS or FAIL** with the `file:line` where the violation occurs. Do not
copy the rule text; reference the source file and section/section-id instead.

### 4. Dynamic verification — run the tests

After static analysis, **build the test project if needed**, then run and capture results:

```bash
dotnet build Test/Test.csproj
dotnet test Test/Test.csproj --verbosity normal
```

Parse the output to determine: total tests discovered, passed, failed (list each with name and
error), and skipped. Do not pipe/truncate the output; capture it fully and search for failures.

### 5. Coverage analysis (optional, if requested)

```bash
dotnet test Test/Test.csproj --collect:"XPlat Code Coverage" --results-directory ./coverage
```

Parse the report and compare against the targets stated in the rules doc (Domain 90%,
Application 80%, Value Objects 95%).

### 6. Report generation

Produce a structured report:

```
=== UNIT TEST VERIFICATION REPORT ===
Target: {file or folder path}
Date: {current date}
Rules source: ai/project/unit-testing-rules.md (+ ai/project/README.md)

--- STATIC ANALYSIS ---
File: {filename}
  [PASS] {rule} - {short description}
  [FAIL] {rule} - {short description} (line {n})
  ...
  Total: {pass} PASS / {fail} FAIL

--- DYNAMIC RESULTS ---
  Total: {n} | Passed: {n} | Failed: {n} | Skipped: {n}
  Failed tests:
    - {FullyQualifiedName}
      Error: {message}

--- COVERAGE (if requested) ---
  {Layer}: {value}% (target: {target}%) - {OK | BELOW}

--- SUMMARY ---
  Verdict: {COMPLIANT | NON-COMPLIANT | PARTIAL}
```

### 7. Verdict logic

- **COMPLIANT**: All static rules PASS and all tests PASS.
- **NON-COMPLIANT**: Any static rule FAIL or any test FAIL.
- **PARTIAL**: All tests PASS but some static rules FAIL (fixable without breaking tests).

## Deliverables

- Return the full verification report.
- For each FAIL: **file path, line number**, and what rule from the rules doc it violates.
- Suggest the fix when possible.
- Do **NOT** modify any files — this skill is read-only verification only.

## Differences from unit-test-generator

| Aspect | Generator | Verifier |
|--------|-----------|----------|
| Creates files | Yes | No |
| Reads rules | Yes | Yes |
| Runs tests | No | Yes |
| Checks compliance | No | Yes |
| Modifies code | Yes | No |
