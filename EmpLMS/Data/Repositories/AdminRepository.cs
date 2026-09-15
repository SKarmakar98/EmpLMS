using System.Data;
using Dapper;
using EmpLMS.Helpers;
using EmpLMS.Models.DTOs;
using EmpLMS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmpLMS.Data.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DapperContext _context;

        public AdminRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardStatsDto> GetDashboardStatsAsync()
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<AdminDashboardStatsDto>(
                "sp_GetAdminDashboardStats",
                commandType: CommandType.StoredProcedure
            );
            return result ?? new AdminDashboardStatsDto();
        }

        public async Task<IEnumerable<AdminLeaveRequestDto>> GetAllLeaveRequestsAsync(string? statusFilter = null)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@StatusFilter", string.IsNullOrWhiteSpace(statusFilter) || statusFilter == "All" ? null : statusFilter, DbType.String);

            return await connection.QueryAsync<AdminLeaveRequestDto>(
                "sp_GetAllLeaveRequests",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> UpdateLeaveStatusAsync(int leaveRequestId, string status, string? adminRemarks, int reviewedByUserId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@LeaveRequestId", leaveRequestId, DbType.Int32);
            parameters.Add("@Status", status, DbType.String);
            parameters.Add("@AdminRemarks", adminRemarks?.Trim() ?? (status == "Approved" ? "Approved by Admin" : "Rejected by Admin"), DbType.String);
            parameters.Add("@ReviewedByUserId", reviewedByUserId, DbType.Int32);

            int rows = await connection.ExecuteAsync(
                "sp_UpdateLeaveStatus",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }

        public async Task<IEnumerable<EmployeeListItemDto>> GetAllEmployeesAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<EmployeeListItemDto>(
                "sp_GetAllEmployees",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<EditEmployeeViewModel?> GetEmployeeForEditAsync(int employeeId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@EmployeeId", employeeId, DbType.Int32);

            var emp = await connection.QuerySingleOrDefaultAsync<EditEmployeeViewModel>(
                "sp_GetEmployeeById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (emp != null)
            {
                var departments = await GetDepartmentsAsync();
                emp.DepartmentList = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name,
                    Selected = d.Id == emp.DepartmentId
                });
            }

            return emp;
        }

        public async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<DepartmentDto>(
                "sp_GetDepartments",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<(bool Success, string Message)> AddEmployeeAsync(AddEmployeeViewModel model)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@Email", model.Email.Trim(), DbType.String);
                // Hash password using PasswordHelper
                parameters.Add("@Password", PasswordHelper.HashPassword(model.Password.Trim()), DbType.String);
                parameters.Add("@FirstName", model.FirstName.Trim(), DbType.String);
                parameters.Add("@LastName", model.LastName.Trim(), DbType.String);
                parameters.Add("@DepartmentId", model.DepartmentId, DbType.Int32);
                parameters.Add("@Designation", model.Designation.Trim(), DbType.String);
                parameters.Add("@ContactNumber", model.ContactNumber?.Trim(), DbType.String);
                parameters.Add("@TotalLeaveQuota", model.TotalLeaveQuota, DbType.Int32);

                int newId = await connection.ExecuteScalarAsync<int>(
                    "sp_AddEmployee",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return (true, "Employee added successfully!");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> UpdateEmployeeAsync(EditEmployeeViewModel model)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeId", model.EmployeeId, DbType.Int32);
                parameters.Add("@Email", model.Email.Trim(), DbType.String);
                parameters.Add("@Password", string.IsNullOrWhiteSpace(model.NewPassword) ? null : PasswordHelper.HashPassword(model.NewPassword.Trim()), DbType.String);
                parameters.Add("@FirstName", model.FirstName.Trim(), DbType.String);
                parameters.Add("@LastName", model.LastName.Trim(), DbType.String);
                parameters.Add("@DepartmentId", model.DepartmentId, DbType.Int32);
                parameters.Add("@Designation", model.Designation.Trim(), DbType.String);
                parameters.Add("@ContactNumber", model.ContactNumber?.Trim(), DbType.String);
                parameters.Add("@TotalLeaveQuota", model.TotalLeaveQuota, DbType.Int32);
                parameters.Add("@IsActive", model.IsActive, DbType.Boolean);

                await connection.ExecuteAsync(
                    "sp_UpdateEmployee",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return (true, "Employee updated successfully!");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<bool> ToggleEmployeeStatusAsync(int employeeId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@EmployeeId", employeeId, DbType.Int32);

            int result = await connection.ExecuteScalarAsync<int>(
                "sp_ToggleEmployeeStatus",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }
    }
}
