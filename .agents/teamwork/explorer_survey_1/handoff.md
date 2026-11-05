# Handoff Report - Explorer 1 (Database & Schema Researcher)

## 1. Observation

### 1.1. Project & Database Configuration
- **Connection Strings** (`appsettings.json` and `appsettings.Development.json`):
  ```json
  "DefaultConnection": "Server=localhost;Database=TinhNguyen;User Id=sa;Password=Vuvinh2910@;TrustServerCertificate=True;MultipleActiveResultSets=true"
  ```
- **Target Framework**: .NET 10.0 (`WEB_TinhNguyen.csproj`), using Entity Framework Core with SQL Server provider (`Microsoft.EntityFrameworkCore.SqlServer`).
- **Baseline SQL Schema**: Located in `/home/hutech/Vinh_Offline/WEB_TinhNguyen/SQLQuery1.sql`.
  Lines 108–132 define `CREATE TABLE CauHinhTrangChu`:
  ```sql
  CREATE TABLE CauHinhTrangChu (
      IDCauHinh INT IDENTITY(1,1) PRIMARY KEY,
      IDChienDich INT NOT NULL UNIQUE, 
      TieuDeChinh NVARCHAR(500) NULL,
      TieuDePhu NVARCHAR(500) NULL,
      DoanVanMoTa NVARCHAR(MAX) NULL,
      LinkAnhBia NVARCHAR(1000) NULL,      -- DB column name
      MauChuDao VARCHAR(20) NULL,
      MauNen VARCHAR(20) NULL,
      MauChu VARCHAR(20) NULL,
      FontTieuDe VARCHAR(100) NULL,        -- DB column name
      FontNoiDung VARCHAR(100) NULL,       -- DB column name
      NgayCapNhat DATETIME DEFAULT GETDATE(),
      CONSTRAINT FK_CauHinhTrangChu_ChienDich FOREIGN KEY (IDChienDich) REFERENCES ChienDich(IDChienDich)
  );
  ```

### 1.2. EF Core Migrations History
- The migration sequence in `WEB_TinhNguyen/Migrations/`:
  1. `20261006032230_AddBannerTable.cs`: Added `TocDoChuyenSlide` to `CauHinhTrangChu` and created `BannerTrangChu`.
  2. `20261006035646_AddChuongTrinhDemNguoc.cs`: Created `ChuongTrinhDemNguoc`.
  3. `20261006070510_AddVeDuAnColors.cs`: Added `VeDuAn_MauChu`, `VeDuAn_MauNenTag`, `VeDuAn_MauTieuDe` to `CauHinhTrangChu`.
  4. `20261006074015_AddTinTucDetail.cs`: Added detail columns to `TinTuc`.
  5. `20261012070914_AddKyUcModule.cs`: Created `CauHinhKyUc`, `AnhKyUc`, `NhaTaiTroKyUc`.
  6. `20261012075303_AddTieuDeTongKet.cs`: Added `FontTieuDeTongKet` to `CauHinhKyUc`.
  7. `20261012083652_AddChuongTrinhModule.cs`: Created `CauHinhChuongTrinh`, `ChangChuongTrinh`.
  8. `20261012090043_FixLoTrinh.cs`: Created `ChiTietLoTrinh`.
  9. `20261013005058_AddModuleGayQuy.cs`: Created `CauHinhGayQuy`, `KenhDongGop`, `NhaTaiTroGayQuy`.
  10. `20261013012535_AddModuleBanToChuc.cs`: Created `CauHinhBanToChuc`, `ThanhVienBanToChuc`.
  11. `20261013015906_AddModuleTinTuc.cs`: Created `CauHinhTinTuc`, `BaiVietTinTuc`.
  12. `20261014034002_AddHieuUng.cs`: Added `IDHieuUng` to `CauHinhTrangChu` and created `ThuVienHieuUng`.
  13. `20261014042058_AddMascot.cs`: Added Mascot columns to `CauHinhTrangChu`:
      - `HienThiMascot` (bit)
      - `LinkAnhMascot` (nvarchar(max))
      - `ViTriMascot` (nvarchar(max))
  14. `20261104000000_XoaCotMauVaVietHoaTenBangCot.cs`: Migration created on Nov 4, 2026.
      In lines 118–125:
      ```csharp
      migrationBuilder.RenameColumn(name: "HienThiCanvas3D", table: "CauHinhTrangChu", newName: "HienThiHieuUng3D");
      migrationBuilder.RenameColumn(name: "HienThiMascot", table: "CauHinhTrangChu", newName: "HienThiLinhVat");
      migrationBuilder.RenameColumn(name: "LinkAnhMascot", table: "CauHinhTrangChu", newName: "DuongDanAnhLinhVat");
      migrationBuilder.RenameColumn(name: "ViTriMascot", table: "CauHinhTrangChu", newName: "ViTriLinhVat");
      migrationBuilder.RenameColumn(name: "FontTieuDe", table: "CauHinhTrangChu", newName: "KieuChuTieuDe");
      migrationBuilder.RenameColumn(name: "FontNoiDung", table: "CauHinhTrangChu", newName: "KieuChuNoiDung");
      migrationBuilder.RenameColumn(name: "LinkAnhBia", table: "CauHinhTrangChu", newName: "DuongDanAnhBia");
      migrationBuilder.RenameColumn(name: "VeDuAn_LinkAnh", table: "CauHinhTrangChu", newName: "VeDuAn_DuongDanAnh");
      ```

