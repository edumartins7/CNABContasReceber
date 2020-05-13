using System;
using System.Collections.Generic;
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



        //dependendo do banco pode vir como "cobrança compartilhada" na documentação
        public IEnumerable<RateioCredito> RateioCredito { get; set; } = new List<RateioCredito>();

        public bool PessoaJuridica()
        {
            var n = CpfCnpj.Normalize(NormalizationForm.FormD);
            var semPontuacao = Regex.Replace(n, "[^A-Za-z0-9| ]", string.Empty);

            return semPontuacao.Length >= 14;
        }
    }
}
