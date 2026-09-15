using EmpLMS.Data.Repositories;
using EmpLMS.Models.DTOs;
using EmpLMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmpLMS.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly ILeaveRepository _leaveRepo;

        public EmployeeController(ILeaveRepository leaveRepo)
        {
            _leaveRepo = leaveRepo;
        }

        private int GetLoggedInEmployeeId()
        {
            var claimValue = User.FindFirst("EmployeeId")?.Value;
            if (int.TryParse(claimValue, out int empId))
                return empId;

            throw new UnauthorizedAccessException("Employee ID claim is missing or invalid.");
        }

        // GET: /Employee/Dashboard?page=1
        [HttpGet]
        public async Task<IActionResult> Dashboard(int page = 1)
        {
            int empId = GetLoggedInEmployeeId();
            int pageSize = 5;
            var summary = await _leaveRepo.GetEmployeeLeaveSummaryAsync(empId);
            var history = await _leaveRepo.GetEmployeeLeaveHistoryAsync(empId);

            var viewModel = new EmployeeDashboardViewModel
            {
                Summary = summary,
                LeaveHistory = PaginatedList<LeaveRequestHistoryDto>.Create(history, page, pageSize)
            };

            return View(viewModel);
        }

        // GET: /Employee/ApplyLeave
        [HttpGet]
        public async Task<IActionResult> ApplyLeave()
        {
            int empId = GetLoggedInEmployeeId();
            var purposes = await _leaveRepo.GetActiveLeavePurposesAsync();
            var summary = await _leaveRepo.GetEmployeeLeaveSummaryAsync(empId);
            var holidays = await _leaveRepo.GetHolidayDatesAsync();

            ViewBag.HolidayDatesJson = System.Text.Json.JsonSerializer.Serialize(holidays.Select(h => h.ToString("yyyy-MM-dd")));

            var model = new ApplyLeaveViewModel
            {
                FromDate = DateTime.Today,
                ToDate = DateTime.Today,
                AvailableLeaves = summary.AvailableLeaves,
                PurposeList = purposes.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Purpose
                })
            };

            return View(model);
        }

        // POST: /Employee/ApplyLeave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyLeave(ApplyLeaveViewModel model)
        {
            int empId = GetLoggedInEmployeeId();
            var holidays = await _leaveRepo.GetHolidayDatesAsync();

            // Validate dates
            if (model.FromDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(model.FromDate), "From Date cannot be in the past.");
            }

            if (model.ToDate.Date < model.FromDate.Date)
            {
                ModelState.AddModelError(nameof(model.ToDate), "To Date cannot be earlier than From Date.");
            }

            // Calculate working days (excluding Saturday, Sunday, and Public Holidays)
            decimal totalDays = 0;
            for (var d = model.FromDate.Date; d <= model.ToDate.Date; d = d.AddDays(1))
            {
                if (d.DayOfWeek != DayOfWeek.Saturday && 
                    d.DayOfWeek != DayOfWeek.Sunday && 
                    !holidays.Contains(d))
                {
                    totalDays++;
                }
            }

            if (model.FromDate.Date <= model.ToDate.Date && totalDays <= 0)
            {
                ModelState.AddModelError(string.Empty, "Selected date range falls entirely on weekends (Saturday/Sunday) or company holidays. Please select at least 1 working day.");
            }

            // Prevent overlapping leave requests
            if (model.FromDate.Date <= model.ToDate.Date && await _leaveRepo.HasOverlappingLeaveAsync(empId, model.FromDate, model.ToDate))
            {
                ModelState.AddModelError(string.Empty, "You already have a Pending or Approved leave request overlapping with the selected dates.");
            }

            // Ensure employee has sufficient balance
            var currentSummary = await _leaveRepo.GetEmployeeLeaveSummaryAsync(empId);
            if (totalDays > currentSummary.AvailableLeaves)
            {
                ModelState.AddModelError(string.Empty, $"You requested {totalDays} working day(s), but only have {currentSummary.AvailableLeaves} day(s) of leave remaining in your quota.");
            }

            if (!ModelState.IsValid)
            {
                // Reload purpose options if validation fails
                var purposes = await _leaveRepo.GetActiveLeavePurposesAsync();
                model.AvailableLeaves = currentSummary.AvailableLeaves;
                model.PurposeList = purposes.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Purpose
                });
                ViewBag.HolidayDatesJson = System.Text.Json.JsonSerializer.Serialize(holidays.Select(h => h.ToString("yyyy-MM-dd")));

                return View(model);
            }

            try
            {
                // Execute stored procedure via Dapper
                await _leaveRepo.ApplyLeaveAsync(
                    employeeId: empId,
                    leavePurposeId: model.LeavePurposeId!.Value,
                    fromDate: model.FromDate,
                    toDate: model.ToDate,
                    totalDays: totalDays,
                    remarks: model.Remarks
                );

                string successMsg = $"Your leave request for {totalDays} working day(s) has been submitted successfully!";
                TempData["SuccessMessage"] = successMsg;

                return RedirectToAction(nameof(Dashboard));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Failed to submit leave request: " + ex.Message);
                var purposes = await _leaveRepo.GetActiveLeavePurposesAsync();
                model.PurposeList = purposes.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Purpose
                });
                ViewBag.HolidayDatesJson = System.Text.Json.JsonSerializer.Serialize(holidays.Select(h => h.ToString("yyyy-MM-dd")));

                return View(model);
            }
        }
    }
}
