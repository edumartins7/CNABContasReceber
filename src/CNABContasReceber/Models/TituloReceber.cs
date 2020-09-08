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

        public string Bairro { get; set; }
        public string Cidade  { get; set; }
        public string UF { get; set; }


        //public Desconto Desconto1 { get; set; }
        //public Desconto Desconto2 { get; set; }
        //public Desconto Desconto3 { get; set; }
        public IEnumerable<Desconto> Descontos { get; private set; }

        public void CalcularDescontos(Opcoes opcoes)
        {
            var res = new List<Desconto>();

            if (opcoes.Desconto1 != null)
                res.Add(opcoes.Desconto1.Calcular(this.Vencimento, this.Valor));

            if (opcoes.Desconto2 != null)
                res.Add(opcoes.Desconto2.Calcular(this.Vencimento, this.Valor));

            if (opcoes.Desconto3 != null)
                res.Add(opcoes.Desconto3.Calcular(this.Vencimento, this.Valor));

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
                return DataLimite.HasValue && DataLimite > DateTime.Today;
            }

        }
    }

    
}
