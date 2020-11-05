using CnabContasReceber.Bancos;
using CnabContasReceber.Models;
using System;
using System.Text;
using Xunit;

namespace CNABContasReceber.Testes.BancoDoBrasil
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
            var valor = linha.Slice(39, 43);

            Assert.Equal("12345", valor);
        }

        [Fact]
        public void Escreveu_Valor_Correto()
        {
            var valor1 = _linha1.Slice(127, 139);
            var valor2 = _linha2.Slice(127, 139);
            var valor3 = _linha3.Slice(127, 139);
            var valor4 = _linha4.Slice(127, 139);

            Assert.Equal("0000000106233", valor1);
            Assert.Equal("0000000001000", valor2);
            Assert.Equal("0193820139099", valor3);
            Assert.Equal("0000000000099", valor4);
        }

        [Fact]
        public void Escreveu_Vencimento_Correto()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var valor = linha.Slice(121, 126);

            Assert.Equal("150520", valor);
        }

        [Fact]
        public void Escreveu_Mora_Correta()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var valor = linha.Slice(161, 173);
            

            Assert.Equal("0000000002125", valor);
        }

        [Fact]
        public void Escreveu_NossoNumero_Correto()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var nossoNumero = linha.Slice(64, 80);
            

            Assert.Equal("23232130000234645", nossoNumero);
        }
        [Fact]
        public void Escreveu_Mensagem_Correta()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var mensagem = linha.Slice(352, 391);
            

            Assert.Equal("ZAZAZA" + new string(' ', 34), mensagem);
        }

        public static string GerarLinhaDetalhe(TituloReceber titulo)
        {
            var cnab = new BancoDoBrasil400(Opcoes());
            var sb = new StringBuilder();
            titulo.CalcularDescontos(titulo);
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
