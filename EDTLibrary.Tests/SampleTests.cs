using Xunit;

namespace EDTLibrary.Tests
{
    public class SampleTests
    {
        [Fact]
        public void Equal()
        {
            Assert.Equal(1, 1);
        }

        [Fact]
        public void NotEqual()
        {
            Assert.NotEqual(1, 2);
        }
    }
}