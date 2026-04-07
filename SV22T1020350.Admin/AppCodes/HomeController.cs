using System.Security.Claims;

namespace SV22T1020350.Web // Thay bằng namespace của bạn
{
    public class WebUserData
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Photo { get; set; } = "";
        public List<string>? Roles { get; set; }

        /// <summary>
        /// Tạo "Giấy chứng nhận" (ClaimsPrincipal) từ dữ liệu người dùng để ghi vào Cookie
        /// </summary>
        public ClaimsPrincipal CreatePrincipal()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, UserId),
                new Claim(ClaimTypes.Name, UserName),
                new Claim("DisplayName", DisplayName),
                new Claim("Email", Email),
                new Claim("Photo", Photo)
            };

            if (Roles != null)
            {
                foreach (var role in Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            return new ClaimsPrincipal(identity);
        }
    }

    /// <summary>
    /// Hàm mở rộng (Extension Method) để đọc dữ liệu từ Cookie
    /// </summary>
    public static class WebUserExtensions
    {
        public static WebUserData? GetUserData(this ClaimsPrincipal principal)
        {
            if (principal == null || !principal.Identity!.IsAuthenticated) return null;

            var userData = new WebUserData
            {
                UserId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
                UserName = principal.FindFirstValue(ClaimTypes.Name) ?? "",
                DisplayName = principal.FindFirstValue("DisplayName") ?? "",
                Email = principal.FindFirstValue("Email") ?? "",
                Photo = principal.FindFirstValue("Photo") ?? "",
                Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
            };

            return userData;
        }
    }
}