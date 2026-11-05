using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace WEB_TinhNguyen.Tests.E2E.Framework
{
    public class E2ETestContext
    {
        public static string SolutionRoot { get; } = FindSolutionRoot();
        public static string ProjectRoot { get; } = Path.Combine(SolutionRoot, "WEB_TinhNguyen");
        public static string AreasRoot { get; } = Path.Combine(ProjectRoot, "Areas", "CuaHang");
        public static string ViewsRoot { get; } = Directory.Exists(Path.Combine(ProjectRoot, "GiaoDien"))
            ? Path.Combine(ProjectRoot, "GiaoDien")
            : Path.Combine(ProjectRoot, "Views");

        private static string FindSolutionRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "WEB_TinhNguyen.slnx")))
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        }

        private static Assembly? _webAssembly;
        public static Assembly WebAssembly
        {
            get
            {
                if (_webAssembly == null)
                {
                    _webAssembly = typeof(WEB_TinhNguyen.BoDieuKhien.HomeController).Assembly;
                }
                return _webAssembly;
            }
        }

        public static Type? FindType(string fullTypeName)
        {
            return WebAssembly.GetType(fullTypeName);
        }

        public static bool ViewFileExists(string relativePath)
        {
            var fullPath = Path.Combine(ProjectRoot, relativePath);
            return File.Exists(fullPath);
        }

        public static string ReadViewContent(string relativePath)
        {
            var fullPath = Path.Combine(ProjectRoot, relativePath);
            if (!File.Exists(fullPath))
            {
                return string.Empty;
            }
            return File.ReadAllText(fullPath);
        }

        public static ISession CreateMockSession()
        {
            return new MockSession();
        }
    }

    public class MockSession : ISession
    {
        private readonly System.Collections.Generic.Dictionary<string, byte[]> _storage 
            = new System.Collections.Generic.Dictionary<string, byte[]>();

        public bool IsAvailable => true;
        public string Id { get; } = Guid.NewGuid().ToString();
        public System.Collections.Generic.IEnumerable<string> Keys => _storage.Keys;

        public void Clear() => _storage.Clear();

        public System.Threading.Tasks.Task CommitAsync(System.Threading.CancellationToken cancellationToken = default) 
            => System.Threading.Tasks.Task.CompletedTask;

        public System.Threading.Tasks.Task LoadAsync(System.Threading.CancellationToken cancellationToken = default) 
            => System.Threading.Tasks.Task.CompletedTask;

        public void Remove(string key) => _storage.Remove(key);

        public void Set(string key, byte[] value) => _storage[key] = value;

        public bool TryGetValue(string key, out byte[]? value)
        {
            if (_storage.TryGetValue(key, out var val))
            {
                value = val;
                return true;
            }
            value = null;
            return false;
        }
    }
}
