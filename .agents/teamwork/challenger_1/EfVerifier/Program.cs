using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WEB_TinhNguyen.DuLieu;
using WEB_TinhNguyen.MoHinh;

namespace WEB_TinhNguyen.Agents.Challenger1
{
    public class Program
    {
        private static int passed = 0;
        private static int failed = 0;

        private static readonly string[] InvalidColumnNames = new[]
        {
            "DuongDanAnhBia",
            "DuongDanAnhLinhVat",
            "HienThiHieuUng3D",
            "HienThiLinhVat",
            "KieuChuNoiDung",
            "KieuChuTieuDe",
            "VeDuAn_DuongDanAnh",
            "ViTriLinhVat"
        };

        public static int Main(string[] args)
        {
            Console.WriteLine("================================================================================");
            Console.WriteLine(" CHALLENGER 1: EF CORE MODEL & SQL GENERATION VERIFIER");
            Console.WriteLine("================================================================================");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:localhost,1433;Database=TinhNguyen;User Id=sa;Password=MockPassword123!;TrustServerCertificate=True;")
                .Options;

            using var context = new ApplicationDbContext(options);

            // 1. Model Entity Type inspections
            Test_01_CauHinhTrangChu_Model_Properties(context);
            Test_02_OtherConfigEntities_Model_Properties(context);

            // 2. SQL Translation Verification via ToQueryString()
            Test_03_SqlTranslation_CauHinhTrangChu(context);
            Test_04_SqlTranslation_OtherEntities(context);

            // 3. Bidirectional Getter/Setter Proxy Tests
            Test_05_Bidirectional_Proxy_CauHinhTrangChu();
            Test_06_Bidirectional_Proxy_OtherEntities();

            // 4. ChangeTracker Proxy Mutation Verification
            Test_07_ChangeTracker_Proxy_Mutation(context);

            Console.WriteLine("================================================================================");
            Console.WriteLine($" RESULTS: {passed} PASSED | {failed} FAILED | TOTAL: {passed + failed}");
            Console.WriteLine("================================================================================");

            return failed == 0 ? 0 : 1;
        }

        private static void Assert(bool condition, string testName, string details = "")
        {
            if (condition)
            {
                passed++;
                Console.WriteLine($" [PASS] {testName}");
            }
            else
            {
                failed++;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" [FAIL] {testName} - {details}");
                Console.ResetColor();
            }
        }

        private static void Test_01_CauHinhTrangChu_Model_Properties(ApplicationDbContext context)
        {
            Console.WriteLine("\n--- TEST SUITE 1: CauHinhTrangChu EF Core Model Metadata ---");
            var entityType = context.Model.FindEntityType(typeof(CauHinhTrangChu));
            Assert(entityType != null, "CauHinhTrangChu exists in EF Core model");
            if (entityType == null) return;

            // Check 8 invalid column names are NOT mapped
            foreach (var invalidName in InvalidColumnNames)
            {
                var prop = entityType.FindProperty(invalidName);
                Assert(prop == null, $"CauHinhTrangChu does NOT map invalid property '{invalidName}'",
                    $"Expected null, but found property mapped to column '{prop?.GetColumnName()}'");
            }

            // Check physical column properties ARE mapped
            var physicalProperties = new[]
            {
                ("LinkAnhBia", typeof(string)),
                ("FontTieuDe", typeof(string)),
                ("FontNoiDung", typeof(string)),
                ("HienThiCanvas3D", typeof(bool)),
                ("VeDuAn_LinkAnh", typeof(string)),
                ("HienThiMascot", typeof(bool)),
                ("LinkAnhMascot", typeof(string)),
                ("ViTriMascot", typeof(string))
            };

            foreach (var (propName, propType) in physicalProperties)
            {
                var prop = entityType.FindProperty(propName);
                Assert(prop != null, $"CauHinhTrangChu maps physical property '{propName}'",
                    $"Property '{propName}' was not found in EF Core entity type");
                if (prop != null)
                {
                    Assert(prop.ClrType == propType, $"Property '{propName}' CLR type is {propType.Name}");
                }
            }
        }

