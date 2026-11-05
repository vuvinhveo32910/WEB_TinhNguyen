# DISPATCH — Worker M1 Remediation (C# Remediation Developer)

**Role**: C# Remediation Developer
**Working Directory**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m1_fix
**Original Request**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
**Project Document**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md
**Explorer 1 Remediation Report**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_m1_fix_1/handoff.md
**Explorer 2 Remediation Report**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_m1_fix_2/handoff.md
**Explorer 3 Remediation Report**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_m1_fix_3/handoff.md

## Scope & Objective:
Apply the verified fixes from the 3 Remediation Explorers for Milestone M1:

### 1. Update `WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs`:
Add constructor overload for non-generic `DbContextOptions`:
```csharp
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
{
}

public ApplicationDbContext(DbContextOptions options) : base(options)
{
}
```

### 2. Update `WEB_TinhNguyen/DuLieu/Compatibility.cs`:
- Remove `global using WEB_TinhNguyen.DuLieu;` (to eliminate CS0104 ambiguity in migrations).
- Add concrete `ApplicationDbContext` in `namespace WEB_TinhNguyen.Data`:
```csharp
namespace WEB_TinhNguyen.Data
{
    [System.Obsolete("Sử dụng WEB_TinhNguyen.DuLieu.ApplicationDbContext thay thế.")]
    public class ApplicationDbContext : WEB_TinhNguyen.DuLieu.ApplicationDbContext
    {
        public ApplicationDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<WEB_TinhNguyen.DuLieu.ApplicationDbContext> options)
            : base(options) { }

        public ApplicationDbContext(Microsoft.EntityFrameworkCore.DbContextOptions options)
            : base(options) { }
    }
}

namespace WEB_TinhNguyen.Controllers
{
    [System.Obsolete("Sử dụng WEB_TinhNguyen.BoDieuKhien.HomeController thay thế.")]
    [Microsoft.AspNetCore.Mvc.NonController]
    public class HomeController { }
}
```

### 3. Update `tailwind.config.js`:
Add `darkMode: 'class',` at root level of `module.exports` (line 3).

### 4. Update `tests/E2ETests/`:
- `Tier1/Tier1_FoundationTests.cs:5`: `using WEB_TinhNguyen.Data;` -> `using WEB_TinhNguyen.DuLieu;`
- `Tier5/Tier5_ChallengerM2Harness.cs:19`: `using WEB_TinhNguyen.Data;` -> `using WEB_TinhNguyen.DuLieu;`
- `Tier5/Tier5_ChallengerM3_2Harness.cs:15`: `using WEB_TinhNguyen.Data;` -> `using WEB_TinhNguyen.DuLieu;`
- `Tier5/Tier5_ChallengerTests.cs:10`: `using WEB_TinhNguyen.Data;` -> `using WEB_TinhNguyen.DuLieu;`
- `Framework/TestContext.cs:32`: `typeof(WEB_TinhNguyen.Controllers.HomeController)` -> `typeof(WEB_TinhNguyen.BoDieuKhien.HomeController)`
- `Framework/TestContext.cs:13`: Set `ViewsRoot = Directory.Exists(Path.Combine(ProjectRoot, "GiaoDien")) ? Path.Combine(ProjectRoot, "GiaoDien") : Path.Combine(ProjectRoot, "Views");`

### 5. Verify Build & Tests:
- `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj` (must succeed with 0 errors).
- `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build tests/E2ETests/E2ETests.csproj --configfile tests/NuGet.Config` (must succeed with 0 errors).
- `bash tests/run_e2e_tests.sh --all` (all tests should pass).

MANDATORY INTEGRITY WARNING:
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.
