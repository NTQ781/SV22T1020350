using System;
using System.Collections.Generic;
using System.Text;

namespace SV22T1020350.BusinessLayers
{
    /// <summary>
    /// Lớp lưu giữ các thông tin cấu hình sự dụng cho BusinessLayer
    /// </summary>
    public static class Configuration
    {
        private static string _connectionString= "";
        
        /// <summary>
        /// Khởi tạo cấu hình cho BusinessLayer, bao gồm chuỗi kết nối đến cơ sở dữ liệu
        /// (hàm này chạy trươc khi chạy ứng dụng)
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString)
        {
            connectionString = connectionString;
        }

        /// <summary>
        /// Lấy chuỗi tham số kết nối đến cơ sở dữ liệu, được khởi tạo từ hàm Initialize
        /// </summary>
        public static string ConnectionString => _connectionString;
           
        }
}
