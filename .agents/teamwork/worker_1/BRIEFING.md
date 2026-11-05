# BRIEFING — 2026-11-05T07:13:00Z

## Mission
Fix EF Core schema mismatch in CauHinhTrangChu and other CauHinh entities by mapping physical database columns (`LinkAnhBia`, `FontTieuDe`, `FontNoiDung`, `HienThiCanvas3D`, `VeDuAn_LinkAnh`, `HienThiMascot`, `LinkAnhMascot`, `ViTriMascot`, etc.), marking old column names as `[NotMapped]` with 2-way property proxying, adding Fluent API `.Ignore(...)` in ApplicationDbContext, updating ApplicationDbContextModelSnapshot.cs, and verifying clean build and 100% test pass.

## 🔒 My Identity
- Archetype: implementer
- Roles: implementer, qa, specialist
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/worker_1/
- Original parent: 13941881-5ddb-4ef5-85c6-23bd414f7662
- Milestone: M1 - Entity, DbContext & ModelSnapshot Synchronization

## 🔒 Key Constraints
- Exclusive write ownership:
  - WEB_TinhNguyen/MoHinh/Entities.cs
  - WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs
  - WEB_TinhNguyen/Migrations/ApplicationDbContextModelSnapshot.cs
  - Admin controllers & CauHinhTrangChuVM if needed
- Genuine implementation only, no cheating or hardcoding.
- Build command: `TMPDIR=/tmp XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build WEB_TinhNguyen.slnx`
- Test command: `TMPDIR=/tmp XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp bash tests/run_e2e_tests.sh --all`

## Current Parent
- Conversation ID: 13941881-5ddb-4ef5-85c6-23bd414f7662
- Updated: 2026-11-05T07:13:00Z

## Task Summary
- **What to build**: Realigned EF Core entities and DB schema for CauHinhTrangChu, CauHinhKyUc, CauHinhChuongTrinh, CauHinhGayQuy, CauHinhBanToChuc, CauHinhTinTuc, ensuring none of the 8 invalid column names are queried by EF Core while preserving full backward compatibility for controllers and views.
- **Success criteria**: 0 build errors, 100% E2E test pass, zero invalid column names in SQL queries.
- **Interface contracts**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator_1/PROJECT.md

## Key Decisions Made
- Realigned primary mapped auto-properties in `Entities.cs` to match physical database columns:
  - `CauHinhTrangChu`: `LinkAnhBia`, `FontTieuDe`, `FontNoiDung`, `HienThiCanvas3D`, `VeDuAn_LinkAnh`, `HienThiMascot`, `LinkAnhMascot`, `ViTriMascot`.
  - Decorated the 8 invalid column names with `[NotMapped]` and implemented bidirectional getter/setter proxies so all consumer code (controllers, viewmodels, tests) works seamlessly without modification.
  - Applied the same mapping/proxying pattern to `CauHinhKyUc`, `CauHinhChuongTrinh`, `CauHinhGayQuy`, `CauHinhBanToChuc`, and `CauHinhTinTuc`.
- Added defense-in-depth Fluent API `.Ignore(...)` calls in `ApplicationDbContext.OnModelCreating` for all invalid/unmapped column names and explicit `.Property(...)` mappings for real columns.
- Synchronized `ApplicationDbContextModelSnapshot.cs` to match the exact schema.

## Artifact Index
- DISPATCH.md — Task instructions from orchestrator
- BRIEFING.md — Persistent working memory
- progress.md — Heartbeat and progress log
- handoff.md — Final 5-component report

## Change Tracker
- **Files modified**:
  - `WEB_TinhNguyen/MoHinh/Entities.cs`: mapped physical DB columns and set `[NotMapped]` on invalid column aliases with 2-way proxies.
  - `WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs`: added Fluent API `.Ignore(...)` and property mappings.
  - `WEB_TinhNguyen/Migrations/ApplicationDbContextModelSnapshot.cs`: synchronized EF Core model snapshot.
- **Build status**: `dotnet build WEB_TinhNguyen.slnx` -> Succeeded with 0 errors.
- **Pending issues**: None.

## Quality Status
- **Build/test result**:
  - `dotnet build`: Succeeded, 0 errors.
  - `tests/run_e2e_tests.sh --all`: 337 / 337 Passed (100%).
  - `tests/M1Challenger`: 67 / 67 Passed (100%).
  - `tests/M3Challenger2`: 128 / 128 Passed (100%).
  - SQL verification: EF Core generated SQL contains 0 invalid column names and queries all physical DB columns.
- **Lint status**: Clean (0 compilation errors).
- **Tests added/modified**: Validated against comprehensive existing test suites and verified EF Core query string generation.

## Loaded Skills
None
