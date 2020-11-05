using CnabContasReceber.Bancos;
using CnabContasReceber.Models;
using System;
using System.Text;
using Xunit;
using static CnabContasReceber.Models.TituloReceber;

namespace CNABContasReceber.Testes.Itau
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
            var valor = linha.Slice(161, 173);


            Assert.Equal("0000000000022", valor);
        }

        [Fact]
        public void Escreveu_Data_Desconto_Correto()
        {
            var linha = GerarLinhaDetalhe(Titulo2());
            var valor1 = linha.Slice(174, 179);
            var valor2 = linha.Slice(354, 359);
            var valor3 = linha.Slice(373, 378);

            Assert.Equal("101021", valor1);
            Assert.Equal("021021", valor2);
            Assert.Equal("041021", valor3);
        }
        [Fact]
        public void Escreveu_Valor_Desconto_Correto()
        {
            var linha = GerarLinhaDetalhe(Titulo2());
            var valor1 = linha.Slice(180, 192);
            var valor2 = linha.Slice(360, 372);
            var valor3 = linha.Slice(379, 391);

            Assert.Equal("0000000000200", valor1);
            Assert.Equal("0000000001000", valor2);
            Assert.Equal("0000000000500", valor3);
        }
        

        public static string GerarLinhaDetalhe(TituloReceber titulo)
        {
            var cnab = new BancoItau400(Opcoes());
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
                Carteira = "57",                
                Msg1 = "zazaza",
                Msg2 = "popopo",
                NumeroAgencia = "8380",
                NumeroContaCorrente = "1558",
                DigitoContaCorrente = '8',                
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
                Valor = 10.99m,
                CobraMulta = false,
                PercentualMoraDiaAtraso = 2m,
                PercentualMulta = 2m,
                Desconto1 = new DescontosTitulo { DiasDesconto = 0, ValorDesconto = 2m },
                Desconto2 = new DescontosTitulo { DiasDesconto = 8, ValorDesconto = 10m },
                Desconto3 = new DescontosTitulo { DiasDesconto = 6, ValorDesconto = 5m }
            };
        }


        public static TituloReceber Titulo2()
        {
            return new TituloReceber()
            {
                Cep = "05201-210",
                CpfCnpj = "32.140.856/0001-59",
                Emissao = new DateTime(2019, 10, 2),
                Vencimento = new DateTime(2021, 10, 10),
                EnderecoCompleto = "RUA ALBION 193",
                NomePagador = "LOJAS RENNER LTDA",
                NossoNumero = "234645",
                NumeroTitulo = "12345",
                Valor = 10m,
                CobraMulta = false,
                PercentualMoraDiaAtraso = 2m,
                PercentualMulta = 2m,
                Desconto1 = new DescontosTitulo { DiasDesconto = 0, ValorDesconto = 2m },
                Desconto2 = new DescontosTitulo { DiasDesconto = 8, ValorDesconto = 10m },
                Desconto3 = new DescontosTitulo { DiasDesconto = 6, ValorDesconto = 5m }
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
                Valor = 1938201390.99000m,
                CobraMulta = false,
                PercentualMoraDiaAtraso = 2m,
                PercentualMulta = 2m,
                Desconto1 = new DescontosTitulo { DiasDesconto = 0, ValorDesconto = 2m },
                Desconto2 = new DescontosTitulo { DiasDesconto = 8, ValorDesconto = 10m },
                Desconto3 = new DescontosTitulo { DiasDesconto = 6, ValorDesconto = 5m }
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
                CobraMulta = false,
                PercentualMoraDiaAtraso = 2m,
                PercentualMulta = 2m,
                Valor = 0.9900000m
            };
        }


    }
}
