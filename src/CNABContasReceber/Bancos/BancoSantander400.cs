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
    public class BancoSantander400 : IBanco
    {
        private int _index = 1;
        private int _qtdeTitulos = 0;
        private decimal _valorTotalTitulos = 0;
        public BancoSantander400(Opcoes opcoes)
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
            b.AppendTexto(15, "COBRANCA"); //12-26
            b.AppendNumero(20, Opcoes.CodigoEmpresa); //27-46 
            b.AppendTexto(30, Opcoes.RazaoSocial); //47-76
            b.Append("033"); //77-79
            b.AppendTexto(15, "SANTANDER"); //80-94
            b.AppendData(DateTime.Now); //95-100
            b.Append(new string('0', 16)); //101-116
            b.AppendTexto(47, Opcoes.Msg1); //117-163
            b.AppendTexto(47, Opcoes.Msg2); //164-210
            b.Append(new string(' ', 47)); //211-257
            b.Append(new string(' ', 47)); //258-304
            b.Append(new string(' ', 47)); //305-351
            b.Append(new string(' ', 34)); //352-385
            b.Append(new string(' ', 6)); //386-391
            b.Append(new string('0', 3)); //392-394
            b.AppendNumero(6, _index++);
            b.Append(Environment.NewLine);
        }

        public void Detalhe1(StringBuilder b, TituloReceber titulo)
        {
            b.Append("1"); //1-1
            b.AppendNumero(2, "02"); //02-03
            b.AppendNumero(14, Opcoes.CnpjBeneficiario); //04-17
            b.AppendNumero(4, Opcoes.NumeroAgencia); //18-21
            b.AppendNumero(8, Opcoes.NumeroContaCorrente); //22-29
            b.AppendNumero(8, Opcoes.NumeroContaCorrente); //30-37
            b.AppendNumero(25, titulo.NumeroTitulo); //38-62
            b.AppendNumero(8, titulo.NossoNumero); //63-70
            b.AppendData(titulo.Vencimento); //71-76
            b.Append(' '); // 77-77
            b.Append(Opcoes.CobraMulta ? "4" : "0"); //78-78
            b.AppendNumero(4, Math.Round(Opcoes.PercentualMulta, 2).ToString("#.00", CultureInfo.InvariantCulture)); //79-82
            b.Append("00"); //83-84
            b.Append(new string('0', 13));   //85-97 FALTAAAA
            b.Append(new string(' ', 4)); //98-101
            b.Append("000000"); //102-107
            b.AppendTexto(1, Opcoes.Carteira); //108-108
            b.Append("01"); //109-110
            b.AppendNumero(10, ++Opcoes.ContadorTitulos); //111-120
            b.AppendData(titulo.Vencimento); //121-126
            b.AppendDinheiro(13, titulo.Valor); //127-139
            b.Append("033");
            b.Append("00000"); //143-147
            b.Append("01"); //148-149
            b.Append("N"); //150-150
            b.AppendData(titulo.Emissao); //151-156
            b.Append("00");//157-158
            b.Append("00"); //159-160
            b.AppendDinheiro(13, Math.Round(Opcoes.PercentualMoraDiaAtraso * titulo.Valor / 100, 2, MidpointRounding.AwayFromZero)); //161-173
            b.Append(new string('0', 6)); //174-179
            b.Append(new string('0', 13)); //180-192
            b.Append(new string('0', 13)); //193-205
            b.Append(new string('0', 13)); //206-218
            b.AppendNumero(2, titulo.PessoaJuridica() ? "02" : "01"); //219-220
            b.AppendNumero(14, titulo.CpfCnpj); //221-234
            b.AppendTexto(40, titulo.NomePagador); //235-274
            b.AppendTexto(40, titulo.EnderecoCompleto); //275-314
            b.Append(new string(' ', 12)); //315-326
            b.AppendNumero(8, titulo.Cep); //327-331
            b.Append(new string(' ', 15)); //335-349
            b.Append(new string(' ', 2));  //350-351
            b.Append(new string(' ', 30)); //352-381
            b.Append(" "); //382-382
            b.Append("0"); //383-383 ??????????????????
            b.Append("00"); //384-385
            b.Append(new string(' ', 6));  //386-391
            b.Append("00"); //392-393
            b.Append(" "); //394-394
            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine);
        }


        public void Trailer(StringBuilder b)
        {
            b.Append("9"); //1=1
            b.AppendNumero(6, _qtdeTitulos); //02-07
            b.AppendDinheiro(13, _valorTotalTitulos); //08-20
            b.Append(new string(' ', 374));
            b.AppendNumero(6, _index++);
        }
        
    }
}