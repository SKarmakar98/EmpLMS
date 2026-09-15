using System.Security.Claims;
using EmpLMS.Data.Repositories;
using EmpLMS.Models.DTOs;
using EmpLMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmpLMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminRepository _adminRepo;

        public AdminController(IAdminRepository adminRepo)
        {
            _adminRepo = adminRepo;
        }

        private int GetCurrentUserId()
        {
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(claimValue, out int userId))
                return userId;
            
            throw new UnauthorizedAccessException("Admin User ID claim is missing or invalid.");
        }

        // GET: /Admin/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var stats = await _adminRepo.GetDashboardStatsAsync();
            var allLeaves = await _adminRepo.GetAllLeaveRequestsAsync();

            var viewModel = new AdminDashboardViewModel
            {
                Stats = stats,
                RecentLeaves = allLeaves.Take(6)
            };

            return View(viewModel);
        }

        // GET: /Admin/Leaves?status=Pending&page=1
        [HttpGet]
        public async Task<IActionResult> Leaves(string? status = "All", int page = 1)
        {
            int pageSize = 5;
            var leaves = await _adminRepo.GetAllLeaveRequestsAsync(status);

            var viewModel = new AdminLeavesViewModel
            {
                Leaves = PaginatedList<AdminLeaveRequestDto>.Create(leaves, page, pageSize),
                CurrentFilter = string.IsNullOrWhiteSpace(status) ? "All" : status
            };

            return View(viewModel);
        }

        // POST: /Admin/ApproveLeave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveLeave(int id, string? adminRemarks, string? returnUrl)
        {
            try
            {
                int adminId = GetCurrentUserId();
                await _adminRepo.UpdateLeaveStatusAsync(id, "Approved", adminRemarks, adminId);
                TempData["SuccessMessage"] = "Leave request has been approved.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to approve leave request: " + ex.Message;
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Dashboard));
        }

        // POST: /Admin/RejectLeave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectLeave(int id, string? adminRemarks, string? returnUrl)
        {
            try
            {
                int adminId = GetCurrentUserId();
                await _adminRepo.UpdateLeaveStatusAsync(id, "Rejected", adminRemarks, adminId);
                TempData["WarningMessage"] = "Leave request has been rejected.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to reject leave request: " + ex.Message;
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Dashboard));
        }

        // GET: /Admin/Employees?page=1
        [HttpGet]
        public async Task<IActionResult> Employees(int page = 1)
        {
            int pageSize = 5;
            var employees = await _adminRepo.GetAllEmployeesAsync();
            var departments = await _adminRepo.GetDepartmentsAsync();

            var viewModel = new ManageEmployeesViewModel
            {
                Employees = PaginatedList<EmployeeListItemDto>.Create(employees, page, pageSize),
                DepartmentList = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
            };

            return View(viewModel);
        }

        // POST: /Admin/AddEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee(ManageEmployeesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                int pageSize = 5;
                var employees = await _adminRepo.GetAllEmployeesAsync();
                var departments = await _adminRepo.GetDepartmentsAsync();
                
                model.Employees = PaginatedList<EmployeeListItemDto>.Create(employees, 1, pageSize);
                model.DepartmentList = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                });
                
                return View("Employees", model);
            }

            var (success, message) = await _adminRepo.AddEmployeeAsync(model.NewEmployee);
            if (success)
            {
                TempData["SuccessMessage"] = "Employee created successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Employees));
        }

        // GET: /Admin/EditEmployee/5
        [HttpGet]
        public async Task<IActionResult> EditEmployee(int id)
        {
            var emp = await _adminRepo.GetEmployeeForEditAsync(id);
            if (emp == null)
            {
                TempData["ErrorMessage"] = "Employee not found.";
                return RedirectToAction(nameof(Employees));
            }

            return View(emp);
        }

        // POST: /Admin/EditEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployee(EditEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _adminRepo.GetDepartmentsAsync();
                model.DepartmentList = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name,
                    Selected = d.Id == model.DepartmentId
                });
                return View(model);
            }

            var (success, message) = await _adminRepo.UpdateEmployeeAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Employee details updated successfully!";
                return RedirectToAction(nameof(Employees));
            }

            ModelState.AddModelError(string.Empty, message);
            var deptList = await _adminRepo.GetDepartmentsAsync();
            model.DepartmentList = deptList.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name,
                Selected = d.Id == model.DepartmentId
            });

            return View(model);
        }

        // POST: /Admin/ToggleEmployeeStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEmployeeStatus(int id)
        {
            try
            {
                await _adminRepo.ToggleEmployeeStatusAsync(id);
                TempData["SuccessMessage"] = "Employee status updated.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to update employee status: " + ex.Message;
            }
            return RedirectToAction(nameof(Employees));
        }
    }
}