        private static void Test_02_OtherConfigEntities_Model_Properties(ApplicationDbContext context)
        {
            Console.WriteLine("\n--- TEST SUITE 2: Other Configuration Entities EF Core Model Metadata ---");

            // CauHinhKyUc
            var kyUc = context.Model.FindEntityType(typeof(CauHinhKyUc));
            Assert(kyUc != null, "CauHinhKyUc exists in EF Core model");
            if (kyUc != null)
            {
                Assert(kyUc.FindProperty("DuongDanAnhBia") == null, "CauHinhKyUc does not map 'DuongDanAnhBia'");
                Assert(kyUc.FindProperty("KieuChuTieuDe") == null, "CauHinhKyUc does not map 'KieuChuTieuDe'");
                Assert(kyUc.FindProperty("KieuChuTieuDeTongKet") == null, "CauHinhKyUc does not map 'KieuChuTieuDeTongKet'");
                Assert(kyUc.FindProperty("LinkAnhBia") != null, "CauHinhKyUc maps physical 'LinkAnhBia'");
                Assert(kyUc.FindProperty("FontTieuDe") != null, "CauHinhKyUc maps physical 'FontTieuDe'");
                Assert(kyUc.FindProperty("FontTieuDeTongKet") != null, "CauHinhKyUc maps physical 'FontTieuDeTongKet'");
            }

            // CauHinhChuongTrinh
            var chuongTrinh = context.Model.FindEntityType(typeof(CauHinhChuongTrinh));
            Assert(chuongTrinh != null, "CauHinhChuongTrinh exists in EF Core model");
            if (chuongTrinh != null)
            {
                Assert(chuongTrinh.FindProperty("KieuChuTieuDe") == null, "CauHinhChuongTrinh does not map 'KieuChuTieuDe'");
                Assert(chuongTrinh.FindProperty("FontTieuDe") != null, "CauHinhChuongTrinh maps physical 'FontTieuDe'");
            }

            // CauHinhGayQuy
            var gayQuy = context.Model.FindEntityType(typeof(CauHinhGayQuy));
            Assert(gayQuy != null, "CauHinhGayQuy exists in EF Core model");
            if (gayQuy != null)
            {
                Assert(gayQuy.FindProperty("KieuChuTieuDe") == null, "CauHinhGayQuy does not map 'KieuChuTieuDe'");
                Assert(gayQuy.FindProperty("KieuChuNoiDung") == null, "CauHinhGayQuy does not map 'KieuChuNoiDung'");
                Assert(gayQuy.FindProperty("FontTieuDe") != null, "CauHinhGayQuy maps physical 'FontTieuDe'");
                Assert(gayQuy.FindProperty("FontNoiDung") != null, "CauHinhGayQuy maps physical 'FontNoiDung'");
            }

            // CauHinhBanToChuc
            var btc = context.Model.FindEntityType(typeof(CauHinhBanToChuc));
            Assert(btc != null, "CauHinhBanToChuc exists in EF Core model");
            if (btc != null)
            {
                Assert(btc.FindProperty("KieuChuTieuDe") == null, "CauHinhBanToChuc does not map 'KieuChuTieuDe'");
                Assert(btc.FindProperty("KieuChuNoiDung") == null, "CauHinhBanToChuc does not map 'KieuChuNoiDung'");
                Assert(btc.FindProperty("FontTieuDe") != null, "CauHinhBanToChuc maps physical 'FontTieuDe'");
                Assert(btc.FindProperty("FontNoiDung") != null, "CauHinhBanToChuc maps physical 'FontNoiDung'");
            }

            // CauHinhTinTuc
            var tinTuc = context.Model.FindEntityType(typeof(CauHinhTinTuc));
            Assert(tinTuc != null, "CauHinhTinTuc exists in EF Core model");
            if (tinTuc != null)
            {
                Assert(tinTuc.FindProperty("KieuChuTieuDe") == null, "CauHinhTinTuc does not map 'KieuChuTieuDe'");
                Assert(tinTuc.FindProperty("KieuChuNoiDung") == null, "CauHinhTinTuc does not map 'KieuChuNoiDung'");
                Assert(tinTuc.FindProperty("FontTieuDe") != null, "CauHinhTinTuc maps physical 'FontTieuDe'");
                Assert(tinTuc.FindProperty("FontNoiDung") != null, "CauHinhTinTuc maps physical 'FontNoiDung'");
            }
        }

