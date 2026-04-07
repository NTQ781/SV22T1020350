using SV22T1020350.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SV22T1020350.BusinessLayers;
using SV22T1020350.Models.Common;
using SV22T1020350.Models.Partner;

namespace SV22T1020350.Admin.Controllers
{
    public class CustomerController : Controller
    {
        /// <summary>
        /// Tên biến lưu điều kiện tìm kiếm trong session
        /// </summary>
        private const string CUSTOMERSEARCHINPUT = "CustomerSearchInput";

        /// <summary>
        /// Hiển thị trang danh sách khách hàng
        /// </summary>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMERSEARCHINPUT);
            if (input == null)
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = ApplicationContext.PageSize,
                    SearchValue = ""
                };

            return View(input);
        }

        /// <summary>
        /// Tìm kiếm khách hàng
        /// </summary>
        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await PartnerDataService.ListCustomersAsync(input);
            ApplicationContext.SetSessionData(CUSTOMERSEARCHINPUT, input);
            return View(result);
        }

        /// <summary>
        /// Thêm mới khách hàng
        /// </summary>
        public async Task<IActionResult> Create()
        {
            ViewBag.Title = "Thêm khách hàng";

            ViewBag.Provinces = new SelectList(await SelectListHelper.Provinces());

            return View("Edit", new Customer { CustomerID = 0 });
        }


        /// <summary>
        /// Chỉnh sửa khách hàng
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa khách hàng";

            var model = await PartnerDataService.GetCustomerAsync(id);


            if (model == null)
                return RedirectToAction("Index");
            ViewBag.Provinces = new SelectList(await SelectListHelper.Provinces());

            return View(model);
        }

        /// <summary>
        /// Lưu dữ liệu (Thêm / Sửa)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveData(Customer data)
        {
            try
            {
                ViewBag.Title = data.CustomerID == 0 ? "Thêm khách hàng" : "Chỉnh sửa khách hàng";

                //TODO: kiem tra du lieu hop le
                //Su dung ModelState de luu cac tinh huong loi va thong bao loi cho nguoi dung(tren view) 
                //Gia dinh: chi yeu cau nhap ten, email, tinh/thanh
                if (string.IsNullOrWhiteSpace(data.CustomerName))
                    ModelState.AddModelError(nameof(data.CustomerName), "Vui lòng nhập tên khách hàng");

                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email");
                else if (await PartnerDataService.ValidatelCustomerEmailAsync(data.Email, data.CustomerID))
                    ModelState.AddModelError(nameof(data.Email), "Email đã được sử dụng");

                if (string.IsNullOrWhiteSpace(data.Province))
                    ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh/thành");

                // ❗ Nếu có lỗi → quay lại view
                if (!ModelState.IsValid)
                {
                    ViewBag.Provinces = new SelectList(await SelectListHelper.Provinces());
                    return View("Edit", data);
                }

                //(tuy chon) hieu chinh du lieu theo quy dinh cua he thong
                if (string.IsNullOrWhiteSpace(data.ContactName))
                    data.ContactName = data.CustomerName;

                data.Phone ??= "";
                data.Address ??= "";

                //Luu du lieu vao CSDL
                if (data.CustomerID == 0)
                    await PartnerDataService.AddCustomerAsync(data);
                else
                    await PartnerDataService.UpdateCustomerAsync(data);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                //ghi log loi dua vao tong tin trong Exception (ex.Message, ex.StackTrace, ex.InnerException, ...)
                ModelState.AddModelError("Error", "Hệ thống đang bận. Vui lòng thử lại.");

                ViewBag.Provinces = new SelectList(await SelectListHelper.Provinces());
                return View("Edit", data);
            }
        }

        /// <summary>
        /// Xóa khách hàng
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            //new mothod la POST thi xoa
            if (Request.Method=="POST")
                { await PartnerDataService.DeleteCustomerAsync(id);
                return RedirectToAction("Index");
            }
            var model = await PartnerDataService.GetCustomerAsync(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.CanDelete = !await PartnerDataService.IsUsedCustomerAsync(id);

            return View(model);
        }

        /// <summary>
        /// Đổi mật khẩu khách hàng
        /// </summary>
        public IActionResult ChangePassword(int id)
        {
            ViewBag.Title = "Đổi mật khẩu khách hàng";
            return View();
        }
    }
}