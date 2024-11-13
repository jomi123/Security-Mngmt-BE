using Preventyon.Models;
using Preventyon.Models.DTO.AdminDTO;

namespace Preventyon.Service.IService
{
    public interface IAdminService
    {

        Task<IEnumerable<GetAllAdminsDto>> GetAllAdminsAsync(int Id);
        Task<Admin> AddAdminAsync(CreateAdminDTO createAdminDTO);
        Task UpdateAdminAsync(UpdateAdminDTO updateAdmin);
    }
}
