using CnabContasReceber.Bancos;
using CnabContasReceber.Models;
using System;
using System.Text;
using Xunit;

namespace CNABContasReceber.Testes.Sicredi
{
    public class HeaderTests
    {
    

        public HeaderTests()
        {

        }

        [Fact]
        public void Tem400Caracteres()
        {

            var header = GerarHeader();

            Assert.Equal(400, header.Length - 2); //o enter pra linha de baixo conta como 2
        }


        [Fact]
        public void Cnpj_Bate()
        {
            var header = GerarHeader();

            var valor = header.Slice(32, 45);

            Assert.Equal("56728735000190", valor);
        }

        [Fact]
        public void Data_Bate()
        {
            var header = GerarHeader();

            var valor = header.Slice(95, 102);

            Assert.Equal(DateTime.Today.ToString("yyyyMMdd"), valor);
        }

        [Fact]
        public void Escreveu_Codigo_Banco_Correto()
        {
            string linha = GerarHeader();

            var valor = linha.Slice(77, 79);

            Assert.Equal("748", valor);
        }


        public static string GerarHeader()
        {
            var cnab = new BancoSicred400(Opcoes());
            var sb = new StringBuilder();
            cnab.Header(sb);

            return sb.ToString();
        }

        public static Opcoes Opcoes()
        {
            return new Opcoes
            {
                CodigoEmpresa = "4321",
                NumeroSequencialRemessaCnab = 1,
                ContadorTitulos = 7,
                BancoEnviaBoleto = false,
                Carteira = "1",
                CobraMulta = true,
                Msg1 = "zazaza",
                Msg2 = "popopo",
                NumeroAgencia = "0989",
                NumeroContaCorrente = "7177",
                DigitoContaCorrente = '3',
                PercentualMoraDiaAtraso = 2m,
                PercentualMulta = 10m,
                RazaoSocial = "EMPRESA TAL LTDA",
                CodigoUaSicredi = "17", 
                CnpjBeneficiario = "56.728.735/0001-90",
                CodigoBanco = "748"
            };
        }

    

    }
}
