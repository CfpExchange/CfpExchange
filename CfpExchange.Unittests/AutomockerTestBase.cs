using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace CfpExchange.UnitTests
{
    public class AutomockerTestBase
    {
        protected AutoMocker AutoMocker { get; private set; }
 
        [SetUp]
        public virtual void SetUp()
        {
            AutoMocker = new AutoMocker(MockBehavior.Strict);
        }
    
        [TearDown]
        public virtual void TearDown()
        {
            AutoMocker.VerifyAll();
        }
    }
}
