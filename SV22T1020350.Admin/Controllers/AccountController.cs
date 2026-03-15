using Microsoft.AspNetCore.Mvc;

namespace SV22T1020350.Admin.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// đăng xuất khỏi hệ thống
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
        /// <summary>
        /// đổi mật khẩu cho tài khoản đang đăng nhập
        /// </summary>
        /// <returns></returns>
        public IActionResult ChangePassword()
        {
            return View();
        }
    }
}
