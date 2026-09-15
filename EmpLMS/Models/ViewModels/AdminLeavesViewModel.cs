using EmpLMS.Models.DTOs;

namespace EmpLMS.Models.ViewModels
{
    public class AdminLeavesViewModel
    {
        public PaginatedList<AdminLeaveRequestDto> Leaves { get; set; } = new PaginatedList<AdminLeaveRequestDto>();
        public string CurrentFilter { get; set; } = "All";
    }
}