        private static void Test_03_SqlTranslation_CauHinhTrangChu(ApplicationDbContext context)
        {
            Console.WriteLine("\n--- TEST SUITE 3: CauHinhTrangChu SQL Translation via ToQueryString() ---");

            // Query 1: Simple select
            var sql1 = context.CauHinhTrangChus.ToQueryString();
            Console.WriteLine($"[Generated SQL 1]:\n{sql1}\n");
            AssertSqlContainsZeroInvalidColumns(sql1, "Simple CauHinhTrangChus query");

            // Query 2: Join with ChienDich (Active campaign)
            var sql2 = context.CauHinhTrangChus
                .Include(c => c.ChienDich)
                .Where(c => c.ChienDich!.TrangThaiHoatDong)
                .ToQueryString();
            Console.WriteLine($"[Generated SQL 2]:\n{sql2}\n");
            AssertSqlContainsZeroInvalidColumns(sql2, "Include(ChienDich) where active query");

            // Assert that physical columns ARE present in SQL projection
            Assert(sql1.Contains("[LinkAnhBia]"), "Generated SQL contains [LinkAnhBia]");
            Assert(sql1.Contains("[FontTieuDe]"), "Generated SQL contains [FontTieuDe]");
            Assert(sql1.Contains("[FontNoiDung]"), "Generated SQL contains [FontNoiDung]");
            Assert(sql1.Contains("[HienThiCanvas3D]"), "Generated SQL contains [HienThiCanvas3D]");
            Assert(sql1.Contains("[VeDuAn_LinkAnh]"), "Generated SQL contains [VeDuAn_LinkAnh]");
            Assert(sql1.Contains("[HienThiMascot]"), "Generated SQL contains [HienThiMascot]");
            Assert(sql1.Contains("[LinkAnhMascot]"), "Generated SQL contains [LinkAnhMascot]");
            Assert(sql1.Contains("[ViTriMascot]"), "Generated SQL contains [ViTriMascot]");
        }

        private static void Test_04_SqlTranslation_OtherEntities(ApplicationDbContext context)
        {
            Console.WriteLine("\n--- TEST SUITE 4: Other Configuration Entities SQL Translation ---");

            // CauHinhKyUc
            var sqlKyUc = context.CauHinhKyUcs.ToQueryString();
            AssertSqlContainsZeroInvalidColumns(sqlKyUc, "CauHinhKyUcs query");
            Assert(sqlKyUc.Contains("[LinkAnhBia]"), "CauHinhKyUcs SQL contains [LinkAnhBia]");
            Assert(sqlKyUc.Contains("[FontTieuDe]"), "CauHinhKyUcs SQL contains [FontTieuDe]");
            Assert(sqlKyUc.Contains("[FontTieuDeTongKet]"), "CauHinhKyUcs SQL contains [FontTieuDeTongKet]");

            // CauHinhChuongTrinh
            var sqlCT = context.CauHinhChuongTrinhs.ToQueryString();
            AssertSqlContainsZeroInvalidColumns(sqlCT, "CauHinhChuongTrinhs query");
            Assert(sqlCT.Contains("[FontTieuDe]"), "CauHinhChuongTrinhs SQL contains [FontTieuDe]");

            // CauHinhGayQuy
            var sqlGQ = context.CauHinhGayQuys.ToQueryString();
            AssertSqlContainsZeroInvalidColumns(sqlGQ, "CauHinhGayQuys query");
            Assert(sqlGQ.Contains("[FontTieuDe]"), "CauHinhGayQuys SQL contains [FontTieuDe]");
            Assert(sqlGQ.Contains("[FontNoiDung]"), "CauHinhGayQuys SQL contains [FontNoiDung]");

            // CauHinhBanToChuc
            var sqlBTC = context.CauHinhBanToChucs.ToQueryString();
            AssertSqlContainsZeroInvalidColumns(sqlBTC, "CauHinhBanToChucs query");
            Assert(sqlBTC.Contains("[FontTieuDe]"), "CauHinhBanToChucs SQL contains [FontTieuDe]");
            Assert(sqlBTC.Contains("[FontNoiDung]"), "CauHinhBanToChucs SQL contains [FontNoiDung]");

            // CauHinhTinTuc
            var sqlTT = context.CauHinhTinTucs.ToQueryString();
            AssertSqlContainsZeroInvalidColumns(sqlTT, "CauHinhTinTucs query");
            Assert(sqlTT.Contains("[FontTieuDe]"), "CauHinhTinTucs SQL contains [FontTieuDe]");
            Assert(sqlTT.Contains("[FontNoiDung]"), "CauHinhTinTucs SQL contains [FontNoiDung]");
        }

