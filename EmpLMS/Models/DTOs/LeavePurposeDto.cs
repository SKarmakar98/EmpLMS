namespace EmpLMS.Models.DTOs
{
    public class LeavePurposeDto
    {
        public int Id { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public int DefaultDaysPerYear { get; set; }
    }
}
