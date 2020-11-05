using CnabContasReceber.Bancos;
using CnabContasReceber.Models;
using System;
using Xunit;

namespace CNABContasReceber.Testes.Sicredi
{
    public class OutrosTestes
    {

        public OutrosTestes()
        {
        }

        [Fact]
        public void Nome_Arquivo_Correto()
        {
            var banco = new BancoSicred400(Opcoes());

            Assert.Equal("76584805.CRM", banco.NomearArquivo(new DateTime(2020, 8, 5)));

            Assert.Equal("76584O05.CRM", banco.NomearArquivo(new DateTime(2020, 10, 5)));
            Assert.Equal("76584N05.CRM", banco.NomearArquivo(new DateTime(2020, 11, 5)));
            Assert.Equal("76584D11.CRM", banco.NomearArquivo(new DateTime(2020, 12, 11)));

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
                NumeroAgencia = "0989",
                NumeroContaCorrente = "7177",
                DigitoContaCorrente = '3',                
                RazaoSocial = "EMPRESA TAL LTDA",
                CodigoUaSicredi = "17",
                CnpjBeneficiario = ""
            };
        }

    }
}