        private static void AssertSqlContainsZeroInvalidColumns(string sql, string contextName)
        {
            foreach (var invalidName in InvalidColumnNames)
            {
                bool contains = sql.Contains(invalidName, StringComparison.OrdinalIgnoreCase);
                Assert(!contains, $"{contextName}: zero occurrence of invalid '{invalidName}' in SQL",
                    $"Found '{invalidName}' in generated SQL:\n{sql}");
            }
        }

        private static void Test_05_Bidirectional_Proxy_CauHinhTrangChu()
        {
            Console.WriteLine("\n--- TEST SUITE 5: CauHinhTrangChu Bidirectional Proxy Properties ---");

            var entity = new CauHinhTrangChu();

            // 1. DuongDanAnhBia <-> LinkAnhBia
            entity.LinkAnhBia = "/uploads/banner.jpg";
            Assert(entity.DuongDanAnhBia == "/uploads/banner.jpg", "LinkAnhBia -> DuongDanAnhBia");
            entity.DuongDanAnhBia = "/uploads/banner_new.png";
            Assert(entity.LinkAnhBia == "/uploads/banner_new.png", "DuongDanAnhBia -> LinkAnhBia");
            entity.DuongDanAnhBia = null;
            Assert(entity.LinkAnhBia == null, "DuongDanAnhBia null -> LinkAnhBia null");

            // 2. KieuChuTieuDe <-> FontTieuDe
            entity.FontTieuDe = "Montserrat";
            Assert(entity.KieuChuTieuDe == "Montserrat", "FontTieuDe -> KieuChuTieuDe");
            entity.KieuChuTieuDe = "Open Sans";
            Assert(entity.FontTieuDe == "Open Sans", "KieuChuTieuDe -> FontTieuDe");

            // 3. KieuChuNoiDung <-> FontNoiDung
            entity.FontNoiDung = "Roboto";
            Assert(entity.KieuChuNoiDung == "Roboto", "FontNoiDung -> KieuChuNoiDung");
            entity.KieuChuNoiDung = "Inter";
            Assert(entity.FontNoiDung == "Inter", "KieuChuNoiDung -> FontNoiDung");

            // 4. HienThiHieuUng3D <-> HienThiCanvas3D
            entity.HienThiCanvas3D = true;
            Assert(entity.HienThiHieuUng3D == true, "HienThiCanvas3D true -> HienThiHieuUng3D true");
            entity.HienThiHieuUng3D = false;
            Assert(entity.HienThiCanvas3D == false, "HienThiHieuUng3D false -> HienThiCanvas3D false");

            // 5. VeDuAn_DuongDanAnh <-> VeDuAn_LinkAnh
            entity.VeDuAn_LinkAnh = "/uploads/project.jpg";
            Assert(entity.VeDuAn_DuongDanAnh == "/uploads/project.jpg", "VeDuAn_LinkAnh -> VeDuAn_DuongDanAnh");
            entity.VeDuAn_DuongDanAnh = "/uploads/project2.jpg";
            Assert(entity.VeDuAn_LinkAnh == "/uploads/project2.jpg", "VeDuAn_DuongDanAnh -> VeDuAn_LinkAnh");

            // 6. HienThiLinhVat <-> HienThiMascot
            entity.HienThiMascot = false;
            Assert(entity.HienThiLinhVat == false, "HienThiMascot false -> HienThiLinhVat false");
            entity.HienThiLinhVat = true;
            Assert(entity.HienThiMascot == true, "HienThiLinhVat true -> HienThiMascot true");

            // 7. DuongDanAnhLinhVat <-> LinkAnhMascot
            entity.LinkAnhMascot = "/uploads/mascot.png";
            Assert(entity.DuongDanAnhLinhVat == "/uploads/mascot.png", "LinkAnhMascot -> DuongDanAnhLinhVat");
            entity.DuongDanAnhLinhVat = "/uploads/mascot_updated.png";
            Assert(entity.LinkAnhMascot == "/uploads/mascot_updated.png", "DuongDanAnhLinhVat -> LinkAnhMascot");

            // 8. ViTriLinhVat <-> ViTriMascot
            entity.ViTriMascot = "left";
            Assert(entity.ViTriLinhVat == "left", "ViTriMascot left -> ViTriLinhVat left");
            entity.ViTriLinhVat = "center";
            Assert(entity.ViTriMascot == "center", "ViTriLinhVat center -> ViTriMascot center");
        }