### 1.3. C# Entity Mapping in `MoHinh/Entities.cs`
- In `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/MoHinh/Entities.cs` lines 24–75:
  ```csharp
  [Table("CauHinhTrangChu")]
  public class CauHinhTrangChu
  {
      [Key]
      public int IDCauHinh { get; set; }
      public int IDChienDich { get; set; }

      public string? TieuDePhu { get; set; }
      public string? TieuDeChinh { get; set; }
      public string? DuongDanAnhBia { get; set; }                    // MAPPED to DB column "DuongDanAnhBia"
      [NotMapped]
      public string? LinkAnhBia { get => DuongDanAnhBia; set => DuongDanAnhBia = value; }
      public string? DoanVanMoTa { get; set; }

      public int TocDoChuyenSlide { get; set; } = 4000;

      public string? KieuChuTieuDe { get; set; }                     // MAPPED to DB column "KieuChuTieuDe"
      [NotMapped]
      public string? FontTieuDe { get => KieuChuTieuDe; set => KieuChuTieuDe = value; }
      public string? KieuChuNoiDung { get; set; }                    // MAPPED to DB column "KieuChuNoiDung"
      [NotMapped]
      public string? FontNoiDung { get => KieuChuNoiDung; set => KieuChuNoiDung = value; }

      public bool HienThiHieuUng3D { get; set; }                     // MAPPED to DB column "HienThiHieuUng3D"
      [NotMapped]
      public bool HienThiCanvas3D { get => HienThiHieuUng3D; set => HienThiHieuUng3D = value; }
      public bool HienThiLaRoi { get; set; }
      public DateTime? NgayCapNhat { get; set; }

      public string? VeDuAn_TieuDe { get; set; }
      public string? VeDuAn_NoiDung { get; set; }
      public string? VeDuAn_DuongDanAnh { get; set; }                // MAPPED to DB column "VeDuAn_DuongDanAnh"
      [NotMapped]
      public string? VeDuAn_LinkAnh { get => VeDuAn_DuongDanAnh; set => VeDuAn_DuongDanAnh = value; }
      public int? IDHieuUng { get; set; }
      [ForeignKey("IDHieuUng")]
      public virtual ThuVienHieuUng? ThuVienHieuUng { get; set; }

      [ForeignKey("IDChienDich")]
      public virtual ChienDich? ChienDich { get; set; }
      public bool HienThiLinhVat { get; set; } = true;               // MAPPED to DB column "HienThiLinhVat"
      [NotMapped]
      public bool HienThiMascot { get => HienThiLinhVat; set => HienThiLinhVat = value; }
      public string? DuongDanAnhLinhVat { get; set; }                // MAPPED to DB column "DuongDanAnhLinhVat"
      [NotMapped]
      public string? LinkAnhMascot { get => DuongDanAnhLinhVat; set => DuongDanAnhLinhVat = value; }
      public string? ViTriLinhVat { get; set; } = "right";           // MAPPED to DB column "ViTriLinhVat"
      [NotMapped]
      public string? ViTriMascot { get => ViTriLinhVat; set => ViTriLinhVat = value; }
      ...
  ```
- **Other affected entity classes with similar renaming in `MoHinh/Entities.cs`**:
  - `CauHinhKyUc`: lines 243 (`DuongDanAnhBia`), 251 (`KieuChuTieuDe`).
  - `CauHinhChuongTrinh`: line 320 (`KieuChuTieuDe`).
  - `CauHinhGayQuy`: lines 377 (`KieuChuTieuDe`), 380 (`KieuChuNoiDung`).
  - `CauHinhBanToChuc`: lines 511 (`KieuChuTieuDe`), 514 (`KieuChuNoiDung`).
  - `CauHinhTinTuc`: lines 591 (`KieuChuTieuDe`), 594 (`KieuChuNoiDung`).

