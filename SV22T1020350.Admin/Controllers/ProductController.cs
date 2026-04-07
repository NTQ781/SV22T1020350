using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020350.BusinessLayers;
using SV22T1020350.Models.Catalog;
using SV22T1020350.Models.Common;

namespace SV22T1020350.Admin.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private const string PRODUCT_SEARCH = "ProductSearchInput";

        public async Task<IActionResult> Index()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = ApplicationContext.PageSize,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0
                };
            }
            ViewBag.Categories = (await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput { PageSize = 0 })).DataItems;
            ViewBag.Suppliers = (await PartnerDataService.ListSuppliersAsync(new PaginationSearchInput { PageSize = 0 })).DataItems;
            return View(input);
        }

        public async Task<IActionResult> Search(ProductSearchInput input)
        {
            var result = await CatalogDataService.ListProductsAsync(input);
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return PartialView(result);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            return View("Edit", new Product { ProductID = 0, IsSelling = true });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật mặt hàng";
            var model = await CatalogDataService.GetProductAsync(id);
            if (model == null) return RedirectToAction("Index");
            ViewBag.Photos = await CatalogDataService.ListPhotosAsync(id);
            ViewBag.Attributes = await CatalogDataService.ListAttributesAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(Product data, IFormFile? uploadPhoto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.ProductName)) ModelState.AddModelError(nameof(data.ProductName), "Tên không được trống");
                if (!ModelState.IsValid) return View("Edit", data);

                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create)) { await uploadPhoto.CopyToAsync(stream); }
                    data.Photo = fileName;
                }

                if (data.ProductID == 0)
                {
                    int id = await CatalogDataService.AddProductAsync(data);
                    return RedirectToAction("Edit", new { id = id });
                }
                await CatalogDataService.UpdateProductAsync(data);
                return RedirectToAction("Index");
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; return View("Edit", data); }
        }

        // --- QUẢN LÝ ẢNH ---
        public IActionResult CreatePhotos(int id)
        {
            ViewBag.Title = "Thêm ảnh mặt hàng";
            return View("EditPhotos", new ProductPhoto { ProductID = id, PhotoID = 0, DisplayOrder = 1 });
        }

        public async Task<IActionResult> EditPhotos(int id, long photoId)
        {
            ViewBag.Title = "Cập nhật ảnh mặt hàng";
            var model = await CatalogDataService.GetPhotoAsync(photoId);
            if (model == null) return RedirectToAction("Edit", new { id });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { await uploadPhoto.CopyToAsync(stream); }
                data.Photo = fileName;
            }
            if (data.PhotoID == 0) await CatalogDataService.AddPhotoAsync(data);
            else await CatalogDataService.UpdatePhotoAsync(data);
            return RedirectToAction("Edit", new { id = data.ProductID });
        }

        [HttpGet]
        public async Task<IActionResult> DeletePhotos(int id, long photoId)
        {
            ViewBag.Title = "Xóa ảnh mặt hàng";
            var model = await CatalogDataService.GetPhotoAsync(photoId);
            if (model == null) return RedirectToAction("Edit", new { id });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhotosConfirm(int id, long photoId)
        {
            await CatalogDataService.DeletePhotoAsync(photoId);
            return RedirectToAction("Edit", new { id });
        }

        // --- QUẢN LÝ THUỘC TÍNH ---
        public IActionResult CreateAttributes(int id)
        {
            ViewBag.Title = "Thêm thuộc tính";
            return View("EditAttributes", new ProductAttribute { ProductID = id, AttributeID = 0, DisplayOrder = 1 });
        }

        public async Task<IActionResult> EditAttributes(int id, long attributeId)
        {
            ViewBag.Title = "Cập nhật thuộc tính";
            var model = await CatalogDataService.GetAttributeAsync(attributeId);
            if (model == null) return RedirectToAction("Edit", new { id });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAttribute(ProductAttribute data)
        {
            if (data.AttributeID == 0) await CatalogDataService.AddAttributeAsync(data);
            else await CatalogDataService.UpdateAttributeAsync(data);
            return RedirectToAction("Edit", new { id = data.ProductID });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAttributes(int id, long attributeId)
        {
            ViewBag.Title = "Xóa thuộc tính";
            var model = await CatalogDataService.GetAttributeAsync(attributeId);
            if (model == null) return RedirectToAction("Edit", new { id });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAttributesConfirm(int id, long attributeId)
        {
            await CatalogDataService.DeleteAttributeAsync(attributeId);
            return RedirectToAction("Edit", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            ViewBag.Title = "Xóa mặt hàng";
            var model = await CatalogDataService.GetProductAsync(id);
            if (model == null) return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            try { await CatalogDataService.DeleteProductAsync(id); return RedirectToAction("Index"); }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; return RedirectToAction("Delete", new { id }); }
        }
    }
}