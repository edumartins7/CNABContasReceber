using CnabContasReceber.Bancos;
using CnabContasReceber.Models;
using System;
using System.Text;
using Xunit;

namespace CNABContasReceber.Testes.Santander
{
    public class Detalhe1Tests
    {
        private string _linha1;
        private string _linha2;
        private string _linha3;
        private string _linha4;

        public Detalhe1Tests()
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
        public void Escreveu_Numero_Titulo_Correto()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var valor = linha.Slice(38, 62);

            Assert.Equal("0000000000000000000012345", valor);
        }


        [Fact]
        public void Escreveu_Valor_Correto()
        {
            var valor1 = _linha1.Slice(127, 139);
            var valor2 = _linha2.Slice(127, 139);
            var valor3 = _linha3.Slice(127, 139);
            var valor4 = _linha4.Slice(127, 139);

            Assert.Equal("0000000001099", valor1);
            Assert.Equal("0000000001000", valor2);
            Assert.Equal("0193820139099", valor3);
            Assert.Equal("0000000000099", valor4);
        }

        [Fact]
        public void Escreveu_Vencimento_Correto()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var valor = linha.Slice(121, 126);

            Assert.Equal("051119", valor);
        }

        [Fact]
        public void Escreveu_Multa_Correta()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var valor = linha.Slice(79, 82);

            Assert.Equal("1000", valor);
        }

        public static string GerarLinhaDetalhe(TituloReceber titulo)
        {
            var cnab = new BancoSantander400(Opcoes());
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
                Carteira = "1",
                CobraMulta = true,
                Msg1 = "zazaza",
                Msg2 = "popopo",
                NumeroAgencia = "0989",
                NumeroContaCorrente = "7177",
                DigitoContaCorrente = '3',
                PercentualMoraDiaAtraso = 2m,
                PercentualMulta = 10m,
                RazaoSocial = "EMPRESA TAL LTDA"
            };
        }

        public static TituloReceber Titulo1()
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