### 1.4. Application Query Entry Points for `CauHinhTrangChu`
1. `GiaoDien/Shared/_Layout.cshtml` lines 10–13:
   ```csharp
   var activeConfig = _context.CauHinhTrangChus
       .Include(c => c.ChienDich)
       .Where(c => c.ChienDich.TrangThaiHoatDong == true)
       .FirstOrDefault();
   ```
   *Note*: Used on virtually all public pages; lines 15, 24, 25 directly access `activeConfig?.LinkAnhBia`, `activeConfig?.FontTieuDe`, `activeConfig?.FontNoiDung`, `activeConfig?.HienThiMascot`, `activeConfig?.LinkAnhMascot`, `activeConfig?.ViTriMascot`.
2. `Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml` lines 10–13:
   Queries `_context.CauHinhTrangChus` on every CuaHang area storefront page.
3. `BoDieuKhien/HomeController.cs` line 29:
   `Index()` executes `await _context.CauHinhTrangChus.Include(...)...FirstOrDefaultAsync();`.
4. `BoDieuKhien/AdminTrangChuController.cs` lines 138, 211:
   Queries and updates `_context.CauHinhTrangChus`.

---

## 2. Logic Chain

1. **Root Cause Identification**:
   - In git commit history, the database schema historically had column names: `LinkAnhBia`, `FontTieuDe`, `FontNoiDung`, `HienThiCanvas3D`, `VeDuAn_LinkAnh`, `HienThiMascot`, `LinkAnhMascot`, `ViTriMascot`.
   - On November 4, code refactoring attempted to rename all properties and columns to Vietnamese names:
     `DuongDanAnhBia`, `KieuChuTieuDe`, `KieuChuNoiDung`, `HienThiHieuUng3D`, `VeDuAn_DuongDanAnh`, `HienThiLinhVat`, `DuongDanAnhLinhVat`, `ViTriLinhVat`.
   - Migration file `20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` was generated, but **never executed** against the actual SQL database instance (`Database=TinhNguyen`).
   - In C# entity `CauHinhTrangChu`, the original property names (`LinkAnhBia`, `FontTieuDe`, etc.) were decorated with `[NotMapped]`, while the new names were left undecorated without explicit `[Column]` mappings.
   - By default convention, EF Core maps undecorated properties to database columns of the exact same name.

2. **Error Mechanism**:
   - When any page loads `_Layout.cshtml`, `_CuaHangLayout.cshtml`, or `HomeController.Index()`, EF Core translates LINQ query `_context.CauHinhTrangChus...` into SQL:
     ```sql
     SELECT c.IDCauHinh, c.DoanVanMoTa, c.DuongDanAnhBia, c.DuongDanAnhLinhVat,
            c.HienThiHieuUng3D, c.HienThiLaRoi, c.HienThiLinhVat, c.IDChienDich,
            c.IDHieuUng, c.KieuChuNoiDung, c.KieuChuTieuDe, c.NgayCapNhat,
            c.TieuDeChinh, c.TieuDePhu, c.TocDoChuyenSlide, c.VeDuAn_DuongDanAnh,
            c.VeDuAn_NoiDung, c.VeDuAn_TieuDe, c.ViTriLinhVat
     FROM CauHinhTrangChu AS c
     ```
   - SQL Server raises `SqlException: Invalid column name 'DuongDanAnhBia'`, `'DuongDanAnhLinhVat'`, `'HienThiHieuUng3D'`, `'HienThiLinhVat'`, `'KieuChuNoiDung'`, `'KieuChuTieuDe'`, `'VeDuAn_DuongDanAnh'`, `'ViTriLinhVat'`.

3. **Comparison Matrix**:

| # | Invalid Column Name (C# Entity Property) | Actual Column Name in Database (`SQLQuery1.sql` / Migrations) | Table Name | DB Data Type | Current C# Alias Property |
|---|------------------------------------------|---------------------------------------------------------------|------------|--------------|---------------------------|
| 1 | `DuongDanAnhBia` | `LinkAnhBia` | `CauHinhTrangChu` | `nvarchar(1000)` / `nvarchar(max)` | `[NotMapped] LinkAnhBia` |
| 2 | `DuongDanAnhLinhVat` | `LinkAnhMascot` | `CauHinhTrangChu` | `nvarchar(max)` | `[NotMapped] LinkAnhMascot` |
| 3 | `HienThiHieuUng3D` | `HienThiCanvas3D` | `CauHinhTrangChu` | `bit` | `[NotMapped] HienThiCanvas3D` |
| 4 | `HienThiLinhVat` | `HienThiMascot` | `CauHinhTrangChu` | `bit` | `[NotMapped] HienThiMascot` |
| 5 | `KieuChuNoiDung` | `FontNoiDung` | `CauHinhTrangChu` | `varchar(100)` / `nvarchar(max)` | `[NotMapped] FontNoiDung` |
| 6 | `KieuChuTieuDe` | `FontTieuDe` | `CauHinhTrangChu` | `varchar(100)` / `nvarchar(max)` | `[NotMapped] FontTieuDe` |
| 7 | `VeDuAn_DuongDanAnh` | `VeDuAn_LinkAnh` | `CauHinhTrangChu` | `nvarchar(max)` | `[NotMapped] VeDuAn_LinkAnh` |
| 8 | `ViTriLinhVat` | `ViTriMascot` | `CauHinhTrangChu` | `nvarchar(max)` | `[NotMapped] ViTriMascot` |

