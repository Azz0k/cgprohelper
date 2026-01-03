using System;
using System.Collections.Generic;
using System.Text;
using static GateKeeper.Core.Utils.Utils;

namespace GateKeeper.Tests
{
    public class UtilsTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("Aa.", false)]
        [InlineData("Aa*.ru", false)]
        [InlineData("aa,aa", false)]
        [InlineData("ну.ну", false)]
        [InlineData("2ip.ru", true)]
        [InlineData("*", true)]
        [InlineData("*.*", true)]
        [InlineData("*.ru", true)]
        [InlineData("me.*.com", true)]
        [InlineData("aA", true)]
        //“We intentionally disallow trailing dots”
        public void IsDomainValid_WithVariousInputs_ReturnsExpected(string? domain, bool expected)
        {
            bool res = isDomainPatternValid(domain);
            Assert.Equal(expected, res);
        }
            
    }
}
