using Microsoft.AspNetCore.Mvc;
using SV22T1020350.BusinessLayers;
using SV22T1020350.Models.Catalog;
using SV22T1020350.Models.Common;

namespace SV22T1020350.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private const string CATEGORY_SEARCH = "CategorySearchInput";

        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH);
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
            var result = await CatalogDataService.ListCategoriesAsync(input);
            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);
            return View(result);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Thêm loại hàng";
            var data = new Category() { CategoryID = 0 };
            return View("Edit", data);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa loại hàng";
            var data = await CatalogDataService.GetCategoryAsync(id);
            if (data == null) return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Save(Category data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.CategoryName))
                    ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng không được để trống");

                if (!ModelState.IsValid) return View("Edit", data);

                if (data.CategoryID == 0)
                    await CatalogDataService.AddCategoryAsync(data);
                else
                    await CatalogDataService.UpdateCategoryAsync(data);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("Edit", data);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (Request.Method == "POST")
                {
                    bool result = await CatalogDataService.DeleteCategoryAsync(id);
                    if (!result)
                        throw new Exception("Không thể xóa loại hàng này vì đang có sản phẩm thuộc loại hàng này.");
                    return RedirectToAction("Index");
                }
                var data = await CatalogDataService.GetCategoryAsync(id);
                if (data == null) return RedirectToAction("Index");
                return View(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}