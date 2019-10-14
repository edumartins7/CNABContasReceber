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
        public void Escreveu_Valor_Correto1()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var valor = linha.Slice(127, 139);

            Assert.Equal("0000000001099", valor);
        }

        [Fact]
        public void Escreveu_Valor_Correto2()
        {
            var linha2 = GerarLinhaDetalhe(Titulo2());
            var valor2 = linha2.Slice(127, 139);
            Assert.Equal("0000000001000", valor2);
        }

        [Fact]
        public void Escreveu_Valor_Correto3()
        {
            var linha3 = GerarLinhaDetalhe(Titulo3());
            var valor3 = linha3.Slice(127, 139);
            Assert.Equal("0193820139099", valor3);
        }


        [Fact]
        public void Escreveu_Valor_Correto4()
         {
            var linha = GerarLinhaDetalhe(Titulo4());
            var valor = linha.Slice(127, 139);
            Assert.Equal("0000000000099", valor);
        }

        [Fact]
        public void Escreveu_Vencimento_Correto()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var valor = linha.Slice(121, 126);

            Assert.Equal("051119", valor);
        }


        public static string GerarLinhaDetalhe(TituloReceber titulo)
        {
            var cnab = new BancoBradesco400(Opcoes());
            var sb = new StringBuilder();
            cnab.Detalhe1(sb, titulo);

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


        public static TituloReceber Titulo2()
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
                Valor = 10m
            };
        }

        public static TituloReceber Titulo3()
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
                Valor = 1938201390.99000m
            };
        }


        public static TituloReceber Titulo4()
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
                Valor = 0.9900000m
            };
        }


    }
}
