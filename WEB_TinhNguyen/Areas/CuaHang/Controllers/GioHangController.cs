using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_TinhNguyen.DuLieu;
using WEB_TinhNguyen.Areas.CuaHang.Models;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;

namespace WEB_TinhNguyen.Areas.CuaHang.Controllers
{
    /// <summary>
    /// Phân hệ Giỏ hàng & Thanh toán Quyên góp Gây quỹ Tình nguyện
    /// Quản lý giỏ hàng lưu Session, xử lý thêm/sửa/xóa sản phẩm, biểu mẫu đặt hàng,
    /// tự động tạo đơn hàng và hiển thị mã VietQR chuyển khoản.
    /// </summary>
    [Area("CuaHang")]
    public class GioHangController : Controller
    {
        private const string CartSessionKey = "GioHangGayQuy";
        private readonly ApplicationDbContext _context;

        public GioHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Helper Methods

        private List<CartItem> GetCart()
        {
            return HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.Set(CartSessionKey, cart);
        }

        private async Task<SanPhamGayQuy?> FindProductAsync(int id)
        {
            try
            {
                if (_context.Database.CanConnect())
                {
                    return await _context.SanPhamGayQuys.FirstOrDefaultAsync(p => p.IDSanPham == id && p.TrangThai);
                }
            }
            catch
            {
                // Fallback khi database offline
            }

            var inMemory = DbInitializer.GetInMemoryProductById(id);
            return (inMemory != null && inMemory.TrangThai) ? inMemory : null;
        }

        private bool IsAjaxRequest()
        {
            var requestedWith = Request.Headers["X-Requested-With"].ToString();
            var accept = Request.Headers["Accept"].ToString();
            return string.Equals(requestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
                   || accept.Contains("application/json", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        // =========================================================================
        // 1. XEM GIỎ HÀNG & BIỂU MẪU ĐẶT HÀNG (INDEX)
        // =========================================================================
        [HttpGet]
        public IActionResult Index()
        {
            var cart = GetCart();
            var cartVm = new CartViewModel
            {
                Items = cart
            };

            var checkoutVm = new CheckoutViewModel
            {
                Items = cart,
                PhuongThucThanhToan = "ChuyenKhoanQR"
            };

            ViewBag.CheckoutModel = checkoutVm;
            return View(cartVm);
        }

        // =========================================================================
        // 2. THÊM SẢN PHẨM VÀO GIỎ HÀNG (THEM VAO GIO)
        // =========================================================================
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> ThemVaoGio(int id, int soLuong = 1)
        {
            if (soLuong <= 0) soLuong = 1;

            var product = await FindProductAsync(id);
            if (product == null)
            {
                if (IsAjaxRequest())
                {
                    return Json(new { success = false, message = "Sản phẩm gây quỹ không tồn tại hoặc đã ngừng kinh doanh." });
                }
                TempData["ErrorMessage"] = "Sản phẩm gây quỹ không tồn tại hoặc đã ngừng cung cấp.";
                return RedirectToAction("Index", "CuaHang");
            }

            if (product.SoLuongTon <= 0)
            {
                if (IsAjaxRequest())
                {
                    return Json(new { success = false, message = "Sản phẩm này hiện đã hết quà tặng/hiện vật trong kho gây quỹ." });
                }
                TempData["ErrorMessage"] = "Sản phẩm này hiện đã hết hàng.";
                return RedirectToAction("ChiTiet", "CuaHang", new { id });
            }

            var cart = GetCart();
            var existing = cart.FirstOrDefault(i => i.IDSanPham == id);

            if (existing != null)
            {
                existing.SoLuong += soLuong;
                if (existing.SoLuong > product.SoLuongTon)
                {
                    existing.SoLuong = product.SoLuongTon;
                }
            }
            else
            {
                cart.Add(new CartItem
                {
                    IDSanPham = product.IDSanPham,
                    TenSanPham = product.TenSanPham,
                    LinkHinhAnh = product.LinkHinhAnh,
                    DonGia = product.GiaBan,
                    SoLuong = Math.Min(soLuong, product.SoLuongTon),
                    ThongDiepGayQuy = product.ThongDiepGayQuy,
                    PhanTramDongGop = product.PhanTramDongGop,
                    SoLuongTon = product.SoLuongTon
                });
            }

            SaveCart(cart);
            TempData["SuccessMessage"] = $"Đã thêm \"{product.TenSanPham}\" vào giỏ quà gây quỹ thành công!";

            if (IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    message = $"Đã thêm {product.TenSanPham} vào giỏ hàng!",
                    totalQuantity = cart.Sum(i => i.SoLuong),
                    cartSubtotal = cart.Sum(i => i.ThanhTien)
                });
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 3. CẬP NHẬT SỐ LƯỢNG SẢN PHẨM (CAP NHAT SO LUONG)
        // =========================================================================
        [HttpPost]
        public IActionResult CapNhatSoLuong(int id, int soLuong)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.IDSanPham == id);

            if (item != null)
            {
                if (soLuong <= 0)
                {
                    cart.Remove(item);
                    TempData["SuccessMessage"] = $"Đã xóa sản phẩm \"{item.TenSanPham}\" khỏi giỏ hàng.";
                }
                else if (item.SoLuongTon <= 0)
                {
                    item.SoLuong = 0;
                    cart.Remove(item);
                    TempData["ErrorMessage"] = $"Sản phẩm \"{item.TenSanPham}\" hiện đã hết hàng.";
                }
                else
                {
                    item.SoLuong = Math.Min(soLuong, item.SoLuongTon);
                    TempData["SuccessMessage"] = $"Đã cập nhật số lượng cho \"{item.TenSanPham}\".";
                }
                SaveCart(cart);
            }

            if (IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    lineTotal = item?.ThanhTien ?? 0,
                    totalQuantity = cart.Sum(i => i.SoLuong),
                    cartSubtotal = cart.Sum(i => i.ThanhTien),
                    totalDonation = cart.Sum(i => i.TienDongGop)
                });
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 4. XÓA MỘT SẢN PHẨM KHỎI GIỎ (XOA KHOI GIO)
        // =========================================================================
        [AcceptVerbs("GET", "POST")]
        public IActionResult XoaKhoiGio(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.IDSanPham == id);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
                TempData["SuccessMessage"] = $"Đã xóa sản phẩm \"{item.TenSanPham}\" khỏi giỏ hàng.";
            }

