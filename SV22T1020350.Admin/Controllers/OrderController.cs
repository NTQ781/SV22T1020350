using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020350.Admin;
using SV22T1020350.Admin.Models;
using SV22T1020350.BusinessLayers;
using SV22T1020350.Models.Catalog;
using SV22T1020350.Models.Common;
using SV22T1020350.Models.Partner;
using SV22T1020350.Models.Sales;
using System.Globalization;

namespace SV22T1020350.Admin.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const string ORDER_SEARCH = "OrderSearchInput";

        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH) ?? new OrderSearchInput { Page = 1, PageSize = ApplicationContext.PageSize, SearchValue = "", Status = 0 };
            return View(input);
        }

        public async Task<IActionResult> Search(OrderSearchInput input, string dateFrom = "", string dateTo = "")
        {
            if (DateTime.TryParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dFrom)) input.DateFrom = dFrom;
            if (DateTime.TryParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dTo)) input.DateTo = dTo;
            var result = await SalesDataService.ListOrdersAsync(input);
            ApplicationContext.SetSessionData(ORDER_SEARCH, input);
            return PartialView(result);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            return View(ShoppingCartService.GetShoppingCart());
        }

        public async Task<IActionResult> SearchProduct(string searchValue = "")
        {
            var result = await CatalogDataService.ListProductsAsync(new ProductSearchInput { Page = 1, PageSize = 20, SearchValue = searchValue ?? "" });
            return PartialView("_SearchProduct", result.DataItems);
        }

        public IActionResult GetShoppingCart() => PartialView("_ShoppingCart", ShoppingCartService.GetShoppingCart());

        [HttpPost]
        public IActionResult AddToCart(OrderDetailViewInfo item)
        {
            if (item.Quantity <= 0) return Json(new { Code = 0, Message = "Số lượng không hợp lệ" });
            ShoppingCartService.AddCartItem(item);
            return Json(new { Code = 1 });
        }

        [HttpPost]
        public async Task<IActionResult> Init(string customerName, string customerPhone, string customerEmail, string deliveryProvince, string deliveryAddress)
        {
            try
            {
                var cart = ShoppingCartService.GetShoppingCart();
                if (cart.Count == 0) return Json(new { Code = 0, Message = "Giỏ hàng trống!" });
                int customerId = 0;
                var customerSearch = await PartnerDataService.ListCustomersAsync(new PaginationSearchInput { Page = 1, PageSize = 1, SearchValue = customerPhone });
                if (customerSearch.RowCount > 0) customerId = customerSearch.DataItems[0].CustomerID;
                else customerId = await PartnerDataService.AddCustomerAsync(new Customer { CustomerName = customerName, ContactName = customerName, Phone = customerPhone, Email = customerEmail ?? "", Province = deliveryProvince, Address = deliveryAddress });

                int orderId = await SalesDataService.AddOrderAsync(new Order { CustomerID = customerId, DeliveryProvince = deliveryProvince, DeliveryAddress = deliveryAddress, EmployeeID = int.Parse(User.GetUserData()!.UserId), OrderTime = DateTime.Now, Status = OrderStatusEnum.New });
                foreach (var item in cart) await SalesDataService.AddDetailAsync(new OrderDetail { OrderID = orderId, ProductID = item.ProductID, Quantity = item.Quantity, SalePrice = item.SalePrice });

                ShoppingCartService.ClearCart();
                return Json(new { Code = 1, OrderID = orderId });
            }
            catch (Exception ex) { return Json(new { Code = 0, Message = ex.Message }); }
        }

        public async Task<IActionResult> Detail(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");
            return View(new OrderDetailModel { Order = order, Details = await SalesDataService.ListDetailsAsync(id) });
        }

        // --- GET MODALS ---
        [HttpGet] public IActionResult Accept(int id) { ViewBag.OrderId = id; return PartialView(); }
        [HttpGet] public IActionResult Cancel(int id) { ViewBag.OrderId = id; return PartialView(); }
        [HttpGet] public IActionResult Reject(int id) { ViewBag.OrderId = id; return PartialView(); }
        [HttpGet] public IActionResult Finish(int id) { ViewBag.OrderId = id; return PartialView(); }
        [HttpGet] public IActionResult Delete(int id) { ViewBag.OrderId = id; return PartialView(); }
        [HttpGet] public IActionResult ClearCart() { return PartialView(); }
        [HttpGet]
        public async Task<IActionResult> Shipping(int id)
        {
            ViewBag.OrderId = id;
            ViewBag.Shippers = (await PartnerDataService.ListShippersAsync(new PaginationSearchInput { PageSize = 0 })).DataItems;
            return PartialView();
        }

        // --- POST ACTIONS (Sửa lỗi CS0111) ---
        [HttpPost][ActionName("Accept")] public async Task<IActionResult> AcceptConfirmed(int id) { await SalesDataService.AcceptOrderAsync(id, int.Parse(User.GetUserData()!.UserId)); return Json(new { Code = 1, OrderID = id }); }
        [HttpPost][ActionName("Cancel")] public async Task<IActionResult> CancelConfirmed(int id) { await SalesDataService.CancelOrderAsync(id); return Json(new { Code = 1, OrderID = id }); }
        [HttpPost][ActionName("Reject")] public async Task<IActionResult> RejectConfirmed(int id) { await SalesDataService.RejectOrderAsync(id, int.Parse(User.GetUserData()!.UserId)); return Json(new { Code = 1, OrderID = id }); }
        [HttpPost][ActionName("Finish")] public async Task<IActionResult> FinishConfirmed(int id) { await SalesDataService.CompleteOrderAsync(id); return Json(new { Code = 1, OrderID = id }); }
        [HttpPost][ActionName("Shipping")] public async Task<IActionResult> ShippingConfirmed(int id, int shipperID) { await SalesDataService.ShipOrderAsync(id, shipperID); return Json(new { Code = 1, OrderID = id }); }
        [HttpPost][ActionName("Delete")] public async Task<IActionResult> DeleteConfirmed(int id) { await SalesDataService.DeleteOrderAsync(id); return Json(new { Code = 1 }); }
        [HttpPost][ActionName("ClearCart")] public IActionResult ClearCartConfirmed() { ShoppingCartService.ClearCart(); return Json(new { Code = 1 }); }

        // --- SỬA/XÓA MÓN HÀNG ---
        [HttpGet]
        public async Task<IActionResult> EditCartItem(int id, int productId)
        {
            ViewBag.OrderId = id;
            if (id == 0) return PartialView(ShoppingCartService.GetCartItem(productId));
            return PartialView(await SalesDataService.GetDetailAsync(id, productId));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(int orderID, int productID, int quantity, decimal salePrice)
        {
            if (quantity <= 0) return Json(new { Code = 0, Message = "Số lượng không hợp lệ" });
            if (orderID == 0)
            {
                ShoppingCartService.UpdateCartItem(productID, quantity, salePrice);
                return Json(new { Code = 1 });
            }
            await SalesDataService.UpdateDetailAsync(new OrderDetail { OrderID = orderID, ProductID = productID, Quantity = quantity, SalePrice = salePrice });
            return Json(new { Code = 1, OrderID = orderID });
        }

        [HttpGet] public IActionResult DeleteCartItem(int id, int productId) { ViewBag.OrderId = id; ViewBag.ProductId = productId; return PartialView(); }

        [HttpPost]
        public async Task<IActionResult> DeleteCartItem(int orderID, int productID, string _unused = "")
        {
            if (orderID == 0) { ShoppingCartService.RemoveCartItem(productID); return Json(new { Code = 1 }); }
            await SalesDataService.DeleteDetailAsync(orderID, productID);
            return Json(new { Code = 1, OrderID = orderID });
        }

        // --- SỬA THÔNG TIN ĐƠN HÀNG ---
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            ViewBag.Customers = (await PartnerDataService.ListCustomersAsync(new PaginationSearchInput { PageSize = 0 })).DataItems;
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Order data)
        {
            try { await SalesDataService.UpdateOrderAsync(data); return RedirectToAction("Detail", new { id = data.OrderID }); }
            catch (Exception ex)
            {
                ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
                ViewBag.Customers = (await PartnerDataService.ListCustomersAsync(new PaginationSearchInput { PageSize = 0 })).DataItems;
                TempData["ErrorMessage"] = ex.Message; return View(data);
            }
        }
    }
}