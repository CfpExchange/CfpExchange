using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CfpExchange.Common.Services.Interfaces;

namespace CfpExchange.Common.Services
{
    public class MailGunEmailSender : IEmailSender
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly EmailSettings _emailSettings;

        #endregion

        #region Constructors

        public MailGunEmailSender(ILoggerFactory loggerFactory, IOptions<EmailSettings> emailOptions)
        {
            _logger = loggerFactory.CreateLogger<MailGunEmailSender>();
            _emailSettings = emailOptions.Value;
        }

        #endregion

        public async Task SendEmailAsync(string emailAddress, string from, string subject, string body)
        {
            var correlationId = Guid.NewGuid();
            _logger.LogInformation($"{correlationId}: Sending email to '{emailAddress}' with subject '{subject}': '{body}'");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{_emailSettings.ApiKey}")));

            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("from", from),
                    new KeyValuePair<string, string>("to", emailAddress),
                    new KeyValuePair<string, string>("subject", subject),
                    new KeyValuePair<string, string>("text", body)
                });

            var result = await client.PostAsync(_emailSettings.ApiUri, content).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogInformation($"{correlationId}: Sending email to '{emailAddress}' FAILED. ({result.StatusCode}: {result.ReasonPhrase})");
            }
            else
            {
                _logger.LogInformation($"{correlationId}: Sent email to '{emailAddress}' successfully.");
            }
        }
    }
}
