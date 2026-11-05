# E2E Test Infrastructure Specification — WEB_TinhNguyen

## 1. Overview
This document specifies the architecture, execution engine, reference oracles, and operational procedures for the End-to-End (E2E) Test Suite of the `WEB_TinhNguyen` Volunteer Fundraising Store.

The test suite provides complete opaque-box and requirement-driven coverage for all 17 features defined in `PROJECT.md` § Feature Inventory across 4 distinct testing tiers, designed with **Progressive Testability** to seamlessly accommodate milestone-based rollout without breaking build or test verification.

---

## 2. Infrastructure Architecture

### 2.1 Directory Structure
```
tests/
├── NuGet.Config                       # Offline NuGet package resolution mapping to local cache
├── run_e2e_tests.sh                   # Executable standalone runner script (chmod +x)
└── E2ETests/
    ├── E2ETests.csproj                # .NET 10.0 test project
    ├── Program.cs                     # Console entry point
    ├── Framework/
    │   ├── Assertions.cs              # Custom assertion library + progressive testability skips
    │   ├── ReferenceOracles.cs        # Authoritative calculation, formatting, and validation oracles
    │   ├── TestContext.cs             # Environment resolution (FindSolutionRoot) & MockSession (ISession)
    │   └── TestRunner.cs              # Reflection discovery engine, CLI parser, timer, colorized output
    ├── Tier1/                         # Feature Coverage (>=5 tests per feature for Features 1-17)
    │   ├── Tier1_FoundationTests.cs       # Features 1-4 (Infrastructure, Session, Models, Seed)
    │   ├── Tier1_StorefrontTests.cs       # Features 5-8 (Navigation, Liquid Bar, Catalog, Detail)
    │   ├── Tier1_CartCheckoutTests.cs     # Features 9-11 (Cart, Checkout, VietQR Confirmation)
    │   ├── Tier1_AdminTests.cs            # Features 12-16 (Admin Navigation, Product CRUD)
    │   └── Tier1_BuildVerificationTests.cs# Feature 17 (Tailwind CSS, Build, Lint)
    ├── Tier2/
    │   └── Tier2_BoundaryCornerTests.cs   # 25 Boundary, Edge, and Stress Cases
    ├── Tier3/
    │   └── Tier3_CrossFeatureTests.cs     # 16 Cross-Feature Integration Flows
    └── Tier4/
        └── Tier4_RealWorldScenarioTests.cs# 12 Real-World Donor and Admin Scenarios
```

### 2.2 Offline Package Management
To support air-gapped / offline build environments where `api.nuget.org:443` is unreachable:
- `tests/NuGet.Config` defines:
  ```xml
  <configuration>
    <packageSources>
      <clear />
      <add key="local-cache" value="/home/hutech/.nuget/packages" />
    </packageSources>
  </configuration>
  ```
- All build commands explicitly pass `--configfile tests/NuGet.Config` ensuring sub-second restore from local pre-cached packages.

### 2.3 Runtime Environment Isolation
To prevent `Read-only file system : '/home/hutech/.templateengine'` errors and ensure sandbox isolation:
- `XDG_RUNTIME_DIR=/tmp`
- `DOTNET_CLI_HOME=/tmp`
- `HOME=/tmp`
All runner scripts and test executions enforce these environment variables.

---

## 3. Test Runner & Discovery Engine

### 3.1 Metadata Attribute (`[E2ETest]`)
Tests are tagged with metadata defining tier, feature index, and description:
```csharp
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class E2ETestAttribute : Attribute
{
    public int Tier { get; }
    public int Feature { get; }
    public string Name { get; }
}
```

### 3.2 Dynamic Test Discovery
`TestRunnerEngine.Run(string[] args)` automatically inspects loaded assemblies for public methods decorated with `[E2ETest]`. Tests can be executed granularly:
- `--all`: Execute all tiers (Tiers 1-4).
- `--tier <1-4>`: Filter execution to a single tier.
- `--feature <1-17>`: Filter execution to a specific feature.
- `--filter <keyword>`: Match test names containing the keyword.
- `--verbose`: Display full stack traces and debug output for skipped/failed tests.

### 3.3 Exit Code Semantics
- Returns code `0` when all executed tests pass (or are progressively skipped).
- Returns code `1` when any test fails an assertion.

---

## 4. Progressive Testability Framework

Because features are developed iteratively across milestones (M1: Foundations & Models, M2: Admin Product Management, M3: Storefront & Cart/Checkout):
- **`Assert.RequireFile(filePath, featureDescription)`**: Checks if a Razor view, CSS, or template file exists. If missing, logs `[SKIP] Pending implementation: {featureDescription}` and records a skip rather than a failure.
- **`Assert.RequireType(typeName, friendlyName)`**: Inspects solution assemblies via reflection. If the class, model, or controller is pending a future milestone, gracefully records a skip.
- **`Assert.RequireDirectory(dirPath, description)`**: Verifies necessary asset directories exist before running view-dependent tests.

When an implementing agent deploys new files, views, or controllers in subsequent milestones, the corresponding skipped tests **automatically activate and verify** without needing manual test alterations.

---

## 5. Reference Oracles

Authoritative specification algorithms implemented in `ReferenceOracles.cs`:

1. **Line Total Calculation**:
   $$\text{LineTotal} = \text{UnitPrice} \times \text{Quantity}$$
2. **Charity Contribution**:
   $$\text{Contribution} = \left\lfloor \text{LineTotal} \times \frac{\text{CharityRate}}{100} \right\rfloor$$
3. **Order Financial Totals**:
   $$\text{TotalContribution} = \sum \text{ItemContributions} + \text{VoluntaryDonation}$$
   $$\text{OrderTotal} = \sum \text{LineTotals} + \text{VoluntaryDonation}$$
4. **VietQR Payload Generation**:
   `https://img.vietqr.io/image/970422-0909090909-compact2.png?amount={orderTotal}&addInfo={orderCode}&accountName=QUY+TINH+NGUYEN+LANGBIANG`
   - Validates URL encoding, truncation of decimals, and uppercase account name.
5. **Vietnamese Phone Oracle**:
   - Strips whitespaces, dots, hyphens.
   - Requires regex: `^(0[3|5|7|8|9])[0-9]{8}$`.
6. **Email Oracle**:
   - RFC 5322 standard regex validation.

---

## 6. How to Run the Test Suite

### Full Suite Run
```bash
bash tests/run_e2e_tests.sh --all
```

### Targeted Tier Run
```bash
bash tests/run_e2e_tests.sh --tier 1
bash tests/run_e2e_tests.sh --tier 2
bash tests/run_e2e_tests.sh --tier 3
bash tests/run_e2e_tests.sh --tier 4
```

### Targeted Feature Run
```bash
bash tests/run_e2e_tests.sh --feature 3   # Domain entities
bash tests/run_e2e_tests.sh --feature 10  # Checkout & VietQR
```
