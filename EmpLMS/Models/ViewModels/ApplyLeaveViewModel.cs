using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmpLMS.Models.ViewModels
{
    public class ApplyLeaveViewModel
    {
        [Required(ErrorMessage = "Please select a leave purpose.")]
        [Display(Name = "Leave Purpose")]
        public int? LeavePurposeId { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please provide a reason or remarks for your leave.")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        [Display(Name = "Reason / Remarks")]
        public string Remarks { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> PurposeList { get; set; } = new List<SelectListItem>();

        public decimal AvailableLeaves { get; set; }
    }
}
