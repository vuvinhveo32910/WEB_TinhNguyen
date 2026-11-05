# Progress — Worker 1

Last visited: 2026-11-05T07:13:00Z

- [x] Initialized DISPATCH.md and BRIEFING.md
- [x] Read survey reports and PROJECT.md
- [x] Inspected Entities.cs, ApplicationDbContext.cs, and ApplicationDbContextModelSnapshot.cs
- [x] Implemented [NotMapped] and 2-way proxies in Entities.cs:
  - `CauHinhTrangChu`: mapped physical DB columns (`LinkAnhBia`, `FontTieuDe`, `FontNoiDung`, `HienThiCanvas3D`, `VeDuAn_LinkAnh`, `HienThiMascot`, `LinkAnhMascot`, `ViTriMascot`), marked 8 invalid column names (`DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat`) with `[NotMapped]` and bidirectional getter/setter proxies.
  - `CauHinhKyUc`: mapped `LinkAnhBia`, `FontTieuDe`, `FontTieuDeTongKet`; marked `DuongDanAnhBia`, `KieuChuTieuDe`, `KieuChuTieuDeTongKet` with `[NotMapped]`.
  - `CauHinhChuongTrinh`: mapped `FontTieuDe`, marked `KieuChuTieuDe` with `[NotMapped]`.
  - `CauHinhGayQuy`: mapped `FontTieuDe`, `FontNoiDung`, marked `KieuChuTieuDe`, `KieuChuNoiDung` with `[NotMapped]`.
  - `CauHinhBanToChuc`: mapped `FontTieuDe`, `FontNoiDung`, marked `KieuChuTieuDe`, `KieuChuNoiDung` with `[NotMapped]`.
  - `CauHinhTinTuc`: mapped `FontTieuDe`, `FontNoiDung`, marked `KieuChuTieuDe`, `KieuChuNoiDung` with `[NotMapped]`.
- [x] Added defense-in-depth Fluent API `.Ignore(...)` and explicit property mapping in `ApplicationDbContext.OnModelCreating`.
- [x] Synchronized `ApplicationDbContextModelSnapshot.cs` removing invalid column scalar properties and registering physical columns.
- [x] Verified controllers and viewmodels compile cleanly without any CS0103, CS1061 errors.
- [x] Built project `WEB_TinhNguyen.slnx` with 0 errors.
- [x] Ran automated test suites:
  - `tests/run_e2e_tests.sh --all`: 337 / 337 PASSED (100%).
  - `tests/M1Challenger`: 67 / 67 PASSED (100%).
  - `tests/M3Challenger2`: 128 / 128 PASSED (100%).
- [x] Empirically validated EF Core model and generated SQL string: ZERO invalid column names present, ALL physical column names queried.
- [ ] Write handoff.md and notify parent
