using EmpLMS.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmpLMS.Models.ViewModels
{
    public class ManageEmployeesViewModel
    {
        public PaginatedList<EmployeeListItemDto> Employees { get; set; } = new PaginatedList<EmployeeListItemDto>();
        public AddEmployeeViewModel NewEmployee { get; set; } = new AddEmployeeViewModel();
        public IEnumerable<SelectListItem> DepartmentList { get; set; } = new List<SelectListItem>();
    }
}
