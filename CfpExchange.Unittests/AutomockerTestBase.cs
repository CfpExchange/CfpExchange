using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.AutoMock;
using CfpExchange.Common.Data;
using CfpExchange.Common.Models;

namespace CfpExchange.UnitTests
{
    public class AutomockerTestBase : IDisposable
    {
        protected AutoMocker AutoMocker { get; private set; }

        public AutomockerTestBase()
        {
            AutoMocker = new AutoMocker(MockBehavior.Strict);
        }

        public void Dispose()
        {
            AutoMocker.VerifyAll();
        }

        protected static Mock<CfpContext> SetupMockedCfpContext(IQueryable<Cfp> cfps)
        {
            var mockSet = new Mock<DbSet<Cfp>>();
            mockSet.As<IQueryable<Cfp>>().Setup(m => m.Provider).Returns(cfps.Provider);
            mockSet.As<IQueryable<Cfp>>().Setup(m => m.Expression).Returns(cfps.Expression);

            var mockContext = new Mock<CfpContext>();
            mockContext.Setup(c => c.Cfps).Returns(mockSet.Object);

            return mockContext;
        }
    }
}