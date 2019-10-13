using System;
using Xunit;

namespace CNABContasReceber.Retorno.Testes
{
    public class UnitTest1
    {

        [Fact]
        public void SliceWorks()
        {
            var str = "ADS3980983223WQ";

            Assert.Equal("3980983223", str.Slice(4, 13));
            Assert.Equal(str, str.Slice(1, str.Length));
        }

        [Fact]
        public void SliceToDecimalWorks()
        {
            var str = "ADS3980983223WQ";

            Assert.Equal(39809832.23m, str.Slice<decimal>(4, 13));
        }


        [Fact]
        public void SliceToDateTimeWorks()
        {
            var str = "9oasid290391asdkkoe";

            Assert.Equal(new DateTime(1991, 3, 29), str.Slice<DateTime>(7, 12));
        }
    }
}
