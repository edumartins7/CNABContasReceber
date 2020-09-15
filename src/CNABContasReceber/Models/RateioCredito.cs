namespace CnabContasReceber.Models
{
    /// <summary>
    /// Em algumas documentações é referido como "Cobrança Compartilhada"
    /// </summary>
    public class RateioCredito
    {
        public string Carteira { get; set; }
        public string AgenciaSemDigito { get; set; }
        public char DigitoAgencia { get; set; }
        public string ContaCorrente { get; set; }
        public char DigitoContaCorrente { get; set; }
        public decimal ValorRateio { get; set; } //pode ser um percentual ou o valor em dinheiro. depende doq for configurado em Opcoes.
        public string CnpjRecebedor { get; set; }
    }
}
