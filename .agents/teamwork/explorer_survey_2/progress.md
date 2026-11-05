# Progress Tracking — Explorer 2 (Models and DbContext Analyst)

- Last visited: 2026-11-05T06:55:00Z
- Status: COMPLETED
- Summary of Work:
  1. Completed thorough codebase survey of the 8 invalid columns: `DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat`.
  2. Identified all declaring Model/Entity classes (`CauHinhTrangChu`, `CauHinhKyUc`, `CauHinhChuongTrinh`, `CauHinhGayQuy`, `CauHinhBanToChuc`, `CauHinhTinTuc`) and ViewModels.
  3. Identified root cause: Absence of `[NotMapped]` on these 8 properties in entities, while `[NotMapped]` was placed on legacy aliases (`LinkAnhBia`, `FontTieuDe`, etc.), causing EF Core to project missing columns into SQL `SELECT` queries.
  4. Identified that `ApplicationDbContext.OnModelCreating` lacks Fluent API mapping for these configuration entities.
  5. Formulated and verified architectural recommendation: Apply `[NotMapped]` to all 8 properties across all entities, add Fluent API `.Ignore()` in `ApplicationDbContext.OnModelCreating`, and remove them from `ApplicationDbContextModelSnapshot.cs`.
  6. Generated complete 5-component report at `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_2/handoff.md`.
