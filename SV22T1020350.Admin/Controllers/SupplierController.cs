using Microsoft.AspNetCore.Mvc;
using SV22T1020350.Models.Common;
using SV22T1020350.BusinessLayers;
using SV22T1020350.Models.Partner;

namespace SV22T1020350.Admin.Controllers
{
    public class SupplierController : Controller
    {
        private const string SUPPLIER_SEARCH = "SupplierSearchInput";

        public IActionResult Index()
        {
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

        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await PartnerDataService.ListSuppliersAsync(input);
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH, input);
            return View(result);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Thêm nhà cung cấp";
            var data = new Supplier() { SupplierID = 0 };
            return View("Edit", data);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa nhà cung cấp";
            var data = await PartnerDataService.GetSupplierAsync(id);
            if (data == null) return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Save(Supplier data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.SupplierName))
                    ModelState.AddModelError(nameof(data.SupplierName), "Tên nhà cung cấp không được để trống");

                if (!ModelState.IsValid) return View("Edit", data);

                if (data.SupplierID == 0)
                    await PartnerDataService.AddSupplierAsync(data);
                else
                    await PartnerDataService.UpdateSupplierAsync(data);

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
                    bool result = await PartnerDataService.DeleteSupplierAsync(id);
                    if (!result)
                        throw new Exception("Dữ liệu đang được sử dụng, không thể xóa.");
                    return RedirectToAction("Index");
                }
                var data = await PartnerDataService.GetSupplierAsync(id);
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