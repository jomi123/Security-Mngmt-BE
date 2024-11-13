using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Preventyon.Hubs;
using Preventyon.Models;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Repository.IRepository;
using Preventyon.Service.IService;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace Preventyon.Service
{
    public class IncidentService : IIncidentService
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly IAssignedIncidentRepository _assignedIncidentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly INotificationRepository _notificationRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IHubContext<IncidentHub> _hubContext;
        private readonly IAssignedIncidentService _assignedIncidentService;

        public IncidentService(IIncidentRepository incidentRepository, IEmployeeRepository employeeRepository, IAssignedIncidentRepository assignedIncidentRepository, IMapper mapper, INotificationRepository notificationRepository, IAdminRepository adminRepository, IHubContext<IncidentHub> hubContext, IAssignedIncidentService assignedIncidentService)
        {
            _incidentRepository = incidentRepository;
            _employeeRepository = employeeRepository;
            _assignedIncidentRepository = assignedIncidentRepository;
            _mapper = mapper;
            _notificationRepository = notificationRepository;
            _adminRepository = adminRepository;
            _hubContext = hubContext;
            _assignedIncidentService = assignedIncidentService;
        }

        public async Task<IEnumerable<Incident>> GetAllIncidents()
        {
            try
            {
                var incidents = await _incidentRepository.GetAllIncidents();
                return incidents ?? Enumerable.Empty<Incident>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<Incident>();
            }
        }

        public async Task<GetIncidentsByEmployeeID> GetIncidentsByEmployeeId(int employeeId)
        {

            var assignments = await _assignedIncidentRepository.GetAssignmentsByEmployeeIdAsync(employeeId);
            var incidentIds = assignments
                .Select(a => a.IncidentId)
                .Distinct()
                .ToList();

            var assignedIncidentsEntities = await _assignedIncidentRepository.GetIncidentsByIdsAsync(employeeId, incidentIds);

            var incidents = await _incidentRepository.GetIncidentsByEmployeeId(employeeId);

            incidents.AssignedIncidents = _mapper.Map<List<TableFetchIncidentsDto>>(assignedIncidentsEntities);

            return incidents;
        }


        public async Task<Incident> GetIncidentById(int id)
        {
            return await _incidentRepository.GetIncidentById(id);
        }

        public async Task<Incident> CreateIncident(CreateIncidentDTO createIncidentDto)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(createIncidentDto.EmployeeId);
            if (employee == null)
            {
                throw new ArgumentException("Invalid employee ID");
            }

            List<string> documentUrls = new List<string>();
            if (createIncidentDto.DocumentUrls != null)
            {
                foreach (IFormFile document in createIncidentDto.DocumentUrls)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(document.FileName);
                    //change the upload path
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(stream);
                    }

                    documentUrls.Add($"/images/{fileName}");
                }
            }

            createIncidentDto.IncidentOccuredDate = createIncidentDto.IncidentOccuredDate.ToUniversalTime();
            var incident = _mapper.Map<Incident>(createIncidentDto);
            incident.ReportedBy = employee.Name;
            incident.RoleOfReporter = employee.Designation;
            incident.DocumentUrls = documentUrls;
            incident.createdAt = DateTime.UtcNow;


            var allincidents = await GetAllIncidents();
            var lastEntry = allincidents.OrderByDescending(i => i.Id).FirstOrDefault();
            if (lastEntry != null)
            {
                if (string.IsNullOrEmpty(lastEntry.IncidentNo) || !lastEntry.IncidentNo.StartsWith("EXPINC"))
                {
                    incident.IncidentNo = "EXPINC1";
                }
                else
                {
                    var numberPart = lastEntry.IncidentNo.Substring(6);
                    if (int.TryParse(numberPart, out int numericValue))
                    {
                        incident.IncidentNo = $"EXPINC{numericValue + 1}";
                    }
                }
            }
            else
            {
                incident.IncidentNo = "EXPINC1";
            }

            if (createIncidentDto.IsDraft)
            {
                incident.IncidentStatus = "draft";
            }
            else
            {
                incident.IncidentStatus = "pending";
            }
            incident.Category = createIncidentDto.Category ?? "Other"; // Or handle null accordingly
            incident.Priority = createIncidentDto.Priority ?? "High";
            incident.IncidentType = createIncidentDto.IncidentType ?? "Quality Incidents";
            await notificationCreate(incident);
            await _incidentRepository.AddIncident(incident);
            await _hubContext.Clients.All.SendAsync("ReceiveIncidentUpdate");
            return incident;
        }

        public async Task UpdateIncident(int id, UpdateIncidentDTO updateIncidentDto)
        {
            var incident = await _incidentRepository.GetIncidentById(id);
            if (incident == null)
            {
                throw new ArgumentException("Invalid incident ID");
            }
            if (!string.IsNullOrWhiteSpace(updateIncidentDto.Correction))
            {
                if (!updateIncidentDto.Correction.Equals(incident.Correction))
                {
                    updateIncidentDto.CorrectionActualCompletionDate = DateTime.UtcNow;
                    updateIncidentDto.CorrectionDetailsTimeTakenToCloseIncident = Math.Round(((updateIncidentDto.CorrectionActualCompletionDate - incident.createdAt).TotalDays), 2);
                }

            }

            if (!string.IsNullOrWhiteSpace(updateIncidentDto.CorrectiveAction))
            {
                if (!updateIncidentDto.CorrectiveAction.Equals(incident.CorrectiveAction))
                {
                    updateIncidentDto.CorrectiveActualCompletionDate = DateTime.UtcNow;
                    updateIncidentDto.CorrectiveDetailsTimeTakenToCloseIncident = Math.Round(((updateIncidentDto.CorrectiveActualCompletionDate - incident.createdAt).TotalDays), 2);
                }

            }

            await _incidentRepository.UpdateIncident(incident, updateIncidentDto);
        }

        public async Task UserUpdateIncident(int id, UpdateIncidentUserDto updateIncidentDto)
        {
            var incident = await _incidentRepository.GetIncidentById(id);
            if (incident == null)
            {
                throw new ArgumentException("Invalid incident ID");
            }


            List<string> UserGivenOldDocumentUrls = updateIncidentDto.OldDocumentUrls ?? new List<string>();
            List<string> NewUploadedDocuments = new List<string>();

            if (updateIncidentDto.NewDocumentUrls != null)
            {
                foreach (IFormFile document in updateIncidentDto.NewDocumentUrls)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(document.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(stream);
                    }

                    NewUploadedDocuments.Add($"/images/{fileName}");
                }
            }


            List<string> finalDocumentUrls = incident.DocumentUrls.Intersect(UserGivenOldDocumentUrls).Concat(NewUploadedDocuments).Distinct().ToList();
            Console.WriteLine(finalDocumentUrls);

            List<string> documentsToDelete = UserGivenOldDocumentUrls.Except(finalDocumentUrls).ToList();

            if (documentsToDelete.Count > 0)
            {
                foreach (string urlToDelete in documentsToDelete)
                {
                    if (urlToDelete != null)
                    {
                        var filePathToDelete = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", urlToDelete.TrimStart('/'));
                        if (File.Exists(filePathToDelete))
                        {
                            File.Delete(filePathToDelete);
                        }
                    }

                }

            }



            incident.DocumentUrls = finalDocumentUrls;

            updateIncidentDto.IncidentOccuredDate = DateTime.SpecifyKind(updateIncidentDto.IncidentOccuredDate, DateTimeKind.Utc);
            updateIncidentDto.EmployeeId = incident.EmployeeId;
            if (updateIncidentDto.IsDraft == true)
            {
                incident.IncidentStatus = "draft";
            }
            else
            {
                incident.IncidentStatus = "pending";
            }

            await _incidentRepository.UserUpdateIncident(incident, updateIncidentDto);
            await _hubContext.Clients.All.SendAsync("ReceiveIncidentUpdate");
        }

        public async Task<GetUserUpdateIncidentDTO> GetUserUpdateIncident(int id)
        {

            var incident = await _incidentRepository.GetIncidentById(id);

            if (incident == null)
            {
                return null;
            }

            return _mapper.Map<GetUserUpdateIncidentDTO>(incident);

        }

        public async Task<GetIncidentsByEmployeeID> GetIncidentsAdmins()
        {

            var incidentsByEmployee = await _incidentRepository.GetAllIncidentsWithBarChart();

            return incidentsByEmployee ?? new GetIncidentsByEmployeeID();

        }

        public async Task DeleteIncidentById(int id)
        {
            await _incidentRepository.DeleteIncidentById(id);
            await _hubContext.Clients.All.SendAsync("ReceiveIncidentUpdate");
        }

        public async Task notificationCreate(Incident incident)
        {
            var incidentDetailsJson = JsonSerializer.Serialize(incident);
            var jsonObject = JsonNode.Parse(incidentDetailsJson);
            jsonObject["Message"] = $"new incident {incident.IncidentNo} created";
            incidentDetailsJson = jsonObject.ToJsonString();
            var admins = await _employeeRepository.GetEmployeesByRolesAsync();
            foreach (var adminEmployeeId in admins)
            {
                var notification = new Notification
                {
                    EmployeeId = adminEmployeeId.Id,
                    Message = incidentDetailsJson,
                    CreatedAt = DateTime.UtcNow
                };

                await _notificationRepository.AddNotification(notification);
            }
            await _notificationRepository.SaveChanges();
        }

        public async Task UploadIncident(IFormFile file)
        {
            var incidents = new List<Incident>();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            int lastNumber;
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    var incidentNo = "";
                    var allIncidents = await GetAllIncidents();
                    var lastEntry = allIncidents.OrderByDescending(i => i.Id).FirstOrDefault();//optimize

                    lastNumber = 1;

                    if (lastEntry != null)
                    {
                        if (string.IsNullOrEmpty(lastEntry.IncidentNo) || !lastEntry.IncidentNo.StartsWith("EXPINC"))
                        {
                            incidentNo = "EXPINC1";
                        }
                        else
                        {
                            var numberPart = lastEntry.IncidentNo.Substring(6);
                            if (int.TryParse(numberPart, out int numericValue))
                            {
                                lastNumber = numericValue + 1;
                            }
                        }
                    }
                    else
                    {
                        incidentNo = "EXPINC1";
                    }

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var ReportedBy = worksheet.Cells[row, 3].Text;
                        var employeeId = await _employeeRepository.GetEmployeeByName(ReportedBy);


                        if (employeeId == 0)
                        {
                            continue;
                        }
                        var correction = worksheet.Cells[row, 15].Text;
                        var isCorrectionFilled = false;
                        if (correction != null)
                        {
                            isCorrectionFilled = true;
                        }
                        incidentNo = $"EXPINC{lastNumber++}";
                        var incident = new Incident
                        {
                            IncidentNo = incidentNo,
                            IncidentTitle = worksheet.Cells[row, 1].Text,
                            IncidentDescription = worksheet.Cells[row, 2].Text,
                            ReportedBy = worksheet.Cells[row, 3].Text,
                            RoleOfReporter = worksheet.Cells[row, 4].Text,
                            EmployeeId = employeeId,
                            IncidentOccuredDate = ParseDate(worksheet.Cells[row, 5].Text),
                            MonthYear = ParseDate(worksheet.Cells[row, 6].Text),
                            IncidentType = worksheet.Cells[row, 7].Text,
                            Category = worksheet.Cells[row, 8].Text,
                            Priority = worksheet.Cells[row, 9].Text,
                            ActionAssignedTo = worksheet.Cells[row, 10].Text,
                            DeptOfAssignee = worksheet.Cells[row, 11].Text,
                            InvestigationDetails = worksheet.Cells[row, 12].Text,
                            AssociatedImpacts = worksheet.Cells[row, 13].Text,
                            CollectionOfEvidence = worksheet.Cells[row, 14].Text,
                            Correction = worksheet.Cells[row, 15].Text,
                            CorrectiveAction = worksheet.Cells[row, 16].Text,
                            CorrectionCompletionTargetDate = ParseDate(worksheet.Cells[row, 17].Text),
                            CorrectionActualCompletionDate = ParseDate(worksheet.Cells[row, 18].Text),
                            CorrectiveActualCompletionDate = ParseDate(worksheet.Cells[row, 19].Text),
                            IncidentStatus = worksheet.Cells[row, 20].Text,
                            CorrectionDetailsTimeTakenToCloseIncident = Convert.ToDouble(worksheet.Cells[row, 21].Text),
                            CorrectiveDetailsTimeTakenToCloseIncident = Convert.ToDouble(worksheet.Cells[row, 22].Text),
                            createdAt = ParseDate(worksheet.Cells[row, 23].Text),
                            PreventiveAction = worksheet.Cells[row, 24].Text,
                            Remarks = worksheet.Cells[row, 25].Text,
                            IsCorrectionFilled = isCorrectionFilled,
                            IsDraft = false,
                            IsSubmittedForReview = true,
                        };
                        incidents.Add(incident);
                    }
                }
            }
            if (incidents.Count > 0)
            {
                var uploadedIncidents = await _incidentRepository.UploadIncident(incidents);
                foreach (var incident in uploadedIncidents)
                {
                    var actionAssignedTo = incident.ActionAssignedTo;
                    var assignedNames = actionAssignedTo.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    List<int> assignedEmpIds = new List<int>();
                    foreach (var name in assignedNames)
                    {
                        int assignedEmpId = await _employeeRepository.GetEmployeeByName(name);
                        if (assignedEmpId != 0)
                        {
                            assignedEmpIds.Add(assignedEmpId);
                        }
                    }

                    var request = new AssignIncidentRequest
                    {
                        AssignedEmployeeIds = assignedEmpIds,
                        Remarks = incident.Remarks,
                    };

                    await _assignedIncidentService.AssignIncidentToEmployeesAsync(incident.Id, request, true);
                }
            }
        }
        private DateTime ParseDate(string dateText)
        {
            if (DateTime.TryParse(dateText, out var date))
            {
                date = DateTime.SpecifyKind(date, DateTimeKind.Unspecified).ToUniversalTime();
                return date;
            }
            return DateTime.MinValue;
        }

    }
}
