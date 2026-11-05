using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;
using WEB_TinhNguyen.Areas.CuaHang.Models;
using WEB_TinhNguyen.DuLieu;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier5
{
    /// <summary>
    /// Tier 5: Adversarial Challenge & Stress Harness
    /// Empirically verifies validation rules, edge cases, negative quantities, boundary conditions,
    /// EF Core DbContext metadata, session resilience, and concurrency.
    /// </summary>
    public class Tier5_ChallengerTests
    {
        private static bool ValidateModel(object model, out List<ValidationResult> results)
        {
            var context = new ValidationContext(model);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        }

        // =========================================================================
        // 1. SanPhamGayQuy Validation & Edge Cases
        // =========================================================================

        [E2ETest(5, "C01: SanPhamGayQuy standard valid model passes validation")]
        public void C01_SanPhamGayQuy_ValidModel_PassesValidation()
        {
            var product = new SanPhamGayQuy
            {
                TenSanPham = "Áo Thun Gây Quỹ",
                GiaBan = 129000m,
                GiaGoc = 150000m,
                SoLuongTon = 100,
                PhanTramDongGop = 100,
                DanhMuc = "Trang phục"
            };

            var isValid = ValidateModel(product, out var results);
            Assert.True(isValid, $"Valid product must pass validation, got errors: {string.Join(", ", results.Select(r => r.ErrorMessage))}");
        }

        [E2ETest(5, "C02: SanPhamGayQuy empty or null name fails validation")]
        public void C02_SanPhamGayQuy_EmptyOrNullName_FailsValidation()
        {
            var product = new SanPhamGayQuy
            {
                TenSanPham = "",
                GiaBan = 50000m,
                SoLuongTon = 10
            };

            var isValid = ValidateModel(product, out var results);
            Assert.False(isValid, "Empty product name must fail validation");
            Assert.True(results.Any(r => r.MemberNames.Contains("TenSanPham")), "Validation error must target TenSanPham");
        }

        [E2ETest(5, "C03: SanPhamGayQuy negative price fails validation")]
        public void C03_SanPhamGayQuy_NegativePrice_FailsValidation()
        {
            var product = new SanPhamGayQuy
            {
                TenSanPham = "Sản phẩm thử nghiệm",
                GiaBan = -1m,
                SoLuongTon = 10
            };

            var isValid = ValidateModel(product, out var results);
            Assert.False(isValid, "Negative GiaBan must fail validation");
            Assert.True(results.Any(r => r.MemberNames.Contains("GiaBan")), "Validation error must target GiaBan");
        }

        [E2ETest(5, "C04: SanPhamGayQuy zero price is allowed for promotional items")]
        public void C04_SanPhamGayQuy_ZeroPrice_AllowedForPromotions()
        {
            var product = new SanPhamGayQuy
            {
                TenSanPham = "Quà tặng 0đ Tri Ân",
                GiaBan = 0m,
                SoLuongTon = 50
            };

            var isValid = ValidateModel(product, out var results);
            Assert.True(isValid, "Zero price should be allowed for promotional items");
        }

        [E2ETest(5, "C05: SanPhamGayQuy price ceiling boundary at 1 billion VNĐ")]
        public void C05_SanPhamGayQuy_MaxPriceBoundary()
        {
            var productAtLimit = new SanPhamGayQuy
            {
                TenSanPham = "Vật phẩm đấu giá từ thiện",
                GiaBan = 1000000000m,
                SoLuongTon = 1
            };
            Assert.True(ValidateModel(productAtLimit, out _), "Price at 1,000,000,000 VNĐ must pass");

            var productOverLimit = new SanPhamGayQuy
            {
                TenSanPham = "Vật phẩm vượt trần",
                GiaBan = 1000000001m,
                SoLuongTon = 1
            };
            Assert.False(ValidateModel(productOverLimit, out var results), "Price above 1B VNĐ must fail");
            Assert.True(results.Any(r => r.MemberNames.Contains("GiaBan")), "Error must target GiaBan");
        }

        [E2ETest(5, "C06: SanPhamGayQuy negative original price fails validation")]
        public void C06_SanPhamGayQuy_NegativeGiaGoc_FailsValidation()
        {
            var product = new SanPhamGayQuy
            {
                TenSanPham = "Sản phẩm",
                GiaBan = 10000m,
                GiaGoc = -5000m,
                SoLuongTon = 10
            };

            Assert.False(ValidateModel(product, out var results), "Negative GiaGoc must fail validation");
            Assert.True(results.Any(r => r.MemberNames.Contains("GiaGoc")), "Error must target GiaGoc");
        }

        [E2ETest(5, "C07: SanPhamGayQuy charity percentage boundary [1, 100]")]
        public void C07_SanPhamGayQuy_CharityPercentage_Boundaries()
        {
            var prod0 = new SanPhamGayQuy { TenSanPham = "A", GiaBan = 10000m, PhanTramDongGop = 0, SoLuongTon = 1 };
            Assert.False(ValidateModel(prod0, out _), "0% donation must fail validation (minimum 1%)");

            var prod1 = new SanPhamGayQuy { TenSanPham = "A", GiaBan = 10000m, PhanTramDongGop = 1, SoLuongTon = 1 };
            Assert.True(ValidateModel(prod1, out _), "1% donation must pass");

            var prod100 = new SanPhamGayQuy { TenSanPham = "A", GiaBan = 10000m, PhanTramDongGop = 100, SoLuongTon = 1 };
            Assert.True(ValidateModel(prod100, out _), "100% donation must pass");

            var prod101 = new SanPhamGayQuy { TenSanPham = "A", GiaBan = 10000m, PhanTramDongGop = 101, SoLuongTon = 1 };
            Assert.False(ValidateModel(prod101, out _), "101% donation must fail validation");

            var prodNeg = new SanPhamGayQuy { TenSanPham = "A", GiaBan = 10000m, PhanTramDongGop = -10, SoLuongTon = 1 };
            Assert.False(ValidateModel(prodNeg, out _), "Negative percentage must fail validation");
        }

        [E2ETest(5, "C08: SanPhamGayQuy stock quantity boundaries")]
        public void C08_SanPhamGayQuy_StockQuantity_Boundaries()
        {
            var prodNeg = new SanPhamGayQuy { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = -1 };
            Assert.False(ValidateModel(prodNeg, out _), "-1 stock must fail validation");

            var prodZero = new SanPhamGayQuy { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 0 };
            Assert.True(ValidateModel(prodZero, out _), "0 stock (sold out) must pass validation");

            var prodOver = new SanPhamGayQuy { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 1000001 };
            Assert.False(ValidateModel(prodOver, out _), ">1M stock must fail validation");
        }

        [E2ETest(5, "C09: SanPhamGayQuy string length boundaries enforced")]
        public void C09_SanPhamGayQuy_StringLengths_Enforced()
        {
            var longDesc = new string('x', 501);
            var prodLongDesc = new SanPhamGayQuy { TenSanPham = "A", GiaBan = 10000m, MoTaNgan = longDesc };
            Assert.False(ValidateModel(prodLongDesc, out var r1), "MoTaNgan > 500 chars must fail");
            Assert.True(r1.Any(r => r.MemberNames.Contains("MoTaNgan")), "Error must target MoTaNgan");

            var longMessage = new string('m', 501);
            var prodLongMsg = new SanPhamGayQuy { TenSanPham = "A", GiaBan = 10000m, ThongDiepGayQuy = longMessage };
            Assert.False(ValidateModel(prodLongMsg, out var r2), "ThongDiepGayQuy > 500 chars must fail");
            Assert.True(r2.Any(r => r.MemberNames.Contains("ThongDiepGayQuy")), "Error must target ThongDiepGayQuy");

            var longImg = "https://example.com/" + new string('a', 1000);
            var prodLongImg = new SanPhamGayQuy { TenSanPham = "A", GiaBan = 10000m, LinkHinhAnh = longImg };
            Assert.False(ValidateModel(prodLongImg, out var r3), "LinkHinhAnh > 1000 chars must fail");
        }

        [E2ETest(5, "C10: SanPhamGayQuy not-mapped properties calculate correctly")]
        public void C10_SanPhamGayQuy_NotMappedProperties_CalculatesCorrectly()
        {
            var inStock = new SanPhamGayQuy { GiaBan = 129000m, PhanTramDongGop = 100, SoLuongTon = 10 };
            Assert.True(inStock.ConHang, "ConHang must be true when SoLuongTon > 0");
            Assert.Equal(129000m, inStock.TienDongGopTrenMoiSp, "TienDongGopTrenMoiSp must equal GiaBan when 100%");

            var outOfStock = new SanPhamGayQuy { GiaBan = 80000m, PhanTramDongGop = 50, SoLuongTon = 0 };
            Assert.False(outOfStock.ConHang, "ConHang must be false when SoLuongTon is 0");
            Assert.Equal(40000m, outOfStock.TienDongGopTrenMoiSp, "50% donation of 80,000 must be 40,000");
        }

        // =========================================================================
        // 2. SanPhamCrudViewModel & Mapping Robustness
        // =========================================================================

        [E2ETest(5, "C11: SanPhamCrudViewModel valid model passes validation")]
        public void C11_SanPhamCrudViewModel_ValidModel_PassesValidation()
        {
            var vm = new SanPhamCrudViewModel
            {
                TenSanPham = "Khăn Quàng Tình Nguyện",
                GiaBan = 65000m,
                GiaGoc = 75000m,
                PhanTramDongGop = 100,
                SoLuongTon = 50
            };

            Assert.True(ValidateModel(vm, out _), "Valid SanPhamCrudViewModel must pass");
        }

        [E2ETest(5, "C12: SanPhamCrudViewModel negative price or stock fails validation")]
        public void C12_SanPhamCrudViewModel_NegativePriceOrStock_FailsValidation()
        {
            var vmPrice = new SanPhamCrudViewModel { TenSanPham = "B", GiaBan = -500m };
            Assert.False(ValidateModel(vmPrice, out _), "Negative price in CrudViewModel must fail");

            var vmStock = new SanPhamCrudViewModel { TenSanPham = "B", GiaBan = 500m, SoLuongTon = -10 };
            Assert.False(ValidateModel(vmStock, out _), "Negative stock in CrudViewModel must fail");

            var vmPerc = new SanPhamCrudViewModel { TenSanPham = "B", GiaBan = 500m, PhanTramDongGop = 0 };
            Assert.False(ValidateModel(vmPerc, out _), "0% in CrudViewModel must fail");
        }

        [E2ETest(5, "C13: SanPhamCrudViewModel ToEntity trims whitespace and sets defaults")]
        public void C13_SanPhamCrudViewModel_ToEntity_TrimsAndMapsCorrectly()
        {
            var vm = new SanPhamCrudViewModel
            {
                TenSanPham = "  Áo Len Ấm Áp   ",
                GiaBan = 150000m,
                MoTaNgan = "   Mô tả ngắn gọn   ",
                ThongDiepGayQuy = "  Trao áo ấm cho em   ",
                DanhMuc = "   ",
                SoLuongTon = 30
            };

            var entity = vm.ToEntity();
            Assert.Equal("Áo Len Ấm Áp", entity.TenSanPham, "TenSanPham must be trimmed");
            Assert.Equal("Mô tả ngắn gọn", entity.MoTaNgan, "MoTaNgan must be trimmed");
            Assert.Equal("Trao áo ấm cho em", entity.ThongDiepGayQuy, "ThongDiepGayQuy must be trimmed");
            Assert.Equal("Quà lưu niệm", entity.DanhMuc, "Empty DanhMuc must default to 'Quà lưu niệm'");
        }

        [E2ETest(5, "C14: SanPhamCrudViewModel ToEntity uploaded image takes precedence")]
        public void C14_SanPhamCrudViewModel_ToEntity_UploadedImageTakesPrecedence()
        {
            var vm = new SanPhamCrudViewModel
            {
                TenSanPham = "Sản phẩm",
                GiaBan = 10000m,
                LinkHinhAnh = "/old/image.jpg"
            };

            var entity = vm.ToEntity("/uploads/new_uploaded.jpg");
            Assert.Equal("/uploads/new_uploaded.jpg", entity.LinkHinhAnh, "Uploaded path must take precedence over LinkHinhAnh");
        }

        [E2ETest(5, "C15: SanPhamCrudViewModel UpdateEntity preserves old image when no new file")]
        public void C15_SanPhamCrudViewModel_UpdateEntity_PreservesExistingImageWhenNoUpload()
        {
            var entity = new SanPhamGayQuy
            {
                IDSanPham = 9,
                TenSanPham = "Tên cũ",
                GiaBan = 100000m,
                LinkHinhAnh = "/assets/original.jpg",
                SoLuongTon = 20
            };

            var vm = new SanPhamCrudViewModel
            {
                IDSanPham = 9,
                TenSanPham = "Tên mới cập nhật",
                GiaBan = 120000m,
                LinkHinhAnh = null, // No new image specified
                SoLuongTon = 25
            };

            vm.UpdateEntity(entity, uploadedImagePath: null);

            Assert.Equal("Tên mới cập nhật", entity.TenSanPham, "Name must update");
            Assert.Equal(120000m, entity.GiaBan, "Price must update");
            Assert.Equal("/assets/original.jpg", entity.LinkHinhAnh, "Existing image must be preserved");
            Assert.NotNull(entity.NgayCapNhat, "NgayCapNhat must be set upon update");
        }

        [E2ETest(5, "C16: SanPhamCrudViewModel UpdateEntity replaces image when uploaded")]
        public void C16_SanPhamCrudViewModel_UpdateEntity_ReplacesImageWhenUploaded()
        {
            var entity = new SanPhamGayQuy
            {
                IDSanPham = 10,
                LinkHinhAnh = "/assets/old.jpg"
            };

            var vm = new SanPhamCrudViewModel
            {
                TenSanPham = "Cập nhật",
                GiaBan = 50000m
            };

            vm.UpdateEntity(entity, uploadedImagePath: "/uploads/brand_new.png");
            Assert.Equal("/uploads/brand_new.png", entity.LinkHinhAnh, "Uploaded image must replace existing image");
        }

        [E2ETest(5, "C17: SanPhamCrudViewModel FromEntity bidirectional fidelity")]
        public void C17_SanPhamCrudViewModel_FromEntity_FullBidirectionalIntegrity()
        {
            var original = new SanPhamGayQuy
            {
                IDSanPham = 42,
                TenSanPham = "Hộp Bút Gỗ Khắc Chữ",
                GiaBan = 45000m,
                GiaGoc = 60000m,
                LinkHinhAnh = "/assets/box.jpg",
                MoTaNgan = "Hộp bút gỗ thông",
                MoTaChiTiet = "Chi tiết hộp bút gỗ",
                ThongDiepGayQuy = "Vì nụ cười trẻ thơ",
                PhanTramDongGop = 80,
                SoLuongTon = 15,
                DanhMuc = "Dụng cụ học tập",
                TrangThai = true,
                ThuTu = 5,
                IDChienDich = 2
            };

            var vm = SanPhamCrudViewModel.FromEntity(original);

            Assert.Equal(original.IDSanPham, vm.IDSanPham, "ID must match");
            Assert.Equal(original.TenSanPham, vm.TenSanPham, "TenSanPham must match");
            Assert.Equal(original.GiaBan, vm.GiaBan, "GiaBan must match");
            Assert.Equal(original.GiaGoc, vm.GiaGoc, "GiaGoc must match");
            Assert.Equal(original.LinkHinhAnh, vm.LinkHinhAnh, "LinkHinhAnh must match");
            Assert.Equal(original.PhanTramDongGop, vm.PhanTramDongGop, "PhanTramDongGop must match");
            Assert.Equal(original.SoLuongTon, vm.SoLuongTon, "SoLuongTon must match");
            Assert.Equal(original.DanhMuc, vm.DanhMuc, "DanhMuc must match");
        }

        // =========================================================================
        // 3. DonHangGayQuy & CheckoutViewModel Validation
        // =========================================================================

        [E2ETest(5, "C18: DonHangGayQuy standard valid model passes validation")]
        public void C18_DonHangGayQuy_ValidModel_PassesValidation()
        {
            var order = new DonHangGayQuy
            {
                MaDonHang = "GQ2026-0001",
                HoTenNguoiMua = "Nguyễn Văn An",
                SoDienThoai = "0901234567",
                Email = "nguyenvanan@gmail.com",
                DiaChiNhanHang = "475A Điện Biên Phủ, P.25, Bình Thạnh, TP.HCM",
                GhiChu = "Giao giờ hành chính",
                TongTienHang = 258000m,
                TienUngHoThem = 50000m,
                TongThanhToan = 308000m,
                PhuongThucThanhToan = "ChuyenKhoanQR",
                TrangThaiDonHang = "MoiDat",
                TrangThaiThanhToan = "ChuaThanhToan"
            };

            Assert.True(ValidateModel(order, out var results), $"Valid order must pass, got: {string.Join(", ", results.Select(r => r.ErrorMessage))}");
        }

        [E2ETest(5, "C19: DonHangGayQuy missing customer fields fails validation")]
        public void C19_DonHangGayQuy_MissingCustomerInfo_FailsValidation()
        {
            var emptyOrder = new DonHangGayQuy
            {
                MaDonHang = "GQ2026-0002",
                HoTenNguoiMua = "",
                SoDienThoai = "",
                DiaChiNhanHang = ""
            };

            Assert.False(ValidateModel(emptyOrder, out var results), "Empty customer info must fail validation");
            Assert.True(results.Any(r => r.MemberNames.Contains("HoTenNguoiMua")), "HoTenNguoiMua required");
            Assert.True(results.Any(r => r.MemberNames.Contains("SoDienThoai")), "SoDienThoai required");
            Assert.True(results.Any(r => r.MemberNames.Contains("DiaChiNhanHang")), "DiaChiNhanHang required");
        }

        [E2ETest(5, "C20: DonHangGayQuy invalid phone format rejected by [Phone] attribute")]
        public void C20_DonHangGayQuy_InvalidPhoneFormats_Rejected()
        {
            var order = new DonHangGayQuy
            {
                MaDonHang = "GQ2026-0003",
                HoTenNguoiMua = "Trần Thị B",
                SoDienThoai = "abc-not-a-number",
                DiaChiNhanHang = "Đà Lạt, Lâm Đồng"
            };

            Assert.False(ValidateModel(order, out var results), "Non-numeric phone string must fail validation");
            Assert.True(results.Any(r => r.MemberNames.Contains("SoDienThoai")), "Error must target SoDienThoai");
        }

        [E2ETest(5, "C21: DonHangGayQuy email format validation")]
        public void C21_DonHangGayQuy_InvalidEmail_Rejected()
        {
            var orderInvalidEmail = new DonHangGayQuy
            {
                MaDonHang = "GQ2026-0004",
                HoTenNguoiMua = "Lê Văn C",
                SoDienThoai = "0987654321",
                Email = "plainstring-without-at",
                DiaChiNhanHang = "Lạc Dương, Lâm Đồng"
            };
            Assert.False(ValidateModel(orderInvalidEmail, out var r1), "Email without @ must fail");
            Assert.True(r1.Any(r => r.MemberNames.Contains("Email")), "Error must target Email");

            var orderNullEmail = new DonHangGayQuy
            {
                MaDonHang = "GQ2026-0005",
                HoTenNguoiMua = "Lê Văn C",
                SoDienThoai = "0987654321",
                Email = null, // Email is optional
                DiaChiNhanHang = "Lạc Dương, Lâm Đồng"
            };
            Assert.True(ValidateModel(orderNullEmail, out _), "Null email must pass as email is optional");
        }

        [E2ETest(5, "C22: DonHangGayQuy calculated properties reflect child items")]
        public void C22_DonHangGayQuy_CalculatedProperties_ReflectChildItems()
        {
            var order = new DonHangGayQuy
            {
                ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                {
                    new ChiTietDonHangGayQuy { SoLuong = 2, DonGia = 100000m, ThanhTien = 200000m },
                    new ChiTietDonHangGayQuy { SoLuong = 3, DonGia = 50000m, ThanhTien = 150000m }
                },
                PhuongThucThanhToan = "COD",
                TrangThaiDonHang = "MoiDat"
            };

            Assert.Equal(5, order.TongSoLuongSanPham, "TongSoLuongSanPham must sum quantities (2 + 3 = 5)");
            Assert.Contains("COD", order.TenPhuongThucHienThi, "Display payment text must mention COD");
            Assert.Equal("Mới đặt hàng", order.TenTrangThaiHienThi, "TrangThai Mới đặt hàng mapped properly");
        }

        [E2ETest(5, "C23: CheckoutViewModel valid model passes validation")]
        public void C23_CheckoutViewModel_ValidModel_PassesValidation()
        {
            var cvm = new CheckoutViewModel
            {
                HoTen = "Đặng Thùy Dương",
                SoDienThoai = "0912345678",
                Email = "duong@hutech.edu.vn",
                DiaChi = "Khu Công Nghệ Cao, TP. Thủ Đức",
                TienUngHoThem = 100000m,
                PhuongThucThanhToan = "ChuyenKhoanQR"
            };

            Assert.True(ValidateModel(cvm, out _), "Valid CheckoutViewModel must pass");
        }

        [E2ETest(5, "C24: CheckoutViewModel negative donation fails Range validation")]
        public void C24_CheckoutViewModel_NegativeDonation_FailsValidation()
        {
            var cvm = new CheckoutViewModel
            {
                HoTen = "Người Dùng",
                SoDienThoai = "0900000000",
                DiaChi = "TP.HCM",
                TienUngHoThem = -50000m
            };

            Assert.False(ValidateModel(cvm, out var results), "Negative TienUngHoThem must fail Range validation");
            Assert.True(results.Any(r => r.MemberNames.Contains("TienUngHoThem")), "Error must target TienUngHoThem");
        }

        [E2ETest(5, "C25: CheckoutViewModel financial totals calculation")]
        public void C25_CheckoutViewModel_FinancialTotals_AccurateCalculations()
        {
            var cvm = new CheckoutViewModel
            {
                TienUngHoThem = 50000m,
                PhiVanChuyen = 20000m,
                Items = new List<CartItem>
                {
                    new CartItem { DonGia = 100000m, SoLuong = 2, PhanTramDongGop = 100 }, // 200k, 200k donate
                    new CartItem { DonGia = 50000m, SoLuong = 1, PhanTramDongGop = 50 }    // 50k, 25k donate
                }
            };

            Assert.Equal(3, cvm.TongSoLuong, "TongSoLuong must be 3");
            Assert.Equal(250000m, cvm.TongTienHang, "TongTienHang must be 250,000 VNĐ");
            Assert.Equal(320000m, cvm.TongThanhToan, "TongThanhToan must be 250k + 20k + 50k = 320,000 VNĐ");
            Assert.Equal(275000m, cvm.TongTienDongGop, "TongTienDongGop must be 200k + 25k + 50k extra = 275,000 VNĐ");
        }

        // =========================================================================
        // 4. ChiTietDonHangGayQuy & Cart Model Integrity
        // =========================================================================

        [E2ETest(5, "C26: ChiTietDonHangGayQuy zero or negative quantity rejected")]
        public void C26_ChiTietDonHangGayQuy_ZeroOrNegativeQuantity_FailsValidation()
        {
            var detailZero = new ChiTietDonHangGayQuy
            {
                IDDonHang = 1,
                TenSanPham = "Sản phẩm",
                DonGia = 50000m,
                SoLuong = 0, // Invalid: must be >= 1
                ThanhTien = 0m
            };
            Assert.False(ValidateModel(detailZero, out var r1), "Quantity 0 must fail Range(1, 100000)");
            Assert.True(r1.Any(r => r.MemberNames.Contains("SoLuong")), "Error must target SoLuong");

            var detailNeg = new ChiTietDonHangGayQuy
            {
                IDDonHang = 1,
                TenSanPham = "Sản phẩm",
                DonGia = 50000m,
                SoLuong = -3,
                ThanhTien = -150000m
            };
            Assert.False(ValidateModel(detailNeg, out var r2), "Negative quantity must fail");
        }

        [E2ETest(5, "C27: ChiTietDonHangGayQuy charity percentage boundaries [0, 100]")]
        public void C27_ChiTietDonHangGayQuy_CharityPercentage_Boundaries()
        {
            var detailNeg = new ChiTietDonHangGayQuy { IDDonHang = 1, TenSanPham = "A", DonGia = 1000m, SoLuong = 1, ThanhTien = 1000m, PhanTramDongGop = -1 };
            Assert.False(ValidateModel(detailNeg, out _), "Negative percentage must fail");

            var detail0 = new ChiTietDonHangGayQuy { IDDonHang = 1, TenSanPham = "A", DonGia = 1000m, SoLuong = 1, ThanhTien = 1000m, PhanTramDongGop = 0 };
            Assert.True(ValidateModel(detail0, out _), "0% in order detail is permitted");

            var detail100 = new ChiTietDonHangGayQuy { IDDonHang = 1, TenSanPham = "A", DonGia = 1000m, SoLuong = 1, ThanhTien = 1000m, PhanTramDongGop = 100 };
            Assert.True(ValidateModel(detail100, out _), "100% in order detail is permitted");

            var detail101 = new ChiTietDonHangGayQuy { IDDonHang = 1, TenSanPham = "A", DonGia = 1000m, SoLuong = 1, ThanhTien = 1000m, PhanTramDongGop = 101 };
            Assert.False(ValidateModel(detail101, out _), ">100% must fail");
        }

        [E2ETest(5, "C28: ChiTietDonHangGayQuy TienDongGop calculation")]
        public void C28_ChiTietDonHangGayQuy_TienDongGop_Calculation()
        {
            var detail = new ChiTietDonHangGayQuy
            {
                DonGia = 120000m,
                SoLuong = 3,
                ThanhTien = 360000m,
                PhanTramDongGop = 50
            };
            Assert.Equal(180000m, detail.TienDongGop, "TienDongGop for 50% of 360,000 must be 180,000");
        }

        [E2ETest(5, "C29: CartItem calculations and defaults")]
        public void C29_CartItem_Calculation_AndDefaults()
        {
            var item = new CartItem
            {
                IDSanPham = 1,
                TenSanPham = "Áo Thun",
                DonGia = 129000m,
                SoLuong = 2,
                PhanTramDongGop = 100
            };

            Assert.Equal(258000m, item.ThanhTien, "ThanhTien must be DonGia * SoLuong");
            Assert.Equal(258000m, item.TienDongGop, "TienDongGop must be 258,000");
        }

        [E2ETest(5, "C30: CartViewModel empty cart totals return 0 and CoSanPham is false")]
        public void C30_CartViewModel_EmptyCart_ReturnsZeroTotals()
        {
            var cart = new CartViewModel();
            Assert.Equal(0, cart.TongSoLuong, "Empty cart quantity must be 0");
            Assert.Equal(0m, cart.TongTienHang, "Empty cart item total must be 0");
            Assert.Equal(0m, cart.TongTienDongGop, "Empty cart charity total must be 0");
            Assert.Equal(0m, cart.TongThanhToan, "Empty cart payment total must be 0");
            Assert.False(cart.CoSanPham, "CoSanPham must be false for empty cart");
        }

        [E2ETest(5, "C31: CartViewModel null items list safety check")]
        public void C31_CartViewModel_NullSafety_DoesNotThrowOnNullItems()
        {
            var cart = new CartViewModel { Items = null! };
            Assert.Equal(0, cart.TongSoLuong, "Null items list must return 0 quantity");
            Assert.Equal(0m, cart.TongTienHang, "Null items list must return 0 total");
            Assert.False(cart.CoSanPham, "Null items list must return false for CoSanPham");
        }

        // =========================================================================
        // 5. EF Core DbContext Metadata Validation (Without Database Connection)
        // =========================================================================

        [E2ETest(5, "C32: DbContext builds entity model cleanly without database")]
        public void C32_DbContext_BuildModel_SucceedsWithoutDatabaseConnection()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WEB_TinhNguyen_MockTest;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            using var context = new ApplicationDbContext(options);
            var model = context.Model; // Force EF Core model builder compilation

            Assert.NotNull(model, "EF Core compiled IModel must not be null");
        }

        [E2ETest(5, "C33: DbContext properly registers 3 fundraising entity types")]
        public void C33_DbContext_DbSets_ProperlyRegisteredInModel()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=dummy;Database=dummy;")
                .Options;

            using var context = new ApplicationDbContext(options);
            var model = context.Model;

            var spType = model.FindEntityType(typeof(SanPhamGayQuy));
            var dhType = model.FindEntityType(typeof(DonHangGayQuy));
            var ctType = model.FindEntityType(typeof(ChiTietDonHangGayQuy));

            Assert.NotNull(spType, "SanPhamGayQuy must be registered in EF Core model");
            Assert.NotNull(dhType, "DonHangGayQuy must be registered in EF Core model");
            Assert.NotNull(ctType, "ChiTietDonHangGayQuy must be registered in EF Core model");
        }

        [E2ETest(5, "C34: DbContext maps tables to expected table names")]
        public void C34_DbContext_TableNames_ConfiguredCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=dummy;Database=dummy;")
                .Options;

            using var context = new ApplicationDbContext(options);
            var model = context.Model;

            var spType = model.FindEntityType(typeof(SanPhamGayQuy));
            var dhType = model.FindEntityType(typeof(DonHangGayQuy));
            var ctType = model.FindEntityType(typeof(ChiTietDonHangGayQuy));

            Assert.Equal("SanPhamGayQuy", spType?.GetTableName(), "SanPhamGayQuy table name must match");
            Assert.Equal("DonHangGayQuy", dhType?.GetTableName(), "DonHangGayQuy table name must match");
            Assert.Equal("ChiTietDonHangGayQuy", ctType?.GetTableName(), "ChiTietDonHangGayQuy table name must match");
        }

        [E2ETest(5, "C35: DbContext configures decimal(18,2) precision on all currency columns")]
        public void C35_DbContext_DecimalPrecisions_ConfiguredAsDecimal18_2()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=dummy;Database=dummy;")
                .Options;

            using var context = new ApplicationDbContext(options);
            var model = context.Model;

            var spType = model.FindEntityType(typeof(SanPhamGayQuy))!;
            Assert.Equal("decimal(18,2)", spType.FindProperty("GiaBan")?.GetColumnType(), "GiaBan column type must be decimal(18,2)");
            Assert.Equal("decimal(18,2)", spType.FindProperty("GiaGoc")?.GetColumnType(), "GiaGoc column type must be decimal(18,2)");

            var dhType = model.FindEntityType(typeof(DonHangGayQuy))!;
            Assert.Equal("decimal(18,2)", dhType.FindProperty("TongTienHang")?.GetColumnType(), "TongTienHang must be decimal(18,2)");
            Assert.Equal("decimal(18,2)", dhType.FindProperty("TienUngHoThem")?.GetColumnType(), "TienUngHoThem must be decimal(18,2)");
            Assert.Equal("decimal(18,2)", dhType.FindProperty("PhiVanChuyen")?.GetColumnType(), "PhiVanChuyen must be decimal(18,2)");
            Assert.Equal("decimal(18,2)", dhType.FindProperty("TongThanhToan")?.GetColumnType(), "TongThanhToan must be decimal(18,2)");

            var ctType = model.FindEntityType(typeof(ChiTietDonHangGayQuy))!;
            Assert.Equal("decimal(18,2)", ctType.FindProperty("DonGia")?.GetColumnType(), "DonGia must be decimal(18,2)");
            Assert.Equal("decimal(18,2)", ctType.FindProperty("ThanhTien")?.GetColumnType(), "ThanhTien must be decimal(18,2)");
        }

        [E2ETest(5, "C36: DbContext configures Cascade delete on Order details and Restrict on Product")]
        public void C36_DbContext_ForeignKeys_ConfiguredWithCorrectDeleteBehaviors()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=dummy;Database=dummy;")
                .Options;

            using var context = new ApplicationDbContext(options);
            var ctType = context.Model.FindEntityType(typeof(ChiTietDonHangGayQuy))!;

            // Foreign key to DonHangGayQuy
            var orderFk = ctType.GetForeignKeys()
                .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(DonHangGayQuy));
            Assert.NotNull(orderFk, "Foreign key ChiTietDonHang -> DonHang must exist");
            Assert.Equal(DeleteBehavior.Cascade, orderFk!.DeleteBehavior, "Delete order must cascade to ChiTietDonHangs");

            // Foreign key to SanPhamGayQuy
            var prodFk = ctType.GetForeignKeys()
                .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(SanPhamGayQuy));
            Assert.NotNull(prodFk, "Foreign key ChiTietDonHang -> SanPham must exist");
            Assert.Equal(DeleteBehavior.Restrict, prodFk!.DeleteBehavior, "Delete product must be Restricted if in orders");
        }

        // =========================================================================
        // 6. SessionExtensions Robustness & Fault Tolerance
        // =========================================================================

        [E2ETest(5, "C37: SessionExtensions Set with null value removes the key")]
        public void C37_SessionExtensions_SetNullValue_RemovesKey()
        {
            var session = E2ETestContext.CreateMockSession();
            session.Set("ActiveKey", new CartItem { TenSanPham = "Test" });
            Assert.NotNull(session.Get<CartItem>("ActiveKey"), "Key should exist");

            session.Set<CartItem>("ActiveKey", null!);
            Assert.Null(session.Get<CartItem>("ActiveKey"), "Setting null must remove key from session");
        }

        [E2ETest(5, "C38: SessionExtensions Get on corrupted JSON returns default safely")]
        public void C38_SessionExtensions_GetCorruptedJson_ReturnsDefaultSafely()
        {
            var session = E2ETestContext.CreateMockSession();
            // Manually inject malformed JSON into the session
            session.Set("CorruptedCart", System.Text.Encoding.UTF8.GetBytes("{ this is bad json: [unterminated"));

            var result = session.Get<CartViewModel>("CorruptedCart");
            Assert.Null(result, "Corrupted JSON must return default/null without unhandled crash");
        }

        [E2ETest(5, "C39: SessionExtensions complex cart object roundtrip")]
        public void C39_SessionExtensions_ComplexCartRoundtrip_PreservesDataIntegrity()
        {
            var session = E2ETestContext.CreateMockSession();
            var originalCart = new List<CartItem>
            {
                new CartItem { IDSanPham = 1, TenSanPham = "Áo Thun \"Trăng Sáng\"", DonGia = 129000m, SoLuong = 2, PhanTramDongGop = 100 },
                new CartItem { IDSanPham = 2, TenSanPham = "Túi Tote Canvas", DonGia = 79000m, SoLuong = 1, PhanTramDongGop = 100 }
            };

            session.Set("GioHangGayQuy", originalCart);
            var loaded = session.Get<List<CartItem>>("GioHangGayQuy");

            Assert.NotNull(loaded, "Deserialized cart must not be null");
            Assert.Equal(2, loaded!.Count, "Cart item count must match");
            Assert.Equal(originalCart[0].TenSanPham, loaded[0].TenSanPham, "Product name with quotes/diacritics preserved");
            Assert.Equal(originalCart[0].ThanhTien, loaded[0].ThanhTien, "Calculated ThanhTien preserved");
            Assert.Equal(originalCart[1].DonGia, loaded[1].DonGia, "Price preserved");
        }

        [E2ETest(5, "C40: SessionExtensions throws ArgumentNullException on null session")]
        public void C40_SessionExtensions_NullArguments_ThrowsArgumentNullException()
        {
            Microsoft.AspNetCore.Http.ISession nullSession = null!;
            Assert.Throws<ArgumentNullException>(() => nullSession.Set("key", "val"), "Null session must throw ArgumentNullException");
        }

        [E2ETest(5, "C41: SessionExtensions GetOrCreate invokes factory only once")]
        public void C41_SessionExtensions_GetOrCreate_InvokesFactoryOnlyOnce()
        {
            var session = E2ETestContext.CreateMockSession();
            int factoryInvocations = 0;

            var firstCall = session.GetOrCreate("CounterKey", () =>
            {
                factoryInvocations++;
                return new List<string> { "item1" };
            });

            var secondCall = session.GetOrCreate("CounterKey", () =>
            {
                factoryInvocations++;
                return new List<string> { "item2" };
            });

            Assert.Equal(1, factoryInvocations, "Factory must only be invoked once when key does not exist");
            Assert.Equal("item1", firstCall[0], "First call value must match");
            Assert.Equal("item1", secondCall[0], "Second call must return cached session value");
        }

        // =========================================================================
        // 7. DbInitializer In-Memory Concurrency & Offline Fallback
        // =========================================================================

        [E2ETest(5, "C42: DbInitializer seed dataset contains authentic products")]
        public void C42_DbInitializer_DefaultProducts_CountAndIntegrity()
        {
            DbInitializer.ResetInMemoryStore();
            var products = DbInitializer.GetInMemoryProducts();

            Assert.True(products.Count >= 4, "DbInitializer must provide at least 4 default products");
            foreach (var p in products)
            {
                Assert.True(!string.IsNullOrWhiteSpace(p.TenSanPham), "Product must have non-empty name");
                Assert.True(p.GiaBan > 0, "Product must have positive GiaBan");
                Assert.True(p.SoLuongTon > 0, "Default product must have available stock");
                Assert.True(!string.IsNullOrWhiteSpace(p.ThongDiepGayQuy), "Default product must have charity message");
            }
        }

        [E2ETest(5, "C43: DbInitializer in-memory returns cloned copies to prevent accidental mutation")]
        public void C43_DbInitializer_InMemoryStore_CloneSafety_MutationDoesNotLeak()
        {
            DbInitializer.ResetInMemoryStore();
            var initialList = DbInitializer.GetInMemoryProducts();
            var first = initialList.First();
            var originalPrice = first.GiaBan;

            // Mutate the returned copy
            first.GiaBan = 999999999m;

            // Fetch again
            var fresh = DbInitializer.GetInMemoryProductById(first.IDSanPham);
            Assert.NotNull(fresh, "Product must be found");
            Assert.Equal(originalPrice, fresh!.GiaBan, "Internal store must not be mutated by external modification of returned objects");
        }

        [E2ETest(5, "C44: DbInitializer AddInMemoryOrder decreases stock and increases DaBan")]
        public void C44_DbInitializer_OrderPlacement_UpdatesStockAndDaBanSafely()
        {
            DbInitializer.ResetInMemoryStore();
            var p1Before = DbInitializer.GetInMemoryProductById(1)!;
            var stockBefore = p1Before.SoLuongTon;
            var soldBefore = p1Before.DaBan;

            var order = new DonHangGayQuy
            {
                HoTenNguoiMua = "Người Mua Test",
                SoDienThoai = "0901234567",
                DiaChiNhanHang = "TP.HCM",
                ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                {
                    new ChiTietDonHangGayQuy { IDSanPham = 1, SoLuong = 5, DonGia = p1Before.GiaBan, ThanhTien = 5 * p1Before.GiaBan }
                }
            };

            DbInitializer.AddInMemoryOrder(order);

            var p1After = DbInitializer.GetInMemoryProductById(1)!;
            Assert.Equal(stockBefore - 5, p1After.SoLuongTon, "Stock must decrease by ordered quantity (5)");
            Assert.Equal(soldBefore + 5, p1After.DaBan, "DaBan must increase by ordered quantity (5)");
        }

        [E2ETest(5, "C45: DbInitializer order exceeding stock clamps stock to zero without negative underflow")]
        public void C45_DbInitializer_OverOrdering_StockClampedToZeroWithoutUnderflow()
        {
            DbInitializer.ResetInMemoryStore();
            var p = DbInitializer.GetInMemoryProductById(1)!;

            var massiveOrder = new DonHangGayQuy
            {
                HoTenNguoiMua = "Người Mua Sỉ Từ Thiện",
                SoDienThoai = "0901234567",
                DiaChiNhanHang = "Lâm Đồng",
                ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                {
                    new ChiTietDonHangGayQuy { IDSanPham = 1, SoLuong = p.SoLuongTon + 500, DonGia = p.GiaBan, ThanhTien = 1000000m }
                }
            };

            DbInitializer.AddInMemoryOrder(massiveOrder);

            var pAfter = DbInitializer.GetInMemoryProductById(1)!;
            Assert.Equal(0, pAfter.SoLuongTon, "Stock must be clamped to 0 and not become negative");
        }

        [E2ETest(5, "C46: DbInitializer gracefully handles non-existent IDs")]
        public void C46_DbInitializer_NonExistentOperations_HandledGracefully()
        {
            DbInitializer.ResetInMemoryStore();
            var missingProd = DbInitializer.GetInMemoryProductById(99999);
            Assert.Null(missingProd, "Missing product ID must return null");

            var missingOrder = DbInitializer.GetInMemoryOrderById(99999);
            Assert.Null(missingOrder, "Missing order ID must return null");

            var deleteResult = DbInitializer.DeleteInMemoryProduct(99999);
            Assert.False(deleteResult, "Deleting non-existent product ID must return false");

            var updateResult = DbInitializer.UpdateInMemoryProduct(new SanPhamGayQuy { IDSanPham = 99999, TenSanPham = "None" });
            Assert.False(updateResult, "Updating non-existent product ID must return false");
        }

        [E2ETest(5, "C47: DbInitializer thread-safe concurrent reads and writes")]
        public void C47_DbInitializer_ConcurrentMultiThreadedAccess_ThreadSafe()
        {
            DbInitializer.ResetInMemoryStore();

            // Run 50 parallel operations mixing reads, product updates, and orders
            Parallel.For(0, 50, i =>
            {
                if (i % 3 == 0)
                {
                    var products = DbInitializer.GetInMemoryProducts();
                    Assert.NotNull(products, "Concurrent read must not return null");
                }
                else if (i % 3 == 1)
                {
                    DbInitializer.AddInMemoryProduct(new SanPhamGayQuy
                    {
                        TenSanPham = $"Sản phẩm song song #{i}",
                        GiaBan = 50000m,
                        SoLuongTon = 10
                    });
                }
                else
                {
                    DbInitializer.AddInMemoryOrder(new DonHangGayQuy
                    {
                        HoTenNguoiMua = $"Khách hàng #{i}",
                        SoDienThoai = "0900000000",
                        DiaChiNhanHang = "Đồng Nai",
                        ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                        {
                            new ChiTietDonHangGayQuy { IDSanPham = 1, SoLuong = 1, DonGia = 129000m, ThanhTien = 129000m }
                        }
                    });
                }
            });

            // Ensure store state is healthy after concurrent bombardments
            var finalProducts = DbInitializer.GetInMemoryProducts();
            Assert.True(finalProducts.Count >= 4, "Store must remain healthy and intact after concurrent operations");
        }
    }
}
