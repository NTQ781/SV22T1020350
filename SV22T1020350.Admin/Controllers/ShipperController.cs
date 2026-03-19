using Microsoft.AspNetCore.Mvc;
using SV22T1020350.Models.Common;

namespace SV22T1020350.Admin.Controllers
{
    public class ShipperController : Controller
    {
        private const string SHIPPER_SEARCH = "ShipperSearchInput";

        public IActionResult Index()
        {
            // Lấy lại điều kiện tìm kiếm từ session (nếu có)
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH);
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
            var result = await PartnerDataService.ListShippersAsync(input);
            // Lưu lại điều kiện tìm kiếm hiện tại vào session
            ApplicationContext.SetSessionData(SHIPPER_SEARCH, input);
            return View(result); // Trả về View "Search.cshtml"
        }
        /// <summary>
        ///  thêm hoặc cập nhật thông tin Shipper
        /// </summary>
        /// <param name="id">mã shipper cần thêm hoặc cập nhật</param>
        /// <returns></returns>
        public IActionResult Create() 
        {
            ViewBag.Title = "Thêm Shipper";
            return View("Edit");
        }

        /// <summary>
        /// chỉnh sửa thông tin Shipper
        /// </summary>
        /// <param name="id">mã shipper cần chỉnh sửa</param>
        /// <returns></returns>
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa Shipper";
            return View();
        }

        /// <summary>
        /// xóa thônng tin Shipper
        /// </summary>
        /// <param name="id">thông tin shipper cần xóa</param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            ViewBag.Title = "Xóa thông tin Shipper";
            return View();
        }
    }
}
