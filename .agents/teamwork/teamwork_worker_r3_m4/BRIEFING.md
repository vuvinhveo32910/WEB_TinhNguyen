# BRIEFING — 2026-11-04T07:55:00Z

## Mission
Triển khai Milestone M4: Tính năng Chuyển đổi Sáng/Tối Light/Dark Mode (Requirement R4) cho WEB_TinhNguyen.

## 🔒 My Identity
- Archetype: worker
- Roles: implementer, qa, specialist
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m4
- Original parent: 656d28b8-bcd4-4f61-b4e3-dd425076f84e
- Milestone: M4 (Requirement R4)

## 🔒 Key Constraints
- DO NOT CHEAT. All implementations must be genuine.
- Khóa an toàn phân hệ Cửa hàng (/CuaHang*): Luôn duy trì light mode, ẩn toggle, không ảnh hưởng view Cửa hàng.
- BẢO TOÀN NGUYÊN VẸN Areas/CuaHang/: KHÔNG chỉnh sửa view nội bộ trong Areas/CuaHang/Views/.
- Build và Test suite E2ETests phải pass 100%.

## Current Parent
- Conversation ID: 656d28b8-bcd4-4f61-b4e3-dd425076f84e
- Updated: 2026-11-04T07:55:00Z

## Task Summary
- **What to build**: Light/Dark mode toggle (Moon/Sun) trên Header _Layout.cshtml (Desktop & Mobile), Anti-FOUC script trong <head>, JS toggleThemeMode(), CSS dark theme variables, Cửa hàng safety lock.
- **Success criteria**: Toggle mượt mà, lưu localStorage site_theme/theme, không giật màn hình (anti-FOUC), Cửa hàng khóa light mode 100%, dotnet build và test pass 100%.
- **Interface contracts**: PROJECT.md, survey handoff.md mục 2.2 và 4.2
- **Code layout**: WEB_TinhNguyen/GiaoDien/Shared/_Layout.cshtml, CSS variables

## Key Decisions Made
1. Thay thế hoàn toàn cơ chế kéo dây vật lý `#pullcord-wrapper` bằng nút toggle icon Mặt trăng (`fa-moon`) / Mặt trời (`fa-sun`) hình tròn tinh gọn, responsive trên cả Header Desktop (`id="themeToggleBtn"`) và Header Mobile (`id="themeToggleBtnMobile"`).
2. Tách biệt hoàn toàn nút toggle khỏi phân hệ Cửa hàng bằng Razor condition `@if (ViewContext.RouteData.Values["area"]?.ToString() != "CuaHang")`.
3. Tích hợp Anti-FOUC script chuẩn tại `<head>` đọc đồng thời `site_theme` và `theme`, cưỡng chế xóa `dark` và gán `light` khi URL thuộc `/cuahang*`.
4. Thiết lập chuẩn biến màu đêm Tây Nguyên cho `html.dark`: `--bg-page: #0a140e`, `--bg-surface: #13241b`, `--bg-surface-soft: #0d1d14`, `--text-main: #f8fafc`, `--border-card: rgba(74, 222, 128, 0.2)` và hiệu ứng glow vàng ấm `#fbbf24` cho nút chuyển chế độ.
5. Xây dựng bộ test harness `Tier5_ChallengerM4_2ThemeHarness.cs` (12 test case chuyên sâu) xác thực mọi ràng buộc của R4.

## Artifact Index
- DISPATCH.md — Nhiệm vụ được giao từ orchestrator
- handoff.md — Báo cáo kết quả công việc hoàn thành
- progress.md — Nhật ký tiến độ

## Change Tracker
- **Files modified**:
  - `WEB_TinhNguyen/GiaoDien/Shared/_Layout.cshtml`: Tích hợp Anti-FOUC script, CSS dark variables & theme toggle styling, nút Toggle Desktop/Mobile, hàm JS `toggleThemeMode()`, gỡ bỏ pullcord.
  - `tests/E2ETests/Tier5/Tier5_ChallengerM4_2ThemeHarness.cs`: Bổ sung 12 tests xác thực R4 Light/Dark Mode.
- **Build status**: PASS (dotnet build: 0 Error, 2 Warning về nuget index offline).
- **Pending issues**: Không còn pending issue nào.

## Quality Status
- **Build/test result**: PASS 100% (E2ETests: 337/337 tests, M3Challenger2: 128/128 tests, M1Challenger: 67/67 tests).
- **Lint status**: Clean.
- **Tests added/modified**: Thêm 12 test cases chuyên sâu trong `Tier5_ChallengerM4_2ThemeHarness.cs`.

## Loaded Skills
- Không có kỹ năng ngoại vi chỉ định.
