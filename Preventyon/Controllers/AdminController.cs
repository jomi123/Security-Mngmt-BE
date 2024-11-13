using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Preventyon.Models.DTO.AdminDTO;
using Preventyon.Service.IService;
using Serilog;

namespace Preventyon.Controllers
{
    [ApiController]
    [Route("api/[controller]/[Action]")]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminsController> _logger;

        public AdminsController(IAdminService adminService, ILogger<AdminsController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = "AdminUserOrSuperAdmin")]
        public async Task<ActionResult<IEnumerable<GetAllAdminsDto>>> GetAllAdmins(int Id)
        {
            _logger.LogInformation("Getting all admins for ID {Id}", Id);

            try
            {
                var admins = await _adminService.GetAllAdminsAsync(Id);
                return Ok(admins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting admins for ID {Id}", Id);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = "AdminUserOrSuperAdmin")]
        public async Task<IActionResult> AddAdmin(CreateAdminDTO createAdminDTO)
        {
            _logger.LogInformation("Adding a new admin");

            try
            {
                var admin = await _adminService.AddAdminAsync(createAdminDTO);
                _logger.LogInformation("Admin created successfully with ID {Id}", admin.AdminId);
                return Ok(admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new admin");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{adminId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = "AdminUserOrSuperAdmin")]
        public async Task<IActionResult> UpdateAdmin(int adminId, UpdateAdminDTO updateAdmin)
        {
            _logger.LogInformation("Updating admin with ID {adminId}", adminId);

            try
            {
                await _adminService.UpdateAdminAsync(updateAdmin);
                _logger.LogInformation("Admin with ID {adminId} updated successfully", adminId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating admin with ID {adminId}", adminId);
                return BadRequest(ex.Message);
            }
        }
    }
}
