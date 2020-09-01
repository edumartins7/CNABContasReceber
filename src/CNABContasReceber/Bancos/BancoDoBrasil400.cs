﻿using CnabContasReceber.Interfaces;
using CnabContasReceber.Models;
using CnabContasReceber.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CnabContasReceber.Bancos
{
    public class BancoDoBrasil400 : IBanco
    {
        private int _index = 1;
        private int _qtdeTitulos = 0;
        private decimal _valorTotalTitulos = 0;

        public BancoDoBrasil400(Opcoes opcoes)
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
            //b.Append("01TESTE  01"); //01-11 Para Teste
            b.AppendTexto(8, "COBRANCA"); //12-19
            b.Append(new string(' ', 7)); //20-26
            b.AppendNumero(4, Opcoes.NumeroAgencia); //27-30
            b.Append(Opcoes.DigitoAgencia); //31-31
            b.AppendNumero(8, Opcoes.NumeroContaCorrente); //32-39
            b.Append(Opcoes.DigitoContaCorrente); //40-40
            b.Append("000000"); //41-46
            b.AppendTexto(30, Opcoes.RazaoSocial); //47-76
            b.AppendTexto(18, "001BANCODOBRASIL"); //77-94
            b.AppendData(DateTime.Now); //95-100
            b.AppendNumero(7, Opcoes.NumeroSequencialRemessaCnab); //101-107
            b.Append(new string(' ', 22)); //108-129
            b.AppendNumero(7, Opcoes.NumeroConvenio); //130-136
            b.Append(new string(' ', 258)); //137-394
            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine);
        }

        public void Detalhe1(StringBuilder b, TituloReceber titulo)
        {
            b.Append("7"); //1-1
            b.AppendNumero(2, "02"); //02-03
            b.AppendNumero(14, Opcoes.CnpjBeneficiario); //04-17
            b.AppendNumero(4, Opcoes.NumeroAgencia); //18-21
            b.Append(Opcoes.DigitoAgencia); //22-22
            b.AppendNumero(8, Opcoes.NumeroContaCorrente); //23-30
            b.Append(Opcoes.DigitoContaCorrente); //31-31
            b.AppendNumero(7, Opcoes.NumeroConvenio); //32-38
            b.AppendTexto(25, titulo.NumeroTitulo); //39-63
            b.AppendNumero(17, titulo.NossoNumero); //64-80
            b.Append("0000"); //81-82 & 83-84
            b.Append(new string(' ', 7)); //85-87 & 88-88 & 89-91
            b.AppendNumero(3, Opcoes.Carteira.Split('/').ElementAt(1)); //92-94
            b.Append("0000000"); //95-95 & 96-101 
            b.Append(new string(' ', 5)); //102-106
            b.AppendNumero(2, Opcoes.Carteira.Split('/').ElementAt(0)); //107-108
            b.Append("01"); //109-110
            b.AppendNumero(10, titulo.NossoNumero); //111-120
            b.AppendData(titulo.Vencimento); //121-126
            b.AppendDinheiro(13, titulo.Valor); //127-139
            b.Append("001"); //140-142
            b.Append("0000"); //143-146
            b.Append(" "); //147-147
            b.AppendNumero(2, 1); //148-149
            b.Append("N"); //150-150
            b.AppendData(titulo.Emissao); //151-156
            b.Append("0700"); //157-158 & 159-160
            b.AppendDinheiro(13, Math.Round(Opcoes.PercentualMoraDiaAtraso * titulo.Valor / 100, 2, MidpointRounding.AwayFromZero)); // 161-173
            b.Append("000000"); //174-179 Data Desconto
            b.AppendDinheiro(13, 0); //180-192 Valor Desconto
            b.AppendDinheiro(13, 0); //193-205 Valor do IOF
            b.AppendDinheiro(13, 0); //206-218 Valor Abatimento
            b.AppendNumero(2, titulo.PessoaJuridica() ? "02" : "01"); //219-220
            b.AppendNumero(14, titulo.CpfCnpj); //221-234
            b.AppendTexto(37, titulo.NomePagador); //235-264
            b.Append(new string(' ', 3)); //265-274
            b.AppendTexto(40, titulo.EnderecoCompleto); //275-314
            b.AppendTexto(12, titulo.Bairro); //315-326
            b.AppendNumero(8, titulo.Cep); //327-334 VERIFICAR TEXTO CEP
            b.AppendTexto(15, titulo.Cidade); //335-349
            b.AppendTexto(2, titulo.UF); //350-351
            b.AppendTexto(40, Opcoes.Msg2); //352-391
            b.Append(new string(' ', 2)); //392-393
            b.Append("S"); //394-394
            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine);
        }

        public void Trailer(StringBuilder b)
        {
            b.Append("9"); //1=1
            b.Append(new string(' ', 393)); //002-394
            b.AppendNumero(6, _index); //395-400
        }

        private string CalcularDvNossoNumero(string input)
        {
            var withZeroes = input.PadLeft(7, '0');

            char[] charArr = withZeroes.ToCharArray().Reverse().ToArray();

            int[] numbersArr = Array.ConvertAll<char, int>(charArr, x => (int)Char.GetNumericValue(x));

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

        public string NomearArquivo(DateTime? dt = null)
        {
            throw new NotImplementedException();
        }
    }
}