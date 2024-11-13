using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Models.DTO.Employee;
using Preventyon.Repository.IRepository;


namespace Preventyon.Repository
{


    public class EmployeeRepository : IEmployeeRepository

    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;
        public EmployeeRepository(ApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        public async Task<Employee> AddEmployee(CreateEmployeeDTO employeedto)
        {
            Employee employee = _mapper.Map<Employee>(employeedto);
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<List<GetEmployeesDTO>> GetEmployees(bool isForAddAdmins)
        {
            List<Employee> employees;

            if (!isForAddAdmins)
            {
                employees = _context.Employees.ToList();
            }
            else
            {
                var employeesNotInAdminIds = _context.Employees
                                .Where(e => e.RoleId == 1 &&
                                            !_context.Admins.Select(a => a.EmployeeId).Contains(e.Id))
                                .Select(e => e.Id)
                                .ToList();
                employees = _context.Employees
                                        .Where(e => !employeesNotInAdminIds.Contains(e.Id))
                                        .ToList();
            }
            return _mapper.Map<List<GetEmployeesDTO>>(employees);
        }

        public async Task<Employee> FindEmployeeAsync(int id)
        {

            var employee = await _context.Employees
             .FirstOrDefaultAsync(e => e.Id == id);

            return employee;
        }
        public async Task<GetEmployeeRoleWithIDDTO> GetEmployeeByIdAsync(int id)
        {

            var employee = await _context.Employees
             .Include(e => e.Role)
             .ThenInclude(r => r.Permission)
             .FirstOrDefaultAsync(e => e.Id == id);

            return _mapper.Map<GetEmployeeRoleWithIDDTO>(employee);
        }
        public async Task<IEnumerable<Employee>> GetEmployeesByRolesAsync()
        {
            return await _context.Employees
                .Include(e => e.Role)  // Include Role entity if needed
                .Where(e => e.Role.Name == "SuperAdmin" || e.Role.Name == "Admin-Incidents")
                .ToListAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }
        public async Task<Employee> FindAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }
        public async Task<int> GetEmployeeByName(string name)
        {
            var employee = await _context.Employees
                                 .Where(e => e.Name.ToLower() == name.ToLower()) // Case-insensitive search
                                 .FirstOrDefaultAsync();

            // If the employee is found, return their ID, otherwise return null
            if (employee != null)
            {
                return employee.Id; // Assuming Id is an int
            }
            return 0;
        }

    }
}
