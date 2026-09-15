using EmpLMS.Models.DTOs;

namespace EmpLMS.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public AdminDashboardStatsDto Stats { get; set; } = new AdminDashboardStatsDto();
        public IEnumerable<AdminLeaveRequestDto> RecentLeaves { get; set; } = new List<AdminLeaveRequestDto>();
    }
}