        private static void Test_06_Bidirectional_Proxy_OtherEntities()
        {
            Console.WriteLine("\n--- TEST SUITE 6: Other Entities Bidirectional Proxy Properties ---");

            // CauHinhKyUc
            var kyUc = new CauHinhKyUc();
            kyUc.LinkAnhBia = "/img/kyuc.jpg";
            Assert(kyUc.DuongDanAnhBia == "/img/kyuc.jpg", "CauHinhKyUc: LinkAnhBia -> DuongDanAnhBia");
            kyUc.DuongDanAnhBia = "/img/kyuc2.jpg";
            Assert(kyUc.LinkAnhBia == "/img/kyuc2.jpg", "CauHinhKyUc: DuongDanAnhBia -> LinkAnhBia");

            kyUc.FontTieuDe = "Dancing Script";
            Assert(kyUc.KieuChuTieuDe == "Dancing Script", "CauHinhKyUc: FontTieuDe -> KieuChuTieuDe");
            kyUc.KieuChuTieuDe = "Pacifico";
            Assert(kyUc.FontTieuDe == "Pacifico", "CauHinhKyUc: KieuChuTieuDe -> FontTieuDe");

            kyUc.FontTieuDeTongKet = "Great Vibes";
            Assert(kyUc.KieuChuTieuDeTongKet == "Great Vibes", "CauHinhKyUc: FontTieuDeTongKet -> KieuChuTieuDeTongKet");
            kyUc.KieuChuTieuDeTongKet = "Lobster";
            Assert(kyUc.FontTieuDeTongKet == "Lobster", "CauHinhKyUc: KieuChuTieuDeTongKet -> FontTieuDeTongKet");

            // CauHinhChuongTrinh
            var ct = new CauHinhChuongTrinh();
            ct.FontTieuDe = "Be Vietnam Pro";
            Assert(ct.KieuChuTieuDe == "Be Vietnam Pro", "CauHinhChuongTrinh: FontTieuDe -> KieuChuTieuDe");
            ct.KieuChuTieuDe = "Nunito";
            Assert(ct.FontTieuDe == "Nunito", "CauHinhChuongTrinh: KieuChuTieuDe -> FontTieuDe");

            // CauHinhGayQuy
            var gq = new CauHinhGayQuy();
            gq.FontTieuDe = "Be Vietnam Pro";
            Assert(gq.KieuChuTieuDe == "Be Vietnam Pro", "CauHinhGayQuy: FontTieuDe -> KieuChuTieuDe");
            gq.KieuChuTieuDe = "Oswald";
            Assert(gq.FontTieuDe == "Oswald", "CauHinhGayQuy: KieuChuTieuDe -> FontTieuDe");

            gq.FontNoiDung = "Roboto";
            Assert(gq.KieuChuNoiDung == "Roboto", "CauHinhGayQuy: FontNoiDung -> KieuChuNoiDung");
            gq.KieuChuNoiDung = "Open Sans";
            Assert(gq.FontNoiDung == "Open Sans", "CauHinhGayQuy: KieuChuNoiDung -> FontNoiDung");

            // CauHinhBanToChuc
            var btc = new CauHinhBanToChuc();
            btc.FontTieuDe = "Dancing Script";
            Assert(btc.KieuChuTieuDe == "Dancing Script", "CauHinhBanToChuc: FontTieuDe -> KieuChuTieuDe");
            btc.KieuChuTieuDe = "Montserrat";
            Assert(btc.FontTieuDe == "Montserrat", "CauHinhBanToChuc: KieuChuTieuDe -> FontTieuDe");

            btc.FontNoiDung = "Be Vietnam Pro";
            Assert(btc.KieuChuNoiDung == "Be Vietnam Pro", "CauHinhBanToChuc: FontNoiDung -> KieuChuNoiDung");
            btc.KieuChuNoiDung = "Inter";
            Assert(btc.FontNoiDung == "Inter", "CauHinhBanToChuc: KieuChuNoiDung -> FontNoiDung");

            // CauHinhTinTuc
            var tt = new CauHinhTinTuc();
            tt.FontTieuDe = "Dancing Script";
            Assert(tt.KieuChuTieuDe == "Dancing Script", "CauHinhTinTuc: FontTieuDe -> KieuChuTieuDe");
            tt.KieuChuTieuDe = "Playfair Display";
            Assert(tt.FontTieuDe == "Playfair Display", "CauHinhTinTuc: KieuChuTieuDe -> FontTieuDe");

            tt.FontNoiDung = "Be Vietnam Pro";
            Assert(tt.KieuChuNoiDung == "Be Vietnam Pro", "CauHinhTinTuc: FontNoiDung -> KieuChuNoiDung");
            tt.KieuChuNoiDung = "Lato";
            Assert(tt.FontNoiDung == "Lato", "CauHinhTinTuc: KieuChuNoiDung -> FontNoiDung");
        }

