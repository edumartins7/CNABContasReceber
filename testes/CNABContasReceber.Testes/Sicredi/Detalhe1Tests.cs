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
            var linha = GerarLinhaDetalhe(Titulo1());
            var valor = linha.Slice(48, 56);

            Assert.Equal("203000071", valor);
        }

        [Fact]
        public void Escreveu_SEU_Numero_Correto()
        {
            var linha = GerarLinhaDetalhe(Titulo1());
            var valor = linha.Slice(111, 120);

            Assert.Equal("12345     ", valor);
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
                EnderecoCompleto = "RUA ALBION 193",
                NomePagador = "CARLOS EDUARDO REIS ",
                NossoNumero = "20300007",
                NumeroTitulo = "12345",
                Valor = 1062.33m
            };
        }



    }
}
