using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020350.BusinessLayers;
using SV22T1020350.Models.Common;
using SV22T1020350.Models.HR;
using System.Text.Json;

namespace SV22T1020350.Admin.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private const string EMPLOYEE_SEARCH = "EmployeeSearchInput";

        private PaginationSearchInput? GetSearchCookie()
        {
            var data = Request.Cookies[EMPLOYEE_SEARCH];
            if (string.IsNullOrEmpty(data)) return null;
            try { return JsonSerializer.Deserialize<PaginationSearchInput>(data); }
            catch { return null; }
        }

        private void SetSearchCookie(PaginationSearchInput input)
        {
            var options = new CookieOptions { HttpOnly = true, Expires = DateTime.Now.AddDays(1) };
            Response.Cookies.Append(EMPLOYEE_SEARCH, JsonSerializer.Serialize(input), options);
        }

        public IActionResult Index()
        {
            var input = GetSearchCookie() ?? new PaginationSearchInput() { Page = 1, PageSize = ApplicationContext.PageSize, SearchValue = "" };
            return View(input);
        }

        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await HRDataService.ListEmployeesAsync(input);
            SetSearchCookie(input);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Employee data, IFormFile? uploadPhoto)
        {
            try
            {
                if (!ModelState.IsValid) return View("Edit", data);

                if (uploadPhoto != null && uploadPhoto.Length > 0)
                {
                    var fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "employees");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                    var filePath = Path.Combine(folderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create)) { await uploadPhoto.CopyToAsync(stream); }
                    data.Photo = fileName;
                }

                if (data.EmployeeID == 0)
                {
                    data.Password = CryptHelper.HashMD5("123456");
                    await HRDataService.AddEmployeeAsync(data);
                }
                else
                {
                    var old = await HRDataService.GetEmployeeAsync(data.EmployeeID);
                    if (old != null) { data.RoleNames = old.RoleNames; data.Password = old.Password; }
                    await HRDataService.UpdateEmployeeAsync(data);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; return View("Edit", data); }
        }

        // ==========================================================
        // ĐÃ BỔ SUNG: Hàm GET để hiển thị trang xác nhận xóa
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await HRDataService.GetEmployeeAsync(id);
            if (model == null) return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            try
            {
                bool result = await HRDataService.DeleteEmployeeAsync(id);
                if (!result) throw new Exception("Không thể xóa nhân viên này vì đang có dữ liệu liên quan (Đơn hàng).");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}