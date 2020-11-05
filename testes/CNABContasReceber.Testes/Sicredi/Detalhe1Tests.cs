using CnabContasReceber.Bancos;
using CnabContasReceber.Models;
using System;
using System.Text;
using Xunit;

namespace CNABContasReceber.Testes.Sicredi
{
    public class Detalhe1Tests
    {
        private string _linha1;

        public Detalhe1Tests()
        {
            _linha1 = GerarLinhaDetalhe(Titulo1());
        }

        [Fact]
        public void Tem400Caracteres()
        {
            Assert.Equal(400, _linha1.Length - 2); //o enter pra linha de baixo conta como 2
        }

        [Fact]
        public void Escreveu_Nosso_Numero_Correto()
        {
            var linha1 = GerarLinhaDetalhe(Titulo1());
            var linha2 = GerarLinhaDetalhe(Titulo2());

            var valor1 = linha1.Slice(48, 56);
            var valor2 = linha2.Slice(48, 56);

            Assert.Equal("202000015", valor1);
            Assert.Equal("202000023", valor2);

        }

        [Fact]
        public void Escreveu_SEU_Numero_Correto()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var valor = linha.Slice(111, 120);

            Assert.Equal("12345     ", valor);
        }

        [Fact]
        public void Calcula_Dv_Correto()
        {
            var banco = new BancoSicred400(Opcoes());
            var dv1 = banco.CalcularDv(Titulo1());
            var dv2 = banco.CalcularDv(Titulo2());

            Assert.Equal("3", dv2);
        }

        [Fact]
        public void Escreveu_Valor_Correto()
        {
            var valor1 = _linha1.Slice(127, 139);

            Assert.Equal("0000000106233", valor1);
        }

        [Fact]
        public void Escreveu_Vencimento_Correto()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var valor = linha.Slice(121, 126);

            Assert.Equal("150520", valor);
        }


        [Fact]
        public void Escreveu_Parcelas_Corretas()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var numeroParcelaCarne = linha.Slice(75, 76);
            var totalParcelas = linha.Slice(77, 78);

            Assert.Equal("00", numeroParcelaCarne);
            Assert.Equal("00", totalParcelas);
        }
 
        public static string GerarLinhaDetalhe(TituloReceber titulo)
        {
            var cnab = new BancoSicred400(Opcoes());
            var sb = new StringBuilder();
            cnab.Detalhe1(sb, titulo);

            return sb.ToString();
        }

        public static Opcoes Opcoes()
        {
            return new Opcoes
            {
                CodigoEmpresa = "76584",
                NumeroSequencialRemessaCnab = 1,
                ContadorTitulos = 7,
                BancoEnviaBoleto = false,
                Carteira = "1",                
                Msg1 = "zazaza",
                Msg2 = "popopo",
                NumeroAgencia = "0727",
                NumeroContaCorrente = "76584",
                DigitoContaCorrente = '8',
                RazaoSocial = "EMPRESA TAL LTDA",
                CodigoUaSicredi = "17", 
                CnpjBeneficiario = ""
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
                EnderecoCompleto = "RUA ALBION 9999",
                NomePagador = "EDUARDO MAR MOR",
                NossoNumero = "20200001",
                NumeroTitulo = "12345",
                Valor = 1062.33m,
                CobraMulta = true,
                PercentualMoraDiaAtraso = 2m,
                PercentualMulta = 10m,
            };
        }


        public static TituloReceber Titulo2()
        {
            return new TituloReceber()
            {
                Cep = "05201-210",
                CpfCnpj = "25840272833",
                Emissao = new DateTime(2020, 1, 2),
                Vencimento = new DateTime(2020, 5, 15),
                EnderecoCompleto = "RUA ALBION 9999",
                NomePagador = "EDUARDO MAR MOR ",
                NossoNumero = "20200002",
                NumeroTitulo = "12345",
                Valor = 99.99m,
                CobraMulta = true,
                PercentualMoraDiaAtraso = 2m,
                PercentualMulta = 10m,
            };
        }

    }
}
