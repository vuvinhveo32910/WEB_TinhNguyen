using Microsoft.AspNetCore.Mvc;

namespace WEB_TinhNguyen.BoDieuKhien
{
    public class AdminController : Controller
    {
        // Khi truy cập /Admin, hệ thống sẽ tự động gọi hàm Index này
        public IActionResult Index()
        {
            return View();
        }
    }
}