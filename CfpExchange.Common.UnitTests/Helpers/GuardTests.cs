using System;

using Xunit;

using CfpExchange.Common.Helpers;

namespace CfpExchange.Common.UnitTests.Helpers
{
    public class GuardTests
    {
        #region Fields

        private static object _nullValue = null;
        private static object _nonNullValue = new object();

        #endregion

        [Fact]
        public void IsNotNull_WithArgumentValueNull_ShouldThrowArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => Guard.IsNotNull(_nullValue, nameof(_nullValue)));

            Assert.Equal(nameof(_nullValue), exception.ParamName);
        }

        [Fact]
        public void IsNotNull_WithArgumentValueNotNull_ShouldNotThrowArgumentNullException()
        {
            Guard.IsNotNull(_nonNullValue, nameof(_nonNullValue));
        }
    }
}
