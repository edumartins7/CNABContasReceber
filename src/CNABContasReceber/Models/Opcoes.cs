

namespace CnabContasReceber.Models
{
    public class Opcoes
    {
        public string CnpjBeneficiario { get; set; } //santander
        public string CodigoEmpresa { get; set; }
        public int NumeroSequencialRemessaCnab { get; set; }
        public int ContadorTitulos { get; set; }
        public string RazaoSocial { get; set; }
        public string NumeroAgencia { get; set; }
        public string NumeroContaCorrente { get; set; }
        public char DigitoContaCorrente { get; set; }
        public bool BancoEnviaBoleto { get; set; }
        public string Carteira { get; set; }
        public bool CobraMulta { get; set; }
        public decimal PercentualMulta { get; set; }
        public decimal PercentualMoraDiaAtraso { get; set; }

        public string Msg1 { get; set; }
        public string Msg2 { get; set; }

        public bool CobrancaCompartilhada { get; set; } = false;
        public ETipoValorRateio ETipoValorRateio { get; set; } = ETipoValorRateio.Valor;

        public string CodigoBanco { get; set; } //alguns bancos tem dois codigos, como o santander (033 e 353)
    }

    public enum ETipoValorRateio : short
    {
        Percentual = 1,
        Valor = 2
    }
}
