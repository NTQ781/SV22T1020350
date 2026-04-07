using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020350.DataLayers.Interfaces;
using SV22T1020350.Models.Common;
using SV22T1020350.Models.HR;

namespace SV22T1020350.DataLayers.SQLServer
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<PagedResult<Employee>> ListAsync(PaginationSearchInput input)
        {
            var result = new PagedResult<Employee>()
            {
                Page = input.Page,
                PageSize = input.PageSize
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    searchValue = input.SearchValue,
                    offset = input.Offset,
                    pageSize = input.PageSize
                };

                string countSql = @"SELECT COUNT(*)
                                    FROM Employees
                                    WHERE FullName LIKE '%' + @searchValue + '%'
                                       OR Phone LIKE '%' + @searchValue + '%'";

                result.RowCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

                string querySql = @"SELECT EmployeeID, FullName, BirthDate, Address,
                                            Phone, Email, Photo, IsWorking, RoleNames
                                    FROM Employees
                                    WHERE FullName LIKE '%' + @searchValue + '%'
                                       OR Phone LIKE '%' + @searchValue + '%'
                                    ORDER BY FullName
                                    OFFSET @offset ROWS
                                    FETCH NEXT @pageSize ROWS ONLY";

                if (input.PageSize == 0)
                {
                    querySql = @"SELECT EmployeeID, FullName, BirthDate, Address,
                                         Phone, Email, Photo, IsWorking, RoleNames
                                  FROM Employees
                                  WHERE FullName LIKE '%' + @searchValue + '%'
                                     OR Phone LIKE '%' + @searchValue + '%'
                                  ORDER BY FullName";
                }

                var data = await connection.QueryAsync<Employee>(querySql, parameters);
                result.DataItems = data.ToList();
            }

            return result;
        }

        public async Task<Employee?> GetAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT *
                                FROM Employees
                                WHERE EmployeeID = @id";

                return await connection.QueryFirstOrDefaultAsync<Employee>(sql, new { id });
            }
        }

        public async Task<int> AddAsync(Employee data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // ĐÃ BỔ SUNG CỘT Password VÀO CÂU LỆNH INSERT
                string sql = @"INSERT INTO Employees
                                (FullName, BirthDate, Address, Phone, Email, Password, Photo, IsWorking, RoleNames)
                                VALUES
                                (@FullName, @BirthDate, @Address, @Phone, @Email, @Password, @Photo, @IsWorking, @RoleNames);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int id = await connection.ExecuteScalarAsync<int>(sql, data);
                return id;
            }
        }

        public async Task<bool> UpdateAsync(Employee data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // ĐÃ BỔ SUNG Password = @Password VÀO CÂU LỆNH UPDATE
                string sql = @"UPDATE Employees
                                SET FullName = @FullName,
                                    BirthDate = @BirthDate,
                                    Address = @Address,
                                    Phone = @Phone,
                                    Email = @Email,
                                    Password = @Password,
                                    Photo = @Photo,
                                    IsWorking = @IsWorking,
                                    RoleNames = @RoleNames
                                WHERE EmployeeID = @EmployeeID";

                int rows = await connection.ExecuteAsync(sql, data);
                return rows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"DELETE FROM Employees
                                WHERE EmployeeID = @id";

                int rows = await connection.ExecuteAsync(sql, new { id });
                return rows > 0;
            }
        }

        public async Task<bool> IsUsedAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT COUNT(*)
                                FROM Orders
                                WHERE EmployeeID = @id";

                int count = await connection.ExecuteScalarAsync<int>(sql, new { id });
                return count > 0;
            }
        }

        public async Task<bool> ValidateEmailAsync(string email, int id = 0)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = id == 0
                    ? @"SELECT COUNT(*) FROM Employees WHERE Email = @email"
                    : @"SELECT COUNT(*) FROM Employees WHERE Email = @email AND EmployeeID <> @id";

                int count = await connection.ExecuteScalarAsync<int>(sql, new { email, id });
                return count == 0;
            }
        }

        public async Task<Employee?> CheckLoginAsync(string email, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Sử dụng LOWER để so sánh chính xác chuỗi MD5
                string sql = @"SELECT * FROM Employees 
                       WHERE LOWER(Email) = LOWER(@email) 
                         AND LOWER(Password) = LOWER(@password) 
                         AND IsWorking = 1";

                return await connection.QueryFirstOrDefaultAsync<Employee>(sql, new { email, password });
            }
        }
    }
}