using System.Data;
using Dapper;
using EmpLMS.Models.ViewModels;

namespace EmpLMS.Data.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DapperContext _context;

        public AuthRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<LoginUserDto?> ValidateUserLoginAsync(string email)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email.Trim(), DbType.String);

            return await connection.QuerySingleOrDefaultAsync<LoginUserDto>(
                "sp_ValidateUserLogin",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
