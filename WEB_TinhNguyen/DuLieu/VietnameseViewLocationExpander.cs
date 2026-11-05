using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

namespace WEB_TinhNguyen.DuLieu
{
    /// <summary>
    /// Bộ mở rộng đường dẫn phân giải Razor View cho cấu trúc thư mục Việt hóa (GiaoDien).
    /// Hỗ trợ cả ứng dụng chính (/GiaoDien/) và phân hệ (/Areas/{Area}/Views/ & /Areas/{Area}/GiaoDien/).
    /// </summary>
    public class VietnameseViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            // Không cần giá trị động cho cache key
        }

        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            if (!string.IsNullOrEmpty(context.AreaName))
            {
                return new[]
                {
                    "/Areas/{2}/Views/{1}/{0}.cshtml",
                    "/Areas/{2}/Views/Shared/{0}.cshtml",
                    "/Areas/{2}/GiaoDien/{1}/{0}.cshtml",
                    "/Areas/{2}/GiaoDien/Shared/{0}.cshtml",
                    "/GiaoDien/Shared/{0}.cshtml",
                    "/Views/Shared/{0}.cshtml"
                };
            }

            return new[]
            {
                "/GiaoDien/{1}/{0}.cshtml",
                "/GiaoDien/Shared/{0}.cshtml",
                "/Views/{1}/{0}.cshtml",
                "/Views/Shared/{0}.cshtml"
            };
        }
    }
}
