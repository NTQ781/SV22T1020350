using SV22T1020350.Models.Common;
using SV22T1020350.Models.HR;

namespace SV22T1020350.DataLayers.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        /// <summary>
        /// Kiểm tra xem email của nhân viên có hợp lệ không (không bị trùng)
        /// </summary>
        Task<bool> ValidateEmailAsync(string email, int id = 0);

        // ==========================================
        // ĐÃ SỬA: Đổi thành CheckLoginAsync trả về Task
        // để đồng bộ với EmployeeRepository và HRDataService
        // ==========================================
        /// <summary>
        /// Kiểm tra thông tin đăng nhập của nhân viên
        /// </summary>
        Task<Employee?> CheckLoginAsync(string email, string password);
    }
}