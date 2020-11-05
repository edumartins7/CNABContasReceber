using CnabContasReceber.Interfaces;
using CnabContasReceber.Models;
using CnabContasReceber.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CnabContasReceber.Bancos
{
    /// <summary>
    /// O "Nosso Número" da Sicredi torna a implementação horrível. Isso porque eles te obrigam a passar um Id sequencial de 1 a 99999. Se ultrapassar 99999 títulos no ano você precisa variar o Byte.
    /// Isso significa que seu Id interno não pode ser utilizado e te força a controlar esse Id Sicredi no seu backend. No nosso caso significou criar uma tabela dedicada a Sicredi que é populada por uma trigger.
    /// </summary>
    /// 
    public class BancoSicred400 : IBanco
    {
        private int _index = 1;

        public BancoSicred400(Opcoes opcoes)
        {
            Opcoes = opcoes;
        }

        public Opcoes Opcoes { get; set; }

        public string MontarArquivo(IEnumerable<TituloReceber> titulos)
        {
            _index = 1;

            var b = new StringBuilder();

            Header(b);

            foreach(TituloReceber t in titulos)
            {
                Detalhe1(b, t);

                if (Opcoes.BancoEnviaBoleto)
                    throw new NotImplementedException("BancoEnviaBoleto");

                if (Opcoes.CobrancaCompartilhada)
                {
                    throw new NotImplementedException("CobrancaCompartilhada");
                }
            }

            Trailer(b);

            return b.ToString();
        }

        public void Header(StringBuilder b)
        {
            b.Append("01REMESSA01"); //01-11
            b.AppendTexto(8, "COBRANCA"); //12-19
            b.Append(new string(' ', 7)); //20
            b.AppendNumero(5, Opcoes.CodigoEmpresa); //27-31
            b.AppendNumero(14, Opcoes.CnpjBeneficiario); //32-45
            b.Append(new string(' ', 31)); //46-76
            b.Append("748"); //77-79
            b.AppendTexto(15, "SICREDI");
            b.AppendData(DateTime.Now, "yyyyMMdd"); //95-102
            b.Append(new string(' ', 8)); //103-110
            b.AppendNumero(7, Opcoes.NumeroSequencialRemessaCnab); //111-117
            b.Append(new string(' ', 273)); //118-390
            b.Append("2.00"); //391-394
            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine);
        }

        public void Detalhe1(StringBuilder b, TituloReceber titulo)
        {
            b.Append("1"); //1-1
            b.AppendTexto(3, "AAA"); //02-04
            b.Append(new string(' ', 12)); //5-16
            b.Append("ABB"); //17-19
            b.Append(new string(' ', 28)); //20-47
            b.AppendNumero(9, titulo.NossoNumero + CalcularDv(titulo)); //48-56 - importante: já trazer no formato AABXXXXX do seu backend. 
            b.Append(new string(' ', 6)); //57-62
            b.AppendData(DateTime.Now, "yyyyMMdd"); //63-70 Data de instrução??
            b.Append(' '); //71
            b.Append('N'); //72, 'S' para a sicredi fazer a postagem 
            b.Append(' ');//73
            b.Append('B'); //74
            b.Append("00"); //75-76
            b.Append("00"); //77-78
            b.Append(new string(' ', 4)); //79-82
            b.Append(new string('0', 10)); //83-92
            b.AppendNumero(4, Math.Round(Opcoes.PercentualMulta, 2).ToString("#.00", CultureInfo.InvariantCulture)); //93-96
            b.Append(new string(' ', 12)); //97-108
            b.Append("01"); //109-110
            b.AppendTexto(10, titulo.NumeroTitulo); //111-120
            b.AppendData(titulo.Vencimento); //121-126
            b.AppendDinheiro(13, titulo.Valor); //127-139
            b.Append(new string(' ', 9)); //140-148
            b.Append("AN"); //149-150
            b.AppendData(titulo.Emissao); //151-156
            b.Append("00"); //157-158
            b.Append("00"); //159-160
            b.AppendNumero(13, Math.Round(Opcoes.PercentualMoraDiaAtraso, 2).ToString("#.00", CultureInfo.InvariantCulture)); //161-173
            b.Append(new string('0', 6)); //174-179
            b.Append(new string('0', 13)); //180-192
            b.Append("00"); //193-194
            b.Append("00"); //195-196
            b.Append(new string('0', 9)); //197-205
            b.Append(new string('0', 13)); //206-218
            b.AppendNumero(2, titulo.PessoaJuridica() ? "20" : "10"); //219-220
            b.AppendNumero(14, titulo.CpfCnpj); //221-234
            b.AppendTexto(40, titulo.NomePagador); //235-274
            b.AppendTexto(40, titulo.EnderecoCompleto); //275-314
            b.Append(new string('0', 5)); //315-319
            b.Append(new string('0', 6)); //320-325
            b.Append(' '); //326-326
            b.AppendNumero(8, titulo.Cep); //327-334
            b.Append(new string('0', 5)); //335-339
            b.Append(new string('0', 14));//340-353
            b.Append(new string(' ', 41));  //354-394
            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine);
        }

        public void Trailer(StringBuilder b)
        {
            b.Append("91748"); //01-05
            b.AppendNumero(5, Opcoes.CodigoEmpresa); //05-10
            b.Append(new string(' ', 384)); //11-394
            b.AppendNumero(6, _index); //395-400      
            b.Append(Environment.NewLine);
        }



        public string CalcularDv(TituloReceber titulo)
        {           

            if (Opcoes.NumeroAgencia.Length > 4 || Opcoes.CodigoUaSicredi.Length > 2)
                throw new Exception("Numero agencia ou código UA invalidos");


            //da documentação: "Relacionar os códigos da Cooperativa (aaaa), posto beneficiário (pp), beneficiário (ccccc), ano atual (yy), byte
            //de geração do Nosso Número(b) e o número sequencial do beneficiário(nnnnn): aaaappcccccyybnnnnn;"

            var ano = titulo.NossoNumero.Substring(0, 2);
            var bytee = titulo.NossoNumero.Substring(2, 1);
            var seq = titulo.NossoNumero.Substring(3, 5);


            return CalcularDv(Opcoes.CodigoEmpresa, Opcoes.NumeroAgencia, Opcoes.CodigoUaSicredi, int.Parse(ano), int.Parse(bytee), int.Parse(seq));
        }



        /// <summary>
        /// Calcula o DV do nosso número
        /// </summary>
        /// <param name="codigoEmpresa">da tabela Bancos</param>
        /// <param name="numeroAgencia">da tabela Bancos</param>
        /// <param name="codigoUa">da tabela Bancos</param>
        /// <param name="ano">no formato 20, 21, 22... da tabela Contas_Receber_Seq_Sicredi</param>
        /// <param name="bytee">da tabela Contas_Receber_Seq_Sicredi</param>
        /// <param name="seq">da tabela Contas_Receber_Seq_Sicredi</param>
        /// <returns></returns>
        public static string CalcularDv(string codigoEmpresa, string numeroAgencia, string codigoUa, int ano, int bytee, int seq)
        {
            if (numeroAgencia.Length > 4 || codigoUa.Length > 2 || ano > 100 || bytee > 9 || seq > 99999)
                throw new Exception("parâmetros inválidos.");

            var baseCalculo = numeroAgencia.PadLeft(4, '0') + codigoUa.PadLeft(2, '0') + codigoEmpresa.PadLeft(5, '0') + ano + bytee + seq.ToString().PadLeft(5, '0');

            int digito, soma = 0, peso = 2, bas = 9;

            //Atribui os pesos de {2..9}
            for (int i = baseCalculo.Length - 1; i >= 0; i--)
            {
                soma = soma + (Convert.ToInt32(baseCalculo.Substring(i, 1)) * peso);
                if (peso < bas)
                    peso = peso + 1;
                else
                    peso = 2;
            }

            digito = 11 - (soma % 11);//Calcula o Módulo 11;

            if (digito > 9)
                digito = 0;

            return digito.ToString();
        }

        public string NomearArquivo(DateTime? dt = null)
        {
            if (dt == null)
                dt = DateTime.Today;

            char digitoMes = dt.Value.Month.ToString()[0];

            if (dt.Value.Month == 10)
                digitoMes = 'O';
            else if (dt.Value.Month == 11)
                digitoMes = 'N';
            else if (dt.Value.Month == 12)
                digitoMes = 'D';

            return $"{Opcoes.CodigoEmpresa}{digitoMes}{dt.Value:dd}.CRM";
        }

    }
}