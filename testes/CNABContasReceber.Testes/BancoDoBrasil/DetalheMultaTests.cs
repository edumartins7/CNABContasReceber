using CnabContasReceber.Bancos;
using CnabContasReceber.Models;
using System;
using System.Text;
using Xunit;

namespace CNABContasReceber.Testes.BancoDoBrasil
{
    public class DetalheMultaTests
    {
        private string _linha1;
        private string _linha2;
        private string _linha3;
        private string _linha4;

        public DetalheMultaTests()
        {
            _linha1 = GerarLinhaDetalhe(Titulo1());
            _linha2 = GerarLinhaDetalhe(Titulo2());
            _linha3 = GerarLinhaDetalhe(Titulo3());
            _linha4 = GerarLinhaDetalhe(Titulo4());
        }

        [Fact]
        public void Tem400Caracteres()
        {
            Assert.Equal(400, _linha1.Length - 2); //o enter pra linha de baixo conta como 2
            Assert.Equal(400, _linha2.Length - 2);
            Assert.Equal(400, _linha3.Length - 2);
            Assert.Equal(400, _linha4.Length - 2);
        }

        [Fact]
        public void Escreveu_Vencimento_Correto()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var data = linha.Slice(5, 10);

            Assert.Equal("160520", data);
        }

        [Fact]
        public void Escreveu_Multa_Correta()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var valor = linha.Slice(11, 22);
            

            Assert.Equal("000000001000", valor);
        }

        [Fact]
        public void Escreveu_QuantidadeDias_Correta()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var quantidadeDias = linha.Slice(23, 25);

            Assert.Equal("030", quantidadeDias);
        }

        public static string GerarLinhaDetalhe(TituloReceber titulo)
        {
            var cnab = new BancoDoBrasil400(Opcoes());
            var sb = new StringBuilder();
            titulo.CalcularDescontos(titulo);
            cnab.DetalheMulta(sb, titulo);

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
                Carteira = "17",
                NumeroConvenio = "2323213",
                CobraMulta = true,
                Msg1 = "zazaza",
                Msg2 = "popopo",
                NumeroAgencia = "0989",
                NumeroContaCorrente = "7177",
                DigitoContaCorrente = '3',
                DigitoAgencia = '1',
                PercentualMoraDiaAtraso = 2m,
                PercentualMulta = 10m,
                DiasAposVencimento = 30,
                RazaoSocial = "EMPRESA TAL LTDA"
            };
        }

        public static TituloReceber Titulo1()
        {
            return new TituloReceber()
            {
                Cep = "05201-210",
                Cidade = "São Paulo",
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
