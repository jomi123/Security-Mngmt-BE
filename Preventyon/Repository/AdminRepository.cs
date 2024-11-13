using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Models.DTO.AdminDTO;
using Preventyon.Repository.IRepository;
using System.Linq;

namespace Preventyon.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public AdminRepository(ApiContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Admin> GetAdminByIdAsync(int id)
        {
            return await _context.Admins
                .Include(a => a.Employee)
                .ThenInclude(e => e.Role)
                .FirstOrDefaultAsync(a => a.AdminId == id);
        }

       public async Task<IEnumerable<GetAllAdminsDto>> GetAllAdminsAsync(int employeeId)
        {
            var employeesNotInAdminIds = _context.Employees
                .Where(e => e.RoleId == 1 &&
                            !_context.Admins.Select(a => a.EmployeeId).Contains(e.Id))
                .Select(e => e.Id)
                .ToList();

            bool employeeExists = employeesNotInAdminIds.Any(e => e == employeeId);

            if (employeeExists)
            {

                var admins = await _context.Admins
                                      .Include(a => a.Employee)
                                      .ThenInclude(e => e.Role)
                                      .ThenInclude(r => r.Permission)
                                      .ToListAsync();


                return admins.Select(a => new GetAllAdminsDto
                {
                    AdminId = a.AdminId,
                    EmployeeName = a.Employee.Name,
                    AssignedOn = a.AssignedOn,
                    AssignedBy = _context.Employees.FindAsync(a.AssignedBy).Result.Name,
                    isIncidentMangenet = a.Employee.Role.Permission.IncidentManagement,
                    isUserMangenet = a.Employee.Role.Permission.UserManagement,
                    Status = a.Status
                }).ToList();
            }
            else
            {

                var admins = await _context.Admins
                    .Where(a => !employeesNotInAdminIds.Contains(a.AssignedBy))
                    .Include(a => a.Employee) // Include related Employee data
                    .ThenInclude(e => e.Role)
                    .ThenInclude(r => r.Permission)
                    .ToListAsync();

                return admins.Select(a => new GetAllAdminsDto
                {
                    AdminId = a.AdminId,
                    EmployeeName = a.Employee.Name,
                    AssignedOn = a.AssignedOn,
                    AssignedBy = _context.Employees.FindAsync(a.AssignedBy).Result.Name,
                    isIncidentMangenet = a.Employee.Role.Permission.IncidentManagement,
                    isUserMangenet = a.Employee.Role.Permission.UserManagement,
                    Status = a.Status
                }).ToList();
            }




  
        }
        public async Task<IEnumerable<Admin>> GetAllAdmins()
        {
            return await _context.Admins
                .Include(a => a.Employee) 
                .ToListAsync();
        }

        public async Task AddAdminAsync(Admin admin)
        {
           await  _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAdminAsync(Admin admin)
        {
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAdminAsync(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            }
        }

    }
}
