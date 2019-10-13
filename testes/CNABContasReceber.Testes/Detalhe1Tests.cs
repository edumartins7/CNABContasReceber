using CnabContasReceber.Bancos;
using CnabContasReceber.Models;
using System;
using System.Text;
using Xunit;

namespace CNABContasReceber.Testes
{
    public class Detalhe1Tests
    {
        [Fact]
        public void Escreveu_Valor_Correto()
        {
            var linha = LinhaDetalheTit1();
            var valor = linha.Slice(127, 139);

            Assert.Equal("0000000001099", valor);
        }

        [Fact]
        public void Escreveu_Vencimento_Correto()
        {
            var linha = LinhaDetalheTit1();
            var valor = linha.Slice(121, 126);

            Assert.Equal("051119", valor);
        }

        public static string LinhaDetalheTit1()
        {
            var cnab = new BancoBradesco400(Opcoes());
            var sb = new StringBuilder();
            cnab.Detalhe1(sb, Titulo1());

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
                Carteira = "57",
                CobraMulta = false,
                Msg1 = "zazaza",
                Msg2 = "popopo",
                NumeroAgencia = "0989",
                NumeroContaCorrente = "7177",
                DigitoContaCorrente = '3',
                PercentualMoraDiaAtraso = 2m,
                PercentualMulta = 2m,
                RazaoSocial = "EMPRESA TAL LTDA"
            };
        }

        public static TituloReceber Titulo1()
        {
            return new TituloReceber()
            {
                Cep = "05201-210",
                CpfCnpj = "32.140.856/0001-59",
                Emissao = new DateTime(2019, 10, 2),
                Vencimento = new DateTime(2019, 11, 5),
                EnderecoCompleto = "RUA ALBION 193",
                NomePagador = "LOJAS RENNER LTDA",
                NossoNumero = "234645",
                NumeroTitulo = "12345",
                Valor = 10.99m
            };
        }
    }
}
