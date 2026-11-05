using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WEB_TinhNguyen.DuLieu;
using WEB_TinhNguyen.MoHinh;

namespace WEB_TinhNguyen.BoDieuKhien
{
    public class AdminBanToChucController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminBanToChucController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 1. DANH SÁCH CHIẾN DỊCH THEO NĂM
        public async Task<IActionResult> Index()
        {
            var list = await _context.ChienDichs.OrderByDescending(c => c.Nam).ToListAsync();
            return View(list);
        }

        // 2. MỞ TRANG CẤU HÌNH (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var chienDich = await _context.ChienDichs.FindAsync(id);
            if (chienDich == null) return NotFound();

            var config = await _context.CauHinhBanToChucs
                .Include(c => c.ThanhViens)
                .FirstOrDefaultAsync(c => c.IDChienDich == id);

            // Khởi tạo dữ liệu mặc định nếu chưa có
            if (config == null)
            {
                config = new CauHinhBanToChuc
                {
                    IDChienDich = id,
                    SoDo_DoanVanMoTa = "Ban Tổ chức dự án Trăng Sáng Langbiang là lực lượng nòng cốt trong việc xây dựng kế hoạch, kết nối nguồn lực xã hội và trực tiếp điều phối các hoạt động tình nguyện vì trẻ em vùng cao. Với mô hình phối hợp chặt chẽ giữa các cơ sở Đoàn - Hội, Ban Thường trực điều hành dự án gồm:"
                };
                _context.CauHinhBanToChucs.Add(config);
                await _context.SaveChangesAsync();

                var seedMembers = new List<ThanhVienBanToChuc>
                {
                    // --- 1. BAN SÁNG LẬP ---
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Founder_Top", HoTen = "anh Phạm Đức Tuyên", ChucVuChinh = "Sáng lập Dự án | Trưởng BTC 2026", MoTaChiTiet = "Đồng sáng lập dự án (2025). Năm nay, với vai trò Trưởng Ban tổ chức, anh tiếp tục trực tiếp dẫn dắt hành trình mang yêu thương về Langbiang.", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/kho/1786685993357_2275546561531407524_2275546561531407524_7dcc02bb81ecb1a3bc82634d70c65844-rWUgvuZZj86o3KgZeg1sJhmxLUPRGj_lFIqTIeXP.webp", LinkFacebook = "https://www.facebook.com/ductuyen.ahihi", CoHuyHieuSao = true, ThuTu = 1 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Founder", HoTen = "anh Lê Minh Vũ", ChucVuChinh = "Sáng lập Dự án | Cố vấn", MoTaChiTiet = "Người đặt nền móng cho Trăng Sáng Langbiang (2025). Hiện nay với vai trò Cố vấn, anh là cầu nối để hỗ trợ, chia sẻ kinh nghiệm cùng Ban tổ chức đương nhiệm.", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/kho/LMV-ajzaNqEqOP6pY0YzgrPOJbLShhNTDK-CGPf5DKswEKLxu3RzzCTj8nYgqFjzA_3RZFAA3JW.webp", LinkFacebook = "https://www.facebook.com/vulea5a7", CoHuyHieuSao = true, ThuTu = 2 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Founder", HoTen = "anh Nguyễn Anh Hào", ChucVuChinh = "Sáng lập Dự án | Đồng Trưởng BTC 2026", MoTaChiTiet = "Người đồng sáng lập dự án từ mùa đầu tiên. Hiện tại, anh tiếp tục gắn bó trong hàng ngũ Ban tổ chức, trực tiếp điều hành các hoạt động năm 2026.", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/kho/nguyen-anh-hao-OGMawNNpPRMgvsvXYfW4wVhOVCW1Kb-gR943OA0zG7ny4FuVSQmUZGnFWGgIo_XX5Tv-Xxi.webp", LinkFacebook = "https://www.facebook.com/kphai.anhhao", CoHuyHieuSao = false, ThuTu = 3 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Founder", HoTen = "anh Phạm Minh Phát", ChucVuChinh = "Sáng lập Dự án | Cố vấn", MoTaChiTiet = "Gắn bó với Trăng Sáng Langbiang từ những ngày đầu tiên. Hiện tại, anh chọn lùi về với vai trò \"Đại sứ hành trình\", sẵn sàng kề vai sát cánh cùng Ban tổ chức đương nhiệm.", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/kho/pham-minh-phat-6f73ECyCN2JNZd5vMjSpa2OILTrxsq_4O1UmilsO.webp", LinkFacebook = "https://www.facebook.com/MinkFats", CoHuyHieuSao = false, ThuTu = 4 },

                    // --- 2. SƠ ĐỒ CÂY BAN THƯỜNG TRỰC (GIỐNG ẢNH MẪU) ---
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Tree_Root", HoTen = "ANH PHẠM ĐỨC TUYÊN", ChucVuPhu = "Đồng sáng lập Dự án Trăng Sáng Langbiang", ChucVuChinh = "TRƯỞNG BAN TỔ CHỨC", DonViCongTac = "Bí thư Đoàn phường Langbiang - Đà Lạt", DuongDanAnh = "https://ik.imagekit.io/ij9ocagrc/kho/P-T-3kPL9JfHVAbrdhNA2U2GjJJavkzS2B_UpctQioSy.webp", LinkFacebook = "https://www.facebook.com/ductuyen.ahihi", ThuTu = 1 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Tree_Branch", HoTen = "ANH NGUYỄN ANH HÀO", ChucVuPhu = "Đồng sáng lập Dự án|Bí thư Đoàn khoa KHXH & QHCC HUTECH", ChucVuChinh = "ĐỒNG TRƯỞNG BAN", DonViCongTac = "Ban Tổ chức Chiến dịch 2026", DuongDanAnh = "https://ik.imagekit.io/ij9ocagrc/kho/NAH-UGP6XGYvGmw1u1VSwO4BaA5oo2z76A_222g3Q9yY.webp", LinkFacebook = "https://www.facebook.com/kphai.anhhao", ThuTu = 2 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Tree_Branch", HoTen = "CHỊ NGUYỄN THỊ QUỲNH DAO", ChucVuPhu = "Bí thư Đoàn khoa Luật HUTECH|Phụ trách đối ngoại & Nhân sự", ChucVuChinh = "ĐỒNG TRƯỞNG BAN", DonViCongTac = "Ban Tổ chức Chiến dịch 2026", DuongDanAnh = "https://ik.imagekit.io/ij9ocagrc/kho/IMG_0206-lLGU0vkY6wX8rJdduRVJOnRv5qzqy8-99wYARcxThvFshSa30JI6HTuyBoZr3_7JSHbLb1u.webp", LinkFacebook = "https://www.facebook.com/ntqd2809", ThuTu = 3 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Tree_Branch", HoTen = "ANH LÊ MINH VŨ", ChucVuPhu = "Đồng sáng lập Dự án (2025)|Hỗ trợ chuyên môn & Kết nối", ChucVuChinh = "BAN CỐ VẤN", DonViCongTac = "Dự án Trăng Sáng Langbiang", DuongDanAnh = "https://ik.imagekit.io/ij9ocagrc/kho/LMV-ajzaNqEqOP6pY0YzgrPOJbLShhNTDK-CGPf5DKswEKLxu3RzzCTj8nYgqFjzA_3RZFAA3JW.webp", LinkFacebook = "https://www.facebook.com/vulea5a7", ThuTu = 4 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Tree_Branch", HoTen = "ANH PHẠM MINH PHÁT", ChucVuPhu = "Đồng sáng lập Dự án (2025)|Phụ trách tiếp nhận quyên góp hiện vật", ChucVuChinh = "ĐẠI SỨ HÀNH TRÌNH", DonViCongTac = "Dự án Trăng Sáng Langbiang", DuongDanAnh = "https://ik.imagekit.io/ij9ocagrc/kho/pham-minh-phat-6f73ECyCN2JNZd5vMjSpa2OILTrxsq_4O1UmilsO.webp", LinkFacebook = "https://www.facebook.com/MinkFats", ThuTu = 5 },

                    // --- 3. THÀNH VIÊN BAN TỔ CHỨC (12 NGƯỜI) ---
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Member", HoTen = "Anh VÕ NGUYỄN DU", ChucVuChinh = "Phó Ban tổ chức 2026", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/tnv/IMG_8479-w4lk58kE5LvDomWQOkFqDk4X6SXMXR__pVMcf5qG.webp", LinkFacebook = "https://www.facebook.com/duuvoo.vnd", ThuTu = 1 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Member", HoTen = "Chị NGUYỄN HỒNG PHƯƠNG VY", ChucVuChinh = "Phó Ban tổ chức 2026", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/kho/IMG_1020-lgv0RjYZuB0PFdUEBexYyOj55zkt9c-MCr0QFm4HYDhuUigR6drf8wgweGaKf_8QRAUlM_q.webp", LinkFacebook = "https://www.facebook.com/hongphvy", ThuTu = 2 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Member", HoTen = "Chị NGUYỄN HUỲNH KHÁNH TƯỜNG", ChucVuChinh = "Phó Ban tổ chức 2026", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/kho/IMG_0956-VgxBFv30dAXK5cbw7LZDELQ9RqPg0x-f412VYm4SZI7gngPSPzhOQV9kLWP4O_rlCHD_h-4.webp", LinkFacebook = "https://web.facebook.com/share/1PTCN42VvS/", ThuTu = 3 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Member", HoTen = "Chị NGUYỄN BÙI DIỆU NGUYÊN", ChucVuChinh = "Thành viên Ban Tổ chức 2026", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/tnv/IMG_4520-WUgTXnKoGpBRQe7DbGJl1Zfg7NWQa2_e6osOYoZH.jpg", LinkFacebook = "https://www.facebook.com/share/1BuepMk4Gp/", ThuTu = 4 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Member", HoTen = "Anh BÙI TẤN NHỰT", ChucVuChinh = "Thành viên Ban Tổ chức 2026", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/tnv/IMG_1720-zKU8jokJj5193eIfPa1uO5dmS3d7DD_PMYt_LgZ_.webp", LinkFacebook = "https://www.facebook.com/share/1EjFLhrfuj/", ThuTu = 5 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Member", HoTen = "Anh NGUYỄN LÊ NGUYÊN QUÂN", ChucVuChinh = "Thành viên Ban Tổ chức 2026", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/tnv/IMG_3067-vSdzjpvINa6LxNOFeEGVYrxGHvJBt8_vC7O3hook.webp", LinkFacebook = "https://www.facebook.com/share/1EYFE3EEda/", ThuTu = 6 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Member", HoTen = "Chị TRỊNH VÂN TRANG", ChucVuChinh = "Thành viên Ban Tổ chức 2026", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/kho/IMG_6308-KlnXPrfo0FgIXqmZ76eeIc5kDj90fF-IdXaxooglFW38z10B2bLKUBtAB6jSB_efpfz2GUe.webp", LinkFacebook = "https://www.facebook.com/vtnhim1402/", ThuTu = 7 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Member", HoTen = "Anh ĐÀO THIÊN PHÚ", ChucVuChinh = "Thành viên Ban Tổ chức 2026", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/tnv/IMG_1744-6CNNEW7fdRxS9gEjzZrZjeuFJFgn8a_W3rEu6eob.webp", LinkFacebook = "https://www.facebook.com/share/1BjmhWfDvN/", ThuTu = 8 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Member", HoTen = "Chị LÊ THỊ TRINH", ChucVuChinh = "Thành viên Ban Tổ chức 2026", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/kho/1000004065-kNU83DoTUTfBps0Ev73N9WeUtRAC4o-uTMf5i5IswIqnJ6do0wb0MBtGYml98_6t5Q-lqs9.webp", LinkFacebook = "https://www.facebook.com/share/1GiZ4CyhY1/", ThuTu = 9 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Member", HoTen = "Chị TRẦN THỊ TUYẾT NGỌC", ChucVuChinh = "Thành viên Ban Tổ chức 2026", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/kho/53.-Tra--n-Thi--Tuye--t-Ngo-c---24802700069---SV5T-Khoa-r6Qz1qCm7yqxp0JJE9NDNFjsbVBz0A-Btrcv7zxdJ2iz65lMEvdfQ8GAQD3VY_LwYgKTL-0.webp", LinkFacebook = "https://web.facebook.com/share/195VMcDdSh/", ThuTu = 10 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Member", HoTen = "Chị TRƯƠNG THỊ YẾN NHI", ChucVuChinh = "Thành viên Ban Tổ chức 2026", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/kho/IMG_3275-b7ztbCesNJfiF1IMhba3At7EScZr7r-egqDwHnnnVvTLCjizhAjqU9wEnmMgb_ek58lNGWI.webp", LinkFacebook = "https://www.facebook.com/innhy07", ThuTu = 11 },
                    new ThanhVienBanToChuc { IDCauHinhBTC = config.IDCauHinhBTC, NhomHienThi = "Member", HoTen = "Anh PHẠM TUẤN HƯNG", ChucVuChinh = "Thành viên Ban Tổ chức 2026", LinkAnh = "https://ik.imagekit.io/ij9ocagrc/tnv/IMG_5381-KsGU1FIuDzr4XbIXa64zNEa0qn6lzN_DkiGJrr7P.webp", LinkFacebook = "https://www.facebook.com/share/1DMvRKE2ei/", ThuTu = 12 }
                };

