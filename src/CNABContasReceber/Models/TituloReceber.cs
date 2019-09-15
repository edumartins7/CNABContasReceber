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

        public bool PessoaJuridica()
        {
            var semPontuacao = Regex.Replace(CpfCnpj.Normalize(NormalizationForm.FormD), "[^A-Za-z0-9| ]", string.Empty);

            if (semPontuacao.Length >= 14)
                return true;

            return false;
        }

    }
}
