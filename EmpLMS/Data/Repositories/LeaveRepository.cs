using System.Data;
using Dapper;
using EmpLMS.Models.DTOs;

namespace EmpLMS.Data.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly DapperContext _context;

        public LeaveRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeavePurposeDto>> GetActiveLeavePurposesAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<LeavePurposeDto>(
                "sp_GetActiveLeavePurposes",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> ApplyLeaveAsync(int employeeId, int leavePurposeId, DateTime fromDate, DateTime toDate, decimal totalDays, string remarks)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@EmployeeId", employeeId, DbType.Int32);
            parameters.Add("@LeavePurposeId", leavePurposeId, DbType.Int32);
            parameters.Add("@FromDate", fromDate.Date, DbType.Date);
            parameters.Add("@ToDate", toDate.Date, DbType.Date);
            parameters.Add("@TotalDays", totalDays, DbType.Decimal);
            parameters.Add("@Remarks", remarks.Trim(), DbType.String);

            return await connection.ExecuteScalarAsync<int>(
                "sp_ApplyLeave",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime fromDate, DateTime toDate)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT COUNT(1) 
                FROM LeaveRequests 
                WHERE EmployeeId = @EmployeeId 
                  AND Status IN ('Pending', 'Approved') 
                  AND FromDate <= @ToDate 
                  AND ToDate >= @FromDate;";

            var count = await connection.ExecuteScalarAsync<int>(sql, new
            {
                EmployeeId = employeeId,
                FromDate = fromDate.Date,
                ToDate = toDate.Date
            });

            return count > 0;
        }

        public async Task<IEnumerable<LeaveRequestHistoryDto>> GetEmployeeLeaveHistoryAsync(int employeeId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@EmployeeId", employeeId, DbType.Int32);

            return await connection.QueryAsync<LeaveRequestHistoryDto>(
                "sp_GetEmployeeLeaveHistory",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<LeaveSummaryDto> GetEmployeeLeaveSummaryAsync(int employeeId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@EmployeeId", employeeId, DbType.Int32);

            var summary = await connection.QuerySingleOrDefaultAsync<LeaveSummaryDto>(
                "sp_GetEmployeeLeaveSummary",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return summary ?? new LeaveSummaryDto();
        }

        public async Task<HashSet<DateTime>> GetHolidayDatesAsync()
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT HolidayDate FROM Holidays WHERE IsActive = 1;";
            var dates = await connection.QueryAsync<DateTime>(sql);
            return new HashSet<DateTime>(dates.Select(d => d.Date));
        }
    }
}
