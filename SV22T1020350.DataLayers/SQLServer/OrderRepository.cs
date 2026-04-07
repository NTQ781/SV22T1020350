using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020350.DataLayers.Interfaces;
using SV22T1020350.Models.Common;
using SV22T1020350.Models.Sales;
using System.Data;

namespace SV22T1020350.DataLayers.SQLServer
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<PagedResult<OrderViewInfo>> ListAsync(OrderSearchInput input)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                Status = (int)input.Status,
                DateFrom = input.DateFrom,
                DateTo = input.DateTo
            };

            var searchCondition = @"
                (@SearchValue = '' OR c.CustomerName LIKE N'%' + @SearchValue + '%' OR c.ContactName LIKE N'%' + @SearchValue + '%')
                AND (@Status = 0 OR o.Status = @Status)
                AND (@DateFrom IS NULL OR o.OrderTime >= @DateFrom)
                AND (@DateTo IS NULL OR o.OrderTime <= @DateTo)";

            // Tối ưu hóa: Dùng JOIN và GROUP BY để tính TotalValue thay vì Subquery trong SELECT
            var sql = $@"
                SELECT COUNT(*) 
                FROM Orders o
                LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                WHERE {searchCondition};

                SELECT o.*, 
                       c.CustomerName, c.ContactName AS CustomerContactName,
                       c.Email AS CustomerEmail, c.Phone AS CustomerPhone,
                       c.Address AS CustomerAddress,
                       e.FullName AS EmployeeName,
                       s.ShipperName, s.Phone AS ShipperPhone,
                       SUM(od.Quantity * od.SalePrice) AS TotalValue
                FROM Orders o
                LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                LEFT JOIN Employees e ON o.EmployeeID = e.EmployeeID
                LEFT JOIN Shippers s ON o.ShipperID = s.ShipperID
                LEFT JOIN OrderDetails od ON o.OrderID = od.OrderID
                WHERE {searchCondition}
                GROUP BY o.OrderID, o.CustomerID, o.OrderTime, o.DeliveryProvince, o.DeliveryAddress, 
                         o.EmployeeID, o.AcceptTime, o.ShipperID, o.ShippedTime, o.FinishedTime, o.Status,
                         c.CustomerName, c.ContactName, c.Email, c.Phone, c.Address,
                         e.FullName, s.ShipperName, s.Phone
                ORDER BY o.OrderTime DESC
                OFFSET (@Page - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY;";

            // Thêm commandTimeout: 60 (giây) để tránh lỗi Timeout
            using var multi = await connection.QueryMultipleAsync(sql, parameters, commandTimeout: 60);

            int rowCount = await multi.ReadSingleAsync<int>();
            var data = (await multi.ReadAsync<OrderViewInfo>()).ToList();

            return new PagedResult<OrderViewInfo>()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                RowCount = rowCount,
                DataItems = data
            };
        }

        public async Task<OrderViewInfo?> GetAsync(int orderID)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                SELECT o.*, 
                       c.CustomerName, c.ContactName AS CustomerContactName,
                       c.Email AS CustomerEmail, c.Phone AS CustomerPhone,
                       c.Address AS CustomerAddress,
                       e.FullName AS EmployeeName,
                       s.ShipperName, s.Phone AS ShipperPhone,
                       ISNULL((SELECT SUM(Quantity * SalePrice) FROM OrderDetails WHERE OrderID = o.OrderID), 0) AS TotalValue
                FROM Orders o
                LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                LEFT JOIN Employees e ON o.EmployeeID = e.EmployeeID
                LEFT JOIN Shippers s ON o.ShipperID = s.ShipperID
                WHERE o.OrderID = @orderID";

            return await connection.QueryFirstOrDefaultAsync<OrderViewInfo>(sql, new { orderID });
        }

        public async Task<int> AddAsync(Order data)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                INSERT INTO Orders(CustomerID, OrderTime, DeliveryProvince, DeliveryAddress, EmployeeID, Status)
                VALUES(@CustomerID, @OrderTime, @DeliveryProvince, @DeliveryAddress, @EmployeeID, @Status);
                SELECT CAST(SCOPE_IDENTITY() AS INT)";
            return await connection.ExecuteScalarAsync<int>(sql, data);
        }

        public async Task<bool> UpdateAsync(Order data)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                UPDATE Orders
                SET CustomerID = @CustomerID,
                    DeliveryProvince = @DeliveryProvince,
                    DeliveryAddress = @DeliveryAddress,
                    EmployeeID = @EmployeeID,
                    AcceptTime = @AcceptTime,
                    ShipperID = @ShipperID,
                    ShippedTime = @ShippedTime,
                    FinishedTime = @FinishedTime,
                    Status = @Status
                WHERE OrderID = @OrderID";
            return await connection.ExecuteAsync(sql, data) > 0;
        }

        public async Task<bool> DeleteAsync(int orderID)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "DELETE FROM Orders WHERE OrderID = @orderID";
            return await connection.ExecuteAsync(sql, new { orderID }) > 0;
        }

        public async Task<List<OrderDetailViewInfo>> ListDetailsAsync(int orderID)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                SELECT d.*, p.ProductName, p.Unit, p.Photo
                FROM OrderDetails d
                JOIN Products p ON d.ProductID = p.ProductID
                WHERE d.OrderID = @orderID";
            var data = await connection.QueryAsync<OrderDetailViewInfo>(sql, new { orderID });
            return data.ToList();
        }

        public async Task<OrderDetailViewInfo?> GetDetailAsync(int orderID, int productID)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                SELECT d.*, p.ProductName, p.Unit, p.Photo
                FROM OrderDetails d
                JOIN Products p ON d.ProductID = p.ProductID
                WHERE d.OrderID = @orderID AND d.ProductID = @productID";
            return await connection.QueryFirstOrDefaultAsync<OrderDetailViewInfo>(sql, new { orderID, productID });
        }

        public async Task<bool> AddDetailAsync(OrderDetail data)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                INSERT INTO OrderDetails(OrderID, ProductID, Quantity, SalePrice)
                VALUES(@OrderID, @ProductID, @Quantity, @SalePrice)";
            return await connection.ExecuteAsync(sql, data) > 0;
        }

        public async Task<bool> UpdateDetailAsync(OrderDetail data)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                UPDATE OrderDetails
                SET Quantity = @Quantity,
                    SalePrice = @SalePrice
                WHERE OrderID = @OrderID
                AND ProductID = @ProductID";
            return await connection.ExecuteAsync(sql, data) > 0;
        }

        public async Task<bool> DeleteDetailAsync(int orderID, int productID)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"DELETE FROM OrderDetails WHERE OrderID = @orderID AND ProductID = @productID";
            return await connection.ExecuteAsync(sql, new { orderID, productID }) > 0;
        }
    }
}