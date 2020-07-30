using CnabContasReceber.Bancos;
using CnabContasReceber.Models;
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

            sb.AppendNumero(8, "2321122");

            Assert.Equal("02321122", sb.ToString());
        }

        [Fact]
        public void AppendNumero2()
        {
            var sb = new StringBuilder();

            sb.AppendNumero(8, "23211222");

            Assert.Equal(8, sb.ToString().Length);
        }

        [Fact]
        public void AppendNumero3()
        {
            var sb = new StringBuilder();

            sb.AppendNumero(8, "232112222");

            Assert.Throws<Exception>(() => sb.AppendNumero(8, "232112222"));
        }

    }
}
