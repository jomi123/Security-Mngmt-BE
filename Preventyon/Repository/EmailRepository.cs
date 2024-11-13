using FluentEmail.Core;
using Preventyon.Repository.IRepository;

namespace Preventyon.Repository
{
    public class EmailRepository : IEmailRepository
    {

        private readonly IFluentEmail _fluentEmail;

        public EmailRepository(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task SendEmailAsync(string toEmail,string subject,string body)
        {

            await _fluentEmail
                .To(toEmail)
                .Subject(subject)
                .Body(body, true)
                .SendAsync();
        }
    }
}
