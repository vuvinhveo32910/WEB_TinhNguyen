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
    public class AdminTinTucController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminTinTucController(ApplicationDbContext context, IWebHostEnvironment env)
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

            var config = await _context.CauHinhTinTucs
                .Include(c => c.BaiViets)
                .FirstOrDefaultAsync(c => c.IDChienDich == id);

            // Nếu chưa có cấu hình thì tự động khởi tạo 14 bài viết mẫu từ HTML gốc
            if (config == null)
            {
                config = new CauHinhTinTuc
                {
                    IDChienDich = id,
                    Hero_Nhan = "Bản tin",
                    Hero_TieuDe = "Câu chuyện từ hành trình",
                    Hero_MoTa = $"Những dòng nhật ký, hình ảnh và tin tức mới nhất về Trăng Sáng Langbiang. Đăng ký bản tin để không bỏ lỡ mùa {chienDich.Nam}!"
                };
                _context.CauHinhTinTucs.Add(config);
                await _context.SaveChangesAsync();

                var seedPosts = new List<BaiVietTinTuc>
                {
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 1, LuotThich = 52, NgayDang = "2026-08-22",
                        ThePhanLoai = "THÔNG BÁO 2026",
                        TieuDe = "TUYỂN TÌNH NGUYỆN VIÊN “TRĂNG SÁNG LANGBIANG” 2026",
                        LinkAnhDaiDien = "https://ik.imagekit.io/ij9ocagrc/kho/Artboard-1_2TB-TUYE--N-TNV-LANGBIANG_7Rm9Q6mFA.webp",
                        TrichDan = "Mùa Trung thu này, bạn đã sẵn sàng làm nên điều ý nghĩa? Hãy trở thành Tình nguyện viên của \"Trăng Sáng Langbiang\" 2026 để cùng chúng mình thắp lên nụ cười và ghép nên một mùa trăng trọn vẹn nhất!",
                        NoiDungChiTiet = @"<p>Mùa Trung thu này, bạn đã sẵn sàng làm nên điều ý nghĩa? Hãy trở thành Tình nguyện viên của ""Trăng Sáng Langbiang"" 2026 để cùng chúng mình thắp lên nụ cười và ghép nên một mùa trăng trọn vẹn nhất!</p><p>Bạn sẽ làm gì trong hành trình này? Đến với dự án, bạn không chỉ là một tình nguyện viên, mà là một ""đại sứ"" thực thụ:</p><p>✨ ”Đại sứ truyền thông” - Phủ sóng mạng xã hội bằng những hình ảnh, câu chuyện ý nghĩa để Dự án chạm đến nhiều trái tim hơn.</p><p>💚 Chung tay vận động GÂY QUỸ qua các “Gói yêu thương”, biến sự sẻ chia thành những phần quà thiết thực.</p><p>🤝 Hỗ trợ kết nối các nhà hảo tâm và những tấm lòng vàng cùng đồng hành tạo nên mùa Trung thu ấm áp.</p><p>⏰ Thời gian hoạt động: từ 25/08 - 25/09/2026 (Lịch trình chi tiết sẽ được linh hoạt sắp xếp theo thời gian rảnh của TNV).</p><p>📍 Địa điểm: tại TP. Hồ Chí Minh.</p><p><strong>📍 Quyền lợi khi trở thành Tình nguyện viên:</strong></p><p>👉 Trải nghiệm một hành trình thiện nguyện thực tế và đầy tự hào.<br>👉 Có cơ hội kết nối, trải nghiệm và đóng góp cùng những người trẻ có chung tinh thần xung kích, tình nguyện vì cộng đồng.<br>👉 Được cấp Giấy chứng nhận tình nguyện chính thức từ Đoàn Phường Langbiang - Đà Lạt.<br>👉 Có cơ hội nhận những món quà kỷ niệm đặc biệt, mang đậm dấu ấn của dự án thay cho lời tri ân sâu sắc từ BTC.</p>"
                    },
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 2, LuotThich = 61, NgayDang = "2026-08-20",
                        ThePhanLoai = "[THƯ NGỎ 2026]",
                        TieuDe = "[THƯ NGỎ] ĐỒNG HÀNH CÙNG \"TRĂNG SÁNG LANGBIANG\" NĂM 2026",
                        LinkAnhDaiDien = "https://ik.imagekit.io/ij9ocagrc/kho/Artboard-1Thu---ngo---a-ng-FB-2026-N8BOkUjPQgf3t20IsIJwqkyit8ghlD-6hKwPVUXfXnMiX7LxezU7Dr89cQ5Mn_SN1vHz523.webp",
                        LinkAnhTrongBai = "https://ik.imagekit.io/ij9ocagrc/tintuc/Artboard-1Thu---ngo---a-ng-FB-2026-FYvriQLqlcUD3xHJg7VkfO5AvBqPIr-0fuOkvbSgTGxdnroNgUlUbvYU5kwJU_Hy7ENyV04.webp",
                        TrichDan = "Để chương trình được trọn vẹn, Ban tổ chức Dự án trân trọng kêu gọi sự đồng hành, đóng góp từ Anh/Chị, quý Doanh nghiệp, quý Mạnh Thường quân. Mỗi sự ủng hộ, dù lớn hay nhỏ đều là ngọn lửa thắp sáng niềm vui đến các em nhỏ có hoàn cảnh khó khăn",
                        NoiDungChiTiet = @"<p>💓 Trung thu là dịp để trẻ em sum vầy trong yêu thương, được vui chơi dưới ánh trăng rằm và nhận những món quà trọn vẹn. Tuy nhiên, tại vùng núi cao - phường Langbiang, tỉnh Lâm Đồng và TP. Hồ Chí Minh vẫn còn nhiều em nhỏ có hoàn cảnh khó khăn đặc biệt là các em đồng bào dân tộc thiểu số chưa một lần được đón một mùa Trung thu trọn vẹn.</p><p>💓 Dự án dự kiến trao tặng 350 phần quà (500.000đ/phần), tổ chức nấu 350 suất ăn (50.000đ/suất) và mang đến cho các em chương trình Hội Trăng rằm, gian hàng trò chơi dân gian đầy màu sắc. Đây không chỉ là hoạt động trao tặng mà còn là hành trình lan tỏa yêu thương, gieo mầm hy vọng và tiếp thêm niềm tin cho các em nhỏ có hoàn cảnh khó khăn trên địa bàn TP.HCM và vùng cao nguyên Langbiang.</p><p>Để chương trình được trọn vẹn, Ban tổ chức Dự án trân trọng kêu gọi sự đồng hành, đóng góp từ Anh/Chị, quý Doanh nghiệp, quý Mạnh Thường quân. Mỗi sự ủng hộ, dù lớn hay nhỏ đều là ngọn lửa thắp sáng niềm vui đến các em nhỏ có hoàn cảnh khó khăn.</p><p><strong>📍 Thông tin tiếp nhận tài trợ hiện kim:</strong><br>👉 Số tài khoản: 0924 044 283 – Ngân hàng TMCP Quân đội (MB Bank)<br>👉 Chủ tài khoản: NGUYEN ANH HAO (Đồng Trưởng BTC dự án năm 2026)<br>👉 Nội dung chuyển khoản: [Họ và tên_đơn vị] + Langbiang2026</p>"
                    },
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 3, LuotThich = 93, NgayDang = "2026-08-14",
                        ThePhanLoai = "Thông báo 2026",
                        TieuDe = "CÔNG BỐ THỜI GIAN VÀ ĐỊA ĐIỂM TỔ CHỨC NĂM 2026",
                        LinkAnhDaiDien = "https://ik.imagekit.io/ij9ocagrc/kho/774432452_122142978381007058_8054767988598408244_n-JLUlQnKAjclH44Xni59ZGOFisoAvau-GxZNilDXjorHaq5YD5kN02JHIW2Xm3_6eFEk95Bc.webp",
                        LinkAnhTrongBai = "https://ik.imagekit.io/ij9ocagrc/tintuc/Artboard-371_1CO-NG-BO---TIME-lanbiang-2026-UzdDHpcCEjkRANUrtSwawg1540CPMK_EZVknTTi6.png",
                        TrichDan = "Trăng sáng Langbiang” năm 2026 tiếp tục hành trình lan tỏa yêu thương đến các em nhỏ có hoàn cảnh khó khăn, đồng bào dân tộc thiểu số nơi vùng cao. Hành trình năm 2026 chính thức được công bố",
                        NoiDungChiTiet = @"<p>🌙 <strong>“TRĂNG SÁNG LANGBIANG” CHÍNH THỨC TRỞ LẠI!</strong></p><p>“Trăng sáng Langbiang” năm 2026 tiếp tục hành trình lan tỏa yêu thương đến các em nhỏ có hoàn cảnh khó khăn, đồng bào dân tộc thiểu số nơi vùng cao. Hành trình năm 2026 chính thức được công bố:</p><p>📍 <strong>CHẶNG 01 | TP. HỒ CHÍ MINH</strong><br>Nơi hành trình được khởi động - nơi những phần quà được chuẩn bị, những tấm lòng được kết nối và sẵn sàng lên đường.<br>📅 <strong>Ngày 22 tháng 09 năm 2026</strong></p><p>📍 <strong>CHẶNG 02 | LANGBIANG – ĐÀ LẠT</strong><br>Nơi những món quà được trao gửi, những hoạt động ý nghĩa được mang đến, để mùa trăng của các em thêm trọn vẹn.<br>📅 <strong>Ngày 26 – 27 tháng 09 năm 2026</strong></p><p>✨ “Trăng sáng Langbiang” năm 2026 không chỉ hướng đến việc trao tặng những phần quà thiện nguyện. Dự án mong muốn tạo nên một không gian vui chơi, sinh hoạt bổ ích; đồng thời lồng ghép những hoạt động giáo dục nhằm nuôi dưỡng tình yêu Quê hương, niềm tự hào Dân tộc.</p>"
                    },
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 4, LuotThich = 50, NgayDang = "2025-09-30",
                        ThePhanLoai = "Hoạt động 2025",
                        TieuDe = "🌟 LỜI TRI ÂN – TRĂNG SÁNG LANGBIANG 🌟",
                        LinkAnhDaiDien = "/gallery/2025/bro02375.jpg",
                        TrichDan = "Để có được một mùa trăng đủ đầy và ấm áp như thế, Ban Tổ chức xin gửi lời cảm ơn chân thành và sâu sắc nhất đến tất cả các đơn vị tài trợ đã đồng hành cùng chương trình. Sự sẻ chia và tấm lòng của Quý đơn vị đã biến những ước mơ nhỏ bé thành hiện thực, thắp sáng niềm tin và tình yêu thương nơi núi rừng LangBiang.",
                        NoiDungChiTiet = @"<p>🌟 <strong>LỜI TRI ÂN – TRĂNG SÁNG LANGBIANG</strong> 🌟</p><p>Dự án tình nguyện “Trăng sáng LangBiang” đã khép lại trong niềm hạnh phúc trọn vẹn. Những nụ cười rạng rỡ, ánh mắt lấp lánh niềm vui của các em nhỏ chính là món quà quý giá nhất mà tập thể chúng tôi nhận được sau hành trình này.</p><p>Để có được một mùa trăng đủ đầy và ấm áp như thế, Ban Tổ chức xin gửi lời cảm ơn chân thành và sâu sắc nhất đến tất cả các đơn vị tài trợ đã đồng hành cùng chương trình. Sự sẻ chia và tấm lòng của Quý đơn vị đã biến những ước mơ nhỏ bé thành hiện thực, thắp sáng niềm tin và tình yêu thương nơi núi rừng LangBiang.</p><p>Trong đó, chúng tôi đặc biệt trân trọng sự đồng hành bền bỉ, đầy nghĩa tình của WESET English Center và Phở Huỳnh Trâm – những người đã truyền thêm sức mạnh để hành trình này thật sự trọn vẹn. Đồng thời, xin ghi nhận và cảm kích sự ủng hộ quý báu từ Sun Taxi Phan Thiết, Bánh kem Kim Phụng và Bệnh viện đa khoa Tâm Phúc.</p>"
                    },
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 5, LuotThich = 30, NgayDang = "2025-09-30",
                        ThePhanLoai = "Tổng kết 2025",
                        TieuDe = "Trọn vẹn mùa trăng đầu tiên tại Langbiang 💚",
                        LinkAnhDaiDien = "/gallery/2025/bro02318.jpg",
                        TrichDan = "Hai ngày một đêm với vô vàn nụ cười. Cảm ơn tất cả những tấm lòng đã cùng chúng mình thắp sáng Trung thu vùng cao!",
                        NoiDungChiTiet = @"<p>💌 <strong>Lời cảm ơn từ Dự án tình nguyện “Trăng sáng Langbiang”</strong></p><p>🌟 Ban Sáng lập và Ban Tổ chức Dự án xin trân trọng gửi lời cảm ơn chân thành và sâu sắc nhất đến Quý Mạnh Thường Quân đã luôn đồng hành, sẻ chia và gửi gắm yêu thương cùng chương trình.</p><p>🌙 Những đóng góp quý báu bằng hiện kim, hiện vật và tinh thần của Quý vị đã góp phần mang đến cho các em thiếu nhi đồng bào dân tộc thiểu số tại phường Langbiang – Đà Lạt một mùa Trung thu ấm áp, đủ đầy và nhiều ý nghĩa.</p>"
                    },
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 6, LuotThich = 16, NgayDang = "2025-09-30",
                        ThePhanLoai = "Hoạt động 2025",
                        TieuDe = "🇻🇳 Trăng sáng LangBiang – nơi tuổi thơ được nuôi dưỡng bằng tình yêu Tổ quốc 🇻🇳",
                        LinkAnhDaiDien = "/gallery/2025/bro01755.jpg",
                        TrichDan = "Các em nhỏ đã cùng nhau tham gia sinh hoạt chủ đề “Yêu Tổ quốc, yêu Đồng bào”. Trong ánh mắt trong veo và giọng nói hồn nhiên, các em được ôn lại ý nghĩa Quốc kỳ, được nhắc nhớ niềm tự hào Dân tộc, tình yêu Đất nước",
                        NoiDungChiTiet = @"<p>Hội Trăng Rằm tại LangBiang không chỉ có tiếng cười rộn ràng của trò chơi dân gian, sắc màu cổ tích của kịch Tấm Cám hay sự tích chú Cuội cung trăng… mà còn có một khoảnh khắc thật sự đặc biệt.</p><p>Các em nhỏ đã cùng nhau tham gia sinh hoạt chủ đề “Yêu Tổ quốc, yêu Đồng bào”. Trong ánh mắt trong veo và giọng nói hồn nhiên, các em được ôn lại ý nghĩa Quốc kỳ, được nhắc nhớ niềm tự hào Dân tộc, tình yêu Đất nước. Khi đồng thanh cất lên “5 Điều Bác Hồ dạy” hay dứt khoát khẳng định ""Hoàng Sa, Trường Sa là của Việt Nam"" – rành rọt, không một chút vấp lời, cả không gian như lắng đọng.</p>"
                    },
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 7, LuotThich = 19, NgayDang = "2025-09-29",
                        ThePhanLoai = "Hoạt động 2025",
                        TieuDe = "Hội Trăng rằm - Tuổi thơ cổ tích tại Langbiang 🏮",
                        LinkAnhDaiDien = "/tintuc/n8.png",
                        TrichDan = "Không gian LangBiang hôm ấy bỗng rộn ràng và đầy ắp tiếng cười. Các em nhỏ được vui chơi hết mình trong những trò chơi dân gian, được sống lại trong thế giới cổ tích qua kịch Tấm Cám, Sự tích chú Cuội cung trăng...",
                        NoiDungChiTiet = @"<p>Không gian LangBiang hôm ấy bỗng rộn ràng và đầy ắp tiếng cười. Các em nhỏ được vui chơi hết mình trong những trò chơi dân gian, được sống lại trong thế giới cổ tích qua kịch Tấm Cám, Sự tích chú Cuội cung trăng, rồi lại say sưa trong những làn điệu văn nghệ mộc mạc, ngọt lành.</p><p>Đặc biệt, khi gần 140 phần quà trung thu được trao tận tay, niềm vui như vỡ òa. Trong ánh mắt trong veo và nụ cười hồn nhiên của các em, chúng tôi cảm nhận được trọn vẹn ý nghĩa của một mùa trăng – không cần đèn lồng rực rỡ, chỉ cần có tình yêu thương và sự sẻ chia, tuổi thơ đã trở nên thật trọn vẹn.</p>"
                    },
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 8, LuotThich = 26, NgayDang = "2025-09-27",
                        ThePhanLoai = "Hoạt động 2025",
                        TieuDe = "Nấu ăn cho em - Hơi ấm lan toả giữa núi rừng Langbiang 🍜",
                        LinkAnhDaiDien = "/gallery/2025/bro01835.jpg",
                        TrichDan = "Trong cái se lạnh và cơn mưa lất phất của núi rừng LangBiang, chương trình “Nấu ăn cho em” đã mang đến một khoảnh khắc thật ấm lòng. 250 suất phở bò nóng hổi đã được gửi trao tận tay các em học sinh và phụ huynh địa phương.",
                        NoiDungChiTiet = @"<p>Trong cái se lạnh và cơn mưa lất phất của núi rừng LangBiang, Dự án tình nguyện “Trăng sáng LangBiang” đã mang đến một khoảnh khắc thật ấm lòng với chương trình “Nấu ăn cho em”.</p><p>250 suất phở bò nóng hổi đã được gửi trao tận tay các em học sinh và phụ huynh địa phương. Với nhiều em nhỏ vốn đã quen với bữa cơm trắng giản dị, bát phở thơm nghi ngút hôm nay không chỉ là một bữa ăn ngon, mà còn là niềm vui, là sự sẻ chia đầy yêu thương.</p><p>🤍 Đặc biệt, xin gửi lời tri ân sâu sắc đến Phở Huỳnh Trâm – đơn vị đã không chỉ mang đến món phở bò thơm ngon, đủ đầy dinh dưỡng mà còn trao gửi trọn vẹn tình thương và trách nhiệm cộng đồng.</p>"
                    },
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 9, LuotThich = 2, NgayDang = "2025-09-22",
                        ThePhanLoai = "Tài trợ 2025",
                        TieuDe = "Trân trọng cảm ơn Trung tâm Anh ngữ WESET English Center – Đơn vị tài trợ dự án \"Trăng Sáng Langbiang\"",
                        LinkAnhDaiDien = "/tintuc/n2.png",
                        TrichDan = "Ban Tổ chức Dự án xin gửi lời cảm ơn chân thành đến Trung tâm Anh ngữ WESET English Center đã đồng hành và hỗ trợ cùng chương trình. Sự đóng góp quý báu từ WESET góp phần tạo nên những giá trị thiết thực cho các em nhỏ tại Langbiang.",
                        NoiDungChiTiet = @"<p>🌟 <strong>TRÂN TRỌNG CẢM ƠN TRUNG TÂM ANH NGỮ WESET ENGLISH CENTER – ĐƠN VỊ TÀI TRỢ DỰ ÁN “TRĂNG SÁNG LANGBIANG”</strong> 🌟</p><p>Ban Tổ chức Dự án “Trăng sáng Langbiang” xin gửi lời cảm ơn chân thành đến Trung tâm Anh ngữ WESET English Center đã đồng hành và hỗ trợ cùng chương trình. Sự đóng góp quý báu từ WESET không chỉ góp phần tạo nên những giá trị thiết thực cho các em nhỏ tại Langbiang, mà còn lan tỏa tinh thần sẻ chia, yêu thương và trách nhiệm cộng đồng.</p>"
                    },
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 10, LuotThich = 10, NgayDang = "2025-09-22",
                        ThePhanLoai = "Tài trợ 2025",
                        TieuDe = "Trân trọng cảm ơn Sun Taxi – Đơn vị tài trợ dự án \"Trăng Sáng Langbiang\"",
                        LinkAnhDaiDien = "/tintuc/n3.png",
                        TrichDan = "Ban Tổ Chức Dự án xin trân trọng gửi lời cảm ơn đến Sun Taxi đã đồng hành và tài trợ cùng chương trình. Sự đóng góp của Sun Taxi không chỉ hỗ trợ thiết thực cho các hoạt động của Dự án, mà còn tiếp thêm động lực để sẻ chia yêu thương.",
                        NoiDungChiTiet = @"<p>🌟 <strong>TRÂN TRỌNG CẢM ƠN SUN TAXI – ĐƠN VỊ TÀI TRỢ DỰ ÁN “TRĂNG SÁNG LANGBIANG”</strong> 🌟</p><p>Ban Tổ Chức Dự án “Trăng sáng Langbiang” xin trân trọng gửi lời cảm ơn đến SUN TAXI đã đồng hành và tài trợ cùng chương trình. Sự đóng góp của SUN TAXI không chỉ hỗ trợ thiết thực cho các hoạt động của Dự án, mà còn tiếp thêm động lực để chúng tôi mang đến nhiều giá trị nhân văn, sẻ chia yêu thương đến các em nhỏ tại Langbiang.</p>"
                    },
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 11, LuotThich = 12, NgayDang = "2025-09-22",
                        ThePhanLoai = "Tài trợ 2025",
                        TieuDe = "Trân trọng cảm ơn Bệnh viện Đa khoa Tâm Phúc – Đơn vị đồng hành dự án \"Trăng Sáng Langbiang\"",
                        LinkAnhDaiDien = "/tintuc/n4.png",
                        TrichDan = "Ban Tổ Chức Dự án xin gửi lời cảm ơn sâu sắc đến Bệnh viện Đa khoa Tâm Phúc đã đồng hành và hỗ trợ cùng chương trình. Sự quan tâm, sẻ chia từ Bệnh viện mang đến nguồn động viên to lớn cho các em nhỏ tại Langbiang.",
                        NoiDungChiTiet = @"<p>🌟 <strong>TRÂN TRỌNG CẢM ƠN BỆNH VIỆN ĐA KHOA TÂM PHÚC – ĐƠN VỊ ĐỒNG HÀNH DỰ ÁN “TRĂNG SÁNG LANGBIANG”</strong> 🌟</p><p>Ban Tổ Chức Dự án “Trăng sáng Langbiang” xin gửi lời cảm ơn sâu sắc đến Bệnh viện Đa khoa Tâm Phúc đã đồng hành và hỗ trợ cùng chương trình. Sự quan tâm, sẻ chia từ Bệnh viện không chỉ mang đến nguồn động viên to lớn cho các em nhỏ tại Langbiang, mà còn khẳng định tinh thần trách nhiệm xã hội, nhân văn cao đẹp.</p>"
                    },
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 12, LuotThich = 9, NgayDang = "2025-09-14",
                        ThePhanLoai = "Thông báo 2025",
                        TieuDe = "Tuyển thành viên dự án \"Trăng Sáng Langbiang\"",
                        LinkAnhDaiDien = "/tintuc/n9.png",
                        TrichDan = "Mỗi mùa trăng rằm tháng Tám về, lòng ta lại rộn vang tiếng trống múa lân, ánh sáng lung linh của những chiếc lồng đèn và niềm háo hức đón Tết Trung thu. Nhưng ở vùng núi cao phường Langbiang (Lâm Đồng), vẫn còn biết bao em nhỏ chưa một lần trọn vẹn niềm vui ấy…",
                        NoiDungChiTiet = @"<p><strong>[THÔNG BÁO] TUYỂN THÀNH VIÊN DỰ ÁN ""TRĂNG SÁNG LANGBIANG""</strong></p><p>Mỗi mùa trăng rằm tháng Tám về, lòng ta lại rộn vang tiếng trống múa lân, ánh sáng lung linh của những chiếc lồng đèn và niềm háo hức đón Tết Trung thu. Nhưng ở vùng núi cao phường Langbiang (Lâm Đồng), vẫn còn biết bao em nhỏ chưa một lần trọn vẹn niềm vui ấy…</p><p>💙 Chính vì thế, Dự án “Trăng sáng Langbiang” được tổ chức từ 26 – 28/9/2025, mang đến 100 phần quà Trung thu, những bữa cơm yêu thương và một đêm hội Trăng rằm ấm áp cho hơn 200 em nhỏ.</p>"
                    },
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 13, LuotThich = 18, NgayDang = "2025-09-11",
                        ThePhanLoai = "Tài trợ 2025",
                        TieuDe = "Trân trọng cảm ơn Phở Huỳnh Trâm - Đơn vị tài trợ dự án \"Trăng Sáng Langbiang\"",
                        LinkAnhDaiDien = "/tintuc/n5.png",
                        TrichDan = "Ban Tổ Chức Dự án xin gửi lời tri ân sâu sắc đến Phở Huỳnh Trâm đã nhận lời đồng hành, tài trợ 250 suất phở trị giá 15.000.000 đồng cho nội dung “Nấu ăn cho em”. Sự chung tay của quý đơn vị góp phần mang đến những bữa ăn ấm áp, trọn vẹn yêu thương.",
                        NoiDungChiTiet = @"<p><strong>TRÂN TRỌNG CẢM ƠN PHỞ HUỲNH TRÂM - ĐƠN VỊ TÀI TRỢ DỰ ÁN ""TRĂNG SÁNG LANGBIANG""</strong></p><p>Ban Tổ Chức Dự án ""Trăng sáng Langbiang"" xin gửi lời tri ân sâu sắc đến Phở Huỳnh Trâm đã nhận lời đồng hành, tài trợ 250 suất phở trị giá 15.000.000 đồng cho nội dung “Nấu ăn cho em”. Sự chung tay của quý đơn vị không chỉ góp phần mang đến những bữa ăn ấm áp, trọn vẹn yêu thương cho các em nhỏ, mà còn lan tỏa tinh thần sẻ chia, nhân ái đến cộng đồng.</p>"
                    },
                    new BaiVietTinTuc {
                        IDCauHinhTT = config.IDCauHinhTT, ThuTu = 14, LuotThich = 5, NgayDang = "2025-09-09",
                        ThePhanLoai = "Thư ngỏ 2025",
                        TieuDe = "Chung tay góp một mùa Trung thu cho em",
                        LinkAnhDaiDien = "/tintuc/n7.png",
                        TrichDan = "Chỉ còn ít ngày nữa là đến hành trình. Mỗi chiếc lồng đèn, mỗi quyển vở đều là món quà ý nghĩa. Cùng đồng hành với Trăng Sáng Langbiang bạn nhé!",
                        NoiDungChiTiet = @"<p><strong>[THƯ NGỎ] TRĂNG SÁNG LANGBIANG – LAN TỎA YÊU THƯƠNG ĐẾN CÁC EM NHỎ VÙNG CAO ✨🌕</strong></p><p>Trung thu là Tết của thiếu nhi, là Tết của đoàn viên, của nụ cười tuổi thơ dưới ánh trăng rằm. Nhưng ở vùng núi cao phường Langbiang, tỉnh Lâm Đồng nhiều em nhỏ dân tộc thiểu số vẫn chưa một lần được đón Trung thu trọn vẹn.</p><p>Hãy cùng chúng tôi viết tiếp câu chuyện yêu thương, để Trung thu này không em nhỏ nào phải thiệt thòi trong niềm vui tuổi thơ.</p>"
                    }
                };

                _context.BaiVietTinTucs.AddRange(seedPosts);
                await _context.SaveChangesAsync();

                config = await _context.CauHinhTinTucs
                    .Include(c => c.BaiViets)
                    .FirstOrDefaultAsync(c => c.IDChienDich == id);
            }

            var vm = new CauHinhTinTucVM
            {
                IDCauHinhTT = config.IDCauHinhTT,
                IDChienDich = config.IDChienDich,
                TenChienDich = chienDich.TieuDe,
                NamChienDich = chienDich.Nam,

                FontTieuDe = config.KieuChuTieuDe,
                FontNoiDung = config.KieuChuNoiDung,

                Hero_Nhan = config.NoiBat_Nhan,
                Hero_TieuDe = config.NoiBat_TieuDe,
                Hero_CoChuTieuDe = config.NoiBat_CoChuTieuDe,
                Hero_MoTa = config.NoiBat_MoTa,

                BaiViets = config.BaiViets?.OrderBy(b => b.ThuTu).Select(b => new BaiVietTinTucVM
                {
                    IDBaiViet = b.IDBaiViet,
                    TieuDe = b.TieuDe,
                    ThePhanLoai = b.ThePhanLoai,
                    TrichDan = b.TrichDan,
                    NoiDungChiTiet = b.NoiDungChiTiet,
                    LinkAnhDaiDienCu = b.DuongDanAnhDaiDien,
                    LinkAnhTrongBaiCu = b.DuongDanAnhTrongBai,
                    LinkBaiVietGoc = b.DuongDanBaiVietGoc,
                    CoChuTieuDe = b.CoChuTieuDe,
                    LuotThich = b.LuotThich,
                    NgayDang = b.NgayDang
                }).ToList() ?? new List<BaiVietTinTucVM>()
            };

            return View(vm);
        }

        // 3. LƯU CẤU HÌNH (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CauHinhTinTucVM vm)
        {
            var config = await _context.CauHinhTinTucs
                .Include(c => c.BaiViets)
                .FirstOrDefaultAsync(c => c.IDCauHinhTT == vm.IDCauHinhTT);

            if (config == null) return NotFound();

            config.KieuChuTieuDe = vm.FontTieuDe;
            config.KieuChuNoiDung = vm.FontNoiDung;

            config.NoiBat_Nhan = vm.Hero_Nhan;
            config.NoiBat_TieuDe = vm.Hero_TieuDe;
            config.NoiBat_CoChuTieuDe = vm.Hero_CoChuTieuDe;
            config.NoiBat_MoTa = vm.Hero_MoTa;

            _context.BaiVietTinTucs.RemoveRange(config.BaiViets);
            if (vm.BaiViets != null)
            {
                int order = 1;
                foreach (var item in vm.BaiViets)
                {
                    if (string.IsNullOrWhiteSpace(item.TieuDe)) continue;

                    string thumbLink = item.LinkAnhDaiDienCu;
                    if (item.FileAnhDaiDien != null)
                        thumbLink = await SaveFileAsync(item.FileAnhDaiDien, "tintuc");

                    string bodyImgLink = item.LinkAnhTrongBaiCu;
                    if (item.FileAnhTrongBai != null)
                        bodyImgLink = await SaveFileAsync(item.FileAnhTrongBai, "tintuc_content");

                    _context.BaiVietTinTucs.Add(new BaiVietTinTuc
                    {
                        IDCauHinhTT = config.IDCauHinhTT,
                        TieuDe = item.TieuDe,
                        ThePhanLoai = item.ThePhanLoai ?? "BẢN TIN",
                        TrichDan = item.TrichDan,
                        NoiDungChiTiet = item.NoiDungChiTiet,
                        DuongDanAnhDaiDien = thumbLink,
                        DuongDanAnhTrongBai = bodyImgLink,
                        DuongDanBaiVietGoc = item.LinkBaiVietGoc,
                        CoChuTieuDe = item.CoChuTieuDe ?? "18px",
                        LuotThich = item.LuotThich,
                        NgayDang = item.NgayDang,
                        ThuTu = order++
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã lưu cấu hình trang Bản Tin cho chiến dịch {vm.TenChienDich} thành công!";
            return RedirectToAction(nameof(Index));
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