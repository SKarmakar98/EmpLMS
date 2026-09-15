using EmpLMS.Models.DTOs;

namespace EmpLMS.Data.Repositories
{
    public interface ILeaveRepository
    {
        Task<IEnumerable<LeavePurposeDto>> GetActiveLeavePurposesAsync();
        Task<int> ApplyLeaveAsync(int employeeId, int leavePurposeId, DateTime fromDate, DateTime toDate, decimal totalDays, string remarks);
        Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<LeaveRequestHistoryDto>> GetEmployeeLeaveHistoryAsync(int employeeId);
        Task<LeaveSummaryDto> GetEmployeeLeaveSummaryAsync(int employeeId);
        Task<HashSet<DateTime>> GetHolidayDatesAsync();
    }
}
