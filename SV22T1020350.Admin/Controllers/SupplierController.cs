using Microsoft.AspNetCore.Mvc;
using SV22T1020350.Models.Common;

namespace SV22T1020350.Admin.Controllers
{
    public class SupplierController : Controller
    {
        private const string SUPPLIER_SEARCH = "SupplierSearchInput";

        public IActionResult Index()
        {
            // Lấy điều kiện tìm kiếm từ Session, nếu chưa có thì tạo mới mặc định
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH);
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

        // Action này chỉ trả về HTML của bảng kết quả (không kèm Layout)
        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await PartnerDataService.ListSuppliersAsync(input);
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH, input); // Lưu vết tìm kiếm
            return View(result);
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
