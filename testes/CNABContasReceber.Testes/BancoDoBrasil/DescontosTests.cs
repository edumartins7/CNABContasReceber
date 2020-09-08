using CnabContasReceber.Bancos;
using CnabContasReceber.Models;
using System;
using System.Text;
using Xunit;

namespace CNABContasReceber.Testes.BancoDoBrasil
{
    public class DescontosTests
    {
        private string _linha1;
        private string _linha2;
        private string _linha3;

        public DescontosTests()
        {
            _linha1 = GerarLinhaDetalhe(Opcoes());
            _linha2 = GerarLinhaDetalhe(Opcoes2());
            _linha3 = GerarLinhaDetalhe(Opcoes3());
        }

        [Fact]
        public void Tem400Caracteres()
        {
            Assert.Equal(400, _linha1.Length - 2); //o enter pra linha de baixo conta como 2
            Assert.Equal(400, _linha2.Length - 2);
            Assert.Equal(400, _linha3.Length - 2);
        }

        [Fact]
        public void DataDescontoEhValida()
        {
            var linha = GerarLinhaDetalhe(Opcoes());
            var data1 = linha.Slice(4, 9);
            var data2 = linha.Slice(27, 32);

            Assert.Equal("160920", data1);
            Assert.Equal("000000", data2);
        }

        [Fact]
        public void ValorDescontoCorreto()
        {
            var linha = GerarLinhaDetalhe(Opcoes());
            var valor = linha.Slice(10, 26);
            var valor2 = linha.Slice(33, 49);

            Assert.Equal("00000000000010623", valor);
            Assert.Equal("00000000000000000", valor2);

        }

        public static string GerarLinhaDetalhe(Opcoes opcoes)
        {
            var cnab = new BancoDoBrasil400(opcoes);
            var sb = new StringBuilder();
            cnab.DescontosAdicionais(sb, Titulo());

            return sb.ToString();
        }

        public static Opcoes Opcoes()
        {
            return new Opcoes
            {
                DiasDesconto1 = 4,
                DiasDesconto2 = 4,
                DiasDesconto3 = 0,
                PorcentagemDesconto1 = 10m,
                PorcentagemDesconto2 = 10m,
                PorcentagemDesconto3 = 20m
            };
        }
        public static Opcoes Opcoes2()
        {
            return new Opcoes
            {
                DiasDesconto2 = 2,
                DiasDesconto3 = 0,
                PorcentagemDesconto2 = 10m,
                PorcentagemDesconto3 = 22m
            };
        }
        public static Opcoes Opcoes3()
        {
            return new Opcoes
            {
                DiasDesconto2 = 4,
                DiasDesconto3 = 3,
                PorcentagemDesconto2 = 0m,
                PorcentagemDesconto3 = 33m
            };
        }
        public static TituloReceber Titulo()
        {
            return new TituloReceber()
            {
                Cep = "05201-210",
                CpfCnpj = "25840272833",
                Emissao = new DateTime(2020, 1, 2),
                Vencimento = new DateTime(2020, 9, 20),
                EnderecoCompleto = "RUA ALBION 193",
                NomePagador = "CARLOS EDUARDO REIS ",
                NossoNumero = "234645",
                NumeroTitulo = "12345",
                Valor = 1062.33m
            };
        }


    }
}
