using Microsoft.AspNetCore.Mvc;
using SV22T1020350.BusinessLayers;
using SV22T1020350.Shop.Models;
using System.Text.Json;

namespace SV22T1020350.Shop.Controllers
{
    public class CartController : Controller
    {
        private const string CART_KEY = "MyShoppingCart";

        public IActionResult Index() => View(GetCart());

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var product = await CatalogDataService.GetProductAsync(id);
            if (product == null) return Json(new { success = false, msg = "Sản phẩm không tồn tại!" });

            var cart = GetCart();
            var item = cart.FirstOrDefault(m => m.ProductID == id);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    Photo = string.IsNullOrEmpty(product.Photo) ? "demo.png" : product.Photo,
                    Unit = product.Unit,
                    Price = product.Price,
                    Quantity = quantity
                });
            }
            else
            {
                item.Quantity += quantity;
            }

            SaveCart(cart);
            return Json(new { success = true, msg = $"Đã thêm {product.ProductName} vào giỏ!", count = cart.Sum(i => i.Quantity) });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(m => m.ProductID == id);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                SaveCart(cart);
                return Json(new { success = true, itemTotal = item.TotalPrice.ToString("N0"), cartCount = cart.Sum(i => i.Quantity) });
            }
            return Json(new { success = false });
        }

        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            cart.RemoveAll(m => m.ProductID == id);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // GET: Hiển thị Modal xác nhận xóa sạch giỏ hàng
        [HttpGet]
        public IActionResult Clear()
        {
            return PartialView();
        }

        // POST: Thực hiện xóa sạch qua AJAX
        [HttpPost]
        [ActionName("Clear")]
        public IActionResult ClearConfirmed()
        {
            HttpContext.Session.Remove(CART_KEY);
            return Json(new { success = true, code = 1 });
        }

        public IActionResult GetCartCount() => Json(new { count = GetCart().Sum(i => i.Quantity) });

        private List<CartItem> GetCart()
        {
            var sessionData = HttpContext.Session.GetString(CART_KEY);
            return string.IsNullOrEmpty(sessionData) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(sessionData)!;
        }

        private void SaveCart(List<CartItem> cart) => HttpContext.Session.SetString(CART_KEY, JsonSerializer.Serialize(cart));
    }
}