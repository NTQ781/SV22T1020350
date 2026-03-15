using SV22T1020350.DataLayers.Interfaces;
using SV22T1020350.DataLayers.SQLServer;
using SV22T1020350.Models.Common;
using SV22T1020350.Models.Partner;
using System.Threading.Tasks;

namespace SV22T1020350.BusinessLayers
{
    /// <summary>
    /// Lớp cung cấp các chức năng xử lý nghiệp vụ liên quan đến đối tác của hệ thống
    /// Bao gồm: Customer, Supplier, Shipper
    /// </summary>
    public static class PartnerDataService
    {
        private static readonly IGenericRepository<Supplier> supplierDB;
        private static readonly IGenericRepository<Shipper> shipperDB;
        private static readonly ICustomerRepository customerDB;

        /// <summary>
        /// Constructor khởi tạo các đối tượng truy cập dữ liệu
        /// </summary>
        static PartnerDataService()
        {
            supplierDB = new SupplierRepository(Configuration.ConnectionString);
            shipperDB = new ShipperRepository(Configuration.ConnectionString);
            customerDB = new CustomerRepository(Configuration.ConnectionString);
        }

        //=====================================================
        //== Các chức năng nghiệp vụ liên quan đến Supplier
        //=====================================================

        /// <summary>
        /// Tìm kiếm và trả về danh sách nhà cung cấp dưới dạng phân trang
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm và phân trang</param>
        /// <returns>Danh sách nhà cung cấp</returns>
        public static async Task<PagedResult<Supplier>> ListSuppliersAsync(PaginationSearchInput input)
        {
            return await supplierDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin của một nhà cung cấp dựa vào mã số của nhà cung cấp đó
        /// </summary>
        /// <param name="supplierID">Mã nhà cung cấp</param>
        /// <returns>Thông tin nhà cung cấp</returns>
        public static Supplier? GetSupplierAsync(int supplierID)
        {
            return supplierDB.GetAsync(supplierID).Result;
        }

        /// <summary>
        /// Bổ sung nhà cung cấp mới
        /// </summary>
        /// <param name="supplier">Thông tin nhà cung cấp</param>
        /// <returns>Mã nhà cung cấp được bổ sung</returns>
        public static async Task<int> AddSupplierAsync(Supplier supplier)
        {
            // TODO: kiểm tra tính hợp lệ của dữ liệu
            return await supplierDB.AddAsync(supplier);
        }

        /// <summary>
        /// Cập nhật thông tin nhà cung cấp
        /// </summary>
        /// <param name="supplier">Thông tin nhà cung cấp cần cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        public static async Task<bool> UpdateSupplierAsync(Supplier supplier)
        {
            return await supplierDB.UpdateAsync(supplier);
        }

        /// <summary>
        /// Xóa một nhà cung cấp
        /// </summary>
        /// <param name="supplierID">Mã nhà cung cấp cần xóa</param>
        /// <returns>Kết quả xóa</returns>
        public static async Task<bool> DeleteSupplierAsync(int supplierID)
        {
            if (await supplierDB.IsUsedAsync(supplierID))
                return false;

            return await supplierDB.DeleteAsync(supplierID);
        }

        /// <summary>
        /// Kiểm tra xem một nhà cung cấp có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="supplierID">Mã nhà cung cấp</param>
        /// <returns>True nếu đang được sử dụng</returns>
        public static async Task<bool> IsUsedSupplierAsync(int supplierID)
        {
            return await supplierDB.IsUsedAsync(supplierID);
        }

        //=====================================================
        //== Các chức năng nghiệp vụ liên quan đến Shipper
        //=====================================================

        /// <summary>
        /// Tìm kiếm và trả về danh sách đơn vị vận chuyển dưới dạng phân trang
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm và phân trang</param>
        /// <returns>Danh sách đơn vị vận chuyển</returns>
        public static async Task<PagedResult<Shipper>> ListShippersAsync(PaginationSearchInput input)
        {
            return await shipperDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin của một đơn vị vận chuyển dựa vào mã
        /// </summary>
        /// <param name="shipperID">Mã đơn vị vận chuyển</param>
        /// <returns>Thông tin đơn vị vận chuyển</returns>
        public static Shipper? GetShipperAsync(int shipperID)
        {
            return shipperDB.GetAsync(shipperID).Result;
        }

        /// <summary>
        /// Bổ sung đơn vị vận chuyển mới
        /// </summary>
        /// <param name="shipper">Thông tin đơn vị vận chuyển</param>
        /// <returns>Mã đơn vị vận chuyển</returns>
        public static async Task<int> AddShipperAsync(Shipper shipper)
        {
            return await shipperDB.AddAsync(shipper);
        }

        /// <summary>
        /// Cập nhật thông tin đơn vị vận chuyển
        /// </summary>
        /// <param name="shipper">Thông tin cần cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        public static async Task<bool> UpdateShipperAsync(Shipper shipper)
        {
            return await shipperDB.UpdateAsync(shipper);
        }

        /// <summary>
        /// Xóa một đơn vị vận chuyển
        /// </summary>
        /// <param name="shipperID">Mã đơn vị vận chuyển</param>
        /// <returns>Kết quả xóa</returns>
        public static async Task<bool> DeleteShipperAsync(int shipperID)
        {
            if (await shipperDB.IsUsedAsync(shipperID))
                return false;

            return await shipperDB.DeleteAsync(shipperID);
        }

        /// <summary>
        /// Kiểm tra đơn vị vận chuyển có đang được sử dụng hay không
        /// </summary>
        /// <param name="shipperID">Mã đơn vị vận chuyển</param>
        /// <returns>True nếu đang được sử dụng</returns>
        public static async Task<bool> IsUsedShipperAsync(int shipperID)
        {
            return await shipperDB.IsUsedAsync(shipperID);
        }

        //=====================================================
        //== Các chức năng nghiệp vụ liên quan đến Customer
        //=====================================================

        /// <summary>
        /// Tìm kiếm và trả về danh sách khách hàng dưới dạng phân trang
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm</param>
        /// <returns>Danh sách khách hàng</returns>
        public static async Task<PagedResult<Customer>> ListCustomersAsync(PaginationSearchInput input)
        {
            return await customerDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin của một khách hàng dựa vào mã khách hàng
        /// </summary>
        /// <param name="customerID">Mã khách hàng</param>
        /// <returns>Thông tin khách hàng</returns>
        public static Customer? GetCustomerAsync(int customerID)
        {
            return customerDB.GetAsync(customerID).Result;
        }

        /// <summary>
        /// Bổ sung khách hàng mới
        /// </summary>
        /// <param name="customer">Thông tin khách hàng</param>
        /// <returns>Mã khách hàng</returns>
        public static async Task<int> AddCustomerAsync(Customer customer)
        {
            return await customerDB.AddAsync(customer);
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="customer">Thông tin cần cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        public static async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            return await customerDB.UpdateAsync(customer);
        }

        /// <summary>
        /// Xóa khách hàng
        /// </summary>
        /// <param name="customerID">Mã khách hàng</param>
        /// <returns>Kết quả xóa</returns>
        public static async Task<bool> DeleteCustomerAsync(int customerID)
        {
            if (await customerDB.IsUsedAsync(customerID))
                return false;

            return await customerDB.DeleteAsync(customerID);
        }

        /// <summary>
        /// Kiểm tra khách hàng có đang được sử dụng hay không
        /// </summary>
        /// <param name="customerID">Mã khách hàng</param>
        /// <returns>True nếu đang được sử dụng</returns>
        public static async Task<bool> IsUsedCustomerAsync(int customerID)
        {
            return await customerDB.IsUsedAsync(customerID);
        }

        /// <summary>
        /// kiểm tra email khách hàng có hợp lệ hay không?
        /// (email hợp lệ nếu không trùng với khách hàng khác)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="customerID"></param>
        /// nếu bằng 0, tức là kiểm tra email của khách hàng mới
        ///, nếu khác 0, tức là kiểm tra email của khách hàng có mã là <paramref name="customerID"/>"
        /// <returns></returns>
        public static async Task<bool> VaidateCustomerEmailAsync(string email, int customerID = 0)
        {
            return await customerDB.ValidateEmailAsync(email, customerID);
        }
    }
}

