using System.Threading.Tasks;

namespace CfpExchange.Common.Services.Interfaces
{
	public interface IEmailService
	{
		Task SendEmailAsync(string emailAddress, string subject, string body);
		Task SendEmailAsync(string emailAddress, string from, string subject, string body);
	}
}
