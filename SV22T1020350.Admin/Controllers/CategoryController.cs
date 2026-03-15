using Microsoft.AspNetCore.Mvc;

namespace SV22T1020350.Admin.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Category";
            return View();
        }

        /// <summary>
        /// thêm loại hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Thêm loại hàng";
            return View("Edit");
        }

        /// <summary>
        /// chỉnh sửa thông tin loại hàng
        /// </summary>
        /// <param name="id">mã loại hàng cần chỉnh sửa</param>
        /// <returns></returns>
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa loại hàng";
            return View();
        }

        /// <summary>
        /// xóa loại hàng
        /// </summary>
        /// <param name="id">mã loại hàng cần xóa</param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            ViewBag.Title = "Xóa loại hàng";
            return View();
        }
    }
}
