using Microsoft.AspNetCore.Mvc;
using SV22T1020350.BusinessLayers;

namespace SV22T1020350.Shop.Controllers
{
    public class ProductController : Controller
    {
        public async Task<IActionResult> Detail(int id)
        {
            // 1. Lấy thông tin cơ bản của sản phẩm
            var product = await CatalogDataService.GetProductAsync(id);
            if (product == null) return RedirectToAction("Index", "Home");

            // 2. Lấy danh sách ảnh phụ (nếu có) để làm Carousel
            var photos = await CatalogDataService.ListPhotosAsync(id);
            ViewBag.Photos = photos;

            // 3. Lấy danh sách thuộc tính (Kích thước, màu sắc...)
            var attributes = await CatalogDataService.ListAttributesAsync(id);
            ViewBag.Attributes = attributes;

            return View(product);
        }
    }
}