        private static void Test_07_ChangeTracker_Proxy_Mutation(ApplicationDbContext context)
        {
            Console.WriteLine("\n--- TEST SUITE 7: EF Core ChangeTracker Proxy Mutation Verification ---");

            var config = new CauHinhTrangChu
            {
                IDCauHinh = 9999,
                IDChienDich = 1,
                LinkAnhBia = "/initial_bia.jpg",
                FontTieuDe = "InitialFont",
                FontNoiDung = "InitialContent",
                HienThiCanvas3D = false,
                VeDuAn_LinkAnh = "/initial_veduan.jpg",
                HienThiMascot = true,
                LinkAnhMascot = "/initial_mascot.png",
                ViTriMascot = "right"
            };

            context.Attach(config);

            // Mutate via bidirectional proxy properties
            config.DuongDanAnhBia = "/mutated_bia.jpg";
            config.KieuChuTieuDe = "MutatedFont";
            config.KieuChuNoiDung = "MutatedContent";
            config.HienThiHieuUng3D = true;
            config.VeDuAn_DuongDanAnh = "/mutated_veduan.jpg";
            config.HienThiLinhVat = false;
            config.DuongDanAnhLinhVat = "/mutated_mascot.png";
            config.ViTriLinhVat = "left";

            var entry = context.Entry(config);

            // Check tracked mapped properties reflect mutation and modified status
            Assert(entry.Property(e => e.LinkAnhBia).IsModified, "ChangeTracker: LinkAnhBia marked modified");
            Assert(entry.Property(e => e.LinkAnhBia).CurrentValue == "/mutated_bia.jpg", "ChangeTracker: LinkAnhBia current value updated");

            Assert(entry.Property(e => e.FontTieuDe).IsModified, "ChangeTracker: FontTieuDe marked modified");
            Assert(entry.Property(e => e.FontTieuDe).CurrentValue == "MutatedFont", "ChangeTracker: FontTieuDe current value updated");

            Assert(entry.Property(e => e.FontNoiDung).IsModified, "ChangeTracker: FontNoiDung marked modified");
            Assert(entry.Property(e => e.FontNoiDung).CurrentValue == "MutatedContent", "ChangeTracker: FontNoiDung current value updated");

            Assert(entry.Property(e => e.HienThiCanvas3D).IsModified, "ChangeTracker: HienThiCanvas3D marked modified");
            Assert((bool)entry.Property(e => e.HienThiCanvas3D).CurrentValue! == true, "ChangeTracker: HienThiCanvas3D current value updated");

            Assert(entry.Property(e => e.VeDuAn_LinkAnh).IsModified, "ChangeTracker: VeDuAn_LinkAnh marked modified");
            Assert(entry.Property(e => e.VeDuAn_LinkAnh).CurrentValue == "/mutated_veduan.jpg", "ChangeTracker: VeDuAn_LinkAnh current value updated");

            Assert(entry.Property(e => e.HienThiMascot).IsModified, "ChangeTracker: HienThiMascot marked modified");
            Assert((bool)entry.Property(e => e.HienThiMascot).CurrentValue! == false, "ChangeTracker: HienThiMascot current value updated");

            Assert(entry.Property(e => e.LinkAnhMascot).IsModified, "ChangeTracker: LinkAnhMascot marked modified");
            Assert(entry.Property(e => e.LinkAnhMascot).CurrentValue == "/mutated_mascot.png", "ChangeTracker: LinkAnhMascot current value updated");

            Assert(entry.Property(e => e.ViTriMascot).IsModified, "ChangeTracker: ViTriMascot marked modified");
            Assert(entry.Property(e => e.ViTriMascot).CurrentValue == "left", "ChangeTracker: ViTriMascot current value updated");

            // Verify that invalid property names are NOT tracked by ChangeTracker
            foreach (var invalidName in InvalidColumnNames)
            {
                bool threw = false;
                try
                {
                    entry.Property(invalidName);
                }
                catch (InvalidOperationException)
                {
                    threw = true;
                }
                Assert(threw, $"ChangeTracker: entry.Property('{invalidName}') throws InvalidOperationException (untracked)");
            }
        }
    }
}

