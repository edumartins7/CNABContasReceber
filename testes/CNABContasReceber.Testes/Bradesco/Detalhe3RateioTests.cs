using CnabContasReceber.Bancos;
using CnabContasReceber.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CNABContasReceber.Testes.Bradesco
{
    public class Detalhe3RateioTests
    {
        private string _linhaTitulo1Rateio;
        private string _linhaTitulo2Rateio;

        public Detalhe3RateioTests()
        {
            _linhaTitulo1Rateio = GerarLinhaDetalhe3(Titulo1());
            _linhaTitulo2Rateio = GerarLinhaDetalhe3(Titulo2());
        }

        [Fact]
        public void Tem400Caracteres()
        {
            Assert.Equal(400, _linhaTitulo2Rateio.Length -2); //o enter pra linha de baixo conta como 2
        }

        //[Fact]
        //public void Se_Soh_Um_Rateio_Nem_Escreve()
        //{
        //    Assert.True(string.IsNullOrEmpty(_linhaTitulo1Rateio));
        //}

        [Fact]
        public void Escreveu_Banco_Correto()
        {
            var valor1 = _linhaTitulo2Rateio.Slice(44, 46);
            var valor2 = _linhaTitulo2Rateio.Slice(161, 163);
            var valor3 = _linhaTitulo2Rateio.Slice(278, 280);

            Assert.Equal("237", valor1);
            Assert.Equal("237", valor2);
            Assert.Equal("237", valor3);
        }

        [Fact]
        public void Soma_Rateios_Igual_Total()
        {
            var valorTotal = GerarLinhaDetalhe(Titulo2()).Slice(127, 139);

            Assert.Equal("0000000001099", valorTotal);

            var valorPrimeiroRateio = _linhaTitulo2Rateio.Slice(66, 80);

            Assert.Equal("000000000001000", valorPrimeiroRateio);

            var valorsegundoRateio = _linhaTitulo2Rateio.Slice(183, 197);
            
            Assert.Equal("000000000000099", valorsegundoRateio);
        }


        public static string GerarLinhaDetalhe(TituloReceber titulo)
        {
            var cnab = new BancoBradesco400(Opcoes());
            var sb = new StringBuilder();
            cnab.Detalhe1(sb, titulo);

            return sb.ToString();
        }

        public static string GerarLinhaDetalhe3(TituloReceber titulo)
        {
            var cnab = new BancoBradesco400(Opcoes());
            var sb = new StringBuilder();

            var batch1 = titulo.RateioCredito.Batch(3).First();

            cnab.Detalhe3(sb, titulo.NossoNumero, batch1);

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
                NumeroAgencia = "0989",
                NumeroContaCorrente = "7177",
                DigitoContaCorrente = '3',
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
                RateioCredito = new List<RateioCredito>()
                {
                    new RateioCredito()
                    {
                        AgenciaSemDigito = "0322",
                        Carteira = "09",
                        ContaCorrente = "4777",
                        DigitoAgencia = '0',
                        DigitoContaCorrente = '5',
                        ValorRateio = 10m
                    }
                }
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
                Valor = 10.99m,
                CobraMulta = false,
                PercentualMoraDiaAtraso = 2m,
                PercentualMulta = 2m,
                RateioCredito = new List<RateioCredito>()
                {
                    new RateioCredito()
                    {
                        AgenciaSemDigito = "0322",
                        Carteira = "09",
                        ContaCorrente = "4777",
                        DigitoAgencia = '0',
                        DigitoContaCorrente = '5',
                        ValorRateio = 10m
                    },
                    new RateioCredito()
                    {
                        AgenciaSemDigito = "0322",
                        Carteira = "09",
                        ContaCorrente = "4888",
                        DigitoAgencia = '0',
                        DigitoContaCorrente = '6',
                        ValorRateio = 0.99m
                    },
                }
            };
        }


    }
}
