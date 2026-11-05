## 2026-11-05T06:43:06Z
Sender: 4eace438-5a89-4ae7-8648-53baac402eb9

Sửa lỗi SqlException về các cột không hợp lệ (DuongDanAnhBia, DuongDanAnhLinhVat, HienThiHieuUng3D, HienThiLinhVat, KieuChuNoiDung, KieuChuTieuDe, VeDuAn_DuongDanAnh, ViTriLinhVat) bằng cách cập nhật mã nguồn (Models, DbContext, truy vấn) khớp với cơ sở dữ liệu hiện tại, đồng thời khắc phục toàn bộ lỗi để dự án biên dịch và khởi chạy thành công.

Integrity mode: development

## Requirements

### R1. Đồng bộ mã nguồn C# với cấu trúc cơ sở dữ liệu hiện tại
Loại bỏ hoặc cập nhật các thuộc tính trong Models, DbContext và các câu truy vấn LINQ/SQL tương ứng với các cột không tồn tại trong cơ sở dữ liệu: `DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat`, đảm bảo không còn phát sinh `SqlException: Invalid column name`.

### R2. Khắc phục toàn bộ lỗi và đảm bảo dự án chạy được
Rà soát và giải quyết triệt để tất cả các lỗi biên dịch và lỗi runtime còn lại trong dự án để toàn bộ ứng dụng web có thể biên dịch và khởi chạy ổn định.

### R3. Thiết lập và thực thi kịch bản kiểm thử
Đội ngũ tự thiết lập các kịch bản kiểm thử độc lập để xác minh các truy vấn dữ liệu hoạt động chính xác và ứng dụng không gặp lỗi máy chủ (HTTP 500) khi truy cập các chức năng.

## Acceptance Criteria

### Đồng bộ cơ sở dữ liệu & Mã nguồn
- [ ] Không còn thuộc tính hoặc truy vấn tham chiếu đến các cột không tồn tại (`DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat`) gây lỗi trong Models, DbContext và các controller/service.
- [ ] Không phát sinh lỗi `SqlException: Invalid column name` khi ứng dụng thực thi truy vấn cơ sở dữ liệu.

### Vận hành & Kiểm thử
- [ ] Lệnh `dotnet build` thực thi thành công với 0 lỗi (0 errors).
- [ ] Lệnh `dotnet run` khởi chạy ứng dụng web thành công.
- [ ] Kịch bản kiểm thử do đội ngũ thiết lập xác nhận các trang web chính tải và truy vấn dữ liệu thành công mà không phát sinh lỗi 500 Internal Server Error.
