using System;
using System.Collections.Generic;
using System.Linq;
using CfpExchange.Data;
using CfpExchange.Models;
using CfpExchange.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace CfpExchange.UnitTests.Services
{
    [TestFixture]
    public class CfpServiceTest : AutomockerTestBase
    {
        [Test]
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
            Assert.AreEqual(id, cfp.Id);
        }

        private static Mock<CfpContext> SetupMockedCfpContext(IQueryable<Cfp> cfps)
        {
            var mockSet = new Mock<DbSet<Cfp>>();
            mockSet.As<IQueryable<Cfp>>().Setup(m => m.Provider).Returns(cfps.Provider);
            mockSet.As<IQueryable<Cfp>>().Setup(m => m.Expression).Returns(cfps.Expression);
            mockSet.As<IQueryable<Cfp>>().Setup(m => m.ElementType).Returns(cfps.ElementType);
            mockSet.As<IQueryable<Cfp>>().Setup(m => m.GetEnumerator()).Returns(cfps.GetEnumerator());

            var mockContext = new Mock<CfpContext>();
            mockContext.Setup(c => c.Cfps).Returns(mockSet.Object);
            return mockContext;
        }
    }
}
