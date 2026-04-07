using Microsoft.AspNetCore.Mvc;
using SV22T1020350.BusinessLayers;
using SV22T1020350.Models.Catalog;
using SV22T1020350.Models.Common;

namespace SV22T1020350.Shop.Controllers
{
    public class HomeController : Controller
    {
        private const int PAGE_SIZE = 12;

        // ĐÃ BỔ SUNG: Tham số minPrice và maxPrice
        public async Task<IActionResult> Index(int page = 1, string searchValue = "", int categoryId = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            var input = new ProductSearchInput()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                CategoryID = categoryId,
                MinPrice = minPrice, // Truyền giá trị lọc min
                MaxPrice = maxPrice  // Truyền giá trị lọc max
            };

            var result = await CatalogDataService.ListProductsAsync(input);

            // Lấy danh mục cho Sidebar
            var categories = await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput { Page = 1, PageSize = 0, SearchValue = "" });
            ViewBag.Categories = categories.DataItems;

            ViewBag.SearchValue = searchValue;
            ViewBag.CategoryID = categoryId;

            // ĐÃ BỔ SUNG: Truyền khoảng giá ra View để hiển thị lại trên Form
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(result);
        }
    }
}