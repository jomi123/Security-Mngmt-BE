using Preventyon.Data;
using Preventyon.Models.DTO.Employee;
using Preventyon.Models;

namespace Preventyon.Service.IService
{
    public interface IEmployeeService
    {
        Task<Employee> AddEmployee(CreateEmployeeDTO employeedto);
        Task<List<GetEmployeesDTO>> GetEmployees(bool isForAddAdmins);
        Task<GetEmployeeRoleWithIDDTO> GetEmployeeByIdAsync(int id);
        Task<GetEmployeeRoleWithIDDTO> GetEmployeeByTokenAsync(string token, ApiContext context);
    }
}
