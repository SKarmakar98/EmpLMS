using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmpLMS.Models.ViewModels
{
    public class EditEmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        // Optional password reset
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Designation is required.")]
        [StringLength(100)]
        public string Designation { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ContactNumber { get; set; }

        [Range(1, 100)]
        public int TotalLeaveQuota { get; set; } = 20;

        public bool IsActive { get; set; } = true;

        public IEnumerable<SelectListItem> DepartmentList { get; set; } = new List<SelectListItem>();
    }
}
