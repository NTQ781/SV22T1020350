using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020350.BusinessLayers;
using SV22T1020350.Models.Common;
using SV22T1020350.Models.Sales;
using SV22T1020350.Shop.Models;
using System.Security.Claims;
using System.Text.Json;

namespace SV22T1020350.Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const string CART_KEY = "MyShoppingCart";

        // ======================= XÁC NHẬN ĐẶT HÀNG (GET) =======================
        public async Task<IActionResult> Checkout(int[] selectedIds)
        {
            if (selectedIds == null || selectedIds.Length == 0)
                return RedirectToAction("Index", "Cart");

            var cart = GetCart();
            // Lấy các mặt hàng mà người dùng đã tích chọn trong giỏ
            var checkoutItems = cart.Where(i => selectedIds.Contains(i.ProductID)).ToList();

            if (checkoutItems.Count == 0)
                return RedirectToAction("Index", "Cart");

            // Lấy thông tin khách hàng đang đăng nhập
            var customerId = User.FindFirstValue("CustomerID");
            var customer = await PartnerDataService.GetCustomerAsync(int.Parse(customerId!));

            ViewBag.CheckoutItems = checkoutItems;
            ViewBag.Customer = customer;

            return View();
        }

        // ======================= THỰC HIỆN ĐẶT HÀNG (POST) =======================
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string deliveryProvince, string deliveryAddress, string notes, string paymentMethod, int[] productIds)
        {
            try
            {
                var customerId = User.FindFirstValue("CustomerID");
                var cart = GetCart();
                var orderItems = cart.Where(i => productIds.Contains(i.ProductID)).ToList();

                if (orderItems.Count == 0) throw new Exception("Vui lòng chọn ít nhất một mặt hàng.");

                // Gộp địa chỉ và ghi chú thanh toán vào địa chỉ giao hàng (Theo nghiệp vụ máy Quân)
                string finalAddress = deliveryAddress + (string.IsNullOrWhiteSpace(notes) ? "" : $" | Ghi chú: {notes}") + $" | Thanh toán: {paymentMethod}";

                // 1. Tạo đơn hàng mới
                int orderID = await SalesDataService.AddOrderAsync(new Order
                {
                    CustomerID = int.Parse(customerId!),
                    DeliveryProvince = deliveryProvince,
                    DeliveryAddress = finalAddress,
                    EmployeeID = null, // Shop đặt hàng thì chưa có nhân viên duyệt
                    OrderTime = DateTime.Now,
                    Status = OrderStatusEnum.New
                });

                // 2. Thêm chi tiết đơn hàng
                foreach (var item in orderItems)
                {
                    await SalesDataService.AddDetailAsync(new OrderDetail
                    {
                        OrderID = orderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        SalePrice = item.Price
                    });
                }

                // 3. Xóa các mặt hàng đã mua khỏi giỏ hàng
                cart.RemoveAll(i => productIds.Contains(i.ProductID));
                SaveCart(cart);

                return RedirectToAction("Success", new { id = orderID });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Checkout", new { selectedIds = productIds });
            }
        }

        // ======================= LỊCH SỬ MUA HÀNG =======================
        public async Task<IActionResult> History()
        {
            var customerId = int.Parse(User.FindFirstValue("CustomerID")!);

            // Lấy tất cả đơn hàng (Sử dụng hàm ListOrdersAsync có sẵn)
            var input = new OrderSearchInput { Page = 1, PageSize = 1000, SearchValue = "" };
            var result = await SalesDataService.ListOrdersAsync(input);

            // Lọc lại các đơn hàng của riêng khách hàng này
            var myOrders = result.DataItems.Where(o => o.CustomerID == customerId).ToList();

            return View(myOrders);
        }

        // ======================= CHI TIẾT ĐƠN HÀNG =======================
        public async Task<IActionResult> Details(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("History");

            // Bảo mật: Chỉ cho xem đơn hàng của chính mình
            var customerId = int.Parse(User.FindFirstValue("CustomerID")!);
            if (order.CustomerID != customerId) return Forbid();

            var details = await SalesDataService.ListDetailsAsync(id);
            ViewBag.Details = details;

            return View(order);
        }

        // ======================= CÁC HÀM PHỤ TRỢ =======================
        public IActionResult Success(int id)
        {
            ViewBag.OrderID = id;
            return View();
        }

        private List<CartItem> GetCart()
        {
            var data = HttpContext.Session.GetString(CART_KEY);
            return string.IsNullOrEmpty(data) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(data)!;
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CART_KEY, JsonSerializer.Serialize(cart));
        }
    }
}