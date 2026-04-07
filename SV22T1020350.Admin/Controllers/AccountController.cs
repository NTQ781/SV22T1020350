using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020350.BusinessLayers;
using System.Security.Claims;

namespace SV22T1020350.Admin.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập Email và Mật khẩu!");
                return View();
            }

            string hashPassword = CryptHelper.HashMD5(password);
            var employee = await HRDataService.CheckLoginAsync(username, hashPassword);

            if (employee != null)
            {
                var userData = new WebUserData()
                {
                    UserId = employee.EmployeeID.ToString(),
                    UserName = employee.Email,
                    DisplayName = employee.FullName,
                    Email = employee.Email,
                    Photo = employee.Photo,
                    Roles = string.IsNullOrEmpty(employee.RoleNames)
                            ? new List<string>()
                            : employee.RoleNames.Split(',').Select(r => r.Trim()).ToList()
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("Error", "Sai tên đăng nhập hoặc mật khẩu!");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
                    throw new Exception("Vui lòng nhập đầy đủ mật khẩu cũ và mật khẩu mới.");

                if (newPassword != confirmPassword)
                    throw new Exception("Xác nhận mật khẩu mới không khớp!");

                // ĐÃ SỬA: Lấy ID từ UserData thay vì ClaimTypes.NameIdentifier để tránh bị NULL
                var userData = User.GetUserData();
                if (userData == null || string.IsNullOrEmpty(userData.UserId))
                    return RedirectToAction("Login");

                int userId = int.Parse(userData.UserId);

                // Lấy thông tin nhân viên từ Database
                var employee = await HRDataService.GetEmployeeAsync(userId);
                if (employee == null) return RedirectToAction("Login");

                // Kiểm tra mật khẩu cũ (Mã hóa MD5 rồi so sánh)
                string hashOldInput = CryptHelper.HashMD5(oldPassword);
                if (employee.Password.ToLower() != hashOldInput.ToLower())
                    throw new Exception("Mật khẩu hiện tại không chính xác!");

                // Cập nhật mật khẩu mới
                employee.Password = CryptHelper.HashMD5(newPassword);
                await HRDataService.UpdateEmployeeAsync(employee);

                ViewBag.Message = "<span class='text-success'><i class='bi bi-check-circle'></i> Đổi mật khẩu thành công!</span>";
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi thân thiện thay vì làm sập trang
                ViewBag.Message = $"<span class='text-danger'>{ex.Message}</span>";
            }
            return View();
        }
    }
}