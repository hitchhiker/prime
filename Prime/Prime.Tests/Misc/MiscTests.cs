
using System.Diagnostics;
using Xunit;

namespace Prime.Tests.KeysManager
{
    public class MiscTests
    {
        [Fact]
        public void BasicTest()
        {
            var a = 5;
            var b = 3;

            var s = a + b;

            Debug.WriteLine("Test");

            Assert.Equal(8, s);
        }
    }
}
