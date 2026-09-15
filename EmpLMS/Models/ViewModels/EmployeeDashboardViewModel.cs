using EmpLMS.Models.DTOs;

namespace EmpLMS.Models.ViewModels
{
    public class EmployeeDashboardViewModel
    {
        public LeaveSummaryDto Summary { get; set; } = new LeaveSummaryDto();
        public PaginatedList<LeaveRequestHistoryDto> LeaveHistory { get; set; } = new PaginatedList<LeaveRequestHistoryDto>();
    }
}
