using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

using CfpExchange.Common.Services.UnitTests.Stubs;

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

        private readonly static EmailSettings _emailSettings = new EmailSettings { ApiKey = "FakeApiKey", ApiUri = "https://www.example.com/api/", From = "Unit Test <ut@example.com>" };
        private MailgunEmailService _mailgunEmailService;

        #endregion

        #region Constructor

        public MailgunEmailServiceTests()
        {
            _mailgunEmailService = new MailgunEmailService(null, new LoggerFactory(), Options.Create(_emailSettings));
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

        [Fact]
        public async Task SendEmail_WithValidParameters_ReturnsTrue()
        {
            SetupWithMockHttpClientFactory();
            var result = await _mailgunEmailService.SendEmailAsync(EMAIL_ADDRESS, FROM, SUBJECT, BODY);
            Assert.True(result);
        }

        [Fact]
        public async Task SendEmail_WithValidParameters_ReturnsFalse()
        {
            SetupWithMockHttpClientFactory(false);
            var result = await _mailgunEmailService.SendEmailAsync(EMAIL_ADDRESS, FROM, SUBJECT, BODY);
            Assert.False(result);
        }

        #region Private methods

        private void SetupWithMockHttpClientFactory(bool success = true)
        {
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var clientHandlerStub = new DelegatingHandlerStub(success);
            var client = new HttpClient(clientHandlerStub);

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            _mailgunEmailService = new MailgunEmailService(mockHttpClientFactory.Object, new LoggerFactory(), Options.Create(_emailSettings));
        }

        #endregion
    }
}
