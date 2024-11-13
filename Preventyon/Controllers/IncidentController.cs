using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Preventyon.Models;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Service.IService;

namespace Preventyon.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[Action]")]
    public class IncidentController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        private readonly IEmployeeService _employeeService;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<IncidentController> _logger;

        public IncidentController(IIncidentService incidentService, IEmployeeService employeeService, IEmailService emailService, ILogger<IncidentController> logger, INotificationService notificationService)
        {
            _incidentService = incidentService;
            _employeeService = employeeService;
            _emailService = emailService;
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Incident>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //not used
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncidents()
        {
            _logger.LogInformation("Fetching all incidents.");
            try
            {
                var incidents = await _incidentService.GetAllIncidents();
                _logger.LogInformation("Successfully fetched {Count} incidents.", incidents.Count());
                return Ok(incidents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching incidents.");
                return BadRequest("Failed to fetch incidents.");
            }
        }

        [HttpGet("{employeeId}/{user}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetIncidentsByEmployeeID))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = "AllowAll")]
        public async Task<IActionResult> GetIncidentsByEmployeeId(int employeeId, bool user)
        {
            _logger.LogInformation("Fetching incidents for EmployeeId: {EmployeeId}, IsUser: {IsUser}", employeeId, user);
            try
            {
                if (!user)
                {
                    var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
                    if (employee.Role.Name == "SuperAdmin" || employee.Role.Name == "Admins-User" || employee.Role.Name == "Admin-Incidents")
                    {
                        _logger.LogInformation("Fetching incidents for admins.");
                        var adminIncidents = await _incidentService.GetIncidentsAdmins();
                        return Ok(adminIncidents);
                    }
                }

                var incidents = await _incidentService.GetIncidentsByEmployeeId(employeeId);
                _logger.LogInformation("Successfully fetched incidents for EmployeeId: {EmployeeId}.", employeeId);
                return Ok(incidents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching incidents for EmployeeId: {EmployeeId}.", employeeId);
                return BadRequest("Failed to fetch incidents.");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Incident))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = "AllowAll")]
        public async Task<ActionResult<Incident>> GetIncident(int id)
        {
            _logger.LogInformation("Fetching incident with ID: {IncidentId}", id);
            try
            {
                var incident = await _incidentService.GetIncidentById(id);

                if (incident == null)
                {
                    _logger.LogWarning("Incident with ID: {IncidentId} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Successfully fetched incident with ID: {IncidentId}.", id);
                return Ok(incident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching incident with ID: {IncidentId}.", id);
                return BadRequest("Failed to fetch incident.");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Incident))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "AllowAll")] //user
        public async Task<ActionResult<Incident>> CreateIncident([FromForm] CreateIncidentDTO createIncidentDto)
        {
            _logger.LogInformation("Creating a new incident.");
            if (createIncidentDto == null)
            {
                _logger.LogWarning("CreateIncidentDTO is null.");
                return BadRequest("Incident data is required");
            }

            try
            {
                var incident = await _incidentService.CreateIncident(createIncidentDto);
                /*                var emailResult = await _emailService.SendNotificationAsync(createIncidentDto.EmployeeId, incident);

                                if (emailResult)
                                {*/

                _logger.LogInformation("Incident created and notification sent successfully. IncidentId: {IncidentId}", incident.Id);

                return CreatedAtAction(nameof(GetIncident), new { id = incident.Id }, incident);
                /*                }

                                _logger.LogWarning("Incident created but email notification failed. IncidentId: {IncidentId}", incident.Id);
                                return BadRequest("Email Not Sent");
                */

            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the incident.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = "AdminIncidentsOrSuperAdmin")] //sa ai
        public async Task<IActionResult> UpdateIncident(int id, [FromBody] UpdateIncidentDTO updateIncidentDto)
        {
            _logger.LogInformation("Updating incident with ID: {IncidentId}", id);
            if (id <= 0)
            {
                _logger.LogWarning("Invalid incident ID: {IncidentId}", id);
                return BadRequest("Invalid incident ID");
            }

            if (updateIncidentDto == null)
            {
                _logger.LogWarning("UpdateIncidentDTO is null for incident ID: {IncidentId}", id);
                return BadRequest("Incident update data is required");
            }

            try
            {
                await _incidentService.UpdateIncident(id, updateIncidentDto);
                _logger.LogInformation("Incident with ID: {IncidentId} updated successfully.", id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while updating incident with ID: {IncidentId}.", id);
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "AllowAll")] //user
        public async Task<IActionResult> UserUpdateIncident(int id, [FromForm] UpdateIncidentUserDto updateIncidentDto)
        {
            _logger.LogInformation("Updating incident (User) with ID: {IncidentId}", id);
            if (id <= 0)
            {
                _logger.LogWarning("Invalid incident ID: {IncidentId}", id);
                return BadRequest("Invalid incident ID");
            }

            if (updateIncidentDto == null)
            {
                _logger.LogWarning("UpdateIncidentUserDto is null for incident ID: {IncidentId}", id);
                return BadRequest("Incident update data is required");
            }

            try
            {
                await _incidentService.UserUpdateIncident(id, updateIncidentDto);
                _logger.LogInformation("Incident (User) with ID: {IncidentId} updated successfully.", id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while updating incident (User) with ID: {IncidentId}.", id);
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Incident))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = "AllowAll")] //user
        public async Task<ActionResult<GetUserUpdateIncidentDTO>> GetUserUpdateIncident(int id)
        {
            _logger.LogInformation("Fetching UserUpdateIncident with ID: {IncidentId}", id);
            try
            {
                var incident = await _incidentService.GetUserUpdateIncident(id);

                if (incident == null)
                {
                    _logger.LogWarning("UserUpdateIncident with ID: {IncidentId} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Successfully fetched UserUpdateIncident with ID: {IncidentId}.", id);
                return Ok(incident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching UserUpdateIncident with ID: {IncidentId}.", id);
                return BadRequest("Failed to fetch incident.");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = "AllowAll")] //user
        public async Task<IActionResult> DeleteIncidentById(int id)
        {
            if (id < 0)
            {
                _logger.LogWarning("DeleteIncidentById: Invalid ID {Id}", id);
                return BadRequest("Invalid id");
            }

            try
            {
                await _incidentService.DeleteIncidentById(id);
                _logger.LogInformation("Incident with ID {Id} deleted successfully", id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Failed to delete incident with ID {Id}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = "AdminsUserOrAdminIncidentsOrSuperAdmin")]
        public async Task<IActionResult> UploadIncidents(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }
            try
            {
                await _incidentService.UploadIncident(file);
                return Ok(new { message = "Incidents uploaded successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        //not used
        public async Task<IActionResult> GetAdminIncidentsWithBarChart()
        {
            _logger.LogInformation("Fetching incidents for admins with bar chart data.");
            try
            {
                var result = await _incidentService.GetIncidentsAdmins();
                _logger.LogInformation("Successfully fetched incidents for admins.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching admin incidents with bar chart data.");
                return BadRequest("Failed to fetch admin incidents.");
            }
        }
    }
}
