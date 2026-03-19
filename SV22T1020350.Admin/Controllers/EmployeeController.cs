using Microsoft.AspNetCore.Mvc;
using SV22T1020350.BusinessLayers;
using SV22T1020350.Models.Common;

namespace SV22T1020350.Admin.Controllers
{
    public class EmployeeController : Controller
    {
        private const string EMPLOYEE_SEARCH = "EmployeeSearchInput";

        public IActionResult Index()
        {
            // Lấy lại điều kiện tìm kiếm từ session (nếu có)
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = ApplicationContext.PageSize,
                    SearchValue = ""
                };
            }
            return View(input);
        }

        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await HRDataService.ListEmployeesAsync(input);
            // Lưu lại điều kiện tìm kiếm hiện tại vào session
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);
            return View(result); // Trả về View "Search.cshtml"
        }

        /// <summary>
        /// thêm nhân viên mới
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Thêm nhân viên";
            return View("Edit");
        }
        /// <summary>
        /// chỉnh sửa thông tin nhân viên
        /// </summary>
        /// <param name="id">mã nhân viên cần cỉnh sửa</param>
        /// <returns></returns>
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa nhân viên";
            return View();
        }
        /// <summary>
        /// xóa nhân viên
        /// </summary>
        /// <param name="id">mã nhân viên cần xóa</param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            ViewBag.Title = "Xóa nhân viên";
            return View();
        }
        /// <summary>
        /// đổi mật khẩu cho nhân viên
        /// </summary>
        /// <param name="id">mã nhân viên cần đổi mật khẩu</param>
        /// <returns></returns>
        public IActionResult ChangePassword(int id)
        {
            ViewBag.Title = "Đổi mật khẩu";
            return View();
        }

        /// <summary>
        /// phân quyền cho nhân viên
        /// </summary>
        /// /// <param name="id">mã nhân viên phân quyền</param>
        /// <returns></returns>
        public IActionResult ChangeRole(int id)
        {
            ViewBag.Title = "Phân quyền nhân viên";
            return View();
        }
    }
}
