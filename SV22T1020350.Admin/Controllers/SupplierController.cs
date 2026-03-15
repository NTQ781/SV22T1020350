using Microsoft.AspNetCore.Mvc;

namespace SV22T1020350.Admin.Controllers
{
    public class SupplierController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Thêm nhà cung cấp";
            return View("Edit");
        }
        /// <summary>
        ///    cập nhật thông tin nhà cung cấp
        /// </summary>
        /// <param name="id">mã nhà cung cấp cần cập nhật</param>
        /// <returns></returns>
        public IActionResult Edit(int id)

        {
            ViewBag.Title = "Chỉnh sửa nhà cung cấp";
            return View();
        }

        /// <summary>
        /// xóa nhà cung cấp
        /// </summary>
        /// param name="id">mã nhà cung cấp cần xóa</param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            ViewBag.Title = "Xóa nhà cung cấp";
            return View();
        }
    }
}
