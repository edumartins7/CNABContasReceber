using System;
using System.Collections.Generic;
using System.Text;

namespace CNABContasReceber.Retorno.Models
{
    public class LinhaHeader
    {
        public string NumeroAgencia { get; set; }
        public string NumeroConta { get; set; }
        public string NUmeroAvisoBancario { get; set; }
        public string DataGravacaoDoArquivo { get; set; }
        public string DataDoCredito { get; set; }
    }


    public class LinhaTransacaoTipo1
    {
        public string IdentificacaoOcorrencia { get; set; }
        public string DataOcorrenciaNoBanco { get; set; }
        public string NumeroDocumento { get; set; }
        public string DatavencimentoTitulo { get; set; }
        public string ValorTitulo { get; set; }
        public string BancoCobrador { get; set; }
        public string AgenciaCobradora { get; set; }
        public string ValorDespesasOcorrencias02E28Motivo04 { get; set; }
        public string ValorOutrasDespesas { get; set; } //no bradesco parece ser sempre zeros
        public string JurosOperacaoEmAtraso { get; set; }
        public string ValorIof { get; set; }
        public decimal ValorDoAbatimentoConcedidoSobreOTitulo { get; set; }
        public string DescontoConcedido { get; set; }
        public string ValorPago { get; set; }
        public string JurosMora { get; set; }
        public string OutrosCreditos { get; set; }
        public string OcorrenciaProtesto { get; set; }
        public string DataDoCredito { get; set; }
        public string OrigemDoPagamento { get; set; }
        public IEnumerable<string> MotivosRejeicoesCodigosDeOcorrencia { get; set; }
    }

    public class Trailer9
    {
        public string QuantidadeDeTitulosEmCobranca { get; set; }
        public string ValorEmCobranca { get; set; }
        public string NumeroAvisoBancario { get; set; }
        public string QuantidadeOcorrencia02 { get; set; }
        public string ValorOcorrencia02 { get; set; }
        public string ValorOcorrencia06Liquidacao { get; set; }
        public string QuantidadeOcorrencia06Liquidacao { get; set; }
        public string ValorOcorrencia06 { get; set; }
        public string QuantidadeOcorrencia09E10 { get; set; }
        public string ValorBaixadosEmOcorrencia09E10 { get; set; }
        public string QuantidadeOcorrencia13 { get; set; }
        public string ValorOcorrencia13 { get; set; }
        public string QuantidadeOcorrencia14 { get; set; }
        public string ValorOcorrencia14 { get; set; }
        public string QuantidadeOcorrencia12 { get; set; }
        public string ValorOcorrencia12 { get; set; }
        public string QuantidadeOcorrencia19 { get; set; }
        public string ValorOcorrencia19 { get; set; }
        public string ValorRateiosEfetuados { get; set; }
        public string QuantidadeRateiosEfetuados { get; set; }
    }
}
