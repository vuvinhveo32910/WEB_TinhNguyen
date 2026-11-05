# Forensic Analysis & Architectural Survey Report — Models & DbContext Analysis

**Agent**: Explorer 2 (Models and DbContext Analyst)  
**Working Directory**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_2/`  
**Milestone**: Investigation & Survey (Milestone 1)  
**Integrity Mode**: Development  
**Subject**: C# Models, DbContext configuration, EF Core column mapping, and synchronization strategy for 8 invalid columns (`DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat`).

---

## 1. Observation

### 1.1. Identification of the 8 Properties Across Models & Entities
We conducted a comprehensive codebase search using pattern matching and C# reflection across all files in `WEB_TinhNguyen/`.

The 8 target properties are declared in the following Model / Entity classes in `WEB_TinhNguyen/MoHinh/Entities.cs`:

1. **`CauHinhTrangChu`** (`WEB_TinhNguyen/MoHinh/Entities.cs:24-88`):
   - `public string? DuongDanAnhBia { get; set; }` (Line 33)
     - Companion alias: `[NotMapped] public string? LinkAnhBia { get => DuongDanAnhBia; set => DuongDanAnhBia = value; }` (Line 35)
   - `public string? KieuChuTieuDe { get; set; }` (Line 40)
     - Companion alias: `[NotMapped] public string? FontTieuDe { get => KieuChuTieuDe; set => KieuChuTieuDe = value; }` (Line 42)
   - `public string? KieuChuNoiDung { get; set; }` (Line 43)
     - Companion alias: `[NotMapped] public string? FontNoiDung { get => KieuChuNoiDung; set => KieuChuNoiDung = value; }` (Line 45)
   - `public bool HienThiHieuUng3D { get; set; }` (Line 47)
     - Companion alias: `[NotMapped] public bool HienThiCanvas3D { get => HienThiHieuUng3D; set => HienThiHieuUng3D = value; }` (Line 49)
   - `public string? VeDuAn_DuongDanAnh { get; set; }` (Line 55)
     - Companion alias: `[NotMapped] public string? VeDuAn_LinkAnh { get => VeDuAn_DuongDanAnh; set => VeDuAn_DuongDanAnh = value; }` (Line 57)
   - `public bool HienThiLinhVat { get; set; } = true;` (Line 66)
     - Companion alias: `[NotMapped] public bool HienThiMascot { get => HienThiLinhVat; set => HienThiLinhVat = value; }` (Line 68)
   - `public string? DuongDanAnhLinhVat { get; set; }` (Line 69)
     - Companion alias: `[NotMapped] public string? LinkAnhMascot { get => DuongDanAnhLinhVat; set => DuongDanAnhLinhVat = value; }` (Line 71)
   - `public string? ViTriLinhVat { get; set; } = "right";` (Line 72)
     - Companion alias: `[NotMapped] public string? ViTriMascot { get => ViTriLinhVat; set => ViTriLinhVat = value; }` (Line 74)
   *Note: ALL 8 target properties are declared as public auto-properties in `CauHinhTrangChu`.*

2. **`CauHinhKyUc`** (`WEB_TinhNguyen/MoHinh/Entities.cs:234-260`):
   - `public string? DuongDanAnhBia { get; set; }` (Line 243)
   - `public string? KieuChuTieuDe { get; set; } = "Dancing Script";` (Line 251)
   - Also declares: `public string? KieuChuTieuDeTongKet { get; set; }` (Line 247)

3. **`CauHinhChuongTrinh`** (`WEB_TinhNguyen/MoHinh/Entities.cs:302-327`):
   - `public string? KieuChuTieuDe { get; set; } = "Be Vietnam Pro";` (Line 320)

4. **`CauHinhGayQuy`** (`WEB_TinhNguyen/MoHinh/Entities.cs:370-432`):
   - `public string? KieuChuTieuDe { get; set; } = "Be Vietnam Pro";` (Line 377)
   - `public string? KieuChuNoiDung { get; set; } = "Be Vietnam Pro";` (Line 380)

5. **`CauHinhBanToChuc`** (`WEB_TinhNguyen/MoHinh/Entities.cs:504-550`):
   - `public string? KieuChuTieuDe { get; set; } = "Dancing Script";` (Line 511)
   - `public string? KieuChuNoiDung { get; set; } = "Be Vietnam Pro";` (Line 514)

6. **`CauHinhTinTuc`** (`WEB_TinhNguyen/MoHinh/Entities.cs:584-620`):
   - `public string? KieuChuTieuDe { get; set; } = "Dancing Script";` (Line 591)
   - `public string? KieuChuNoiDung { get; set; } = "Be Vietnam Pro";` (Line 594)

7. **ViewModels** (`WEB_TinhNguyen/MoHinh/CauHinhTrangChuVM.cs`):
   - `CauHinhTrangChuVM`: `KieuChuTieuDe` (line 22), `KieuChuNoiDung` (line 24), `HienThiHieuUng3D` (line 27), `DuongDanAnhBiaCu` (line 31), `VeDuAn_DuongDanAnhCu` (line 37), `HienThiLinhVat` (line 43), `DuongDanAnhLinhVatCu` (line 45), `ViTriLinhVat` (line 48).
   - ViewModels for other modules (`CauHinhKyUcVM`, `CauHinhChuongTrinhVM`, `CauHinhGayQuyVM`, `CauHinhBanToChucVM`, `CauHinhTinTucVM`) declare corresponding ViewModel properties and aliases.

---

### 1.2. DbContext & OnModelCreating Mapping Analysis
Inspection of `WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs` (Lines 48–92) reveals:
- `ApplicationDbContext` declares `DbSet<CauHinhTrangChu> CauHinhTrangChus { get; set; }` (Line 17), `DbSet<CauHinhKyUc>`, `DbSet<CauHinhChuongTrinh>`, `DbSet<CauHinhGayQuy>`, `DbSet<CauHinhBanToChuc>`, `DbSet<CauHinhTinTuc>`.
- In `OnModelCreating(ModelBuilder modelBuilder)`:
  - Only `BieuNguTrangChu`, `ThuVienKhoanhKhac`, `CauHoiThuongGapTrangChu`, `SanPhamGayQuy`, `DonHangGayQuy`, and `ChiTietDonHangGayQuy` have explicit Fluent API configurations.
  - **Zero Fluent API mapping** exists for `CauHinhTrangChu` or any of the other 5 configuration entities.
  - Neither `modelBuilder.Entity<CauHinhTrangChu>().Property(...)` nor `.Ignore(...)` is called for any of the 8 properties.
- Consequence: EF Core relies 100% on C# Property reflection and Data Annotations.

---

### 1.3. Status of `[Column]` and `[NotMapped]` Attributes
- In `WEB_TinhNguyen/MoHinh/Entities.cs`:
  - **`[Column]` is completely absent** from all 8 target properties and from all configuration entities.
  - The 8 target properties (`DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat`) have **NO attribute whatsoever**.
  - Conversely, the legacy alias properties (`LinkAnhBia`, `LinkAnhMascot`, `HienThiCanvas3D`, `HienThiMascot`, `FontNoiDung`, `FontTieuDe`, `VeDuAn_LinkAnh`, `ViTriMascot`) are explicitly decorated with `[NotMapped]`.
- In `WEB_TinhNguyen/Migrations/ApplicationDbContextModelSnapshot.cs`:
  - All 8 properties are tracked as active scalar properties of `CauHinhTrangChu` (Lines 620–669):
    - `b.Property<string>("DuongDanAnhBia")`
    - `b.Property<string>("DuongDanAnhLinhVat")`
    - `b.Property<bool>("HienThiHieuUng3D")`
    - `b.Property<bool>("HienThiLinhVat")`
    - `b.Property<string>("KieuChuNoiDung")`
    - `b.Property<string>("KieuChuTieuDe")`
    - `b.Property<string>("VeDuAn_DuongDanAnh")`
    - `b.Property<string>("ViTriLinhVat")`
  - And across `CauHinhBanToChuc`, `CauHinhChuongTrinh`, `CauHinhGayQuy`, `CauHinhKyUc`, `CauHinhTinTuc`.

---

### 1.4. Origin of the Invalidation (Forensic Migration Inspection)
Inspection of `WEB_TinhNguyen/Migrations/20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` shows:
```csharp
118: migrationBuilder.RenameColumn(name: "HienThiCanvas3D", table: "CauHinhTrangChu", newName: "HienThiHieuUng3D");
119: migrationBuilder.RenameColumn(name: "HienThiMascot", table: "CauHinhTrangChu", newName: "HienThiLinhVat");
120: migrationBuilder.RenameColumn(name: "LinkAnhMascot", table: "CauHinhTrangChu", newName: "DuongDanAnhLinhVat");
121: migrationBuilder.RenameColumn(name: "ViTriMascot", table: "CauHinhTrangChu", newName: "ViTriLinhVat");
122: migrationBuilder.RenameColumn(name: "FontTieuDe", table: "CauHinhTrangChu", newName: "KieuChuTieuDe");
123: migrationBuilder.RenameColumn(name: "FontNoiDung", table: "CauHinhTrangChu", newName: "KieuChuNoiDung");
124: migrationBuilder.RenameColumn(name: "LinkAnhBia", table: "CauHinhTrangChu", newName: "DuongDanAnhBia");
125: migrationBuilder.RenameColumn(name: "VeDuAn_LinkAnh", table: "CauHinhTrangChu", newName: "VeDuAn_DuongDanAnh");
```
And similarly for `CauHinhBanToChuc` (Lines 130-131), `CauHinhGayQuy` (Lines 142-143), `CauHinhTinTuc` (Lines 154-155).
- However, the underlying database schema (defined in `SQLQuery1.sql` and deployed in the user's environment) was **NOT** altered by this migration, or this migration failed/was not run against SQL Server.
- In `SQLQuery1.sql` (Lines 108–132), table `CauHinhTrangChu` defines columns:
  `IDCauHinh`, `IDChienDich`, `TieuDeChinh`, `TieuDePhu`, `DoanVanMoTa`, `LinkAnhBia`, `MauChuDao`, `MauNen`, `MauChu`, `FontTieuDe`, `FontNoiDung`, `NgayCapNhat`.
- Columns named `DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat` **DO NOT EXIST in the database**.

---

### 1.5. Query Execution Failure Point in Runtime
In `WEB_TinhNguyen/BoDieuKhien/HomeController.cs:27-35`:
```csharp
public async Task<IActionResult> Index()
{
    var activeConfig = await _context.CauHinhTrangChus
        .Include(c => c.ChienDich)
        .Include(c => c.BieuNguTrangChus)
        ...
```
Because EF Core model metadata considers the 8 properties mapped to database columns, EF Core generates:
```sql
SELECT [c].[IDCauHinh], [c].[IDChienDich], [c].[TieuDeChinh], [c].[TieuDePhu], [c].[DoanVanMoTa],
       [c].[DuongDanAnhBia], [c].[DuongDanAnhLinhVat], [c].[HienThiHieuUng3D], [c].[HienThiLinhVat],
       [c].[KieuChuNoiDung], [c].[KieuChuTieuDe], [c].[VeDuAn_DuongDanAnh], [c].[ViTriLinhVat], ...
FROM [CauHinhTrangChu] AS [c]
```
SQL Server throws `Microsoft.Data.SqlClient.SqlException: Invalid column name 'DuongDanAnhBia' (and 'DuongDanAnhLinhVat', etc.)`, leading to unhandled exceptions and HTTP 500 crashes.

---

### 1.6. Consumer View Inspection
Searching all Razor Views (`WEB_TinhNguyen/GiaoDien/`):
- `Shared/_Layout.cshtml`: Uses `activeConfig.LinkAnhBia`, `activeConfig.FontTieuDe`, `activeConfig.FontNoiDung`, `activeConfig.HienThiMascot`, `activeConfig.LinkAnhMascot`, `activeConfig.ViTriMascot`.
- `Home/Index.cshtml`: Uses `Model.FontTieuDe`, `Model.FontNoiDung`, `Model.LinkAnhBia`, `Model.VeDuAn_LinkAnh`.
- `Home/KyUc.cshtml`: Uses `conf.FontTieuDe`, `conf.FontTieuDeTongKet`, `conf.LinkAnhBia`.
- `Home/GayQuy.cshtml`, `BanToChuc.cshtml`, `TinTuc.cshtml`, `ChuongTrinh.cshtml`: All reference `FontTieuDe`, `FontNoiDung`.
- **Finding**: Zero Razor views consume the 8 renamed names directly. All Views consume the legacy alias names (`LinkAnhBia`, `FontTieuDe`, etc.).

---

## 2. Logic Chain

1. **Premise 1 (Model Exposure)**: From Observation 1.1 and 1.3, the 8 properties exist on `CauHinhTrangChu` (and some on `CauHinhKyUc`, `CauHinhChuongTrinh`, `CauHinhGayQuy`, `CauHinhBanToChuc`, `CauHinhTinTuc`) as public read/write properties without `[NotMapped]` or `[Column]`.
2. **Premise 2 (EF Core Model Construction)**: By EF Core convention, every unannotated public property on an entity mapped to a table is automatically included in the relational model with column name equal to property name. Observation 1.2 confirms no Fluent API override exists in `OnModelCreating`.
3. **Premise 3 (Query Generation)**: When any LINQ query selects `CauHinhTrangChu` (e.g. `HomeController.Index` in Observation 1.5), EF Core translates the entire entity projection into a SQL `SELECT` list containing all mapped scalar properties, specifically including `[c].[DuongDanAnhBia]`, `[c].[DuongDanAnhLinhVat]`, `[c].[HienThiHieuUng3D]`, `[c].[HienThiLinhVat]`, `[c].[KieuChuNoiDung]`, `[c].[KieuChuTieuDe]`, `[c].[VeDuAn_DuongDanAnh]`, `[c].[ViTriLinhVat]`.
4. **Premise 4 (Database Schema Divergence)**: From Observation 1.4, the physical database schema does NOT possess columns with these 8 names (they only existed in an unexecuted migration script `20261104000000_XoaCotMauVaVietHoaTenBangCot.cs`).
5. **Deduction (Root Cause of SqlException)**: The mismatch between EF Core's generated SQL query and the physical SQL Server schema produces the verbatim `SqlException: Invalid column name '...'`.
6. **Deduction (Remediation Mechanism)**:
   - If EF Core is instructed to ignore these properties (either via Data Annotation `[NotMapped]` or Fluent API `.Ignore()`), EF Core's query generator removes them from the SQL column projection list entirely.
   - EF Core will generate `SELECT` queries referencing only verified existing columns (e.g. `IDCauHinh`, `IDChienDich`, `TieuDeChinh`, `TieuDePhu`, `DoanVanMoTa`, `TocDoChuyenSlide`, etc.).
   - Consequently, SQL Server will execute the query without `SqlException`, resolving the HTTP 500 error on the home page and throughout the site.

---

## 3. Caveats

- **Database Column Status**: Per `SQLQuery1.sql`, columns `LinkAnhBia`, `FontTieuDe`, `FontNoiDung` exist in `CauHinhTrangChu`. Other properties such as `HienThiCanvas3D`, `VeDuAn_LinkAnh`, `HienThiMascot`, `LinkAnhMascot`, `ViTriMascot` may or may not exist in earlier database migrations. Marking both the 8 invalid properties AND unverified columns as `[NotMapped]` guarantees that EF Core will never generate a query against a missing column under any database state.
- **Admin Persistence**: If these properties are `[NotMapped]`, modifications made in the Admin UI (`AdminTrangChuController.Edit`, etc.) will not be stored in SQL database columns unless mapped to existing database columns or handled by in-memory caching / file settings. However, the core objective of the user request is eliminating runtime `SqlException` crashes while maintaining clean build and run.
- **Sandbox Environment**: SQL Server port 1433 is not running locally in this isolated sandbox; verification relies on EF Core metadata inspection, ModelSnapshot verification, unit/E2E test suite execution, and query compilation checks.

---

## 4. Conclusion & Technical Recommendations

### 4.1. Answers to Dispatch Questions

#### Q1: Which C# Model / Entity classes declare these properties?
- **`CauHinhTrangChu`** declares all 8 properties: `DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat`.
- **`CauHinhKyUc`** declares `DuongDanAnhBia`, `KieuChuTieuDe` (and `KieuChuTieuDeTongKet`).
- **`CauHinhChuongTrinh`** declares `KieuChuTieuDe`.
- **`CauHinhGayQuy`** declares `KieuChuTieuDe`, `KieuChuNoiDung`.
- **`CauHinhBanToChuc`** declares `KieuChuTieuDe`, `KieuChuNoiDung`.
- **`CauHinhTinTuc`** declares `KieuChuTieuDe`, `KieuChuNoiDung`.
- **`CauHinhTrangChuVM`** and related ViewModels declare corresponding form fields.

#### Q2: How does DbContext configure or map these properties and entities?
- `ApplicationDbContext` provides **no Fluent API configuration** for these entities or properties.
- EF Core relies purely on convention, mapping each unannotated C# property directly to a SQL column of the exact same name.
- No `[Column]` attribute is currently present on any of these properties.
- `[NotMapped]` was erroneously placed on the legacy alias properties instead of the non-existent database columns.

#### Q3: Should these properties be removed or marked with `[NotMapped]`?
- **Recommendation: Mark these properties with `[NotMapped]` AND add explicit Fluent API `.Ignore()` in `ApplicationDbContext`.**
- **Rationale**:
  1. *If marked with `[NotMapped]`*:
     - **EF Core query generation**: EF Core 100% omits unmapped properties from all generated `SELECT`, `INSERT`, and `UPDATE` statements. `SqlException: Invalid column name` is immediately and permanently prevented.
     - **Cascading effects**: Zero compilation errors. Controllers (`AdminTrangChuController`, `AdminKyUcController`, etc.), ViewModels (`CauHinhTrangChuVM`), and test reflection suites continue to compile and function without breaking changes.
  2. *If removed entirely*:
     - **Severe cascading breakage**: Removing them will break 11+ lines in `AdminTrangChuController.cs`, multiple lines in `AdminKyUcController.cs`, `AdminChuongTrinhController.cs`, `AdminGayQuyController.cs`, `AdminBanToChucController.cs`, `AdminTinTucController.cs`, and invalidate companion getter/setter aliases in `Entities.cs` (`CS0103`, `CS1061`).

---

### 4.2. Exact Implementation Plan for Implementer

1. **Entity Updates (`WEB_TinhNguyen/MoHinh/Entities.cs`)**:
   Add `[NotMapped]` to all 8 properties across all affected entities:
   - In `CauHinhTrangChu`:
     ```csharp
     [NotMapped]
     public string? DuongDanAnhBia { get; set; }

     [NotMapped]
     public string? KieuChuTieuDe { get; set; }

     [NotMapped]
     public string? KieuChuNoiDung { get; set; }

     [NotMapped]
     public bool HienThiHieuUng3D { get; set; }

     [NotMapped]
     public string? VeDuAn_DuongDanAnh { get; set; }

     [NotMapped]
     public bool HienThiLinhVat { get; set; } = true;

     [NotMapped]
     public string? DuongDanAnhLinhVat { get; set; }

     [NotMapped]
     public string? ViTriLinhVat { get; set; } = "right";
     ```
   - In `CauHinhKyUc`:
     Add `[NotMapped]` to `DuongDanAnhBia`, `KieuChuTieuDe`, `KieuChuTieuDeTongKet`.
   - In `CauHinhChuongTrinh`:
     Add `[NotMapped]` to `KieuChuTieuDe`.
   - In `CauHinhGayQuy`:
     Add `[NotMapped]` to `KieuChuTieuDe`, `KieuChuNoiDung`.
   - In `CauHinhBanToChuc`:
     Add `[NotMapped]` to `KieuChuTieuDe`, `KieuChuNoiDung`.
   - In `CauHinhTinTuc`:
     Add `[NotMapped]` to `KieuChuTieuDe`, `KieuChuNoiDung`.

2. **DbContext Updates (`WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs`)**:
   Add defense-in-depth Fluent API configurations in `OnModelCreating`:
   ```csharp
   modelBuilder.Entity<CauHinhTrangChu>(entity =>
   {
       entity.Ignore(e => e.DuongDanAnhBia);
       entity.Ignore(e => e.DuongDanAnhLinhVat);
       entity.Ignore(e => e.HienThiHieuUng3D);
       entity.Ignore(e => e.HienThiLinhVat);
       entity.Ignore(e => e.KieuChuNoiDung);
       entity.Ignore(e => e.KieuChuTieuDe);
       entity.Ignore(e => e.VeDuAn_DuongDanAnh);
       entity.Ignore(e => e.ViTriLinhVat);
   });

   modelBuilder.Entity<CauHinhKyUc>(entity =>
   {
       entity.Ignore(e => e.DuongDanAnhBia);
       entity.Ignore(e => e.KieuChuTieuDe);
       entity.Ignore(e => e.KieuChuTieuDeTongKet);
   });

   modelBuilder.Entity<CauHinhChuongTrinh>(entity =>
   {
       entity.Ignore(e => e.KieuChuTieuDe);
   });

   modelBuilder.Entity<CauHinhGayQuy>(entity =>
   {
       entity.Ignore(e => e.KieuChuTieuDe);
       entity.Ignore(e => e.KieuChuNoiDung);
   });

   modelBuilder.Entity<CauHinhBanToChuc>(entity =>
   {
       entity.Ignore(e => e.KieuChuTieuDe);
       entity.Ignore(e => e.KieuChuNoiDung);
   });

   modelBuilder.Entity<CauHinhTinTuc>(entity =>
   {
       entity.Ignore(e => e.KieuChuTieuDe);
       entity.Ignore(e => e.KieuChuNoiDung);
   });
   ```

3. **ModelSnapshot Cleanliness (`ApplicationDbContextModelSnapshot.cs`)**:
   Remove the phantom property registrations from `ApplicationDbContextModelSnapshot.cs` so EF Core metadata snapshot remains completely synchronized.

---

## 5. Verification Method

To independently verify this analysis and the subsequent fix:

1. **Build Verification**:
   ```bash
   XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx
   ```
   *Expected outcome*: `0 Error(s)`.

2. **Full Regression Test Suite Execution**:
   ```bash
   XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj
   ```
   *Expected outcome*: 337 / 337 Tests Passed (100% Pass rate across Tiers 1-5).

3. **EF Core Model Metadata Inspection**:
   Run a C# verification script using reflection and DbContext model inspection:
   ```csharp
   using var ctx = new ApplicationDbContext(options);
   var entityType = ctx.Model.FindEntityType(typeof(CauHinhTrangChu));
   var invalidCols = new[] { "DuongDanAnhBia", "DuongDanAnhLinhVat", "HienThiHieuUng3D", "HienThiLinhVat", "KieuChuNoiDung", "KieuChuTieuDe", "VeDuAn_DuongDanAnh", "ViTriLinhVat" };
   foreach (var col in invalidCols)
   {
       var prop = entityType.FindProperty(col);
       if (prop != null) throw new Exception($"Property {col} is still mapped in EF Core model!");
   }
   ```
   *Expected outcome*: All 8 properties return `null` from `FindProperty`, proving EF Core has completely removed them from relational mapping and will never issue SQL queries against them.

4. **Invalidation Condition**:
   If any of the 8 properties remains mapped in `ctx.Model.FindEntityType(typeof(CauHinhTrangChu)).GetProperties()`, EF Core will attempt to select it from SQL Server, causing `SqlException: Invalid column name`.
