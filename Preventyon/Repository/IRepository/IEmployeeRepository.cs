using Preventyon.Models;
using Preventyon.Models.DTO.Employee;

namespace Preventyon.Repository.IRepository
{
    public interface IEmployeeRepository
    {
        Task<List<GetEmployeesDTO>> GetEmployees(bool isForAddAdmins);
        Task<Employee> AddEmployee(CreateEmployeeDTO employeedto);
        Task<Employee> FindEmployeeAsync(int id);
        Task<GetEmployeeRoleWithIDDTO> GetEmployeeByIdAsync(int employeeId);
        Task<IEnumerable<Employee>> GetEmployeesByRolesAsync();
        Task UpdateAsync(Employee employee);
        Task<Employee> FindAsync(int id);
        Task<int> GetEmployeeByName(string name);
    }
}
