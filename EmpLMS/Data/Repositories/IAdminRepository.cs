using EmpLMS.Models.DTOs;
using EmpLMS.Models.ViewModels;

namespace EmpLMS.Data.Repositories
{
    public interface IAdminRepository
    {
        Task<AdminDashboardStatsDto> GetDashboardStatsAsync();
        Task<IEnumerable<AdminLeaveRequestDto>> GetAllLeaveRequestsAsync(string? statusFilter = null);
        Task<bool> UpdateLeaveStatusAsync(int leaveRequestId, string status, string? adminRemarks, int reviewedByUserId);
        Task<IEnumerable<EmployeeListItemDto>> GetAllEmployeesAsync();
        Task<EditEmployeeViewModel?> GetEmployeeForEditAsync(int employeeId);
        Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync();
        Task<(bool Success, string Message)> AddEmployeeAsync(AddEmployeeViewModel model);
        Task<(bool Success, string Message)> UpdateEmployeeAsync(EditEmployeeViewModel model);
        Task<bool> ToggleEmployeeStatusAsync(int employeeId);
    }
}
