using System;
using System.Linq;
using CfpExchange.Data;
using CfpExchange.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.AutoMock;

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
            mockSet.As<IQueryable<Cfp>>().Setup(m => m.ElementType).Returns(cfps.ElementType);
            mockSet.As<IQueryable<Cfp>>().Setup(m => m.GetEnumerator()).Returns(cfps.GetEnumerator());

            var mockContext = new Mock<CfpContext>();
            mockContext.Setup(c => c.Cfps).Returns(mockSet.Object);

            return mockContext;
        }
    }
}
