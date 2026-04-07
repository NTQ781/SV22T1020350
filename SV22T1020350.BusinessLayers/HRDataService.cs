using SV22T1020350.DataLayers.Interfaces;
using SV22T1020350.DataLayers.SQLServer;
using SV22T1020350.Models.Common;
using SV22T1020350.Models.HR;

namespace SV22T1020350.BusinessLayers
{
    public static class HRDataService
    {
        private static readonly IEmployeeRepository employeeDB;

        static HRDataService()
        {
            employeeDB = new EmployeeRepository(Configuration.ConnectionString);
        }

        #region Employee

        public static async Task<PagedResult<Employee>> ListEmployeesAsync(PaginationSearchInput input)
        {
            return await employeeDB.ListAsync(input);
        }

        public static async Task<Employee?> GetEmployeeAsync(int employeeID)
        {
            return await employeeDB.GetAsync(employeeID);
        }

        public static async Task<int> AddEmployeeAsync(Employee data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.FullName))
                throw new Exception("Họ tên nhân viên không được để trống");

            if (string.IsNullOrWhiteSpace(data.Address)) data.Address = "";
            if (string.IsNullOrWhiteSpace(data.Phone)) data.Phone = "";
            if (string.IsNullOrWhiteSpace(data.Email)) data.Email = "";
            if (string.IsNullOrWhiteSpace(data.Photo)) data.Photo = "";

            data.FullName = data.FullName.Trim();
            data.Address = data.Address.Trim();
            data.Phone = data.Phone.Trim();
            data.Email = data.Email.Trim();

            return await employeeDB.AddAsync(data);
        }

        public static async Task<bool> UpdateEmployeeAsync(Employee data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.FullName))
                throw new Exception("Họ tên nhân viên không được để trống");

            if (string.IsNullOrWhiteSpace(data.Address)) data.Address = "";
            if (string.IsNullOrWhiteSpace(data.Phone)) data.Phone = "";
            if (string.IsNullOrWhiteSpace(data.Email)) data.Email = "";
            if (string.IsNullOrWhiteSpace(data.Photo)) data.Photo = "";

            data.FullName = data.FullName.Trim();
            data.Address = data.Address.Trim();
            data.Phone = data.Phone.Trim();
            data.Email = data.Email.Trim();

            return await employeeDB.UpdateAsync(data);
        }

        public static async Task<bool> DeleteEmployeeAsync(int employeeID)
        {
            if (await employeeDB.IsUsedAsync(employeeID))
                return false;

            return await employeeDB.DeleteAsync(employeeID);
        }

        public static async Task<bool> IsUsedEmployeeAsync(int employeeID)
        {
            return await employeeDB.IsUsedAsync(employeeID);
        }

        public static async Task<bool> ValidateEmployeeEmailAsync(string email, int employeeID = 0)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email không được để trống");

            return await employeeDB.ValidateEmailAsync(email.Trim(), employeeID);
        }

        /// <summary>
        /// Kiểm tra thông tin đăng nhập của nhân viên
        /// </summary>
        public static async Task<Employee?> CheckLoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            return await employeeDB.CheckLoginAsync(email.Trim(), password.Trim());
        }

        #endregion
    }
}