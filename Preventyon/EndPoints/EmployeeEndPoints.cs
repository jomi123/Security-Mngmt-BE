using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Preventyon.Data;
using Preventyon.Models.DTO.Employee;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Repository.IRepository;
using RequestDemoMinimal.models;
using System.Net;

namespace Preventyon.EndPoints
{
    public static class EmployeeEndPoints
    {


        public static void ConfigureEndPoints(this WebApplication app)
        {
            app.MapPut("/api/updateEmployee/{id}", UpdateEmployee)
                .WithName("UpdateEmployee")
                .Accepts<UpdateEmployeeRoleDTO>("application/json")
                .Produces<APIResponse>(200)
                .Produces(400)
                .Produces(404)
                .RequireAuthorization("AdminUserOrSuperAdmin");


            app.MapPut("/api/updateIncidentByReview/{id}", updateIncidentByReview)
                .WithName("updateIncidentByReview")
                .Accepts<UpdateIncidentByReviewDto>("application/json")
                .Produces<APIResponse>(200)
                .Produces(400)
                .Produces(404)
                .RequireAuthorization("AdminIncidentsOrSuperAdmin");


            app.MapPut("/api/acceptIncidents/{incidentId}", acceptIncidents)
                .WithName("acceptIncidents")
                .Accepts<UpdateIncidentByReviewDto>("application/json")
                .Produces<APIResponse>(200)
                .Produces(400)
                .Produces(404)
                .RequireAuthorization("AllowAll");

            app.MapGet("/api/incidentApproval/{id}", incidentApproval)
                .WithName("incidentApproval")
                .Produces<APIResponse>(200)
                .Produces(400)
                .Produces(404)
                .RequireAuthorization("AdminIncidentsOrSuperAdmin");
        }
        private async static Task<IResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeRoleDTO employeeDTO, IMapper _mapper, IEmployeeRepository _employeeRepo)
        {
            APIResponse response = new APIResponse();
            var existingEmployee = await _employeeRepo.FindAsync(id);

            if (existingEmployee == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.isSuccess = false;
                return Results.NotFound(response);
            }
            _mapper.Map(employeeDTO, existingEmployee);

            await _employeeRepo.UpdateAsync(existingEmployee);

            response.Result = _mapper.Map<UpdateEmployeeRoleDTO>(existingEmployee);
            response.StatusCode = HttpStatusCode.OK;
            response.StatusCode = HttpStatusCode.OK;
            response.isSuccess = true;
            return Results.Ok(response);
        }



        private async static Task<IResult> updateIncidentByReview(int id, [FromBody] UpdateIncidentByReviewDto incidentByReviewDto, IMapper _mapper, IIncidentRepository incidentRepository, IEmailRepository emailRepository)
        {
            APIResponse response = new APIResponse();
            var existingIncident = await incidentRepository.GetIncidentById(id);

            if (existingIncident == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.isSuccess = false;
                return Results.NotFound(response);
            }
            if (existingIncident.IsSubmittedForReview)
            {
                incidentByReviewDto.IsSubmittedForReview = false;
            }
            /*else
            {
                var confirmationUrl = "http://localhost:7209/api/incidentApproval/" + id;
                var rejectiontionUrl = "http://localhost:4200/admin";

                var subject = existingIncident.IncidentTitle;
                var approveButtonStyle = $@"
                                        display: inline-block;
                                        padding: 10px 20px;
                                        font-size: 16px;
                                        color: #fff;
                                        background-color: #007bff; 
                                        text-decoration: none;
                                        border-radius: 5px;
                                        cursor: pointer;";

                var rejectButtonStyle = $@"
                                        display: inline-block;
                                        padding: 10px 20px;
                                        font-size: 16px;
                                        color: #fff;
                                        background-color: #dc3545; 
                                        text-decoration: none;
                                        border-radius: 5px;
                                        cursor: pointer;";

                 var body = $@"
                            {existingIncident.IncidentTitle} {DateTime.Now:yyyy-MM-dd}
                            <br><br>
                            <strong>Description:</strong> {existingIncident.IncidentDescription}<br>
                            <strong>Type:</strong> {existingIncident.IncidentType}<br>
                            <strong>Priority:</strong> <span style='color: #dc3545;'>{existingIncident.Priority}</span><br><br> <!-- Red text for Priority -->
                            <strong>Corrective Action:</strong> <span style='background-color: yellow; padding: 2px 5px;'>{existingIncident.CorrectiveAction}</span><br><br>
                            <a href='{confirmationUrl}' style='{approveButtonStyle}'>Approve</a> 
                            <a href='{confirmationUrl}' style='{rejectButtonStyle}'>Reject</a>";

                await emailRepository.SendEmailAsync("sreejith.shaji@experionglobal.com", subject, body);
            }*/
            _mapper.Map(incidentByReviewDto, existingIncident);
            existingIncident.IncidentStatus = "review";
            await incidentRepository.UpdateIncidentAsync(existingIncident);
            response.Result = _mapper.Map<UpdateIncidentByReviewDto>(existingIncident);
            response.StatusCode = HttpStatusCode.OK;
            response.StatusCode = HttpStatusCode.OK;
            response.isSuccess = true;
            return Results.Ok(response);
        }


        private async static Task<IResult> acceptIncidents(int incidentId, [FromBody] int employeeId, ApiContext apiContext)
        {
            APIResponse response = new APIResponse();

            var assignedIncidents = await apiContext.AssignedIncidents
                .Where(a => a.IncidentId == incidentId)
                .ToListAsync();

            var employee = await apiContext.Employees.FindAsync(employeeId);

            if (employee == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return Results.NotFound(response);
            }
            var incident = await apiContext.Incident.FindAsync(incidentId);
            if (incident == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return Results.NotFound(response);
            }
            incident.ActionAssignedTo = employee.Name;
            if (assignedIncidents == null || !assignedIncidents.Any())
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.isSuccess = false;
                return Results.NotFound(response);
            }

            foreach (var assignedIncident in assignedIncidents)
            {
                assignedIncident.Accepted = employeeId;
            }


            await apiContext.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.OK;
            response.isSuccess = true;
            return Results.Ok(response);
        }

        private async static Task<IResult> incidentApproval(int id, [FromServices] IIncidentRepository _incidentRepository)
        {
            APIResponse response = new APIResponse();
            var existingIncident = await _incidentRepository.GetIncidentById(id);

            if (existingIncident == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.isSuccess = false;
                return Results.NotFound(response);
            }

            if (existingIncident.IsSubmittedForReview && string.IsNullOrEmpty(existingIncident.Correction))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                return Results.BadRequest(response);
            }

            existingIncident.IncidentStatus = "closed";
            await _incidentRepository.UpdateIncidentAsync(existingIncident);

            response.Result = existingIncident;
            response.StatusCode = HttpStatusCode.OK;
            response.isSuccess = true;
            var redirectUrl = "http://localhost:4200/admin";
            return Results.Ok(response);
        }
    }
}
