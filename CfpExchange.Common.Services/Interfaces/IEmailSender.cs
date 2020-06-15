using System.Threading.Tasks;

namespace CfpExchange.Common.Services.Interfaces
{
	public interface IEmailSender
	{
		Task SendEmailAsync(string emailAddress, string from, string subject, string body);
	}
}
