using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace WEB_TinhNguyen.Areas.CuaHang.Helpers
{
    /// <summary>
    /// Tiện ích mở rộng ISession cho phép đọc/ghi đối tượng phức tạp (Giỏ hàng, Đơn hàng tạm) qua JSON.
    /// Sử dụng System.Text.Json chuẩn của ASP.NET Core (.NET 10.0).
    /// </summary>
    public static class SessionExtensions
    {
        private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = false
        };

        /// <summary>
        /// Tuần tự hóa đối tượng thành chuỗi JSON và lưu vào Session.
        /// Nếu value null, tự động xóa key tương ứng khỏi Session.
        /// </summary>
        public static void Set<T>(this ISession session, string key, T value)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            if (value == null)
            {
                session.Remove(key);
                return;
            }

            var json = JsonSerializer.Serialize(value, DefaultOptions);
            session.SetString(key, json);
        }

        /// <summary>
        /// Đọc và giải tuần tự đối tượng kiểu T từ Session.
        /// Trả về default(T) nếu key không tồn tại, chuỗi rỗng hoặc xảy ra lỗi parse JSON.
        /// </summary>
        public static T? Get<T>(this ISession session, string key)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (string.IsNullOrEmpty(key)) return default;

            var value = session.GetString(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return default;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(value, DefaultOptions);
            }
            catch
            {
                // Bảo vệ chống lỗi khi định dạng session bị thay đổi hoặc corrupt
                return default;
            }
        }

        /// <summary>
        /// Lấy đối tượng từ Session. Nếu chưa có thì khởi tạo mới qua factory function và lưu vào Session.
        /// </summary>
        public static T GetOrCreate<T>(this ISession session, string key, Func<T> factory) where T : class
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var item = session.Get<T>(key);
            if (item == null)
            {
                item = factory();
                session.Set(key, item);
            }
            return item;
        }

        /// <summary>
        /// Xóa an toàn một key khỏi Session.
        /// </summary>
        public static void RemoveKey(this ISession session, string key)
        {
            if (session != null && !string.IsNullOrEmpty(key))
            {
                session.Remove(key);
            }
        }
    }
}
