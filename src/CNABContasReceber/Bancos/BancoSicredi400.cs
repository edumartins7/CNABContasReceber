using CnabContasReceber.Interfaces;
using CnabContasReceber.Models;
using CnabContasReceber.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace CnabContasReceber.Bancos
{
    public class BancoSicred400 : IBanco
    {
        private int _index = 1;
        private int _qtdeTitulos = 0;
        private decimal _valorTotalTitulos = 0;


        public BancoSicred400(Opcoes opcoes)
        {
            Opcoes = opcoes;
        }

        public Opcoes Opcoes { get; set; }

        public string MontarArquivo(IEnumerable<TituloReceber> titulos)
        {
            _index = 1;
            _qtdeTitulos = titulos.Count();
            _valorTotalTitulos = titulos.Sum(x => x.Valor);

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
            b.AppendNumero(3, "AAA"); //02-04
            b.Append(new string(' ', 12)); //5-16
            b.Append("ABB"); //17-19
            b.Append(new string(' ', 28)); //20-47
            b.AppendNumero(9, titulo.NossoNumero); //48-56
            b.Append(new string(' ', 6)); //57-62
            b.AppendData(DateTime.Now, "yyyyMMdd"); //63-70 Data de instrução??
            b.Append(' '); //71
            b.Append(' '); //72, 'S' para a sicredi fazer a postagem 
            b.Append(' ');//73
            b.Append('B'); //74
            b.Append("01"); //75-76
            b.Append("01"); //77-78
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
        }

        //private void RecalcularNossoNumero()
        //{
        //    //da documentação: "Relacionar os códigos da Cooperativa (aaaa), posto beneficiário (pp), beneficiário (ccccc), ano atual (yy), byte
        //    //de geração do Nosso Número(b) e o número sequencial do beneficiário(nnnnn): aaaappcccccyybnnnnn;"

        //    var baseCalculo = $"{}";
        //}


        public string DigNossoNumeroSicredi(string numeroSequencialTitulo)
        {
            
            var sequencialInt = int.Parse(numeroSequencialTitulo);
            var bytte = 2; //a sicred aceita numeros sequenciais até 99.999



            if (Opcoes.NumeroAgencia.Length > 4 || Opcoes.CodigoUaSicredi.Length > 2 || numeroSequencialTitulo.Length > 4)
                throw new Exception("Numero agencia, código UA ou sequencial invalidos");


            //da documentação: "Relacionar os códigos da Cooperativa (aaaa), posto beneficiário (pp), beneficiário (ccccc), ano atual (yy), byte
            //de geração do Nosso Número(b) e o número sequencial do beneficiário(nnnnn): aaaappcccccyybnnnnn;"

            var isNumeric = int.TryParse(Opcoes.CodigoUaSicredi, out _);

            var baseCalculo = $"{Opcoes.NumeroAgencia}{Opcoes.CodigoUaSicredi}{numeroSequencialTitulo.Length > 4}"; 

           /* Variáveis
             * -------------
             * d - Dígito
             * s - Soma
             * p - Peso
             * b - Base
             * r - Resto
             */

            int d, s = 0, p = 2, b = 9;
            //Atribui os pesos de {2..9}
            for (int i = baseCalculo.Length - 1; i >= 0; i--)
            {
                s = s + (Convert.ToInt32(baseCalculo.Substring(i, 1)) * p);
                if (p < b)
                    p = p + 1;
                else
                    p = 2;
            }
            d = 11 - (s % 11);//Calcula o Módulo 11;
            if (d > 9)
                d = 0;
            return d.ToString();
        }



        private string CalcularDvNossoNumero(string input)
        {
            var withZeroes = input.PadLeft(7, '0');

            char[] charArr = withZeroes.ToCharArray().Reverse().ToArray();

            int[] numbersArr = Array.ConvertAll<char, int>(charArr, x => (int) Char.GetNumericValue(x));

            var multiplier = 2;
            var total = 0;

            foreach (var n in numbersArr)
            {
                if (multiplier > 9)
                    multiplier = 2;

                total += n * multiplier++;
            }

            var resto = total % 11;

            if (resto == 10)
                return "1";
            else if (resto == 1 || resto == 0)
                return "0";

            return Math.Abs(resto - 11).ToString();
        }

    }
}