using System.Threading.Tasks;

namespace CfpExchange.Common.Services.Interfaces
{
	public interface IEmailService
	{
		Task<bool> SendEmailAsync(string emailAddress, string subject, string body);

		Task<bool> SendEmailAsync(string emailAddress, string from, string subject, string body);
	}
}
