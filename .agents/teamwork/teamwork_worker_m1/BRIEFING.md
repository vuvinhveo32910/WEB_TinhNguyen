# BRIEFING — 2026-11-03T12:58:30Z

## Mission
Implement Milestone 1 (Area & Data Foundation) for WEB_TinhNguyen fundraising shop system.

## 🔒 My Identity
- Archetype: worker
- Roles: implementer, qa, specialist
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m1
- Original parent: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Milestone: M1 (Area & Data Foundation)

## 🔒 Key Constraints
- Exclusive write ownership limited to 14 files (Program.cs, ApplicationDbContext.cs, tailwind.config.js, 7 Models, 2 Helpers, 2 Views setup).
- DO NOT CHEAT: genuine implementation, no fake or dummy data. Real EF Core DbSets, real session extension methods, real seed data and in-memory dual mode fallback.
- Compile check with `XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx` must yield 0 errors.
- Never write source code or test code to `.agents/teamwork/`.

## Current Parent
- Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Updated: 2026-11-03T12:58:30Z

## Task Summary
- **What to build**: Area & Data Foundation: configure Program.cs, ApplicationDbContext.cs, tailwind.config.js; implement 3 Entities, 4 ViewModels, SessionExtensions, DbInitializer (Seed & Fallback), _ViewImports and _ViewStart.
- **Success criteria**: 14 files created/updated; dotnet build succeeds with 0 errors; full support for session, routing, entities, and seed data.
- **Interface contracts**: PROJECT.md lines 84-98.
- **Code layout**: PROJECT.md lines 13-54.

## Key Decisions Made
- All 14 files implemented in full accordance with Explorer blueprints:
  - `Program.cs`: In-memory cache, Session (60m timeout, HttpOnly, IsEssential), HttpContextAccessor, Area route before default route, safe DbInitializer initialization.
  - `ApplicationDbContext.cs`: 3 DbSets (`SanPhamGayQuys`, `DonHangGayQuys`, `ChiTietDonHangGayQuys`) with fluent API configurations for decimal(18,2) precision and relationships.
  - `tailwind.config.js`: Added `./Areas/**/*.cshtml` and `./WEB_TinhNguyen/Areas/**/*.cshtml` scan paths.
  - 3 Entities: `SanPhamGayQuy`, `DonHangGayQuy`, `ChiTietDonHangGayQuy` with full data annotations, decimal types, utility computed properties.
  - 4 ViewModels: `CartItem`, `CartViewModel`, `CheckoutViewModel`, `SanPhamCrudViewModel` (with 2-way entity mapping and image upload support).
  - 2 Helpers: `SessionExtensions` (System.Text.Json serialization, cycle handling, GetOrCreate) and `DbInitializer` (4 seed products with real Langbiang assets + full thread-safe in-memory store for offline fallback).
  - 2 Views config: `_ViewImports.cshtml` and `_ViewStart.cshtml` (smart layout dispatcher: _AdminLayout for Admin, _Layout for Storefront).

## Artifact Index
- handoff.md — Final handoff report for Milestone 1.
- progress.md — Liveness heartbeat and step-by-step progress.

## Change Tracker
- **Files modified**:
  - `WEB_TinhNguyen/Program.cs`: Configured Session, Cache, HttpContextAccessor, Area route, DbInitializer.
  - `WEB_TinhNguyen/Data/ApplicationDbContext.cs`: Added 3 DbSets and OnModelCreating configuration.
  - `tailwind.config.js`: Added Area paths to content array.
  - `WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamGayQuy.cs`: Created entity.
  - `WEB_TinhNguyen/Areas/CuaHang/Models/DonHangGayQuy.cs`: Created entity.
  - `WEB_TinhNguyen/Areas/CuaHang/Models/ChiTietDonHangGayQuy.cs`: Created entity.
  - `WEB_TinhNguyen/Areas/CuaHang/Models/CartItem.cs`: Created model.
  - `WEB_TinhNguyen/Areas/CuaHang/Models/CartViewModel.cs`: Created viewmodel.
  - `WEB_TinhNguyen/Areas/CuaHang/Models/CheckoutViewModel.cs`: Created viewmodel.
  - `WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamCrudViewModel.cs`: Created viewmodel.
  - `WEB_TinhNguyen/Areas/CuaHang/Helpers/SessionExtensions.cs`: Created helper.
  - `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs`: Created helper with dual-mode.
  - `WEB_TinhNguyen/Areas/CuaHang/Views/_ViewImports.cshtml`: Created Razor imports.
  - `WEB_TinhNguyen/Areas/CuaHang/Views/_ViewStart.cshtml`: Created Razor layout dispatcher.
- **Build status**: `dotnet build` succeeded with 0 Error(s).
- **Pending issues**: None.

## Quality Status
- **Build/test result**: Pass (0 errors).
- **Lint status**: Clean, zero new compiler warnings or errors.
- **Tests added/modified**: Compilation verification and Tailwind compilation verified.

## Loaded Skills
- None specified by orchestrator.
