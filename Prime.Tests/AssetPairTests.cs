using System;
using Prime.Common;
using Xunit;

namespace Prime.Tests
{
    public class AssetPairTests
    {
        [Theory]
        [InlineData("BTCUSD")]
        [InlineData("bTCUsD")]
        [InlineData("btcusd")]
        public void Should_BeUpperCase_ForAllInput(string input)
        {
            var pair = new AssetPair(input);

            Assert.Equal("BTCUSD", pair.ToString());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_ThrowArgumentNullException_When_RawIsNullOrEmptyOrWhiteSpace(string input)
        {
            Assert.Throws<ArgumentNullException>(() => { var pair = new AssetPair(input); });
        }

        [Theory]
        [InlineData("BTC:USD")]
        [InlineData("BTC/USD")]
        [InlineData("BTC_USDT")]
        [InlineData("BTC-USDT")]
        public void Should_ThrowFormatException_When_RawHasSeparators(string input)
        {
            Assert.Throws<FormatException>(() => { var pair = new AssetPair(input); });
        }
    }
}