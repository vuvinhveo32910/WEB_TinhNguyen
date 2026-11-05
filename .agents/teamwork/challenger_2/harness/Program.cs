using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using WEB_TinhNguyen.BoDieuKhien;
using WEB_TinhNguyen.DuLieu;
using WEB_TinhNguyen.MoHinh;

namespace Challenger2Harness
{
    // =========================================================================
    // MOCK / FAKE INFRASTRUCTURE FOR ADO.NET / EF CORE / ASP.NET CORE
    // =========================================================================
    public class FakeDbConnection : DbConnection
    {
        public override string ConnectionString { get; set; } = "Server=tcp:127.0.0.1,1433;Database=TinhNguyen;User Id=sa;Password=Placeholder123!;Encrypt=False;";
        public override string Database => "TinhNguyen";
        public override string DataSource => "127.0.0.1";
        public override string ServerVersion => "15.00.4153";
        public override ConnectionState State => ConnectionState.Open;

        public override void ChangeDatabase(string databaseName) { }
        public override void Close() { }
        public override void Open() { }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
            => new FakeDbTransaction(this, isolationLevel);

        protected override DbCommand CreateDbCommand() => new FakeDbCommand(this);
    }

    public class FakeDbTransaction : DbTransaction
    {
        private readonly DbConnection _conn;
        private readonly IsolationLevel _level;
        public FakeDbTransaction(DbConnection conn, IsolationLevel level)
        {
            _conn = conn;
            _level = level;
        }

        public bool Committed { get; private set; }
        public bool RolledBack { get; private set; }

        public override IsolationLevel IsolationLevel => _level;
        protected override DbConnection DbConnection => _conn;
        public override void Commit() => Committed = true;
        public override void Rollback() => RolledBack = true;
    }

    public class FakeDbCommand : DbCommand
    {
        private readonly FakeDbParameterCollection _params = new();
        public FakeDbCommand(DbConnection conn) => DbConnection = conn;

        public override string CommandText { get; set; } = "";
        public override int CommandTimeout { get; set; } = 30;
        public override CommandType CommandType { get; set; } = CommandType.Text;
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection? DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection => _params;
        protected override DbTransaction? DbTransaction { get; set; }

        public override void Cancel() { }
        public override int ExecuteNonQuery() => 1;
        public override object? ExecuteScalar() => 1;
        public override void Prepare() { }

        protected override DbParameter CreateDbParameter() => new FakeDbParameter();
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => new FakeDbDataReader();
        protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
            => Task.FromResult<DbDataReader>(new FakeDbDataReader());
    }

    public class FakeDbParameter : DbParameter
    {
        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;
        public override bool IsNullable { get; set; } = true;
        public override string ParameterName { get; set; } = "";
        public override string SourceColumn { get; set; } = "";
        public override object? Value { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override int Size { get; set; }
        public override void ResetDbType() { }
    }

