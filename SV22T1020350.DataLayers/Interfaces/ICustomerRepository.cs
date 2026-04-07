using SV22T1020350.Models.Partner;

namespace SV22T1020350.DataLayers.Interfaces
{
    /// <summary>
    /// Định nghĩa các phép xử lý dữ liệu trên Customer
    /// </summary>
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        /// <summary>
        /// Kiểm tra xem một địa chỉ email có hợp lệ hay không?
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <param name="id">
        /// Nếu id = 0: Kiểm tra email của khách hàng mới.
        /// Nếu id <> 0: Kiểm tra email đối với khách hàng đã tồn tại
        /// </param>
        /// <returns></returns>
        Task<bool> ValidateEmailAsync(string email, int id = 0);

        // ==========================================
        // ĐÂY LÀ HÀM BẠN ĐANG BỊ THIẾU CẦN THÊM VÀO
        // ==========================================
        /// <summary>
        /// Kiểm tra thông tin đăng nhập của khách hàng
        /// </summary>
        /// <param name="email">Email đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Trả về thông tin khách hàng nếu đăng nhập thành công, ngược lại trả về null</returns>
        Customer? CheckLogin(string email, string password);

        /// <summary>
        /// Đổi mật khẩu khách hàng
        /// </summary>
        bool ChangePassword(int customerID, string newPassword);
    }
}