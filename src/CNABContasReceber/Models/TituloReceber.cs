using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CnabContasReceber.Models
{
    public class TituloReceber
    {
        public string NumeroTitulo { get; set; }
        public DateTime Emissao { get; set; }
        public DateTime Vencimento { get; set; }
        public decimal Valor { get; set; }
        public string CpfCnpj { get; set; }
        public string NomePagador { get; set; }
        public string EnderecoCompleto { get; set; }
        public string Cep { get; set; }
        public string NossoNumero { get; set; }

        public bool ProtestavelAposVencimento { get; set; }
        public int DiasParaProtestar { get; set; }

        public bool CobraMulta { get; set; }
        public decimal PercentualMulta { get; set; }
        public int DiasAdicionaisAposVencimento { get; set; }
        public decimal PercentualMoraDiaAtraso { get; set; }

        public string Bairro { get; set; }
        public string Cidade  { get; set; }
        public string UF { get; set; }
        public DescontosTitulo Desconto1 { get; set; }
        public DescontosTitulo Desconto2 { get; set; }
        public DescontosTitulo Desconto3 { get; set; }


        public IEnumerable<Desconto> Descontos { get; private set; }

        public void CalcularDescontos(TituloReceber titulo)
        {
            var res = new List<Desconto>();

            if (titulo.Desconto1 != null)
                res.Add(Calcular(this.Vencimento, Desconto1));

            if (titulo.Desconto2 != null)
                res.Add(Calcular(this.Vencimento, Desconto2));

            if (titulo.Desconto3 != null)
                res.Add(Calcular(this.Vencimento, Desconto3));

            Descontos = res.Where(x => x.DataValida());
        }


        //dependendo do banco pode vir como "cobrança compartilhada" na documentação
        public IEnumerable<RateioCredito> RateioCredito { get; set; } = new List<RateioCredito>();

        public bool PessoaJuridica()
        {
            var n = CpfCnpj.Normalize(NormalizationForm.FormD);
            var semPontuacao = Regex.Replace(n, "[^A-Za-z0-9| ]", string.Empty);

            return semPontuacao.Length >= 14;
        }

        public class Desconto
        {
            public DateTime? DataLimite { get; set; }
            public decimal Valor { get; set; }

            public bool DataValida()
            {
                return DataLimite.HasValue && DataLimite >= DateTime.Today;
            }

        }

        public class DescontosTitulo
        {
            public int DiasDesconto { get; set; }
            public decimal ValorDesconto { get; set; }
        }

        public Desconto Calcular(DateTime vencimentoTitulo, DescontosTitulo desconto)
        {
            return new Desconto()
            {
                DataLimite = CalcularData(vencimentoTitulo, desconto),
                Valor = desconto.ValorDesconto
            };
        }

        private DateTime? CalcularData(DateTime vencimentoTitulo, DescontosTitulo descontos)
        {
            DateTime d = vencimentoTitulo.AddDays(-descontos.DiasDesconto);

            if (d < DateTime.Today || descontos.ValorDesconto <= 0)
                return null;

            if (descontos.DiasDesconto == 0)
                return vencimentoTitulo;

            return d;
        }
    }

    
}
