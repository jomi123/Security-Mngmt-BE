using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Preventyon.Models;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Repository.IRepository;
using Preventyon.Service.IService;


namespace Preventyon.Service
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IEmployeeRepository _employeeRepository;
        public EmailService(IEmailRepository emailRepository, IEmployeeRepository employeeRepository) {
            _emailRepository = emailRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<bool> SendNotificationAsync(int employeeId, Incident incident)
        {
          
            var employee = await _employeeRepository.FindAsync(employeeId);

            if (employee == null)
            {
                return false;
            }

            
            var subject = incident.IncidentTitle;
            var body = $@"
          {incident.IncidentTitle} {DateTime.Now:yyyy-MM-dd}
        <br><br>
         Description: {incident.IncidentDescription}<br>
        Type: {incident.IncidentType}<br>
        Priority: {incident.Priority}<br><br>
       
    ";

            await _emailRepository.SendEmailAsync(employee.Email, subject, body);

            
            return true;
        }

    }
}

