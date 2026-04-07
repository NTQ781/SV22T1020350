using SV22T1020350.DataLayers.Interfaces;
using SV22T1020350.DataLayers.SQLServer;
using SV22T1020350.Models.Common;
using SV22T1020350.Models.Partner;

namespace SV22T1020350.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý dữ liệu liên quan đến các đối tác của hệ thống
    /// bao gồm: nhà cung cấp (Supplier), khách hàng (Customer) và người giao hàng (Shipper)
    /// </summary>
    public static class PartnerDataService
    {
        private static readonly IGenericRepository<Supplier> supplierDB;
        private static readonly ICustomerRepository customerDB;
        private static readonly IGenericRepository<Shipper> shipperDB;

        /// <summary>
        /// Ctor
        /// </summary>
        static PartnerDataService()
        {
            supplierDB = new SupplierRepository(Configuration.ConnectionString);
            customerDB = new CustomerRepository(Configuration.ConnectionString);
            shipperDB = new ShipperRepository(Configuration.ConnectionString);
        }

        #region Supplier

        /// <summary>
        /// Tìm kiếm và lấy danh sách nhà cung cấp dưới dạng phân trang.
        /// </summary>
        public static async Task<PagedResult<Supplier>> ListSuppliersAsync(PaginationSearchInput input)
        {
            return await supplierDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một nhà cung cấp dựa vào mã nhà cung cấp.
        /// </summary>
        public static async Task<Supplier?> GetSupplierAsync(int supplierID)
        {
            return await supplierDB.GetAsync(supplierID);
        }

        /// <summary>
        /// Bổ sung một nhà cung cấp mới vào hệ thống.
        /// </summary>
        public static async Task<int> AddSupplierAsync(Supplier data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.SupplierName))
                throw new Exception("Tên nhà cung cấp không được để trống");

            if (string.IsNullOrWhiteSpace(data.ContactName)) data.ContactName = "";
            if (string.IsNullOrWhiteSpace(data.Province)) data.Province = "";
            if (string.IsNullOrWhiteSpace(data.Address)) data.Address = "";
            if (string.IsNullOrWhiteSpace(data.Phone)) data.Phone = "";
            if (string.IsNullOrWhiteSpace(data.Email)) data.Email = "";

            data.SupplierName = data.SupplierName.Trim();
            data.ContactName = data.ContactName.Trim();
            data.Province = data.Province.Trim();
            data.Address = data.Address.Trim();
            data.Phone = data.Phone.Trim();
            data.Email = data.Email.Trim();

            return await supplierDB.AddAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một nhà cung cấp.
        /// </summary>
        public static async Task<bool> UpdateSupplierAsync(Supplier data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.SupplierName))
                throw new Exception("Tên nhà cung cấp không được để trống");

            if (string.IsNullOrWhiteSpace(data.ContactName)) data.ContactName = "";
            if (string.IsNullOrWhiteSpace(data.Province)) data.Province = "";
            if (string.IsNullOrWhiteSpace(data.Address)) data.Address = "";
            if (string.IsNullOrWhiteSpace(data.Phone)) data.Phone = "";
            if (string.IsNullOrWhiteSpace(data.Email)) data.Email = "";

            data.SupplierName = data.SupplierName.Trim();
            data.ContactName = data.ContactName.Trim();
            data.Province = data.Province.Trim();
            data.Address = data.Address.Trim();
            data.Phone = data.Phone.Trim();
            data.Email = data.Email.Trim();

            return await supplierDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa một nhà cung cấp dựa vào mã nhà cung cấp.
        /// </summary>
        public static async Task<bool> DeleteSupplierAsync(int supplierID)
        {
            if (await supplierDB.IsUsedAsync(supplierID))
                return false;

            return await supplierDB.DeleteAsync(supplierID);
        }

        /// <summary>
        /// Kiểm tra xem một nhà cung cấp có đang được sử dụng trong dữ liệu hay không.
        /// </summary>
        public static async Task<bool> IsUsedSupplierAsync(int supplierID)
        {
            return await supplierDB.IsUsedAsync(supplierID);
        }

        #endregion

        #region Customer

        /// <summary>
        /// Đổi mật khẩu khách hàng
        /// </summary>
        public static bool ChangeCustomerPassword(int customerID, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new Exception("Mật khẩu mới không được để trống");

            return customerDB.ChangePassword(customerID, newPassword.Trim());
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng dưới dạng phân trang.
        /// </summary>
        public static async Task<PagedResult<Customer>> ListCustomersAsync(PaginationSearchInput input)
        {
            return await customerDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một khách hàng dựa vào mã khách hàng.
        /// </summary>
        public static async Task<Customer?> GetCustomerAsync(int customerID)
        {
            return await customerDB.GetAsync(customerID);
        }

        /// <summary>
        /// Bổ sung một khách hàng mới vào hệ thống.
        /// </summary>
        public static async Task<int> AddCustomerAsync(Customer data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                throw new Exception("Tên khách hàng không được để trống");

            if (string.IsNullOrWhiteSpace(data.ContactName)) data.ContactName = "";
            if (string.IsNullOrWhiteSpace(data.Province)) data.Province = "";
            if (string.IsNullOrWhiteSpace(data.Address)) data.Address = "";
            if (string.IsNullOrWhiteSpace(data.Phone)) data.Phone = "";
            if (string.IsNullOrWhiteSpace(data.Email)) data.Email = "";

            data.CustomerName = data.CustomerName.Trim();
            data.ContactName = data.ContactName.Trim();
            data.Province = data.Province.Trim();
            data.Address = data.Address.Trim();
            data.Phone = data.Phone.Trim();
            data.Email = data.Email.Trim();

            return await customerDB.AddAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một khách hàng.
        /// </summary>
        public static async Task<bool> UpdateCustomerAsync(Customer data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                throw new Exception("Tên khách hàng không được để trống");

            if (string.IsNullOrWhiteSpace(data.ContactName)) data.ContactName = "";
            if (string.IsNullOrWhiteSpace(data.Province)) data.Province = "";
            if (string.IsNullOrWhiteSpace(data.Address)) data.Address = "";
            if (string.IsNullOrWhiteSpace(data.Phone)) data.Phone = "";
            if (string.IsNullOrWhiteSpace(data.Email)) data.Email = "";

            data.CustomerName = data.CustomerName.Trim();
            data.ContactName = data.ContactName.Trim();
            data.Province = data.Province.Trim();
            data.Address = data.Address.Trim();
            data.Phone = data.Phone.Trim();
            data.Email = data.Email.Trim();

            return await customerDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa một khách hàng dựa vào mã khách hàng.
        /// </summary>
        public static async Task<bool> DeleteCustomerAsync(int customerID)
        {
            if (await customerDB.IsUsedAsync(customerID))
                return false;

            return await customerDB.DeleteAsync(customerID);
        }

        /// <summary>
        /// Kiểm tra xem một khách hàng có đang được sử dụng trong dữ liệu hay không.
        /// </summary>
        public static async Task<bool> IsUsedCustomerAsync(int customerID)
        {
            return await customerDB.IsUsedAsync(customerID);
        }

        /// <summary>
        /// Kiểm tra xem email của khách hàng có hợp lệ không
        /// </summary>
        public static async Task<bool> ValidatelCustomerEmailAsync(string email, int customerID = 0)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email không được để trống");

            return await customerDB.ValidateEmailAsync(email.Trim(), customerID);
        }

        /// <summary>
        /// Kiểm tra thông tin đăng nhập của khách hàng
        /// </summary>
        public static Customer? CheckCustomerLogin(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            return customerDB.CheckLogin(email.Trim(), password.Trim());
        }

        #endregion

        #region Shipper

        /// <summary>
        /// Tìm kiếm và lấy danh sách người giao hàng dưới dạng phân trang.
        /// </summary>
        public static async Task<PagedResult<Shipper>> ListShippersAsync(PaginationSearchInput input)
        {
            return await shipperDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một người giao hàng dựa vào mã người giao hàng.
        /// </summary>
        public static async Task<Shipper?> GetShipperAsync(int shipperID)
        {
            return await shipperDB.GetAsync(shipperID);
        }

        /// <summary>
        /// Bổ sung một người giao hàng mới vào hệ thống.
        /// </summary>
        public static async Task<int> AddShipperAsync(Shipper data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.ShipperName))
                throw new Exception("Tên người giao hàng không được để trống");

            if (string.IsNullOrWhiteSpace(data.Phone)) data.Phone = "";

            data.ShipperName = data.ShipperName.Trim();
            data.Phone = data.Phone.Trim();

            return await shipperDB.AddAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một người giao hàng.
        /// </summary>
        public static async Task<bool> UpdateShipperAsync(Shipper data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.ShipperName))
                throw new Exception("Tên người giao hàng không được để trống");

            if (string.IsNullOrWhiteSpace(data.Phone)) data.Phone = "";

            data.ShipperName = data.ShipperName.Trim();
            data.Phone = data.Phone.Trim();

            return await shipperDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa một người giao hàng dựa vào mã người giao hàng.
        /// </summary>
        public static async Task<bool> DeleteShipperAsync(int shipperID)
        {
            if (await shipperDB.IsUsedAsync(shipperID))
                return false;

            return await shipperDB.DeleteAsync(shipperID);
        }

        /// <summary>
        /// Kiểm tra xem một người giao hàng có đang được sử dụng trong dữ liệu hay không.
        /// </summary>
        public static async Task<bool> IsUsedShipperAsync(int shipperID)
        {
            return await shipperDB.IsUsedAsync(shipperID);
        }

        #endregion
    }
}