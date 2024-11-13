using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Preventyon.Data;
using Preventyon.Hubs;
using Preventyon.Models;
using Preventyon.Models.DTO.AdminDTO;
using Preventyon.Models.DTO.Employee;
using Preventyon.Repository;
using Preventyon.Repository.IRepository;
using Preventyon.Service.IService;

 

namespace Preventyon.Service
{
    public class AdminService : IAdminService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IHubContext<IncidentHub> _hubContext;
        public AdminService(ApiContext context, IEmployeeRepository employeeRepository, IAdminRepository adminRepository, IHubContext<IncidentHub> hubContext)
        {
            _employeeRepository = employeeRepository;
            _adminRepository = adminRepository;
            _hubContext = hubContext;
        }

 

        public async Task<IEnumerable<GetAllAdminsDto>> GetAllAdminsAsync(int Id)
        {
            return await _adminRepository.GetAllAdminsAsync(Id);
        }

 

        public async Task<Admin> AddAdminAsync(CreateAdminDTO createAdminDTO)
        {
            var existingEmployee = await _employeeRepository.FindAsync(createAdminDTO.EmployeeId);
            if (existingEmployee == null)
            {
                throw new Exception("Employee not found");
            }

 

            try
            {
                existingEmployee.RoleId = AssignRole(createAdminDTO.isIncidentMangenet, createAdminDTO.isUserMangenet);
                await _employeeRepository.UpdateAsync(existingEmployee);

 

                var admin = new Admin
                {
                    EmployeeId = createAdminDTO.EmployeeId,
                    AssignedBy = createAdminDTO.AssignedBy,
                    Status = createAdminDTO.Status,
                };

 

                await _adminRepository.AddAdminAsync(admin);
                await _hubContext.Clients.All.SendAsync("ReceiveAdminUpdate");
                return admin;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

 

        public async Task UpdateAdminAsync(UpdateAdminDTO updateAdmin)
        {
            var admin = await _adminRepository.GetAdminByIdAsync(updateAdmin.AdminId);
            if (admin == null)
            {
                throw new Exception("Admin not found");
            }

 

            var employee = await _employeeRepository.FindEmployeeAsync(admin.EmployeeId);
            if (employee == null)
            {
                throw new Exception("Employee not found");
            }

 

            employee.RoleId = AssignRole(updateAdmin.isIncidentMangenet, updateAdmin.isUserMangenet);

 

            if (!updateAdmin.Status)
            {
                employee.RoleId = 4;
            }

 

            await _employeeRepository.UpdateAsync(employee);

 

            admin.Status = updateAdmin.Status;
            await _adminRepository.UpdateAdminAsync(admin);
            await _hubContext.Clients.All.SendAsync("ReceiveAdminUpdate");
        }

 

 

        private int AssignRole(bool isIncidentManagement, bool isUserManagement)
        {
            if (isIncidentManagement && isUserManagement) return 1;
            if (isIncidentManagement) return 3;
            if (isUserManagement) return 2;
            return 4; 
        }
    }

 

}