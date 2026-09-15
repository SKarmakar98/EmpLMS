using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmpLMS.Models.ViewModels
{
    public class AddEmployeeViewModel
    {
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Initial Password is required.")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Password must be at least 4 characters.")]
        public string Password { get; set; } = "emp123";

        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Designation is required.")]
        [StringLength(100)]
        public string Designation { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? ContactNumber { get; set; }

        [Range(1, 100, ErrorMessage = "Leave Quota must be between 1 and 100.")]
        public int TotalLeaveQuota { get; set; } = 20;

        public IEnumerable<SelectListItem> DepartmentList { get; set; } = new List<SelectListItem>();
    }
}
