using CnabContasReceber.Interfaces;
using CnabContasReceber.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CnabContasReceber.Util;
using System.Collections;

namespace CnabContasReceber.Bancos
{
    public class BancoItau400 : IBanco
    {
        private int _index = 1;

        public BancoItau400(Opcoes opcoes)
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
                t.CalcularDescontos(Opcoes);
                Detalhe1(b, t);

                if (Opcoes.BancoEnviaBoleto)
                    throw new NotImplementedException("BancoEnviaBoleto");

                if (Opcoes.CobrancaCompartilhada)
                {
                    if (t.RateioCredito.Count() < 1)
                        throw new ArgumentOutOfRangeException("RateioCredito");

                    foreach(var lote in t.RateioCredito.Batch(3))
                    {
                        Detalhe3(b, t.NossoNumero, lote);
                    }
                }
            }

            Trailer(b);

            return b.ToString();
        }

        public void Header(StringBuilder b)
        {
            b.Append("01REMESSA01");
            b.AppendTexto(15, "COBRANCA"); //12-26
            b.AppendNumero(4, Opcoes.NumeroAgencia); //27-30
            b.AppendNumero(2, 00); //31-32
            b.AppendNumero(5, Opcoes.NumeroContaCorrente); //33-37
            b.Append(Opcoes.DigitoContaCorrente); //38-38
            b.Append(new string(' ', 8)); //39-46
            b.AppendTexto(30, Opcoes.RazaoSocial); //47-76
            b.AppendNumero(3, 341); //77-79
            b.AppendTexto(15, "BANCO ITAU SA"); //80-94
            b.AppendData(DateTime.Now); //95-100
            b.Append(new string(' ', 294)); //101-394
            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine);
        }

        public void Detalhe1(StringBuilder b, TituloReceber titulo)
        {
            TituloReceber.Desconto desconto1 = titulo.Descontos.FirstOrDefault();
            TituloReceber.Desconto desconto2 = titulo.Descontos.ElementAtOrDefault(1);
            TituloReceber.Desconto desconto3 = titulo.Descontos.ElementAtOrDefault(2);

            b.Append("1");//1-1
            b.Append("02"); //2-3
            b.AppendNumero(14, Opcoes.CnpjBeneficiario);//4-17
            b.AppendNumero(4, Opcoes.NumeroAgencia);//18-21
            b.AppendNumero(2, 00); //22-23
            b.AppendNumero(5, Opcoes.NumeroContaCorrente);//24-28
            b.Append(Opcoes.DigitoContaCorrente); //29-29
            b.Append(new string(' ', 4)); //30-33
            b.AppendNumero(4, 0); //34-37
            b.AppendTexto(25, titulo.NumeroTitulo); //38-62
            b.AppendNumero(8, titulo.NossoNumero); //65-70
            b.AppendNumero(13, 0); //71-83
            b.Append(Opcoes.Carteira.PadLeft(3, '0')); //84-86
            b.Append(new string(' ', 21)); //87-107
            b.AppendTexto(1, DigitoCarteira(Opcoes.Carteira.PadLeft(3, '0'))); //108-108 Depende do cadastro no banco
            b.Append("01"); //109-110
            b.AppendNumero(10, ++Opcoes.ContadorTitulos);  //111-120 No arquivo remessa, sugerimos o preenchimento com o nº do documento que originou a cobrança (nº duplicata, Nota fiscal, etc.)
            b.AppendData(titulo.Vencimento); //121-126
            b.AppendDinheiro(13, titulo.Valor); //127-139
            b.AppendNumero(3, 341); //140-142
            b.Append("00000"); //143-147
            b.Append("06"); //148-149 Especie de Titulo - Contrato
            b.Append("N"); //150-150 Aceite/Não Aceite
            b.AppendData(titulo.Emissao); //151-156
            b.Append("39"); //157-158 
            b.Append("00"); //159-160 
            b.AppendDinheiro(13, Math.Round(Opcoes.PercentualMoraDiaAtraso * titulo.Valor / 100, 2, MidpointRounding.AwayFromZero)); // 161-173

            if (desconto1 != null)
            {
                b.AppendData(desconto1.DataLimite); //174-179 Data 1° Desconto
                b.AppendDinheiro(13, desconto1.Valor); //180-192 Valor 1° Desconto
            }
            else
            {
                b.Append(new string('0', 19));
            }

            b.AppendDinheiro(13, 0); //193-205 Valor do IOF
            b.AppendDinheiro(13, 0); //206-218 Valor Abatimento
            b.AppendNumero(2, titulo.PessoaJuridica() ? "02" : "01"); //219-220
            b.AppendNumero(14, titulo.CpfCnpj); //221-234
            b.AppendTexto(40, titulo.NomePagador); //235-264 & 265 274
            b.AppendTexto(40, titulo.EnderecoCompleto); //275-314
            b.AppendTexto(12, titulo.Bairro); //315-326
            b.AppendNumero(8, titulo.Cep); //327-334 VERIFICAR TEXTO CEP
            b.AppendTexto(15, titulo.Cidade); //335-349
            b.AppendTexto(2, titulo.UF); //350-351

            if (desconto2 != null)
            {
                b.Append(new string(' ', 2));//352-353
                b.AppendData(desconto2.DataLimite); //354-359 Data 2° Desconto
                b.AppendDinheiro(13, desconto2.Valor); //360-372 Valor 2° Desconto

                if (desconto3 != null)
                {
                    b.AppendData(desconto3.DataLimite); //373-378 Data 3° Desconto
                    b.AppendDinheiro(13, desconto3.Valor); //379-391 Valor 3° Desconto
                }
                else
                {
                    b.Append(new string('0', 19));
                }

                b.Append(new string(' ', 3)); //392-394 

            }
            else
            {
                b.Append(new string(' ', 34)); //352-381 & 382-385
                b.Append("00000000 "); //386-391 & 392-393 & 394-394
            }

            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine);
        }


        public void Detalhe3(StringBuilder b, string nossoNumero, IEnumerable<RateioCredito> rateios)
        {
            b.Append("402"); //1-1 & 2-3
            b.AppendNumero(14, Opcoes.CnpjBeneficiario);//4-17
            b.AppendNumero(4, Opcoes.NumeroAgencia);//18-21
            b.AppendNumero(2, 00); //22-23
            b.AppendNumero(5, Opcoes.NumeroContaCorrente);//24-28
            b.Append(Opcoes.DigitoContaCorrente); //29-29
            b.AppendNumero(3, Opcoes.Carteira); //30-32
            b.AppendNumero(8, nossoNumero); //33-40
            b.AppendNumero(1, 1); //41-41
            b.AppendNumero(2, Opcoes.NumeroSequencialRemessaCnab); //42-43

            for(int i = 0; i < 14; i++)
            {
                EscreverRateioCredito(b, rateios.ElementAtOrDefault(i));
            }
            b.Append("4");// 394-394
            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine); 
        }


        public void Trailer(StringBuilder b)
        {
            b.Append("9");
            b.Append(new string(' ', 393));
            b.AppendNumero(6, _index++);
            b.Append(Environment.NewLine);
        }



        private void EscreverRateioCredito(StringBuilder b, RateioCredito rat)
        {
            if (rat != null)
            {
                b.AppendNumero(4, rat.AgenciaSemDigito);
                b.AppendNumero(7, rat.ContaCorrente);
                b.Append(rat.DigitoAgencia);
                b.AppendDinheiro(13, rat.ValorRateio);
            }
            else
            {
                b.Append(new string('0', 12));
                b.AppendDinheiro(13, 0);
            }
        }

        private string DigitoCarteira(string carteira)
        {
            switch (carteira)
            {
                case "150":
                    return "U";
                case "191":
                    return "1";
                case "147":
                    return "E";
                default:
                    return "I";
            } 
        }

        public string NomearArquivo(DateTime? dt = null)
        {
            throw new NotImplementedException();
        }
    }

}