namespace EmpLMS.Models.DTOs
{
    public class LeaveSummaryDto
    {
        public int TotalQuota { get; set; } = 20;
        public decimal UsedLeaves { get; set; }
        public decimal PendingLeaves { get; set; }
        public decimal AvailableLeaves { get; set; }
    }
}