                _context.ThanhVienBanToChucs.AddRange(seedMembers);
                await _context.SaveChangesAsync();

                config = await _context.CauHinhBanToChucs
                    .Include(c => c.ThanhViens)
                    .FirstOrDefaultAsync(c => c.IDChienDich == id);
            }

            var allMembers = config.ThanhViens?.OrderBy(t => t.ThuTu).ToList() ?? new List<ThanhVienBanToChuc>();

            var vm = new CauHinhBanToChucVM
            {
                IDCauHinhBTC = config.IDCauHinhBTC,
                IDChienDich = config.IDChienDich,
                TenChienDich = chienDich.TieuDe,
                NamChienDich = chienDich.Nam,

                FontTieuDe = config.KieuChuTieuDe,
                FontNoiDung = config.KieuChuNoiDung,

                HienThiBanSangLap = config.HienThiBanSangLap,
                SangLap_Nhan = config.SangLap_Nhan,
                SangLap_TieuDe = config.SangLap_TieuDe,
                SangLap_CoChuTieuDe = config.SangLap_CoChuTieuDe,

                HienThiSoDoToChuc = config.HienThiSoDoToChuc,
                SoDo_Nhan = config.SoDo_Nhan,
                SoDo_TieuDe = config.SoDo_TieuDe,
                SoDo_DoanVanMoTa = config.SoDo_DoanVanMoTa,

                HienThiDanhSachBTC = config.HienThiDanhSachBTC,
                BTC_Nhan = config.BTC_Nhan,
                BTC_TieuDe = config.BTC_TieuDe,
                SoLuongHienThiBanDau = config.SoLuongHienThiBanDau,

                CTA_MoTa = config.KeuGoi_MoTa,
                CTA_TextNut = config.KeuGoi_ChuNut,
                CTA_LinkNut = config.KeuGoi_DuongDanNut,

                BanSangLaps = allMembers.Where(m => m.NhomHienThi == "Founder_Top" || m.NhomHienThi == "Founder").Select(MapToVM).ToList(),
                SoDoThuongTrucs = allMembers.Where(m => m.NhomHienThi == "Tree_Root" || m.NhomHienThi == "Tree_Branch").Select(MapToVM).ToList(),
                ThanhVienBTCs = allMembers.Where(m => m.NhomHienThi == "Member").Select(MapToVM).ToList()
            };

            return View(vm);
        }

        private ThanhVienBTC_VM MapToVM(ThanhVienBanToChuc m) => new ThanhVienBTC_VM
        {
            NhomHienThi = m.NhomHienThi,
            HoTen = m.HoTen,
            ChucVuChinh = m.ChucVuChinh,
            DonViCongTac = m.DonViCongTac,
            ChucVuPhu = m.ChucVuPhu,
            MoTaChiTiet = m.MoTaChiTiet,
            LinkAnhCu = m.DuongDanAnh,
            LinkFacebook = m.LinkFacebook,
            CoHuyHieuSao = m.CoHuyHieuSao
        };

        // 3. LƯU CẤU HÌNH (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CauHinhBanToChucVM vm)
        {
            var config = await _context.CauHinhBanToChucs
                .Include(c => c.ThanhViens)
                .FirstOrDefaultAsync(c => c.IDCauHinhBTC == vm.IDCauHinhBTC);

            if (config == null) return NotFound();

            config.KieuChuTieuDe = vm.FontTieuDe;
            config.KieuChuNoiDung = vm.FontNoiDung;

            config.HienThiBanSangLap = vm.HienThiBanSangLap;
            config.SangLap_Nhan = vm.SangLap_Nhan;
            config.SangLap_TieuDe = vm.SangLap_TieuDe;
            config.SangLap_CoChuTieuDe = vm.SangLap_CoChuTieuDe;

            config.HienThiSoDoToChuc = vm.HienThiSoDoToChuc;
            config.SoDo_Nhan = vm.SoDo_Nhan;
            config.SoDo_TieuDe = vm.SoDo_TieuDe;
            config.SoDo_DoanVanMoTa = vm.SoDo_DoanVanMoTa;

            config.HienThiDanhSachBTC = vm.HienThiDanhSachBTC;
            config.BTC_Nhan = vm.BTC_Nhan;
            config.BTC_TieuDe = config.BTC_TieuDe;
            config.SoLuongHienThiBanDau = vm.SoLuongHienThiBanDau;

            config.KeuGoi_MoTa = vm.CTA_MoTa;
            config.KeuGoi_ChuNut = vm.CTA_TextNut;
            config.KeuGoi_DuongDanNut = vm.CTA_LinkNut;

            // Xóa danh sách cũ và nạp lại từ 3 danh sách trên Form
            _context.ThanhVienBanToChucs.RemoveRange(config.ThanhViens);

            await SaveGroupMembersAsync(config.IDCauHinhBTC, vm.BanSangLaps, "Founder");
            await SaveGroupMembersAsync(config.IDCauHinhBTC, vm.SoDoThuongTrucs, "Tree_Branch");
            await SaveGroupMembersAsync(config.IDCauHinhBTC, vm.ThanhVienBTCs, "Member");

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã lưu cấu hình Ban Tổ Chức cho chiến dịch {vm.TenChienDich} thành công!";
            return RedirectToAction(nameof(Index));
        }

        private async Task SaveGroupMembersAsync(int idCauHinh, List<ThanhVienBTC_VM> list, string defaultGroup)
        {
            if (list == null) return;
            int order = 1;
            foreach (var item in list)
            {
                if (string.IsNullOrWhiteSpace(item.HoTen)) continue;

                string imgLink = item.LinkAnhCu;
                if (item.FileAnh != null)
                    imgLink = await SaveFileAsync(item.FileAnh, "bantochuc");

                _context.ThanhVienBanToChucs.Add(new ThanhVienBanToChuc
                {
                    IDCauHinhBTC = idCauHinh,
                    NhomHienThi = string.IsNullOrWhiteSpace(item.NhomHienThi) ? defaultGroup : item.NhomHienThi,
                    HoTen = item.HoTen,
                    ChucVuChinh = item.ChucVuChinh,
                    DonViCongTac = item.DonViCongTac,
                    ChucVuPhu = item.ChucVuPhu,
                    MoTaChiTiet = item.MoTaChiTiet,
                    DuongDanAnh = imgLink,
                    LinkFacebook = item.LinkFacebook,
                    CoHuyHieuSao = item.CoHuyHieuSao,
                    ThuTu = order++
                });
            }
        }

        private async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", subFolder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string ext = Path.GetExtension(file.FileName);
            string uniqueName = $"{Guid.NewGuid():N}{ext}";
            string filePath = Path.Combine(uploadsFolder, uniqueName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"/uploads/{subFolder}/{uniqueName}";
        }
    }
}