namespace EmpLMS.Models.ViewModels
{
    public class LoginUserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DepartmentName { get; set; }
        public string? Designation { get; set; }

        public string FullName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(FirstName))
                    return $"{FirstName} {LastName}".Trim();
                return Role;
            }
        }
    }
}
