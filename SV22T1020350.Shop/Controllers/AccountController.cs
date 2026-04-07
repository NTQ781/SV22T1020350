using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020350.BusinessLayers;
using SV22T1020350.Models.Partner;
using System.Security.Claims;

namespace SV22T1020350.Shop.Controllers
{
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
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập Email và Mật khẩu");
                return View();
            }

            string hashPassword = CryptHelper.HashMD5(password);
            var customer = PartnerDataService.CheckCustomerLogin(email, hashPassword);

            if (customer == null)
            {
                ModelState.AddModelError("", "Đăng nhập thất bại. Email hoặc mật khẩu không đúng!");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, customer.CustomerName),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim("CustomerID", customer.CustomerID.ToString())
            };

            var identity = new ClaimsIdentity(claims, "CustomerCookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CustomerCookie", principal);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(Customer data, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.Email) || string.IsNullOrWhiteSpace(data.Password))
                    ModelState.AddModelError("", "Email và Mật khẩu không được để trống!");
                else if (data.Password != confirmPassword)
                    ModelState.AddModelError("", "Mật khẩu xác nhận không khớp!");

                if (!string.IsNullOrWhiteSpace(data.Email))
                {
                    bool isValidEmail = await PartnerDataService.ValidatelCustomerEmailAsync(data.Email, 0);
                    if (!isValidEmail) ModelState.AddModelError("", $"Email '{data.Email}' đã được sử dụng.");
                }

                if (!ModelState.IsValid) return View(data);

                data.ContactName = data.CustomerName;
                data.IsLocked = false;
                data.Password = CryptHelper.HashMD5(data.Password);

                int id = await PartnerDataService.AddCustomerAsync(data);
                if (id > 0)
                {
                    // Đăng ký xong gọi hàm Login bên trên để đăng nhập ngay lập tức
                    return await Login(data.Email, confirmPassword);
                }
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo tài khoản.");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View(data);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var customerId = User.FindFirstValue("CustomerID");
            if (string.IsNullOrEmpty(customerId)) return RedirectToAction("Login");
            var customer = await PartnerDataService.GetCustomerAsync(int.Parse(customerId));
            return View(customer);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(Customer data)
        {
            try
            {
                // Lấy ID từ User đang đăng nhập để bảo mật
                var customerIdClaim = User.FindFirstValue("CustomerID");
                if (string.IsNullOrEmpty(customerIdClaim) || data.CustomerID.ToString() != customerIdClaim)
                    return BadRequest();

                var existing = await PartnerDataService.GetCustomerAsync(data.CustomerID);
                if (existing == null) throw new Exception("Khách hàng không tồn tại.");

                // Giữ lại các thông tin không cho phép sửa từ giao diện
                data.CustomerName = existing.CustomerName;
                data.Email = existing.Email;
                data.Password = existing.Password;
                data.IsLocked = existing.IsLocked;

                bool result = await PartnerDataService.UpdateCustomerAsync(data);
                if (result)
                {
                    TempData["SuccessMessage"] = "Cập nhật thông tin cá nhân thành công!";
                }
                else
                {
                    throw new Exception("Không thể cập nhật dữ liệu vào cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Profile"); // Redirect để tránh việc F5 nạp lại form
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            try
            {
                if (newPassword != confirmPassword) throw new Exception("Mật khẩu xác nhận không khớp.");
                var email = User.FindFirstValue(ClaimTypes.Email);
                var check = PartnerDataService.CheckCustomerLogin(email ?? "", CryptHelper.HashMD5(oldPassword));
                if (check == null) throw new Exception("Mật khẩu hiện tại không chính xác.");

                PartnerDataService.ChangeCustomerPassword(check.CustomerID, CryptHelper.HashMD5(newPassword));
                ViewBag.Message = "Đổi mật khẩu thành công!";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CustomerCookie");
            return RedirectToAction("Index", "Home");
        }
    }
}