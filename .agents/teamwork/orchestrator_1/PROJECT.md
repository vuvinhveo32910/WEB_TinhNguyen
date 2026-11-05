# Project: WEB_TinhNguyen - Fix SqlException & Sync C# Models with Database

## Architecture
- **Framework**: ASP.NET Core (.NET 10.0), Entity Framework Core (SQL Server provider).
- **Core Entities Affected**: `CauHinhTrangChu`, `CauHinhKyUc`, `CauHinhChuongTrinh`, `CauHinhGayQuy`, `CauHinhBanToChuc`, `CauHinhTinTuc`.
- **Database Schema**: Relational SQL Server database `TinhNguyen` (tables created via `SQLQuery1.sql` and EF Migrations 1-13).
  - Physical columns in `CauHinhTrangChu`: `LinkAnhBia`, `FontTieuDe`, `FontNoiDung`, `HienThiCanvas3D`, `VeDuAn_LinkAnh`, `HienThiMascot`, `LinkAnhMascot`, `ViTriMascot`.
  - Non-existent columns: `DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat`.
- **Resolution Strategy**:
  - Update `MoHinh/Entities.cs` to map to existing physical columns (`[Column("...")]` or primary mapped properties with `[NotMapped]` on the Vietnamese aliases).
  - Add defense-in-depth Fluent API `.Ignore(...)` or explicit column mapping in `DuLieu/ApplicationDbContext.cs`.
  - Update `Migrations/ApplicationDbContextModelSnapshot.cs` to remove phantom column definitions.
  - Keep Admin controllers and ViewModels fully compatible.
  - Zero changes needed to Razor `.cshtml` files (they already use valid DB column aliases).

## Feature Inventory
| # | Feature | Description | Milestone | Source |
|---|---------|-------------|-----------|--------|
| 1 | Sync `CauHinhTrangChu` columns | Map or ignore 8 invalid columns in `CauHinhTrangChu` so EF Core never queries them | M1 | Survey |
| 2 | Sync `CauHinhKyUc` columns | Ensure `DuongDanAnhBia`, `KieuChuTieuDe` map to `LinkAnhBia`, `FontTieuDe` | M1 | Survey |
| 3 | Sync `CauHinhChuongTrinh` columns | Ensure `KieuChuTieuDe` maps to `FontTieuDe` | M1 | Survey |
| 4 | Sync `CauHinhGayQuy` columns | Ensure `KieuChuTieuDe`, `KieuChuNoiDung` map to `FontTieuDe`, `FontNoiDung` | M1 | Survey |
| 5 | Sync `CauHinhBanToChuc` columns | Ensure `KieuChuTieuDe`, `KieuChuNoiDung` map to `FontTieuDe`, `FontNoiDung` | M1 | Survey |
| 6 | Sync `CauHinhTinTuc` columns | Ensure `KieuChuTieuDe`, `KieuChuNoiDung` map to `FontTieuDe`, `FontNoiDung` | M1 | Survey |
| 7 | DbContext Model Configuration | Add Fluent API configuration in `ApplicationDbContext.OnModelCreating` to ignore invalid columns / map physical columns | M1 | Survey |
| 8 | ModelSnapshot Cleanup | Remove invalid column registrations from `ApplicationDbContextModelSnapshot.cs` | M1 | Survey |
| 9 | Controller & ViewModel Parity | Verify Admin controllers and ViewModels maintain property access compatibility with 0 build errors | M1 | Survey |
| 10| Clean Build Verification | Verify `dotnet build` succeeds with 0 errors | M1 | Acceptance Criteria |
| 11| Application Startup & Query Verification | Verify EF Core generated SQL contains zero invalid column names and test endpoints | M1 | Acceptance Criteria |
| 12| Test Suite Execution | Execute automated test suite (337+ tests) with 100% pass rate | M1 | Acceptance Criteria |

## Milestones
| # | Name | Scope | Dependencies | Status |
|---|------|-------|-------------|--------|
| 1 | C# Entity & DbContext Synchronization & Runtime Verification | Features 1-12: Update Entities.cs, ApplicationDbContext.cs, ModelSnapshot, verify Admin controllers, run builds, execute tests and empirical validation | none | IN_PROGRESS |

## Interface Contracts
### `MoHinh/Entities.cs` ↔ `DuLieu/ApplicationDbContext.cs`
- `CauHinhTrangChu` exposes both physical DB column names (`LinkAnhBia`, `FontTieuDe`, `FontNoiDung`, `HienThiCanvas3D`, `VeDuAn_LinkAnh`, `HienThiMascot`, `LinkAnhMascot`, `ViTriMascot`) and compatibility properties (`DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat`).
- In EF Core relational model, ONLY existing physical columns are mapped to table columns. The 8 invalid column names are marked `[NotMapped]` and/or ignored via Fluent API `.Ignore(...)`.
- EF Core generated queries for `CauHinhTrangChus` contain only valid columns.

## Code Layout
- `WEB_TinhNguyen/MoHinh/Entities.cs`: Entity classes.
- `WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs`: EF Core DbContext.
- `WEB_TinhNguyen/Migrations/ApplicationDbContextModelSnapshot.cs`: Snapshot of current EF model.
- `WEB_TinhNguyen/BoDieuKhien/`: Controllers.
- `WEB_TinhNguyen/GiaoDien/`: Razor views.
- `tests/`: Automated tests and test runners.
