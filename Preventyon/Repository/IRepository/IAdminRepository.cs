using Preventyon.Models;
using Preventyon.Models.DTO.AdminDTO;

namespace Preventyon.Repository.IRepository
{
    public interface IAdminRepository
    {
        Task<Admin> GetAdminByIdAsync(int id);
        Task<IEnumerable<GetAllAdminsDto>> GetAllAdminsAsync(int employeeId);
        Task<IEnumerable<Admin>> GetAllAdmins();
        Task AddAdminAsync(Admin admin);
        Task UpdateAdminAsync(Admin admin);
        Task DeleteAdminAsync(int id);
    }
}
