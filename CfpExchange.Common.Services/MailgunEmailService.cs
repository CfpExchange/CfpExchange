using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CfpExchange.Common.Services.Interfaces;
using CfpExchange.Common.Helpers;

namespace CfpExchange.Common.Services
{
    public class MailgunEmailService : IEmailService
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly EmailSettings _emailSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        #endregion

        #region Constructors

        public MailgunEmailService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, IOptions<EmailSettings> emailOptions)
        {
            _httpClientFactory = httpClientFactory;
            _logger = loggerFactory.CreateLogger<MailgunEmailService>();
            _emailSettings = emailOptions.Value;
        }

        #endregion

        public async Task SendEmailAsync(string emailAddress, string subject, string body)
        {
            await SendEmailAsync(emailAddress, _emailSettings.From, subject, body);
        }

        public async Task SendEmailAsync(string emailAddress, string from, string subject, string body)
        {
            Guard.IsNotNull(emailAddress, nameof(emailAddress));
            Guard.IsNotNull(from, nameof(from));
            Guard.IsNotNull(subject, nameof(subject));
            Guard.IsNotNull(body, nameof(body));

            var correlationId = Guid.NewGuid();
            _logger.LogInformation($"{correlationId}: Sending email to '{emailAddress}' with subject '{subject}': '{body}'");

            using var client = _httpClientFactory.CreateClient();
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
