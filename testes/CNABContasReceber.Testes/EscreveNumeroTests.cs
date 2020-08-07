using System;
using System.Text;
using Xunit;
using CnabContasReceber.Util;

namespace CNABContasReceber.Testes
{
    public class EscreveNumeroTests
    {

        public EscreveNumeroTests()
        {
        }

        [Fact]
        public void AppendNumero()
        {
            var sb = new StringBuilder();

            sb.AppendNumero(8, "123");

            Assert.Equal("00000123", sb.ToString());
        }

        [Fact]
        public void AppendNumero2()
        {
            var sb = new StringBuilder();

            sb.AppendNumero(8, "12345678");

            Assert.Equal(8, sb.ToString().Length);
        }

        [Fact]
        public void AppendNumero3()
        {
            var sb = new StringBuilder();

            Assert.Throws<Exception>(() => sb.AppendNumero(8, "123456789"));
        }

    }
}
