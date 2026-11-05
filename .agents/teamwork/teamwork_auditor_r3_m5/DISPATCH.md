## 2026-11-04T08:08:12Z

BẠN LÀ FINAL FORENSIC INTEGRITY AUDITOR CHO ĐỒ ÁN TÁI CẤU TRÚC WEBSITE TÌNH NGUYỆN.
Working directory của bạn: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r3_m5/
Path to ORIGINAL_REQUEST.md: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
Path to PROJECT.md: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md

NHIỆM VỤ:
Thực hiện thẩm định pháp y độc lập, toàn diện trên toàn bộ đồ án theo 4 nhóm yêu cầu chính và các tiêu chí nghiệm thu:
1. R1: Tái cấu trúc CSDL và dọn dẹp mã nguồn:
   - Quét kiểm tra xóa sạch 100% thuộc tính màu trong MoHinh/Entities.cs và CauHinhTrangChuVM.cs.
   - Kiểm tra tệp Migration EF Core `20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` và snapshot `ApplicationDbContextModelSnapshot.cs`.
   - Kiểm tra đổi tên bảng/cột sang tiếng Việt và cập nhật truy vấn LINQ/SQL.
   - Quét sạch các input màu `type="color"` trong GiaoDien/Admin*.
2. R2: Việt hóa cấu trúc thư mục & Razor ViewEngine:
   - Kiểm tra 4 thư mục chính: `BoDieuKhien`, `MoHinh`, `GiaoDien`, `DuLieu`.
   - Kiểm tra `VietnameseViewLocationExpander.cs` và cấu hình trong `Program.cs`.
   - Kiểm tra 22 layout paths trỏ về `~/GiaoDien/Shared/`.
3. R3: Đổi mới giao diện Tailwind CSS & Bảo toàn Cửa hàng:
   - Kiểm tra Volunteer Identity Palette (nền trắng `#ffffff`, chữ `#143823` WCAG AAA > 12:1).
   - Kiểm tra 4 animations (`float`, `pulse-slow`, `heartbeat`, `fade-in-up`) và `wwwroot/css/site.css`.
   - Kiểm tra `git diff HEAD WEB_TinhNguyen/Areas/CuaHang/Views/` = 0 (bảo toàn 100% 9 views và layout riêng).
4. R4: Chuyển đổi Sáng/Tối (Light/Dark Mode):
   - Nút icon Mặt trăng/Mặt trời trên Header Desktop và Mobile ngoài cửa hàng.
   - Script Anti-FOUC trong `<head>` nạp `localStorage.site_theme` và cô lập `/cuahang*`.
   - Biến CSS Dark mode: `--bg-page: #0a140e`, `--bg-surface: #13241b`, `--text-main: #f8fafc`.
   - Khóa an toàn phân hệ Cửa Hàng.
5. Thực thi các lệnh kiểm tra độc lập:
   - `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
   - `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj`
   - `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/M3Challenger2/M3Challenger2.csproj`
   - `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/M1Challenger/M1Challenger.csproj`
6. Báo cáo:
   - Viết đầy đủ vào `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r3_m5/handoff.md`.
   - Đưa ra kết luận rõ ràng: **CLEAN** hoặc **INTEGRITY VIOLATION**.
   - Gửi tin nhắn kết luận cho orchestrator.