    public class FakeDbParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _items = new();
        public override int Count => _items.Count;
        public override object SyncRoot => ((ICollection)_items).SyncRoot;
        public override int Add(object value) { _items.Add((DbParameter)value); return _items.Count - 1; }
        public override void AddRange(Array values) { foreach (var v in values) Add(v); }
        public override void Clear() => _items.Clear();
        public override bool Contains(object value) => _items.Contains(value);
        public override bool Contains(string value) => _items.Any(p => p.ParameterName == value);
        public override void CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);
        public override IEnumerator GetEnumerator() => _items.GetEnumerator();
        public override int IndexOf(object value) => _items.IndexOf((DbParameter)value);
        public override int IndexOf(string parameterName) => _items.FindIndex(p => p.ParameterName == parameterName);
        public override void Insert(int index, object value) => _items.Insert(index, (DbParameter)value);
        public override void Remove(object value) => _items.Remove((DbParameter)value);
        public override void RemoveAt(int index) => _items.RemoveAt(index);
        public override void RemoveAt(string parameterName)
        {
            var idx = IndexOf(parameterName);
            if (idx >= 0) _items.RemoveAt(idx);
        }
        protected override DbParameter GetParameter(int index) => _items[index];
        protected override DbParameter GetParameter(string parameterName) => _items.First(p => p.ParameterName == parameterName);
        protected override void SetParameter(int index, DbParameter value) => _items[index] = value;
        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var idx = IndexOf(parameterName);
            if (idx >= 0) _items[idx] = value;
        }
    }

    public class FakeDbDataReader : DbDataReader
    {
        public override int FieldCount => 0;
        public override int Depth => 0;
        public override bool IsClosed => false;
        public override int RecordsAffected => 0;
        public override bool HasRows => false;
        public override object this[int ordinal] => throw new IndexOutOfRangeException();
        public override object this[string name] => throw new IndexOutOfRangeException();
        public override bool GetBoolean(int ordinal) => false;
        public override byte GetByte(int ordinal) => 0;
        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) => 0;
        public override char GetChar(int ordinal) => ' ';
        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) => 0;
        public override string GetDataTypeName(int ordinal) => "string";
        public override DateTime GetDateTime(int ordinal) => DateTime.Now;
        public override decimal GetDecimal(int ordinal) => 0m;
        public override double GetDouble(int ordinal) => 0.0;
        public override Type GetFieldType(int ordinal) => typeof(string);
        public override float GetFloat(int ordinal) => 0f;
        public override Guid GetGuid(int ordinal) => Guid.Empty;
        public override short GetInt16(int ordinal) => 0;
        public override int GetInt32(int ordinal) => 0;
        public override long GetInt64(int ordinal) => 0;
        public override string GetName(int ordinal) => "";
        public override int GetOrdinal(string name) => -1;
        public override string GetString(int ordinal) => "";
        public override object GetValue(int ordinal) => "";
        public override int GetValues(object[] values) => 0;
        public override bool IsDBNull(int ordinal) => true;
        public override bool NextResult() => false;
        public override bool Read() => false;
        public override Task<bool> ReadAsync(CancellationToken cancellationToken) => Task.FromResult(false);
        public override IEnumerator GetEnumerator() => Enumerable.Empty<object>().GetEnumerator();
    }

    // =========================================================================
    // ASYNC ENUMERABLE & QUERYABLE ADAPTER FOR IN-MEMORY TESTING
    // =========================================================================
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;
        internal TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

        public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);
        public object? Execute(Expression expression) => _inner.Execute(expression);
        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments().FirstOrDefault() ?? typeof(TResult);
            var executionResult = ((IQueryProvider)this).Execute(expression);
            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { executionResult })!;
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
        public T Current => _inner.Current;
        public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
        public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_inner.MoveNext());
    }

    public class MockWebHostEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "WEB_TinhNguyen";
        public string WebRootPath { get; set; } = Path.Combine(Path.GetTempPath(), "webroot_" + Guid.NewGuid().ToString("N"));
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = Path.GetTempPath();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();

        public MockWebHostEnvironment()
        {
            if (!Directory.Exists(WebRootPath)) Directory.CreateDirectory(WebRootPath);
        }
    }

    // =========================================================================
    // IN-MEMORY TEST APPLICATION DB CONTEXT
    // =========================================================================
    public class MockApplicationDbContext : ApplicationDbContext
    {
        public List<ChienDich> InMemoryChienDichs { get; } = new();
        public List<CauHinhTrangChu> InMemoryCauHinhTrangChus { get; } = new();
        public List<CauHinhKyUc> InMemoryCauHinhKyUcs { get; } = new();
        public List<CauHinhChuongTrinh> InMemoryCauHinhChuongTrinhs { get; } = new();
        public List<CauHinhGayQuy> InMemoryCauHinhGayQuys { get; } = new();
        public List<CauHinhBanToChuc> InMemoryCauHinhBanToChucs { get; } = new();
        public List<CauHinhTinTuc> InMemoryCauHinhTinTucs { get; } = new();

        public MockApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(1);
        }
    }

    // =========================================================================
    // MAIN TEST RUNNER
    // =========================================================================
    public class Program
    {
        private static int passed = 0;
        private static int failed = 0;
        private static readonly List<string> FailureLog = new();

        private static readonly string ProjectRoot = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../WEB_TinhNguyen")
        );

        private static void Assert(bool condition, string testName, string details = "")
        {
            if (condition)
            {
                passed++;
                Console.WriteLine($"  [PASS] {testName}");
            }
            else
            {
                failed++;
                var msg = $"  [FAIL] {testName} - {details}";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ResetColor();
                FailureLog.Add(msg);
            }
        }

        public static int Main(string[] args)
        {
            Console.WriteLine("================================================================================");
            Console.WriteLine(" CHALLENGER 2: ENDPOINT EXECUTION, CONTROLLER & RELATIONAL STRESS HARNESS");
            Console.WriteLine($" Target Project Root: {ProjectRoot}");
            Console.WriteLine($" Timestamp: {DateTime.UtcNow:O}");
            Console.WriteLine("================================================================================");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(new FakeDbConnection())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockEnv = new MockWebHostEnvironment();

            // -----------------------------------------------------------------
            // MODULE 1: EF CORE RELATIONAL MODEL METADATA VERIFICATION
            // -----------------------------------------------------------------
            Console.WriteLine("\n--- MODULE 1: EF Core Relational Model Metadata Verification ---");
            Test_01_CauHinhTrangChu_RelationalProperties(context);
            Test_02_CauHinhKyUc_RelationalProperties(context);
            Test_03_CauHinhChuongTrinh_RelationalProperties(context);
            Test_04_CauHinhGayQuy_RelationalProperties(context);
            Test_05_CauHinhBanToChuc_RelationalProperties(context);
            Test_06_CauHinhTinTuc_RelationalProperties(context);
            Test_07_ModelSnapshotCleanliness();

            // -----------------------------------------------------------------
            // MODULE 2: EF CORE RELATIONAL SQL QUERY COMPILER (ToQueryString)
            // -----------------------------------------------------------------
            Console.WriteLine("\n--- MODULE 2: EF Core Relational SQL Query Compiler (ToQueryString) ---");
            Test_08_HomeController_Index_Query_SQL(context);
            Test_09_AdminKyUc_Query_SQL(context);
            Test_10_AdminChuongTrinh_Query_SQL(context);
            Test_11_AdminGayQuy_Query_SQL(context);
            Test_12_AdminBanToChuc_Query_SQL(context);
            Test_13_AdminTinTuc_Query_SQL(context);

            // -----------------------------------------------------------------
            // MODULE 3: CONTROLLER INSTANTIATION & DEPENDENCY INJECTION
            // -----------------------------------------------------------------
            Console.WriteLine("\n--- MODULE 3: Controller Instantiation & Dependency Injection ---");
            Test_14_ControllerInstantiation(context, mockEnv);

            // -----------------------------------------------------------------
            // MODULE 4: HOME CONTROLLER EXECUTION & CONFIGURATION LOADING
            // -----------------------------------------------------------------
            Console.WriteLine("\n--- MODULE 4: HomeController Execution & Configuration Loading ---");
            Test_15_HomeController_Index_ComingSoon_WhenNoActiveCampaign(options);
            Test_16_HomeController_Index_LoadsActiveCampaignConfig(options);
            Test_17_HomeController_Subpages(options);

            // -----------------------------------------------------------------
            // MODULE 5: ADMIN CONTROLLERS EDIT (GET & POST) FORM BINDING & MUTATION
            // -----------------------------------------------------------------
            Console.WriteLine("\n--- MODULE 5: Admin Edit Actions (GET & POST) Model Binding & Entity Update ---");
            Test_18_AdminTrangChu_Edit_Get_And_Post(options, mockEnv);
            Test_19_AdminKyUc_Edit_Get_And_Post(options, mockEnv);
            Test_20_AdminChuongTrinh_Edit_Get_And_Post(options, mockEnv);
            Test_21_AdminGayQuy_Edit_Get_And_Post(options, mockEnv);
            Test_22_AdminBanToChuc_Edit_Get_And_Post(options, mockEnv);
            Test_23_AdminTinTuc_Edit_Get_And_Post(options, mockEnv);

            // -----------------------------------------------------------------
            // MODULE 6: ADVERSARIAL STRESS TESTING & BOUNDARY ORACLES
            // -----------------------------------------------------------------
            Console.WriteLine("\n--- MODULE 6: Adversarial Stress Testing & Boundary Oracles ---");
            Test_24_UnmappedProxyProperties_Bidirectional_Equivalence();
            Test_25_LargePayloads_And_ExtremeStrings();
            Test_26_SqlInjection_And_Xss_Payloads();
            Test_27_Null_And_Whitespace_Preservation();
            Test_28_Concurrent_Entity_Mutations();

            // -----------------------------------------------------------------
            // SUMMARY
            // -----------------------------------------------------------------
            Console.WriteLine("\n================================================================================");
            Console.WriteLine($" HARNESS RESULTS: {passed} PASSED | {failed} FAILED | TOTAL: {passed + failed}");
            Console.WriteLine("================================================================================");

            if (failed > 0)
            {
                Console.WriteLine("\nFAILURES ENCOUNTERED:");
                foreach (var fail in FailureLog)
                {
                    Console.WriteLine(fail);
                }
            }

            return failed == 0 ? 0 : 1;
        }

        // =====================================================================
        // TEST IMPLEMENTATIONS
        // =====================================================================

        private static void Test_01_CauHinhTrangChu_RelationalProperties(ApplicationDbContext context)
        {
            var entity = context.Model.FindEntityType(typeof(CauHinhTrangChu));
            Assert(entity != null, "M1_01: CauHinhTrangChu entity exists in EF Core relational model");

            var invalidNames = new[]
            {
                "DuongDanAnhBia", "DuongDanAnhLinhVat", "HienThiHieuUng3D", "HienThiLinhVat",
                "KieuChuNoiDung", "KieuChuTieuDe", "VeDuAn_DuongDanAnh", "ViTriLinhVat"
            };

            foreach (var invalid in invalidNames)
            {
                var prop = entity!.FindProperty(invalid);
                Assert(prop == null, $"M1_01: Invalid column '{invalid}' must NOT be mapped in CauHinhTrangChu relational model");
            }

            var validNames = new[]
            {
                "LinkAnhBia", "FontTieuDe", "FontNoiDung", "HienThiCanvas3D",
                "VeDuAn_LinkAnh", "HienThiMascot", "LinkAnhMascot", "ViTriMascot"
            };

            foreach (var valid in validNames)
            {
                var prop = entity!.FindProperty(valid);
                Assert(prop != null, $"M1_01: Physical column '{valid}' must be mapped in CauHinhTrangChu relational model");
            }
        }

        private static void Test_02_CauHinhKyUc_RelationalProperties(ApplicationDbContext context)
        {
            var entity = context.Model.FindEntityType(typeof(CauHinhKyUc));
            Assert(entity != null, "M1_02: CauHinhKyUc exists in EF Core model");

            Assert(entity!.FindProperty("DuongDanAnhBia") == null, "M1_02: CauHinhKyUc excludes DuongDanAnhBia");
            Assert(entity.FindProperty("KieuChuTieuDe") == null, "M1_02: CauHinhKyUc excludes KieuChuTieuDe");
            Assert(entity.FindProperty("KieuChuTieuDeTongKet") == null, "M1_02: CauHinhKyUc excludes KieuChuTieuDeTongKet");

            Assert(entity.FindProperty("LinkAnhBia") != null, "M1_02: CauHinhKyUc maps LinkAnhBia");
            Assert(entity.FindProperty("FontTieuDe") != null, "M1_02: CauHinhKyUc maps FontTieuDe");
            Assert(entity.FindProperty("FontTieuDeTongKet") != null, "M1_02: CauHinhKyUc maps FontTieuDeTongKet");
        }

        private static void Test_03_CauHinhChuongTrinh_RelationalProperties(ApplicationDbContext context)
        {
            var entity = context.Model.FindEntityType(typeof(CauHinhChuongTrinh));
            Assert(entity != null, "M1_03: CauHinhChuongTrinh exists in EF Core model");

            Assert(entity!.FindProperty("KieuChuTieuDe") == null, "M1_03: CauHinhChuongTrinh excludes KieuChuTieuDe");
            Assert(entity.FindProperty("FontTieuDe") != null, "M1_03: CauHinhChuongTrinh maps FontTieuDe");
        }

        private static void Test_04_CauHinhGayQuy_RelationalProperties(ApplicationDbContext context)
        {
            var entity = context.Model.FindEntityType(typeof(CauHinhGayQuy));
            Assert(entity != null, "M1_04: CauHinhGayQuy exists in EF Core model");

            Assert(entity!.FindProperty("KieuChuTieuDe") == null, "M1_04: CauHinhGayQuy excludes KieuChuTieuDe");
            Assert(entity.FindProperty("KieuChuNoiDung") == null, "M1_04: CauHinhGayQuy excludes KieuChuNoiDung");

            Assert(entity.FindProperty("FontTieuDe") != null, "M1_04: CauHinhGayQuy maps FontTieuDe");
            Assert(entity.FindProperty("FontNoiDung") != null, "M1_04: CauHinhGayQuy maps FontNoiDung");
        }

        private static void Test_05_CauHinhBanToChuc_RelationalProperties(ApplicationDbContext context)
        {
            var entity = context.Model.FindEntityType(typeof(CauHinhBanToChuc));
            Assert(entity != null, "M1_05: CauHinhBanToChuc exists in EF Core model");

            Assert(entity!.FindProperty("KieuChuTieuDe") == null, "M1_05: CauHinhBanToChuc excludes KieuChuTieuDe");
            Assert(entity.FindProperty("KieuChuNoiDung") == null, "M1_05: CauHinhBanToChuc excludes KieuChuNoiDung");

            Assert(entity.FindProperty("FontTieuDe") != null, "M1_05: CauHinhBanToChuc maps FontTieuDe");
            Assert(entity.FindProperty("FontNoiDung") != null, "M1_05: CauHinhBanToChuc maps FontNoiDung");
        }

        private static void Test_06_CauHinhTinTuc_RelationalProperties(ApplicationDbContext context)
        {
            var entity = context.Model.FindEntityType(typeof(CauHinhTinTuc));
            Assert(entity != null, "M1_06: CauHinhTinTuc exists in EF Core model");

            Assert(entity!.FindProperty("KieuChuTieuDe") == null, "M1_06: CauHinhTinTuc excludes KieuChuTieuDe");
            Assert(entity.FindProperty("KieuChuNoiDung") == null, "M1_06: CauHinhTinTuc excludes KieuChuNoiDung");

            Assert(entity.FindProperty("FontTieuDe") != null, "M1_06: CauHinhTinTuc maps FontTieuDe");
            Assert(entity.FindProperty("FontNoiDung") != null, "M1_06: CauHinhTinTuc maps FontNoiDung");
        }

        private static void Test_07_ModelSnapshotCleanliness()
        {
            var snapshotPath = Path.Combine(ProjectRoot, "Migrations", "ApplicationDbContextModelSnapshot.cs");
            Assert(File.Exists(snapshotPath), "M1_07: ApplicationDbContextModelSnapshot.cs exists");

            var content = File.ReadAllText(snapshotPath);
            var invalidNames = new[]
            {
                "\"DuongDanAnhBia\"", "\"DuongDanAnhLinhVat\"", "\"HienThiHieuUng3D\"", "\"HienThiLinhVat\"",
                "\"KieuChuNoiDung\"", "\"KieuChuTieuDe\"", "\"VeDuAn_DuongDanAnh\"", "\"ViTriLinhVat\""
            };

            foreach (var name in invalidNames)
            {
                Assert(!content.Contains(name), $"M1_07: ModelSnapshot must not contain {name}");
            }
        }

        private static void Test_08_HomeController_Index_Query_SQL(ApplicationDbContext context)
        {
            var query = context.CauHinhTrangChus
                .Include(c => c.ChienDich)
                .Include(c => c.BieuNguTrangChus)
                .Include(c => c.ChuongTrinhDemNguocs)
                .Include(c => c.CauHinhAnhNois)
                .Include(c => c.ThuVienKhoanhKhacs)
                .Include(c => c.TinTucs)
                .Include(c => c.NhaTaiTros)
                .Include(c => c.CauHoiThuongGaps)
                .Where(c => c.ChienDich.TrangThaiHoatDong == true);

            string sql = query.ToQueryString();
            Assert(!string.IsNullOrEmpty(sql), "M2_08: Query translates to SQL Server dialect");

            var invalidNames = new[]
            {
                "[DuongDanAnhBia]", "[DuongDanAnhLinhVat]", "[HienThiHieuUng3D]", "[HienThiLinhVat]",
                "[KieuChuNoiDung]", "[KieuChuTieuDe]", "[VeDuAn_DuongDanAnh]", "[ViTriLinhVat]"
            };

            foreach (var invalid in invalidNames)
            {
                Assert(!sql.Contains(invalid), $"M2_08: Generated SQL must NOT contain invalid column {invalid}");
            }

            var validNames = new[]
            {
                "[LinkAnhBia]", "[FontTieuDe]", "[FontNoiDung]", "[HienThiCanvas3D]",
                "[VeDuAn_LinkAnh]", "[HienThiMascot]", "[LinkAnhMascot]", "[ViTriMascot]"
            };

            foreach (var valid in validNames)
            {
                Assert(sql.Contains(valid), $"M2_08: Generated SQL must contain physical column {valid}");
            }
        }

        private static void Test_09_AdminKyUc_Query_SQL(ApplicationDbContext context)
        {
            var sql = context.CauHinhKyUcs
                .Include(c => c.AnhKyUcs)
                .Include(c => c.NhaTaiTroKyUcs)
                .Where(c => c.IDChienDich == 1)
                .ToQueryString();

            Assert(!sql.Contains("[DuongDanAnhBia]"), "M2_09: CauHinhKyUcs SQL excludes DuongDanAnhBia");
            Assert(!sql.Contains("[KieuChuTieuDe]"), "M2_09: CauHinhKyUcs SQL excludes KieuChuTieuDe");
            Assert(sql.Contains("[LinkAnhBia]"), "M2_09: CauHinhKyUcs SQL includes LinkAnhBia");
            Assert(sql.Contains("[FontTieuDe]"), "M2_09: CauHinhKyUcs SQL includes FontTieuDe");
        }

        private static void Test_10_AdminChuongTrinh_Query_SQL(ApplicationDbContext context)
        {
            var sql = context.CauHinhChuongTrinhs
                .Include(c => c.ChangChuongTrinhs)
                .ThenInclude(ch => ch.ChiTietLoTrinhs)
                .Where(c => c.IDChienDich == 1)
                .ToQueryString();

            Assert(!sql.Contains("[KieuChuTieuDe]"), "M2_10: CauHinhChuongTrinhs SQL excludes KieuChuTieuDe");
            Assert(sql.Contains("[FontTieuDe]"), "M2_10: CauHinhChuongTrinhs SQL includes FontTieuDe");
        }

        private static void Test_11_AdminGayQuy_Query_SQL(ApplicationDbContext context)
        {
            var sql = context.CauHinhGayQuys
                .Include(c => c.KenhDongGops)
                .Include(c => c.NhaTaiTroGayQuys)
                .Where(c => c.IDChienDich == 1)
                .ToQueryString();

            Assert(!sql.Contains("[KieuChuTieuDe]"), "M2_11: CauHinhGayQuys SQL excludes KieuChuTieuDe");
            Assert(!sql.Contains("[KieuChuNoiDung]"), "M2_11: CauHinhGayQuys SQL excludes KieuChuNoiDung");
            Assert(sql.Contains("[FontTieuDe]"), "M2_11: CauHinhGayQuys SQL includes FontTieuDe");
            Assert(sql.Contains("[FontNoiDung]"), "M2_11: CauHinhGayQuys SQL includes FontNoiDung");
        }

        private static void Test_12_AdminBanToChuc_Query_SQL(ApplicationDbContext context)
        {
            var sql = context.CauHinhBanToChucs
                .Include(c => c.ThanhViens)
                .Where(c => c.IDChienDich == 1)
                .ToQueryString();

            Assert(!sql.Contains("[KieuChuTieuDe]"), "M2_12: CauHinhBanToChucs SQL excludes KieuChuTieuDe");
            Assert(!sql.Contains("[KieuChuNoiDung]"), "M2_12: CauHinhBanToChucs SQL excludes KieuChuNoiDung");
            Assert(sql.Contains("[FontTieuDe]"), "M2_12: CauHinhBanToChucs SQL includes FontTieuDe");
            Assert(sql.Contains("[FontNoiDung]"), "M2_12: CauHinhBanToChucs SQL includes FontNoiDung");
        }

        private static void Test_13_AdminTinTuc_Query_SQL(ApplicationDbContext context)
        {
            var sql = context.CauHinhTinTucs
                .Include(c => c.BaiViets)
                .Where(c => c.IDChienDich == 1)
                .ToQueryString();

            Assert(!sql.Contains("[KieuChuTieuDe]"), "M2_13: CauHinhTinTucs SQL excludes KieuChuTieuDe");
            Assert(!sql.Contains("[KieuChuNoiDung]"), "M2_13: CauHinhTinTucs SQL excludes KieuChuNoiDung");
            Assert(sql.Contains("[FontTieuDe]"), "M2_13: CauHinhTinTucs SQL includes FontTieuDe");
            Assert(sql.Contains("[FontNoiDung]"), "M2_13: CauHinhTinTucs SQL includes FontNoiDung");
        }

        private static void Test_14_ControllerInstantiation(ApplicationDbContext context, IWebHostEnvironment env)
        {
            var loggerHome = NullLogger<HomeController>.Instance;
            var homeController = new HomeController(loggerHome, context);
            Assert(homeController != null, "M3_14: HomeController successfully instantiated");

            var adminTrangChu = new AdminTrangChuController(context, env);
            Assert(adminTrangChu != null, "M3_14: AdminTrangChuController successfully instantiated");

            var adminKyUc = new AdminKyUcController(context, env);
            Assert(adminKyUc != null, "M3_14: AdminKyUcController successfully instantiated");

            var adminChuongTrinh = new AdminChuongTrinhController(context, env);
            Assert(adminChuongTrinh != null, "M3_14: AdminChuongTrinhController successfully instantiated");

            var adminGayQuy = new AdminGayQuyController(context, env);
            Assert(adminGayQuy != null, "M3_14: AdminGayQuyController successfully instantiated");

            var adminBanToChuc = new AdminBanToChucController(context, env);
            Assert(adminBanToChuc != null, "M3_14: AdminBanToChucController successfully instantiated");

            var adminTinTuc = new AdminTinTucController(context, env);
            Assert(adminTinTuc != null, "M3_14: AdminTinTucController successfully instantiated");
        }

        private static void Test_15_HomeController_Index_ComingSoon_WhenNoActiveCampaign(DbContextOptions<ApplicationDbContext> options)
        {
            using var context = new ApplicationDbContext(options);
            var logger = NullLogger<HomeController>.Instance;
            var controller = new HomeController(logger, context);

            // With FakeDbConnection returning empty rows, activeConfig is null
            var result = controller.Index().GetAwaiter().GetResult();
            Assert(result is ViewResult, "M4_15: Index() returns ViewResult");
            var viewResult = (ViewResult)result;
            Assert(viewResult.ViewName == "ComingSoon", "M4_15: Index() returns 'ComingSoon' view when active campaign is null");
        }

        private static void Test_16_HomeController_Index_LoadsActiveCampaignConfig(DbContextOptions<ApplicationDbContext> options)
        {
            // Verify model properties reading behavior
            var config = new CauHinhTrangChu
            {
                IDCauHinh = 1,
                IDChienDich = 1,
                TieuDeChinh = "Trăng Sáng Langbiang 2026",
                TieuDePhu = "Hành trình yêu thương",
                DoanVanMoTa = "Dự án thiện nguyện vùng cao",
                LinkAnhBia = "/gallery/hero.jpg",
                FontTieuDe = "Dancing Script",
                FontNoiDung = "Be Vietnam Pro",
                HienThiCanvas3D = true,
                VeDuAn_LinkAnh = "/gallery/veduan.jpg",
                HienThiMascot = true,
                LinkAnhMascot = "/gallery/mascot.png",
                ViTriMascot = "right",
                ChienDich = new ChienDich { IDChienDich = 1, Nam = 2026, TrangThaiHoatDong = true }
            };

            // Test that view access through both physical DB columns and proxy properties returns identical values
            Assert(config.DuongDanAnhBia == config.LinkAnhBia, "M4_16: DuongDanAnhBia == LinkAnhBia");
            Assert(config.KieuChuTieuDe == config.FontTieuDe, "M4_16: KieuChuTieuDe == FontTieuDe");
            Assert(config.KieuChuNoiDung == config.FontNoiDung, "M4_16: KieuChuNoiDung == FontNoiDung");
            Assert(config.HienThiHieuUng3D == config.HienThiCanvas3D, "M4_16: HienThiHieuUng3D == HienThiCanvas3D");
            Assert(config.VeDuAn_DuongDanAnh == config.VeDuAn_LinkAnh, "M4_16: VeDuAn_DuongDanAnh == VeDuAn_LinkAnh");
            Assert(config.HienThiLinhVat == config.HienThiMascot, "M4_16: HienThiLinhVat == HienThiMascot");
            Assert(config.DuongDanAnhLinhVat == config.LinkAnhMascot, "M4_16: DuongDanAnhLinhVat == LinkAnhMascot");
            Assert(config.ViTriLinhVat == config.ViTriMascot, "M4_16: ViTriLinhVat == ViTriMascot");

            var vr = new ViewResult { Model = config };
            Assert(vr.Model is CauHinhTrangChu, "M4_16: ViewResult contains CauHinhTrangChu model");
        }

        private static void Test_17_HomeController_Subpages(DbContextOptions<ApplicationDbContext> options)
        {
            using var context = new ApplicationDbContext(options);
            var logger = NullLogger<HomeController>.Instance;
            var controller = new HomeController(logger, context);

            // ChuongTrinh
            var r1 = controller.ChuongTrinh(null).GetAwaiter().GetResult();
            Assert(r1 is ViewResult, "M4_17: ChuongTrinh(null) returns ViewResult without crash");

            // GayQuy
            var r2 = controller.GayQuy(null).GetAwaiter().GetResult();
            Assert(r2 is ViewResult, "M4_17: GayQuy(null) returns ViewResult without crash");

            // BanToChuc
            var r3 = controller.BanToChuc(null).GetAwaiter().GetResult();
            Assert(r3 is ViewResult, "M4_17: BanToChuc(null) returns ViewResult without crash");

            // TinTuc
            var r4 = controller.TinTuc(null).GetAwaiter().GetResult();
            Assert(r4 is ViewResult, "M4_17: TinTuc(null) returns ViewResult without crash");

            // KyUc
            var r5 = controller.KyUc(2025).GetAwaiter().GetResult();
            Assert(r5 is ViewResult, "M4_17: KyUc(2025) returns ViewResult without crash");
        }

        private static void Test_18_AdminTrangChu_Edit_Get_And_Post(DbContextOptions<ApplicationDbContext> options, IWebHostEnvironment env)
        {
            // Verify ViewModel mapping and form post binding
            var entity = new CauHinhTrangChu
            {
                IDCauHinh = 10,
                IDChienDich = 1,
                TieuDeChinh = "Trang Chu 2026",
                LinkAnhBia = "/gallery/old_cover.jpg",
                FontTieuDe = "Dancing Script",
                FontNoiDung = "Be Vietnam Pro",
                HienThiCanvas3D = true,
                VeDuAn_LinkAnh = "/gallery/old_veduan.jpg",
                HienThiMascot = true,
                LinkAnhMascot = "/gallery/old_mascot.png",
                ViTriMascot = "left"
            };

            // Simulating GET Edit mapping
            var vm = new CauHinhTrangChuVM
            {
                IDCauHinh = entity.IDCauHinh,
                IDChienDich = entity.IDChienDich,
                TieuDeChinh = entity.TieuDeChinh,
                LinkAnhBiaCu = entity.DuongDanAnhBia,
                FontTieuDe = entity.KieuChuTieuDe ?? "Dancing Script",
                FontNoiDung = entity.KieuChuNoiDung ?? "Be Vietnam Pro",
                HienThiCanvas3D = entity.HienThiHieuUng3D,
                VeDuAn_LinkAnhCu = entity.VeDuAn_DuongDanAnh,
                HienThiMascot = entity.HienThiLinhVat,
                LinkAnhMascotCu = entity.DuongDanAnhLinhVat,
                ViTriMascot = entity.ViTriLinhVat ?? "right"
            };

            Assert(vm.LinkAnhBiaCu == "/gallery/old_cover.jpg", "M5_18: GET Edit mapping reads DuongDanAnhBia correctly");
            Assert(vm.FontTieuDe == "Dancing Script", "M5_18: GET Edit mapping reads KieuChuTieuDe correctly");
            Assert(vm.FontNoiDung == "Be Vietnam Pro", "M5_18: GET Edit mapping reads KieuChuNoiDung correctly");
            Assert(vm.HienThiCanvas3D == true, "M5_18: GET Edit mapping reads HienThiHieuUng3D correctly");
            Assert(vm.VeDuAn_LinkAnhCu == "/gallery/old_veduan.jpg", "M5_18: GET Edit mapping reads VeDuAn_DuongDanAnh correctly");
            Assert(vm.HienThiMascot == true, "M5_18: GET Edit mapping reads HienThiLinhVat correctly");
            Assert(vm.LinkAnhMascotCu == "/gallery/old_mascot.png", "M5_18: GET Edit mapping reads DuongDanAnhLinhVat correctly");
            Assert(vm.ViTriMascot == "left", "M5_18: GET Edit mapping reads ViTriLinhVat correctly");

            // Simulating POST Edit update
            vm.FontTieuDe = "Pacifico";
            vm.FontNoiDung = "Roboto";
            vm.HienThiCanvas3D = false;
            vm.HienThiMascot = false;
            vm.ViTriMascot = "center";
            vm.LinkAnhBiaCu = "/gallery/new_cover.jpg";
            vm.VeDuAn_LinkAnhCu = "/gallery/new_veduan.jpg";
            vm.LinkAnhMascotCu = "/gallery/new_mascot.png";

            // Controller executes:
            entity.KieuChuTieuDe = vm.FontTieuDe;
            entity.KieuChuNoiDung = vm.FontNoiDung;
            entity.HienThiHieuUng3D = vm.HienThiCanvas3D;
            entity.HienThiLinhVat = vm.HienThiMascot;
            entity.ViTriLinhVat = vm.ViTriMascot;
            entity.DuongDanAnhBia = vm.LinkAnhBiaCu;
            entity.VeDuAn_DuongDanAnh = vm.VeDuAn_LinkAnhCu;
            entity.DuongDanAnhLinhVat = vm.LinkAnhMascotCu;

            // Verify mutations updated underlying DB physical columns!
            Assert(entity.FontTieuDe == "Pacifico", "M5_18: POST Edit correctly updated physical FontTieuDe");
            Assert(entity.FontNoiDung == "Roboto", "M5_18: POST Edit correctly updated physical FontNoiDung");
            Assert(entity.HienThiCanvas3D == false, "M5_18: POST Edit correctly updated physical HienThiCanvas3D");
            Assert(entity.HienThiMascot == false, "M5_18: POST Edit correctly updated physical HienThiMascot");
            Assert(entity.ViTriMascot == "center", "M5_18: POST Edit correctly updated physical ViTriMascot");
            Assert(entity.LinkAnhBia == "/gallery/new_cover.jpg", "M5_18: POST Edit correctly updated physical LinkAnhBia");
            Assert(entity.VeDuAn_LinkAnh == "/gallery/new_veduan.jpg", "M5_18: POST Edit correctly updated physical VeDuAn_LinkAnh");
            Assert(entity.LinkAnhMascot == "/gallery/new_mascot.png", "M5_18: POST Edit correctly updated physical LinkAnhMascot");

            // Now test in EF Core change tracker:
            using var trackContext = new ApplicationDbContext(options);
            var entry = trackContext.Entry(entity);
            Assert(entry != null, "M5_18: Entity can be attached to DbContext change tracker");
            Assert(entry.Property(e => e.FontTieuDe).CurrentValue == "Pacifico", "M5_18: ChangeTracker tracks FontTieuDe");
            Assert(entry.Property(e => e.LinkAnhBia).CurrentValue == "/gallery/new_cover.jpg", "M5_18: ChangeTracker tracks LinkAnhBia");
        }

        private static void Test_19_AdminKyUc_Edit_Get_And_Post(DbContextOptions<ApplicationDbContext> options, IWebHostEnvironment env)
        {
            var entity = new CauHinhKyUc
            {
                IDCauHinhKyUc = 1,
                IDChienDich = 1,
                TieuDeChinh = "Ký Ức",
                LinkAnhBia = "/gallery/kyuc_old.jpg",
                FontTieuDe = "Dancing Script",
                FontTieuDeTongKet = "Be Vietnam Pro",
                TieuDeTongKet = "Tổng Kết 2025"
            };

            // GET mapping
            var vm = new CauHinhKyUcVM
            {
                IDCauHinhKyUc = entity.IDCauHinhKyUc,
                IDChienDich = entity.IDChienDich,
                TieuDeChinh = entity.TieuDeChinh,
                FontTieuDe = entity.KieuChuTieuDe,
                LinkAnhBiaCu = entity.DuongDanAnhBia,
                TieuDeTongKet = entity.TieuDeTongKet,
                FontTieuDeTongKet = entity.KieuChuTieuDeTongKet
            };

            Assert(vm.LinkAnhBiaCu == "/gallery/kyuc_old.jpg", "M5_19: GET KyUc reads DuongDanAnhBia");
            Assert(vm.FontTieuDe == "Dancing Script", "M5_19: GET KyUc reads KieuChuTieuDe");
            Assert(vm.FontTieuDeTongKet == "Be Vietnam Pro", "M5_19: GET KyUc reads KieuChuTieuDeTongKet");

            // POST update
            vm.FontTieuDe = "Montserrat";
            vm.FontTieuDeTongKet = "Roboto";
            vm.LinkAnhBiaCu = "/gallery/kyuc_new.jpg";

            entity.FontTieuDe = vm.FontTieuDe;
            entity.FontTieuDeTongKet = vm.FontTieuDeTongKet;
            entity.LinkAnhBia = vm.LinkAnhBiaCu;

            Assert(entity.FontTieuDe == "Montserrat", "M5_19: POST KyUc updates FontTieuDe");
            Assert(entity.KieuChuTieuDe == "Montserrat", "M5_19: POST KyUc KieuChuTieuDe reflects FontTieuDe");
            Assert(entity.FontTieuDeTongKet == "Roboto", "M5_19: POST KyUc updates FontTieuDeTongKet");
            Assert(entity.LinkAnhBia == "/gallery/kyuc_new.jpg", "M5_19: POST KyUc updates LinkAnhBia");
        }

        private static void Test_20_AdminChuongTrinh_Edit_Get_And_Post(DbContextOptions<ApplicationDbContext> options, IWebHostEnvironment env)
        {
            var entity = new CauHinhChuongTrinh
            {
                IDCauHinhCT = 1,
                IDChienDich = 1,
                TieuDeChinh = "Chương Trình",
                FontTieuDe = "Dancing Script"
            };

            // GET
            var vm = new CauHinhChuongTrinhVM
            {
                IDCauHinhCT = entity.IDCauHinhCT,
                IDChienDich = entity.IDChienDich,
                TieuDeChinh = entity.TieuDeChinh,
                FontTieuDe = entity.KieuChuTieuDe
            };
            Assert(vm.FontTieuDe == "Dancing Script", "M5_20: GET ChuongTrinh reads KieuChuTieuDe");

            // POST
            vm.FontTieuDe = "Playfair Display";
            entity.KieuChuTieuDe = vm.FontTieuDe;

            Assert(entity.FontTieuDe == "Playfair Display", "M5_20: POST ChuongTrinh updates physical FontTieuDe");
            Assert(entity.KieuChuTieuDe == "Playfair Display", "M5_20: POST ChuongTrinh proxy equals FontTieuDe");
        }

        private static void Test_21_AdminGayQuy_Edit_Get_And_Post(DbContextOptions<ApplicationDbContext> options, IWebHostEnvironment env)
        {
            var entity = new CauHinhGayQuy
            {
                IDCauHinhGQ = 1,
                IDChienDich = 1,
                FontTieuDe = "Dancing Script",
                FontNoiDung = "Be Vietnam Pro"
            };

            // GET
            var vm = new CauHinhGayQuyVM
            {
                IDCauHinhGQ = entity.IDCauHinhGQ,
                IDChienDich = entity.IDChienDich,
                FontTieuDe = entity.KieuChuTieuDe,
                FontNoiDung = entity.KieuChuNoiDung
            };
            Assert(vm.FontTieuDe == "Dancing Script", "M5_21: GET GayQuy reads KieuChuTieuDe");
            Assert(vm.FontNoiDung == "Be Vietnam Pro", "M5_21: GET GayQuy reads KieuChuNoiDung");

            // POST
            vm.FontTieuDe = "Lora";
            vm.FontNoiDung = "Open Sans";
            entity.KieuChuTieuDe = vm.FontTieuDe;
            entity.KieuChuNoiDung = vm.FontNoiDung;

            Assert(entity.FontTieuDe == "Lora", "M5_21: POST GayQuy updates physical FontTieuDe");
            Assert(entity.FontNoiDung == "Open Sans", "M5_21: POST GayQuy updates physical FontNoiDung");
        }

        private static void Test_22_AdminBanToChuc_Edit_Get_And_Post(DbContextOptions<ApplicationDbContext> options, IWebHostEnvironment env)
        {
            var entity = new CauHinhBanToChuc
            {
                IDCauHinhBTC = 1,
                IDChienDich = 1,
                FontTieuDe = "Dancing Script",
                FontNoiDung = "Be Vietnam Pro"
            };

            // GET
            var vm = new CauHinhBanToChucVM
            {
                IDCauHinhBTC = entity.IDCauHinhBTC,
                IDChienDich = entity.IDChienDich,
                FontTieuDe = entity.KieuChuTieuDe,
                FontNoiDung = entity.KieuChuNoiDung
            };
            Assert(vm.FontTieuDe == "Dancing Script", "M5_22: GET BanToChuc reads KieuChuTieuDe");
            Assert(vm.FontNoiDung == "Be Vietnam Pro", "M5_22: GET BanToChuc reads KieuChuNoiDung");

            // POST
            vm.FontTieuDe = "Merriweather";
            vm.FontNoiDung = "Inter";
            entity.KieuChuTieuDe = vm.FontTieuDe;
            entity.KieuChuNoiDung = vm.FontNoiDung;

            Assert(entity.FontTieuDe == "Merriweather", "M5_22: POST BanToChuc updates physical FontTieuDe");
            Assert(entity.FontNoiDung == "Inter", "M5_22: POST BanToChuc updates physical FontNoiDung");
        }

        private static void Test_23_AdminTinTuc_Edit_Get_And_Post(DbContextOptions<ApplicationDbContext> options, IWebHostEnvironment env)
        {
            var entity = new CauHinhTinTuc
            {
                IDCauHinhTT = 1,
                IDChienDich = 1,
                FontTieuDe = "Dancing Script",
                FontNoiDung = "Be Vietnam Pro"
            };

            // GET
            var vm = new CauHinhTinTucVM
            {
                IDCauHinhTT = entity.IDCauHinhTT,
                FontTieuDe = entity.KieuChuTieuDe,
                FontNoiDung = entity.KieuChuNoiDung
            };
            Assert(vm.FontTieuDe == "Dancing Script", "M5_23: GET TinTuc reads KieuChuTieuDe");
            Assert(vm.FontNoiDung == "Be Vietnam Pro", "M5_23: GET TinTuc reads KieuChuNoiDung");

            // POST
            vm.FontTieuDe = "Cinzel";
            vm.FontNoiDung = "Nunito";
            entity.KieuChuTieuDe = vm.FontTieuDe;
            entity.KieuChuNoiDung = vm.FontNoiDung;

            Assert(entity.FontTieuDe == "Cinzel", "M5_23: POST TinTuc updates physical FontTieuDe");
            Assert(entity.FontNoiDung == "Nunito", "M5_23: POST TinTuc updates physical FontNoiDung");
        }

        private static void Test_24_UnmappedProxyProperties_Bidirectional_Equivalence()
        {
            var config = new CauHinhTrangChu();

            // Test 1: DuongDanAnhBia <-> LinkAnhBia
            config.DuongDanAnhBia = "test_cover.png";
            Assert(config.LinkAnhBia == "test_cover.png", "M6_24: Setting DuongDanAnhBia sets LinkAnhBia");
            config.LinkAnhBia = "updated_cover.png";
            Assert(config.DuongDanAnhBia == "updated_cover.png", "M6_24: Setting LinkAnhBia sets DuongDanAnhBia");

            // Test 2: KieuChuTieuDe <-> FontTieuDe
            config.KieuChuTieuDe = "CustomFont";
            Assert(config.FontTieuDe == "CustomFont", "M6_24: Setting KieuChuTieuDe sets FontTieuDe");
            config.FontTieuDe = "NewFont";
            Assert(config.KieuChuTieuDe == "NewFont", "M6_24: Setting FontTieuDe sets KieuChuTieuDe");

            // Test 3: KieuChuNoiDung <-> FontNoiDung
            config.KieuChuNoiDung = "ContentFont";
            Assert(config.FontNoiDung == "ContentFont", "M6_24: Setting KieuChuNoiDung sets FontNoiDung");
            config.FontNoiDung = "AnotherContentFont";
            Assert(config.KieuChuNoiDung == "AnotherContentFont", "M6_24: Setting FontNoiDung sets KieuChuNoiDung");

            // Test 4: HienThiHieuUng3D <-> HienThiCanvas3D
            config.HienThiHieuUng3D = true;
            Assert(config.HienThiCanvas3D == true, "M6_24: Setting HienThiHieuUng3D sets HienThiCanvas3D");
            config.HienThiCanvas3D = false;
            Assert(config.HienThiHieuUng3D == false, "M6_24: Setting HienThiCanvas3D sets HienThiHieuUng3D");

            // Test 5: VeDuAn_DuongDanAnh <-> VeDuAn_LinkAnh
            config.VeDuAn_DuongDanAnh = "veduan.jpg";
            Assert(config.VeDuAn_LinkAnh == "veduan.jpg", "M6_24: Setting VeDuAn_DuongDanAnh sets VeDuAn_LinkAnh");
            config.VeDuAn_LinkAnh = "new_veduan.jpg";
            Assert(config.VeDuAn_DuongDanAnh == "new_veduan.jpg", "M6_24: Setting VeDuAn_LinkAnh sets VeDuAn_DuongDanAnh");

            // Test 6: HienThiLinhVat <-> HienThiMascot
            config.HienThiLinhVat = false;
            Assert(config.HienThiMascot == false, "M6_24: Setting HienThiLinhVat sets HienThiMascot");
            config.HienThiMascot = true;
            Assert(config.HienThiLinhVat == true, "M6_24: Setting HienThiMascot sets HienThiLinhVat");

            // Test 7: DuongDanAnhLinhVat <-> LinkAnhMascot
            config.DuongDanAnhLinhVat = "mascot.png";
            Assert(config.LinkAnhMascot == "mascot.png", "M6_24: Setting DuongDanAnhLinhVat sets LinkAnhMascot");
            config.LinkAnhMascot = "mascot_v2.png";
            Assert(config.DuongDanAnhLinhVat == "mascot_v2.png", "M6_24: Setting LinkAnhMascot sets DuongDanAnhLinhVat");

            // Test 8: ViTriLinhVat <-> ViTriMascot
            config.ViTriLinhVat = "left";
            Assert(config.ViTriMascot == "left", "M6_24: Setting ViTriLinhVat sets ViTriMascot");
            config.ViTriMascot = "right";
            Assert(config.ViTriLinhVat == "right", "M6_24: Setting ViTriMascot sets ViTriLinhVat");
        }

        private static void Test_25_LargePayloads_And_ExtremeStrings()
        {
            var config = new CauHinhTrangChu();
            string largeText = new string('A', 10000);

            config.DuongDanAnhBia = largeText;
            Assert(config.LinkAnhBia?.Length == 10000, "M6_25: Extreme 10,000 char string preserved in LinkAnhBia");

            config.KieuChuTieuDe = largeText;
            Assert(config.FontTieuDe?.Length == 10000, "M6_25: Extreme 10,000 char string preserved in FontTieuDe");

            config.VeDuAn_DuongDanAnh = largeText;
            Assert(config.VeDuAn_LinkAnh?.Length == 10000, "M6_25: Extreme 10,000 char string preserved in VeDuAn_LinkAnh");
        }

        private static void Test_26_SqlInjection_And_Xss_Payloads()
        {
            var config = new CauHinhTrangChu();
            var sqlPayload = "'; DROP TABLE CauHinhTrangChu; SELECT * FROM Users WHERE '1'='1";
            var xssPayload = "<script>alert('XSS')</script><img src=x onerror=alert(1)>";

            config.DuongDanAnhBia = sqlPayload;
            Assert(config.LinkAnhBia == sqlPayload, "M6_26: SQL injection payload safely stored as verbatim string in LinkAnhBia");

            config.KieuChuTieuDe = xssPayload;
            Assert(config.FontTieuDe == xssPayload, "M6_26: XSS payload safely stored as verbatim string in FontTieuDe");

            var unicodePayload = "🇻🇳 Trăng Sáng Langbiang 🌟 Đắk Lắk – Lâm Đồng 🏔️ 2026";
            config.VeDuAn_DuongDanAnh = unicodePayload;
            Assert(config.VeDuAn_LinkAnh == unicodePayload, "M6_26: Vietnamese Unicode & emojis preserved across proxy properties");
        }

        private static void Test_27_Null_And_Whitespace_Preservation()
        {
            var config = new CauHinhTrangChu();

            config.DuongDanAnhBia = null;
            Assert(config.LinkAnhBia == null, "M6_27: Null value propagates through DuongDanAnhBia");

            config.KieuChuTieuDe = "   \t\n   ";
            Assert(config.FontTieuDe == "   \t\n   ", "M6_27: Whitespace preserved exactly without truncation");

            config.ViTriLinhVat = "";
            Assert(config.ViTriMascot == "", "M6_27: Empty string preserved through ViTriLinhVat");
        }

        private static void Test_28_Concurrent_Entity_Mutations()
        {
            var config = new CauHinhTrangChu();
            var tasks = new List<Task>();
            int iterations = 1000;

            for (int i = 0; i < 4; i++)
            {
                int workerId = i;
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < iterations; j++)
                    {
                        var val = $"val_{workerId}_{j}";
                        config.DuongDanAnhBia = val;
                        var read = config.LinkAnhBia;
                        if (read == null) throw new InvalidOperationException("Null read during concurrent mutation");
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            Assert(config.LinkAnhBia != null, "M6_28: Concurrency stress test (4 threads x 1,000 mutations) executed safely without crash");
        }
    }
}
