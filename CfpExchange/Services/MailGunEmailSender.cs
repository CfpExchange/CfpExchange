using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CfpExchange.Services
{
    public class MailGunEmailSender : IEmailSender
    {
        private ILogger _logger;
        private readonly EmailSettings _emailSettings;

        public MailGunEmailSender(ILoggerFactory loggerFactory, IOptions<EmailSettings> emailOptions)
        {
            _logger = loggerFactory.CreateLogger<MailGunEmailSender>();
            _emailSettings = emailOptions.Value;
        }

        public async Task SendEmailAsync(string emailAddress, string subject, string body)
        {
            var correlationId = new Guid();
            _logger.LogInformation($"{correlationId}: Sending email to '{emailAddress}' with subject '{subject}': '{body}'");

            using (var client = new HttpClient { BaseAddress = new Uri(_emailSettings.ApiBaseUri) })
            {
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes(_emailSettings.ApiKey)));
            
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("from", _emailSettings.From),
                    new KeyValuePair<string, string>("to", emailAddress),
                    new KeyValuePair<string, string>("subject", subject),
                    new KeyValuePair<string, string>("html", body)
                });
            
                await client.PostAsync(_emailSettings.RequestUri, content).ConfigureAwait(false);
            }
        
            _logger.LogInformation($"{correlationId}: Sent email to '{emailAddress}' successfully.");    
        }
    }
}