            if (IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    totalQuantity = cart.Sum(i => i.SoLuong),
                    cartSubtotal = cart.Sum(i => i.ThanhTien)
                });
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 5. XÓA TOÀN BỘ GIỎ HÀNG (XOA GIO HANG)
        // =========================================================================
        [HttpPost]
        public IActionResult XoaGioHang()
        {
            HttpContext.Session.RemoveKey(CartSessionKey);
            TempData["SuccessMessage"] = "Đã làm trống giỏ hàng gây quỹ.";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 6. XỬ LÝ ĐẶT HÀNG & ỦNG HỘ GÂY QUỸ (DAT HANG)
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatHang(CheckoutViewModel model)
        {
            var cart = GetCart();
            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng chọn sản phẩm gây quỹ trước khi thanh toán.";
                return RedirectToAction(nameof(Index));
            }

            // Xác thực bổ sung số điện thoại di động Việt Nam chuẩn
            if (!string.IsNullOrWhiteSpace(model.SoDienThoai))
            {
                var cleanPhone = model.SoDienThoai.Trim().Replace(" ", "").Replace(".", "").Replace("-", "");
                if (!Regex.IsMatch(cleanPhone, @"^(03|05|07|08|09)\d{8}$"))
                {
                    ModelState.AddModelError(nameof(model.SoDienThoai), "Số điện thoại không đúng định dạng di động 10 số của Việt Nam (đầu 03, 05, 07, 08, 09).");
                }
            }

            if (!ModelState.IsValid)
            {
                model.Items = cart;
                var cartVm = new CartViewModel
                {
                    Items = cart,
                    TienUngHoThem = model.TienUngHoThem
                };
                ViewBag.CheckoutModel = model;
                return View("Index", cartVm);
            }

            // Kiểm tra tồn kho trước khi tạo đơn hàng (Pre-checkout stock validation)
            foreach (var item in cart)
            {
                int availableStock = item.SoLuongTon;
                string prodName = item.TenSanPham;

                try
                {
                    if (_context.Database.CanConnect())
                    {
                        var dbProd = await _context.SanPhamGayQuys.FindAsync(item.IDSanPham);
                        if (dbProd != null)
                        {
                            availableStock = dbProd.SoLuongTon;
                            prodName = dbProd.TenSanPham;
                        }
                    }
                    else
                    {
                        var memProd = DbInitializer.GetInMemoryProductById(item.IDSanPham);
                        if (memProd != null)
                        {
                            availableStock = memProd.SoLuongTon;
                            prodName = memProd.TenSanPham;
                        }
                    }
                }
                catch
                {
                    var memProd = DbInitializer.GetInMemoryProductById(item.IDSanPham);
                    if (memProd != null)
                    {
                        availableStock = memProd.SoLuongTon;
                        prodName = memProd.TenSanPham;
                    }
                }

                if (item.SoLuong > availableStock)
                {
                    var errorMsg = $"Sản phẩm {prodName} chỉ còn {availableStock} sản phẩm trong kho.";
                    TempData["ErrorMessage"] = errorMsg;
                    if (IsAjaxRequest())
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    model.Items = cart;
                    var cartVm = new CartViewModel
                    {
                        Items = cart,
                        TienUngHoThem = model.TienUngHoThem
                    };
                    ViewBag.CheckoutModel = model;
                    return View("Index", cartVm);
                }
            }

            // Sinh mã đơn hàng chuẩn 15 ký tự dạng GQ2026MMDD-XXXX
            var dateStamp = DateTime.Now.ToString("yyyyMMdd");
            var orderCode = $"GQ{dateStamp}-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();

            decimal tongTienHang = cart.Sum(i => i.ThanhTien);
            decimal tienUngHoThem = model.TienUngHoThem > 0 ? model.TienUngHoThem : 0m;
            decimal phiVanChuyen = 0m;
            decimal tongThanhToan = tongTienHang + tienUngHoThem + phiVanChuyen;

            var donHang = new DonHangGayQuy
            {
                MaDonHang = orderCode,
                HoTenNguoiMua = model.HoTen.Trim(),
                SoDienThoai = model.SoDienThoai.Trim().Replace(" ", "").Replace(".", "").Replace("-", ""),
                Email = model.Email?.Trim(),
                DiaChiNhanHang = model.DiaChi.Trim(),
                GhiChu = model.GhiChu?.Trim(),
                TongTienHang = tongTienHang,
                TienUngHoThem = tienUngHoThem,
                PhiVanChuyen = phiVanChuyen,
                TongThanhToan = tongThanhToan,
                PhuongThucThanhToan = string.IsNullOrWhiteSpace(model.PhuongThucThanhToan) ? "ChuyenKhoanQR" : model.PhuongThucThanhToan,
                TrangThaiDonHang = "MoiDat",
                TrangThaiThanhToan = "ChuaThanhToan",
                NgayDatHang = DateTime.Now
            };

            foreach (var item in cart)
            {
                donHang.ChiTietDonHangs.Add(new ChiTietDonHangGayQuy
                {
                    IDSanPham = item.IDSanPham > 0 ? item.IDSanPham : null,
                    TenSanPham = item.TenSanPham,
                    LinkHinhAnh = item.LinkHinhAnh,
                    DonGia = item.DonGia,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.ThanhTien,
                    ThongDiepGayQuy = item.ThongDiepGayQuy,
                    PhanTramDongGop = item.PhanTramDongGop
                });
            }

            int orderId = 0;
            try
            {
                if (_context.Database.CanConnect())
                {
                    _context.DonHangGayQuys.Add(donHang);

                    // Trừ tồn kho và tăng số lượng đã bán của sản phẩm trong cơ sở dữ liệu
                    foreach (var detail in donHang.ChiTietDonHangs)
                    {
                        if (detail.IDSanPham.HasValue)
                        {
                            var prod = await _context.SanPhamGayQuys.FindAsync(detail.IDSanPham.Value);
                            if (prod != null)
                            {
                                prod.SoLuongTon = Math.Max(0, prod.SoLuongTon - detail.SoLuong);
                                prod.DaBan += detail.SoLuong;
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    orderId = donHang.IDDonHang;
                }
                else
                {
                    var saved = DbInitializer.AddInMemoryOrder(donHang);
                    orderId = saved.IDDonHang;
                }
            }
            catch
            {
                var saved = DbInitializer.AddInMemoryOrder(donHang);
                orderId = saved.IDDonHang;
            }

            // Xóa sạch giỏ hàng trong session
            HttpContext.Session.RemoveKey(CartSessionKey);

            return RedirectToAction(nameof(DatHangThanhCong), new { id = orderId });
        }

        // =========================================================================
        // 7. XÁC NHẬN ĐƠN HÀNG THÀNH CÔNG & VIETQR (DAT HANG THANH CONG)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> DatHangThanhCong(int id)
        {
            DonHangGayQuy? order = null;

            try
            {
                if (_context.Database.CanConnect())
                {
                    order = await _context.DonHangGayQuys
                        .Include(d => d.ChiTietDonHangs)
                        .FirstOrDefaultAsync(d => d.IDDonHang == id);
                }
            }
            catch
            {
                // Fallback khi database offline
            }

            if (order == null)
            {
                order = DbInitializer.GetInMemoryOrderById(id);
            }

            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng.";
                return RedirectToAction("Index", "CuaHang");
            }

            return View(order);
        }
    }
}
