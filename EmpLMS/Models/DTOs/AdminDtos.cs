namespace EmpLMS.Models.DTOs
{
    public class AdminDashboardStatsDto
    {
        public int TotalEmployees { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
    }

    public class AdminLeaveRequestDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string LeavePurposeName { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalDays { get; set; }
        public string? Remarks { get; set; }
        public string Status { get; set; } = "Pending";
        public string? AdminRemarks { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public string? ReviewedByName { get; set; }
    }

    public class EmployeeListItemDto
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? Designation { get; set; }
        public string? ContactNumber { get; set; }
        public int TotalLeaveQuota { get; set; }
        public DateTime DateOfJoining { get; set; }
        public bool IsActive { get; set; }
        public decimal LeavesTaken { get; set; }
    }

    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
