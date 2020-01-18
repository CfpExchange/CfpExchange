using System;
using System.Collections.Generic;
using System.Linq;
using CfpExchange.Models;
using CfpExchange.Services;
using Moq.AutoMock;
using Xunit;

namespace CfpExchange.UnitTests.Services
{
    public class CfpServiceTest : AutomockerTestBase
    {
        [Fact]
        public void GetCfpById_WithValidId_ShouldReturnCorrectCfp()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cfps = new List<Cfp>
            {
                new Cfp
                {
                    Id = id
                }
            }.AsQueryable();

            var mockContext = SetupMockedCfpContext(cfps);
            
            AutoMocker.Use(mockContext);
            var service = AutoMocker.CreateInstance<CfpService>();

            // Act
            var cfp = service.GetCfpById(id);

            // Assert
            Assert.Equal(id, cfp.Id);
        }
    }
}
