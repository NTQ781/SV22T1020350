using SV22T1020350.DataLayers.Interfaces;
using SV22T1020350.DataLayers.SQLServer;
using SV22T1020350.Models.Common;
using SV22T1020350.Models.Sales;

namespace SV22T1020350.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý dữ liệu liên quan đến bán hàng
    /// bao gồm: đơn hàng (Order) và chi tiết đơn hàng (OrderDetail).
                /// </summary>
    public static class SalesDataService
    {
        private static readonly IOrderRepository orderDB;

        /// <summary>
        /// Constructor khởi tạo kết nối thông qua Repository[cite: 81].
                    /// </summary>
        static SalesDataService()
        {
            orderDB = new OrderRepository(Configuration.ConnectionString);
        }

        #region Order Management

        /// <summary>
        /// Tìm kiếm và lấy danh sách đơn hàng dưới dạng phân trang
        /// </summary>
        public static async Task<PagedResult<OrderViewInfo>> ListOrdersAsync(OrderSearchInput input)
        {
            return await orderDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một đơn hàng
        /// </summary>
        public static async Task<OrderViewInfo?> GetOrderAsync(int orderID)
        {
            return await orderDB.GetAsync(orderID);
        }

        /// <summary>
        /// Tạo đơn hàng mới
        /// </summary>
        public static async Task<int> AddOrderAsync(Order data)
        {
            if (data == null) throw new Exception("Thông tin đơn hàng không hợp lệ");
            if (data.CustomerID <= 0) throw new Exception("Vui lòng chọn khách hàng");
            if (string.IsNullOrWhiteSpace(data.DeliveryProvince)) throw new Exception("Vui lòng chọn tỉnh thành giao hàng");
            if (string.IsNullOrWhiteSpace(data.DeliveryAddress)) throw new Exception("Vui lòng nhập địa chỉ giao hàng chi tiết");

            data.Status = OrderStatusEnum.New;
            data.OrderTime = DateTime.Now;
            data.DeliveryProvince = data.DeliveryProvince.Trim();
            data.DeliveryAddress = data.DeliveryAddress.Trim();

            return await orderDB.AddAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin chung của đơn hàng (chỉ khi ở trạng thái Mới).
        /// </summary>
        public static async Task<bool> UpdateOrderAsync(Order data)
        {
            var order = await orderDB.GetAsync(data.OrderID);
            if (order == null) throw new Exception("Đơn hàng không tồn tại");

            if (order.Status != OrderStatusEnum.New)
                throw new Exception("Đơn hàng đã được xử lý, không thể thay đổi thông tin chung");

            if (data.CustomerID <= 0) throw new Exception("Vui lòng chọn khách hàng");
            if (string.IsNullOrWhiteSpace(data.DeliveryProvince)) throw new Exception("Vui lòng chọn tỉnh thành giao hàng");
            if (string.IsNullOrWhiteSpace(data.DeliveryAddress)) throw new Exception("Vui lòng nhập địa chỉ giao hàng chi tiết");

            data.DeliveryProvince = data.DeliveryProvince.Trim();
            data.DeliveryAddress = data.DeliveryAddress.Trim();

            return await orderDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa đơn hàng
        /// </summary>
        public static async Task<bool> DeleteOrderAsync(int orderID)
        {
            var order = await orderDB.GetAsync(orderID);
            if (order == null) return false;

            if (order.Status == OrderStatusEnum.Accepted ||
                order.Status == OrderStatusEnum.Shipping ||
                order.Status == OrderStatusEnum.Completed)
                throw new Exception("Đơn hàng đang xử lý hoặc đã hoàn tất, không được xóa");

            return await orderDB.DeleteAsync(orderID);
        }

        #endregion

        #region Order Status Processing (Nghiệp vụ chuyển trạng thái)

        /// <summary>
        /// Duyệt đơn hàng
        /// </summary>
        public static async Task<bool> AcceptOrderAsync(int orderID, int employeeID)
        {
            var order = await orderDB.GetAsync(orderID);
            if (order == null) throw new Exception("Đơn hàng không tồn tại");
            if (order.Status != OrderStatusEnum.New) throw new Exception("Trạng thái đơn hàng không phù hợp để duyệt");

            order.EmployeeID = employeeID;
            order.AcceptTime = DateTime.Now;
            order.Status = OrderStatusEnum.Accepted;
            return await orderDB.UpdateAsync(order);
        }

        /// <summary>
        /// Từ chối đơn hàng
        /// </summary>
        public static async Task<bool> RejectOrderAsync(int orderID, int employeeID)
        {
            var order = await orderDB.GetAsync(orderID);
            if (order == null) throw new Exception("Đơn hàng không tồn tại");
            if (order.Status != OrderStatusEnum.New) throw new Exception("Chỉ có thể từ chối đơn hàng đang ở trạng thái mới");

            order.EmployeeID = employeeID;
            order.FinishedTime = DateTime.Now;
            order.Status = OrderStatusEnum.Rejected;
            return await orderDB.UpdateAsync(order);
        }

        /// <summary>
        /// Hủy bỏ đơn hàng
        /// </summary>
        public static async Task<bool> CancelOrderAsync(int orderID)
        {
            var order = await orderDB.GetAsync(orderID);
            if (order == null) throw new Exception("Đơn hàng không tồn tại");

            if (order.Status == OrderStatusEnum.Shipping || order.Status == OrderStatusEnum.Completed)
                throw new Exception("Đơn hàng đang giao hoặc đã hoàn tất, không thể hủy");

            order.FinishedTime = DateTime.Now;
            order.Status = OrderStatusEnum.Cancelled;
            return await orderDB.UpdateAsync(order);
        }

        /// <summary>
        /// Xác nhận bắt đầu giao hàng
        /// </summary>
        public static async Task<bool> ShipOrderAsync(int orderID, int shipperID)
        {
            var order = await orderDB.GetAsync(orderID);
            if (order == null) throw new Exception("Đơn hàng không tồn tại");
            if (order.Status != OrderStatusEnum.Accepted) throw new Exception("Đơn hàng phải được duyệt trước khi giao hàng");
            if (shipperID <= 0) throw new Exception("Vui lòng chọn người giao hàng");

            order.ShipperID = shipperID;
            order.ShippedTime = DateTime.Now;
            order.Status = OrderStatusEnum.Shipping;
            return await orderDB.UpdateAsync(order);
        }

        /// <summary>
        /// Xác nhận đã giao hàng thành công và kết thúc đơn hàng
        /// </summary>
        public static async Task<bool> CompleteOrderAsync(int orderID)
        {
            var order = await orderDB.GetAsync(orderID);
            if (order == null) throw new Exception("Đơn hàng không tồn tại");
            if (order.Status != OrderStatusEnum.Shipping) throw new Exception("Đơn hàng phải ở trạng thái đang giao mới có thể hoàn tất");

            order.FinishedTime = DateTime.Now;
            order.Status = OrderStatusEnum.Completed;
            return await orderDB.UpdateAsync(order);
        }

        #endregion

        #region Order Details Management

        /// <summary>
        /// Lấy danh sách mặt hàng của một đơn hàng
        /// </summary>
        public static async Task<List<OrderDetailViewInfo>> ListDetailsAsync(int orderID)
        {
            return await orderDB.ListDetailsAsync(orderID);
        }

        /// <summary>
        /// Lấy thông tin một mặt hàng cụ thể trong đơn hàng.
        /// </summary>
        public static async Task<OrderDetailViewInfo?> GetDetailAsync(int orderID, int productID)
        {
            return await orderDB.GetDetailAsync(orderID, productID);
        }

        /// <summary>
        /// Thêm mặt hàng vào đơn hàng
        /// </summary>
        public static async Task<bool> AddDetailAsync(OrderDetail data)
        {
            var order = await orderDB.GetAsync(data.OrderID);
            if (order == null) throw new Exception("Đơn hàng không tồn tại");

            if (order.Status != OrderStatusEnum.New)
                throw new Exception("Chỉ được thêm mặt hàng khi đơn hàng đang ở trạng thái mới");

            if (data.ProductID <= 0) throw new Exception("Vui lòng chọn mặt hàng");
            if (data.Quantity <= 0) throw new Exception("Số lượng mua phải lớn hơn 0");
            if (data.SalePrice < 0) throw new Exception("Giá bán không hợp lệ");

            return await orderDB.AddDetailAsync(data);
        }

        /// <summary>
        /// Cập nhật số lượng và giá bán của mặt hàng trong đơn
        /// </summary>
        public static async Task<bool> UpdateDetailAsync(OrderDetail data)
        {
            var order = await orderDB.GetAsync(data.OrderID);
            if (order == null) throw new Exception("Đơn hàng không tồn tại");

            if (order.Status != OrderStatusEnum.New)
                throw new Exception("Chỉ được cập nhật mặt hàng khi đơn hàng đang ở trạng thái mới");

            if (data.Quantity <= 0) throw new Exception("Số lượng mua phải lớn hơn 0");
            if (data.SalePrice < 0) throw new Exception("Giá bán không hợp lệ");

            return await orderDB.UpdateDetailAsync(data);
        }

        /// <summary>
        /// Xóa mặt hàng khỏi đơn hàng.
        /// </summary>
        public static async Task<bool> DeleteDetailAsync(int orderID, int productID)
        {
            var order = await orderDB.GetAsync(orderID);
            if (order == null) throw new Exception("Đơn hàng không tồn tại");

            if (order.Status != OrderStatusEnum.New)
                throw new Exception("Chỉ được xóa mặt hàng khi đơn hàng đang ở trạng thái mới");

            return await orderDB.DeleteDetailAsync(orderID, productID);
        }

        #endregion
    }
}