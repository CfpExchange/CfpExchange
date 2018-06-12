using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CfpExchange.Services
{
	public class MockEmailSender : IEmailSender
	{
		private readonly ILogger _logger;

		public MockEmailSender(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<MockEmailSender>();
		}

		public Task SendEmailAsync(string emailAddress, string subject, string body)
		{
			return SendEmailAsync(emailAddress, "Mocky Mockings <mock@example.com>", subject, body);
		}

		public Task SendEmailAsync(string emailAddress, string from, string subject, string body)
		{
			_logger.LogInformation("Sending email to {EmailAddress}, from {From} with subject {Subject}: {Body}", emailAddress, from, subject, body);
			return Task.CompletedTask;
		}
	}
}