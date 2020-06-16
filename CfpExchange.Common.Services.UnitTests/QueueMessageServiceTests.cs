using System;
using System.Threading.Tasks;

using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

using CfpExchange.Common.Models;

namespace CfpExchange.Common.Services.UnitTests
{
    public class QueueMessageServiceTests
    {
        #region Constants

        private const string URL_TO_CFP = "https://cfp.exchange/cfp/details/cfp-slug";

        #endregion

        #region Fields

        private Mock<IQueueClient> _mockQueueClient;
        private static Guid _cfpPublicId = Guid.NewGuid();
        private static CfpInformation _cfpInformation = new CfpInformation();
        private readonly QueueMessageService _queueMessageService;

        #endregion

        #region Constructors

        public QueueMessageServiceTests()
        {
            var builder = new ConfigurationBuilder();
            _mockQueueClient = new Mock<IQueueClient>();
            _queueMessageService = new QueueMessageService(builder.Build(), _mockQueueClient.Object);
        }

        #endregion

        [Fact]
        public async Task SendTwitterMessage_WithCfpInformationNull_ShouldThrowArgumentNullException()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _queueMessageService.SendTwitterMessageAsync(null, URL_TO_CFP));
            Assert.Equal("cfpInfo", exception.ParamName);
        }

        [Fact]
        public async Task SendTwitterMessage_WithUrlToCfpNull_ShouldThrowArgumentNullException()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _queueMessageService.SendTwitterMessageAsync(_cfpInformation, null));
            Assert.Equal("urlToCfp", exception.ParamName);
        }

        [Fact]
        public async Task SendTwitterMessage_WithValidInformation_ShouldSendMessage()
        {
            await _queueMessageService.SendTwitterMessageAsync(_cfpInformation, URL_TO_CFP);
            _mockQueueClient.Verify(qc => qc.SendAsync(It.IsAny<Message>()), Times.Once);
        }

        [Fact]
        public async Task SendDownloadEventImageMessage_WithCfpEventImageUrlNull_ShouldThrowArgumentNullException()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _queueMessageService.SendDownloadEventImageMessageAsync(_cfpPublicId, null));
            Assert.Equal("cfpEventImageUrl", exception.ParamName);
        }

        [Fact]
        public async Task SendDownloadEventImageMessage_WithValidInformation_ShouldSendMessage()
        {
            await _queueMessageService.SendDownloadEventImageMessageAsync(_cfpPublicId, URL_TO_CFP);
            _mockQueueClient.Verify(qc => qc.SendAsync(It.IsAny<Message>()), Times.Once);
        }
    }
}
