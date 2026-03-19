using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020350.DataLayers.Interfaces;
using SV22T1020350.Models.DataDictionary;

namespace SV22T1020350.DataLayers.SQLServer
{
    /// <summary>
    /// Xử lý dữ liệu tỉnh thành
    /// </summary>
    public class ProvinceRepository : IDataDictionaryRepository<Province>
    {
        private readonly string _connectionString;

        public ProvinceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Lấy danh sách tỉnh thành
        /// </summary>
        public async Task<List<Province>> ListAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"SELECT ProvinceName
                        FROM Provinces
                        ORDER BY ProvinceName";

            var data = await connection.QueryAsync<Province>(sql);

            return data.ToList();
        }
    }
}
