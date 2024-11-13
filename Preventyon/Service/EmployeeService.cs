using Microsoft.IdentityModel.Tokens;
using Preventyon.Data;
using Preventyon.Models.DTO.Employee;
using Preventyon.Models;
using Preventyon.Repository;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Preventyon.Service.IService;
using Preventyon.Repository.IRepository;

namespace Preventyon.Service
{
    public class EmployeeService: IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(EmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Employee> AddEmployee(CreateEmployeeDTO employeedto)
        {
            return await _repository.AddEmployee(employeedto);
        }

        public async Task<List<GetEmployeesDTO>> GetEmployees(bool isForAddAdmins)
        {
            return await _repository.GetEmployees(isForAddAdmins);
        }

        public async Task<GetEmployeeRoleWithIDDTO> GetEmployeeByIdAsync(int id)
        {
            return await _repository.GetEmployeeByIdAsync(id);
        }

        public async Task<GetEmployeeRoleWithIDDTO> GetEmployeeByTokenAsync(string token, ApiContext context)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;

            if (tokenS == null)
                throw new SecurityTokenException("Invalid token");

            var claims = tokenS.Claims;
            var upn = claims.FirstOrDefault(c => c.Type == "upn")?.Value;

            if (string.IsNullOrEmpty(upn))
                throw new SecurityTokenException("UPN claim not found in token");

            var employeeData = await context.Employees.FirstOrDefaultAsync(e => e.Email == upn);

            if (employeeData == null)
                throw new KeyNotFoundException("Employee not found");

            return await _repository.GetEmployeeByIdAsync(employeeData.Id);
        }
    }
}
