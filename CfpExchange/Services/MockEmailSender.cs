using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CfpExchange.Services
{
    public class MockEmailSender : IEmailSender
    {
        private ILogger _logger;

        public MockEmailSender(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MockEmailSender>();
        }

        public Task SendEmailAsync(string emailAddress, string subject, string body)
        {
            _logger.LogInformation("Sending email to {EmailAddress} with subject {Subject}: {Body}", emailAddress, subject, body);
            return Task.CompletedTask;
        }
    }
}