---

## 3. Caveats

1. **Running Database Service**: The current execution environment is a sandboxed container without a live local SQL Server daemon running in background. Connection to `localhost:1433` cannot be established directly inside this sandbox unless SQL Server is hosted externally or started.
2. **Offline Fallback Architecture**: The CuaHang subsystem (`Areas/CuaHang/Helpers/DbInitializer.cs`) includes in-memory fallbacks when DB connection is offline, but root controllers (`HomeController`, `AdminTrangChuController`, and `_Layout.cshtml`) directly query `_context.CauHinhTrangChus` without catch/fallback, making them susceptible to runtime `SqlException` if EF Core column metadata does not match the actual database schema.
3. **Other Entities**: `CauHinhKyUc`, `CauHinhChuongTrinh`, `CauHinhGayQuy`, `CauHinhBanToChuc`, and `CauHinhTinTuc` also have properties like `KieuChuTieuDe`, `KieuChuNoiDung`, `DuongDanAnhBia` in C# where the original DB migrations specified `FontTieuDe`, `FontNoiDung`, `LinkAnhBia`. The implementer should check these entities to ensure they do not produce secondary `SqlException` errors when queried.

---

## 4. Conclusion

- The 8 invalid columns reported in the user request (`DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat`) belong to the table **`CauHinhTrangChu`**.
- In the actual database schema (established by `SQLQuery1.sql` and baseline migrations 1–13), these columns exist under their original names:
  1. `LinkAnhBia`
  2. `LinkAnhMascot`
  3. `HienThiCanvas3D`
  4. `HienThiMascot`
  5. `FontNoiDung`
  6. `FontTieuDe`
  7. `VeDuAn_LinkAnh`
  8. `ViTriMascot`
- The 8 columns indeed **do not exist** in the database under the Vietnamese names.
- To resolve the `SqlException`, the C# codebase must map these entity properties to their existing database column names:
  - **Option A (Recommended - Non-breaking Property Mapping)**:
    In `MoHinh/Entities.cs` or `ApplicationDbContext.OnModelCreating`, explicitly map the properties to the real DB column names using `[Column("...")]` or Fluent API:
    ```csharp
    [Column("LinkAnhBia")]
    public string? DuongDanAnhBia { get; set; }

    [Column("FontTieuDe")]
    public string? KieuChuTieuDe { get; set; }

    [Column("FontNoiDung")]
    public string? KieuChuNoiDung { get; set; }

    [Column("HienThiCanvas3D")]
    public bool HienThiHieuUng3D { get; set; }

    [Column("VeDuAn_LinkAnh")]
    public string? VeDuAn_DuongDanAnh { get; set; }

    [Column("HienThiMascot")]
    public bool HienThiLinhVat { get; set; } = true;

    [Column("LinkAnhMascot")]
    public string? DuongDanAnhLinhVat { get; set; }

    [Column("ViTriMascot")]
    public string? ViTriLinhVat { get; set; } = "right";
    ```
    Alternatively, flip the primary mapped properties back to `LinkAnhBia`, `FontTieuDe`, etc., and mark `DuongDanAnhBia`, etc. as `[NotMapped]` proxies.
  - Apply corresponding mapping to other entities (`CauHinhKyUc`, `CauHinhChuongTrinh`, `CauHinhGayQuy`, `CauHinhBanToChuc`, `CauHinhTinTuc`) for `FontTieuDe`, `FontNoiDung`, `LinkAnhBia` if applicable.

---

## 5. Verification Method

To independently verify these findings:
1. **Inspect SQL Schema & Migrations**:
   - `view_file` on `/home/hutech/Vinh_Offline/WEB_TinhNguyen/SQLQuery1.sql` lines 108–132 to observe column definitions for `CauHinhTrangChu`.
   - `view_file` on `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Migrations/20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` lines 118–125 to observe the 8 renamed columns.
2. **Inspect Entity Definitions**:
   - `view_file` on `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/MoHinh/Entities.cs` lines 24–75 to observe how the 8 invalid columns are mapped without `[Column]` attributes.
3. **Build & Test Suite Execution**:
   - Run compilation:
     ```bash
     export XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp
     dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj
     ```
   - Run E2E and challenger tests:
     ```bash
     bash /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/run_e2e_tests.sh --all
     dotnet run --no-build --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/M1Challenger/M1Challenger.csproj
     dotnet run --no-build --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/M3Challenger2/M3Challenger2.csproj
     ```
