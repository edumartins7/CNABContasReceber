

using System;

namespace CnabContasReceber.Models
{
    public class Opcoes
    {
        private string _codigoUaSicredi;

        public string CnpjBeneficiario { get; set; } //santander
        public string CodigoEmpresa { get; set; }
        public int NumeroSequencialRemessaCnab { get; set; }
        public int ContadorTitulos { get; set; }
        public string RazaoSocial { get; set; }
        public string NumeroAgencia { get; set; }
        public string NumeroContaCorrente { get; set; }
        public string NumeroConvenio { get; set; }
        public char DigitoContaCorrente { get; set; }
        public char DigitoAgencia { get; set; }
        public bool BancoEnviaBoleto { get; set; }
        public string Carteira { get; set; }
        public string VariacaoCarteira { get; set; }
        public bool CobraMulta { get; set; }
        public decimal PercentualMulta { get; set; }
        public decimal PercentualMoraDiaAtraso { get; set; }

        

        public string Msg1 { get; set; }
        public string Msg2 { get; set; }

        public bool CobrancaCompartilhada { get; set; } = false;
        public ETipoValorRateio ETipoValorRateio { get; set; } = ETipoValorRateio.Valor;

        public string CodigoBanco { get; set; } //alguns bancos tem dois codigos, como o santander (033 e 353)

        public string CodigoUaSicredi { //o SICREDI exige um código especial que eles chamam de "código UA" ou "posto beneficiário" (?) que é usado no cálculo do DV.
            get {
                return _codigoUaSicredi;
            } set {
                if(int.TryParse(value, out _) && value.Length <= 2)
                    _codigoUaSicredi = value.PadLeft(2, '0');
                else    
                    _codigoUaSicredi = "00";
            } 
        }

    }

    public enum ETipoValorRateio : short
    {
        Percentual = 1,
        Valor = 2
    }

    public class OpcoesDesconto
    {
        public int DiasDesconto { get; set; }
        public decimal Porcentagem { get; set; }


        public TituloReceber.Desconto Calcular(DateTime vencimentoTitulo, decimal valor)
        {
            return new TituloReceber.Desconto()
            {
                DataLimite = CalcularData(vencimentoTitulo),
                Valor = CalcularValor(valor)
            };
        }

        private DateTime? CalcularData(DateTime vencimentoTitulo)
        {
            DateTime d = vencimentoTitulo.AddDays(-DiasDesconto);

            if (d < DateTime.Today || DiasDesconto < 1)
                return null;

            return d;
        }

        private decimal CalcularValor(decimal valorTitulo)
        {
            return (valorTitulo * Porcentagem) / 100;
        }
    }
}
