using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using CfpExchange.Common.Services.Interfaces;

namespace CfpExchange.Common.Services
{
    public class MockEmailService : IEmailService
    {
        private readonly ILogger _logger;

        public MockEmailService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MockEmailService>();
        }

        public Task<bool> SendEmailAsync(string emailAddress, string subject, string body)
        {
            return SendEmailAsync(emailAddress, "Mocky Mockings <mock@example.com>", subject, body);
        }

        public Task<bool> SendEmailAsync(string emailAddress, string from, string subject, string body)
        {
            _logger.LogInformation("Sending email to {EmailAddress}, from {From} with subject {Subject}: {Body}", emailAddress, from, subject, body);
            return Task.FromResult(true);
        }
    }
}
