using Xunit;

namespace Waddle.Test
{
    public class StringExtensionsTest
    {
        [Fact]
        public void AutoTrim()
        {
            var source = @"
                this
                    should
                        be
                            trimmed
                ";
            var trimmed = @"this
    should
        be
            trimmed
";
            Assert.Equal(trimmed, source.AutoTrim());
        }
    }
}