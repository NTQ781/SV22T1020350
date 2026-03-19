using Microsoft.AspNetCore.Mvc;
using SV22T1020350.BusinessLayers;
using SV22T1020350.Models.Common;

namespace SV22T1020350.Admin.Controllers
{
    public class CustomerController : Controller
    {
        
        /// <summary>
        /// tên của biến dùng để lưu điều kiện tìm kiếm của khách hàng trong session
        /// </summary>
        private const string CUSTOMERSEARCHINPUT = "CustomerSearchInput";

        /// <summary>
        /// Nhập đầu vào tìm kiếm -> hiển thị kết quả tìm kiếm
        /// </summary>
        /// <returns></returns>
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
            return View( input);
        }

        public async Task<IActionResult> Search(PaginationSearchInput input)
        {    
            var result = await PartnerDataService.ListCustomersAsync(input);
            ApplicationContext.SetSessionData(CUSTOMERSEARCHINPUT, input);
            return View(result);
           
        }

        /// <summary>
        /// bổ sung khách hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Thêm khách hàng";
            return View();
        }
        /// <summary>
        /// cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="id">mã khách hàng cần cập nhật </param>
        /// <returns></returns>
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa khách hàng";
            return View();
        }

        /// <summary>
        /// xóa khách hàng
        /// </summary>
        /// <param name="id">mã khách hàng cần xóa</param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            ViewBag.Title = "Xóa khách hàng";
            return View();
        }

        /// <summary>
        /// đổi mật khẩu cho khách hàng
        /// </summary>
        /// <param name="id">mã khách hàng cần đổi mật khẩu</param>
        /// <returns></returns>
        public IActionResult ChangePassword(int id)
        {
            ViewBag.Title = "Đổi mật khẩu khách hàng";
            return View();
        }
    }
}
