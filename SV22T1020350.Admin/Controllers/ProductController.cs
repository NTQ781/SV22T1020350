using Microsoft.AspNetCore.Mvc;

namespace LiteCommerce.Web.Controllers

{
    /// <summary>
    /// các dữ liệu của mặt hàng
    /// </summary>

    public class ProductController : Controller

    {
        public IActionResult Index()

        {
            ViewBag.Title = "Danh sách mặt hàng";

            return View();
        }

        /// <summary>
        /// chi tiết mặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public IActionResult Detail(int id)

        {

            ViewBag.Title = "Chi tiết mặt hàng";

            ViewBag.ProductId = id;

            return View();

        }

        /// <summary>
        /// Bổ sung mặt hàng
        /// </summary>
        /// <returns></returns>

        public IActionResult Create()

        {
            ViewBag.Title = "Bổ sung mặt hàng";
            return View("Edit");

        }

        /// <summary>
        /// Cập nhật mặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id)

        { 
            ViewBag.Title = "Cập nhật mặt hàng";

            ViewBag.ProductId = id;

            return View();

        }

        /// <summary>
        /// Xóa mặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id)

        {
            ViewBag.Title = "Xóa mặt hàng";
            ViewBag.ProductId = id;
            return View();

        }

        /// <summary>
        /// Danh sách thuộc tính
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ListAttributes(int id)

        {
            ViewBag.Title = "Danh sách thuộc tính";
            ViewBag.ProductId = id;
            return View();

        }

        /// <summary>
        /// Thêm thuộc tính
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public IActionResult CreateAttributes(int id)

        {
            ViewBag.Title = "Thêm thuộc tính";

            ViewBag.ProductId = id;

            return View("EditAttributes");

        }

        /// <summary>
        /// Cập nhật thuộc tính
        /// </summary>
        /// <param name="id"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>

        public IActionResult EditAttributes(int id, int attributeId)

        {

            ViewBag.Title = "Cập nhật thuộc tính";
            ViewBag.ProductId = id;
            ViewBag.AttributeId = attributeId;
            return View();

        }
        /// <summary>
        /// Xóa thuộc tính
        /// </summary>

        /// <param name="id"></param>

        /// <param name="attributeId"></param>

        /// <returns></returns>

        public IActionResult DeleteAttributes(int id, int attributeId)

        {

            ViewBag.Title = "Xóa thuộc tính";

            ViewBag.ProductId = id;

            ViewBag.AttributeId = attributeId;

            return View();

        }





        /// <summary>

        /// Danh sách hình ảnh

        /// </summary>

        /// <param name="id"></param>

        /// <returns></returns>

        public IActionResult ListPhotos(int id)

        {
            ViewBag.Title = "Danh sách hình ảnh";
            ViewBag.ProductId = id;
            return View();

        }

        /// <summary>
        /// Thêm hình ảnh
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult CreatePhotos(int id)

        {
            ViewBag.Title = "Thêm hình ảnh";
            ViewBag.ProductId = id;
            return View("EditPhotos");

        }

        /// <summary>
        /// Cập nhật hình ảnh
        /// </summary>
        /// <param name="id"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        public IActionResult EditPhotos(int id, int photoId)

        {

            ViewBag.Title = "Cập nhật hình ảnh";

            ViewBag.ProductId = id;

            ViewBag.PhotoId = photoId;

            return View();

        }



        /// <summary>

        /// Xóa hình ảnh

        /// </summary>

        /// <param name="id"></param>

        /// <param name="photoId"></param>

        /// <returns></returns>

        public IActionResult DeletePhotos(int id, int photoId)

        {

            ViewBag.Title = "Xóa hình ảnh";

            ViewBag.ProductId = id;

            ViewBag.PhotoId = photoId;

            return View();

        }

    }

}