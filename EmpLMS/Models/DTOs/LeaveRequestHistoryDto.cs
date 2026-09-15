namespace EmpLMS.Models.DTOs
{
    public class LeaveRequestHistoryDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string LeavePurposeName { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalDays { get; set; }
        public string? Remarks { get; set; }
        public string Status { get; set; } = "Pending";
        public string? AdminRemarks { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
