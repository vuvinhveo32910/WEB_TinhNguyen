# BRIEFING — 2026-11-03T12:39:00Z

## Mission
Investigate storefront capabilities, shopping cart implementation patterns (session/cookie), fundraising product domain entities & seed data mechanisms, and build/run verification (`dotnet build`) for the fundraising store feature.

## 🔒 My Identity
- Archetype: explorer
- Roles: investigation, synthesis
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_survey_3
- Original parent: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Milestone: initial_survey

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Do NOT copy code from existing open-source projects for core logic
- Integrity mode: demo
- Write only inside working directory

## Current Parent
- Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Updated: 2026-11-03T12:39:00Z

## Investigation State
- **Explored paths**:
  - `WEB_TinhNguyen/Program.cs` (state, routing, middleware)
  - `WEB_TinhNguyen/WEB_TinhNguyen.csproj` (net10.0, EF Core 10.0.12, Tailwind MSBuild target)
  - `WEB_TinhNguyen/Data/ApplicationDbContext.cs` & `Models/Entities.cs`
  - `WEB_TinhNguyen/Controllers/` (Admin and Home controllers, lazy seeding patterns)
  - `WEB_TinhNguyen/Views/Shared/_Layout.cshtml` & `_AdminLayout.cshtml` (Header, Liquid Bottom Nav, Sidebar)
  - `tailwind.config.js` & `package.json`
  - Peer explorer reports from explorer 1 and explorer 2
- **Key findings**:
  - `dotnet build` succeeds 100% (0 errors) using `XDG_RUNTIME_DIR=/tmp` (avoiding mutex lock on `/run/user/1000`).
  - Session/Cache not configured in `Program.cs`. Needs `AddDistributedMemoryCache()`, `AddSession()`, `UseSession()`, and `MapControllerRoute("areas", ...)`.
  - No existing Product, Cart, or Order entities. Designed `SanPhamGayQuy`, `DonHangGayQuy`, `ChiTietDonHangGayQuy`.
  - Existing seed data pattern is lazy controller-level seeding; designed 2-tier robust seeding with 4 Langbiang 2026 charity products.
  - Designed clean `Areas/CuaHang` encapsulating storefront, cart, checkout with dynamic VietQR, and Admin CRUD.
- **Unexplored areas**: None within the survey scope.

## Key Decisions Made
- Recommend Session-based shopping cart with JSON extensions and 60-minute timeout.
- Encapsulate all feature controllers, models, and views in `Areas/CuaHang` per R3 requirement.
- Propose 4 rich seed products tailored to "Trăng Sáng Langbiang 2026".
- Provide dynamic VietQR payment integration for charity transparency matching BTC bank info.

## Artifact Index
- DISPATCH.md — Task instructions from orchestrator
- progress.md — Liveness heartbeat and step tracking
- handoff.md — 5-component comprehensive survey report for orchestrator
