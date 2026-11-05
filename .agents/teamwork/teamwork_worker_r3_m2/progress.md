# TIẾN ĐỘ THỰC HIỆN — WORKER M2 (Kiến trúc sư Cơ sở dữ liệu & EF Core)

Last visited: 2026-11-04T03:37:30Z

## Kế hoạch thực hiện:
- [ ] Bước 1: Xóa 66 thuộc tính màu và đổi tên bảng/cột tiếng Việt trong `WEB_TinhNguyen/MoHinh/Entities.cs`
- [ ] Bước 2: Xóa 66 thuộc tính màu và đổi tên thuộc tính tương ứng trong `WEB_TinhNguyen/MoHinh/CauHinhTrangChuVM.cs`
- [ ] Bước 3: Cập nhật `WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs` (DbSet BieuNguTrangChus, ThuVienKhoanhKhacs, CauHoiThuongGapTrangChus)
- [ ] Bước 4: Tạo Code First Migration `20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` và Designer, cập nhật `ApplicationDbContextModelSnapshot.cs`
- [ ] Bước 5: Cập nhật `Program.cs` thêm `context.Database.Migrate()` khi `CanConnect()`
- [ ] Bước 6: Cập nhật LINQ queries và controllers trong `WEB_TinhNguyen/BoDieuKhien/` (HomeController, AdminTrangChuController, AdminChuongTrinhController, AdminGayQuyController, AdminBanToChucController, AdminTinTucController, AdminKyUcController)
- [ ] Bước 7: Dọn dẹp form Edit trong `WEB_TinhNguyen/GiaoDien/Admin*` (loại bỏ `<input type="color">` và hex code inputs)
- [ ] Bước 8: Kiểm tra Areas/CuaHang hoàn toàn nguyên vẹn
- [ ] Bước 9: Chạy build WEB_TinhNguyen, build E2ETests, chạy E2E test suite và kiểm tra không còn thuộc tính màu
- [ ] Bước 10: Viết handoff.md và thông báo hoàn thành
