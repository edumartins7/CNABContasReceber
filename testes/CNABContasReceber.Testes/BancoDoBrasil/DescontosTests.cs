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
        public void DataDesconto1()
        {
            var linha = GerarLinhaDetalhe(Opcoes());
            var valor = linha.Slice(4, 9);

            Assert.Equal("110520", valor);
        }

        [Fact]
        public void ValorDesconto1()
        {
            var linha = GerarLinhaDetalhe(Opcoes());
            var valor = linha.Slice(10, 26);

            Assert.Equal("00000000000010623", valor);
        }

        [Fact]
        public void DataDesconto2()
        {
            var linha = GerarLinhaDetalhe(Opcoes2());
            var valor = linha.Slice(27, 32);

            Assert.Equal("000000", valor);
        }

        [Fact]
        public void ValorDesconto2()
        {
            var linha = GerarLinhaDetalhe(Opcoes2());
            var valor = linha.Slice(33, 49);

            Assert.Equal("00000000000000000", valor);
        }

        public static string GerarLinhaDetalhe(Opcoes opcoes)
        {
            var cnab = new BancoDoBrasil400(opcoes);
            var sb = new StringBuilder();
            cnab.Descontos(sb, Titulo());

            return sb.ToString();
        }

        public static Opcoes Opcoes()
        {
            return new Opcoes
            {
                DiasDesconto2= 4,
                DiasDesconto3= 2,
                PorcentagemDesconto2 = 10m,
                PorcentagemDesconto3 = 0m
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
                Vencimento = new DateTime(2020, 5, 15),
                EnderecoCompleto = "RUA ALBION 193",
                NomePagador = "CARLOS EDUARDO REIS ",
                NossoNumero = "234645",
                NumeroTitulo = "12345",
                Valor = 1062.33m
            };
        }


    }
}
