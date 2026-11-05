## 2026-11-05T07:00:32Z
[Message] timestamp=2026-11-05T07:00:32Z sender=13941881-5ddb-4ef5-85c6-23bd414f7662 priority=MESSAGE_PRIORITY_HIGH content=You are Worker 1 (Entity, DbContext and Synchronization Implementer).
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/worker_1/
Write only to your working directory for metadata (progress.md, handoff.md).
You have exclusive write ownership of:
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/MoHinh/Entities.cs
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Migrations/ApplicationDbContextModelSnapshot.cs
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/BoDieuKhien/AdminTrangChuController.cs (if needed)
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/BoDieuKhien/AdminKyUcController.cs (if needed)
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/BoDieuKhien/AdminChuongTrinhController.cs (if needed)
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/BoDieuKhien/AdminGayQuyController.cs (if needed)
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/BoDieuKhien/AdminBanToChucController.cs (if needed)
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/BoDieuKhien/AdminTinTucController.cs (if needed)
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/MoHinh/CauHinhTrangChuVM.cs (if needed)

Read the user request at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
Read the survey reports:
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_1/handoff.md
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_2/handoff.md
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_3/handoff.md
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator_1/PROJECT.md

MANDATORY INTEGRITY WARNING:
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

Implementation Tasks:
1. In `WEB_TinhNguyen/MoHinh/Entities.cs`:
   - In `CauHinhTrangChu`:
     Configure the 8 invalid columns (`DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat`) with `[NotMapped]`.
     Ensure the actual physical database columns (`LinkAnhBia`, `FontTieuDe`, `FontNoiDung`, `HienThiCanvas3D`, `VeDuAn_LinkAnh`, `HienThiMascot`, `LinkAnhMascot`, `ViTriMascot`) are the primary mapped properties (remove `[NotMapped]` from them so EF Core maps them directly to the DB columns).
     Provide two-way getter/setter proxying between the pairs so code accessing either name works seamlessly and preserves values.
   - In `CauHinhKyUc`:
     Ensure `LinkAnhBia`, `FontTieuDe`, `FontTieuDeTongKet` are mapped properties (no `[NotMapped]`). Mark `DuongDanAnhBia`, `KieuChuTieuDe`, `KieuChuTieuDeTongKet` with `[NotMapped]` with proper proxying.
   - In `CauHinhChuongTrinh`:
     Ensure `FontTieuDe` is mapped, mark `KieuChuTieuDe` with `[NotMapped]`.
   - In `CauHinhGayQuy`:
     Ensure `FontTieuDe` and `FontNoiDung` are mapped, mark `KieuChuTieuDe` and `KieuChuNoiDung` with `[NotMapped]`.
   - In `CauHinhBanToChuc`:
     Ensure `FontTieuDe` and `FontNoiDung` are mapped, mark `KieuChuTieuDe` and `KieuChuNoiDung` with `[NotMapped]`.
   - In `CauHinhTinTuc`:
     Ensure `FontTieuDe` and `FontNoiDung` are mapped, mark `KieuChuTieuDe` and `KieuChuNoiDung` with `[NotMapped]`.

2. In `WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs`:
   - In `OnModelCreating`, add explicit Fluent API `.Ignore(...)` on `CauHinhTrangChu` for all 8 invalid column names:
     `DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat`.
   - Add Fluent API `.Ignore(...)` for unmapped aliases on `CauHinhKyUc`, `CauHinhChuongTrinh`, `CauHinhGayQuy`, `CauHinhBanToChuc`, `CauHinhTinTuc`.
   - Ensure `LinkAnhBia`, `FontTieuDe`, `FontNoiDung`, `HienThiCanvas3D`, `VeDuAn_LinkAnh`, `HienThiMascot`, `LinkAnhMascot`, `ViTriMascot` are mapped.

3. In `WEB_TinhNguyen/Migrations/ApplicationDbContextModelSnapshot.cs`:
   - Synchronize the snapshot so that `CauHinhTrangChu` contains scalar properties for `LinkAnhBia`, `FontTieuDe`, `FontNoiDung`, `HienThiCanvas3D`, `VeDuAn_LinkAnh`, `HienThiMascot`, `LinkAnhMascot`, `ViTriMascot` and removes `DuongDanAnhBia`, etc. Do the same for `CauHinhKyUc`, `CauHinhChuongTrinh`, `CauHinhGayQuy`, `CauHinhBanToChuc`, `CauHinhTinTuc`.

4. Check and verify controllers and viewmodels:
   - Ensure clean compilation without any CS0103, CS1061 or other errors.

5. Verification:
   - Run `TMPDIR=/tmp XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build WEB_TinhNguyen.slnx` (must succeed with 0 errors).
   - Run `TMPDIR=/tmp XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp bash tests/run_e2e_tests.sh --all` (must pass 100%).
   - Verify EF Core generated SQL does NOT contain any of the 8 invalid column names.

6. Document all changes, files modified, build/test commands and output in `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/worker_1/handoff.md`.
7. When done, send a message to parent with the summary and path to your handoff.md.
