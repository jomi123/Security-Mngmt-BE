using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Models.DTO.Employee;
using Preventyon.Service;
using Preventyon.Service.IService;
using Serilog;

namespace Preventyon.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[Action]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly ApiContext _context;
        private readonly ILogger<EmployeeController> _logger;
        private readonly AccessTokenService _accessTokenService;

        public EmployeeController(IEmployeeService service, ApiContext context, AccessTokenService accessTokenService, ILogger<EmployeeController> logger)
        {
            _service = service;
            _context = context;
            _logger = logger;
            _accessTokenService = accessTokenService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // not used
        public async Task<IActionResult> PostEmployee([FromBody] CreateEmployeeDTO employeedto)
        {
            _logger.LogInformation("Attempting to add a new employee.");

            try
            {
                Employee employee = await _service.AddEmployee(employeedto);
                _logger.LogInformation("Employee added successfully with ID {EmployeeId}.", employee.Id);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new employee.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{isForAddAdmins}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = "AdminsUserOrAdminIncidentsOrSuperAdmin")] //user not
        public async Task<IActionResult> GetEmployees(bool isForAddAdmins)
        {
            _logger.LogInformation("Fetching all employees.");

            try
            {
                List<GetEmployeesDTO> getEmployees = await _service.GetEmployees(isForAddAdmins);
                _logger.LogInformation("Successfully fetched {Count} employees.", getEmployees.Count);
                return Ok(getEmployees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching employees.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //not used
        public async Task<IActionResult> GetEmployeeByIdAsync(int id)
        {
            _logger.LogInformation("Fetching employee with ID {EmployeeId}.", id);

            try
            {
                GetEmployeeRoleWithIDDTO employee = await _service.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    _logger.LogWarning("Employee with ID {EmployeeId} not found.", id);
                    return NotFound($"Employee with ID {id} not found.");
                }

                _logger.LogInformation("Successfully fetched employee with ID {EmployeeId}.", id);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching employee with ID {EmployeeId}.", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getUserRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmployeeByTokenAsync()
        {
            _logger.LogInformation("Attempting to fetch employee role based on token.");

            var jwtStream = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(jwtStream))
            {
                _logger.LogWarning("Authorization header missing or invalid.");
                return BadRequest("Authorization header missing or invalid");
            }

            try
            {
                var employee = await _service.GetEmployeeByTokenAsync(jwtStream, _context);
                _logger.LogInformation("Successfully fetched employee role based on token.");
                var token = _accessTokenService.GetAccessToken(employee.Email, employee.Role.Name);
                Response.Headers.Append("AccessToken", token);
                return Ok(employee);
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Security token error occurred.");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Employee not found for the provided token.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching employee role based on token.");
                return BadRequest(ex.Message);
            }
        }
    }
}
