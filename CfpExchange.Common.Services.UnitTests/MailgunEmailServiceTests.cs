using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace CfpExchange.Common.Services.UnitTests
{
    public class MailgunEmailServiceTests
    {
        #region Constants

        private const string BODY = "Here's the body of the email.";
        private const string EMAIL_ADDRESS = "recipient@example.com";
        private const string FROM = "ut@example.com";
        private const string SUBJECT = "This is a (unit) test email";

        #endregion

        #region Fields

        private readonly MailgunEmailService _mailgunEmailService;

        #endregion

        #region Constructor

        public MailgunEmailServiceTests()
        {
            var emailSettings = new EmailSettings { ApiKey = string.Empty, ApiUri = string.Empty, From = "Unit Test <ut@example.com>" };
            _mailgunEmailService = new MailgunEmailService(new LoggerFactory(), Options.Create(emailSettings));
        }

        #endregion


        [Theory]
        [InlineData(null, SUBJECT, BODY)]
        [InlineData(EMAIL_ADDRESS, null, BODY)]
        [InlineData(EMAIL_ADDRESS, SUBJECT, null)]
        public async Task SendEmail_WithOneParamNull_ShouldThrowArgumentException(string emailAddress, string subject, string body)
        {
            string nullParam = null;
            if (emailAddress == null)
            {
                nullParam = nameof(emailAddress);
            }
            else if (subject == null)
            {
                nullParam = nameof(subject);
            }
            else if (body == null)
            {
                nullParam = nameof(body);
            }
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _mailgunEmailService.SendEmailAsync(emailAddress, subject, body));
            Assert.Equal(nullParam, exception.ParamName);
        }

        [Fact]
        public async Task SendEmail_WithFromNull_ShouldThrowArgumentException()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _mailgunEmailService.SendEmailAsync(EMAIL_ADDRESS, null, SUBJECT, BODY));
            Assert.Equal("from", exception.ParamName);
        }
    }
}
