using Microsoft.AspNetCore.Mvc;

namespace SV22T1020350.Admin.Controllers
{
    public class ShipperController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
