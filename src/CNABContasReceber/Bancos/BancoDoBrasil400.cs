using CnabContasReceber.Interfaces;
using CnabContasReceber.Models;
using CnabContasReceber.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CnabContasReceber.Bancos
{
    public class BancoDoBrasil400 : IBanco
    {
        private int _index = 1;

        public BancoDoBrasil400(Opcoes opcoes)
        {
            Opcoes = opcoes;
        }

        public Opcoes Opcoes { get; set; }

        public string MontarArquivo(IEnumerable<TituloReceber> titulos)
        {
            _index = 1;

            var b = new StringBuilder();

            Header(b);

            foreach (TituloReceber t in titulos)
            {
                t.CalcularDescontos(t);

                Detalhe1(b, t);

                if (Opcoes.BancoEnviaBoleto)
                    throw new NotImplementedException("BancoEnviaBoleto");

                if (Opcoes.CobrancaCompartilhada)
                {
                    if (t.RateioCredito.Count() < 1 || t.RateioCredito.Count() > 4)
                        throw new ArgumentOutOfRangeException("RateioCredito");

                    DetalheRateios(b, t.NossoNumero, t.RateioCredito);
                }

                if (t.Descontos.Count() >= 2)
                    DescontosAdicionais(b, t);
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
            TituloReceber.Desconto desconto1 = titulo.Descontos.FirstOrDefault();

            b.Append("7"); //1-1
            b.AppendNumero(2, "02"); //02-03
            b.AppendNumero(14, Opcoes.CnpjBeneficiario); //04-17
            b.AppendNumero(4, Opcoes.NumeroAgencia); //18-21
            b.Append(Opcoes.DigitoAgencia); //22-22
            b.AppendNumero(8, Opcoes.NumeroContaCorrente); //23-30
            b.Append(Opcoes.DigitoContaCorrente); //31-31
            b.AppendNumero(7, Opcoes.NumeroConvenio); //32-38
            b.AppendTexto(25, titulo.NumeroTitulo); //39-63
            b.AppendTexto(17, FazerNossoNumero(Opcoes.NumeroConvenio, titulo.NossoNumero)); //64-80 
            b.Append("0000"); //81-82 & 83-84
            b.Append(new string(' ', 7)); //85-87 & 88-88 & 89-91
            b.AppendNumero(3, Opcoes.VariacaoCarteira); //92-94
            b.Append("0000000"); //95-95 & 96-101 
            b.Append(new string(' ', 5)); //102-106
            b.AppendNumero(2, Opcoes.Carteira); //107-108
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

            if(desconto1 != null)
            {
                b.AppendData(desconto1.DataLimite); //174-179 Data Desconto
                b.AppendDinheiro(13, desconto1.Valor); //180-192 Valor Desconto
            }
            else
            {
                b.Append(new string('0', 19));
            }

            b.AppendDinheiro(13, 0); //193-205 Valor do IOF
            b.AppendDinheiro(13, 0); //206-218 Valor Abatimento
            b.AppendNumero(2, titulo.PessoaJuridica() ? "02" : "01"); //219-220
            b.AppendNumero(14, titulo.CpfCnpj); //221-234
            b.AppendTexto(37, titulo.NomePagador); //235-271
            b.Append(new string(' ', 3)); //272-274
            b.AppendTexto(40, titulo.EnderecoCompleto); //275-314
            b.AppendTexto(12, titulo.Bairro); //315-326
            b.AppendNumero(8, titulo.Cep); //327-334 VERIFICAR TEXTO CEP
            b.AppendTexto(15, titulo.Cidade); //335-349
            b.AppendTexto(2, titulo.UF); //350-351
            b.AppendTexto(40, Opcoes.Msg2); //352-391
            b.Append(new string(' ', 2)); //392-393
            b.Append(" "); //394-394
            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine);
        }

        public void DetalheRateios(StringBuilder b, string nossoNumero, IEnumerable<RateioCredito> rateios)
        {
            b.Append('2'); //1-1
            b.AppendNumero(17, nossoNumero);//2-18

            for (int i = 0; i < 4; i++)
            {
                EscreverRateioCredito(b, rateios.ElementAtOrDefault(i)); //19-334
            }

            for (int i = 0; i < 4; i++)
            {
                EscreverDocumentoFavorecido(b, rateios.ElementAtOrDefault(i)); //19-334
            }

            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine);
        }

        public void DescontosAdicionais(StringBuilder b, TituloReceber titulo)
        {
            TituloReceber.Desconto desconto2 = titulo.Descontos.ElementAt(1);
            TituloReceber.Desconto desconto3 = titulo.Descontos.ElementAtOrDefault(2);


            b.Append("5"); //1-1
            b.AppendNumero(2, "07"); //2-3
            b.AppendData(desconto2.DataLimite); //4-9 Data Desconto 2
            b.AppendDinheiro(17, desconto2.Valor); //10-26 Valor Desconto 2

            if(desconto3 != null)
            {
                b.AppendData(desconto3.DataLimite); //27-32 Data Desconto 3
                b.AppendDinheiro(17, desconto3.Valor); //33-49 Valor Desconto 3
            }
            else
            {
                b.Append(new string('0', 23));
            }
            
            b.Append(new string(' ', 345)); //50-394
            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine);
        }

        public void Trailer(StringBuilder b)
        {
            b.Append("9"); //1=1
            b.Append(new string(' ', 393)); //002-394
            b.AppendNumero(6, _index); //395-400
        }

        public string NomearArquivo(DateTime? dt = null)
        {
            throw new NotImplementedException();
        }

        

        private void EscreverRateioCredito(StringBuilder b, RateioCredito rat)
        {
            if (rat != null)
            {
                b.Append("001"); //Banco para Credito
                b.Append(new string('0', 3)); //Câmara de Compensação
                b.AppendNumero(4, rat.AgenciaSemDigito); //Pref. Agencia para Credito
                b.Append(rat.DigitoAgencia); //DV-Prefixo Ag. Credito
                b.AppendNumero(11, rat.ContaCorrente); //Conta para Credito
                b.Append(rat.DigitoContaCorrente); //DV-Conta para Credito
                b.AppendTexto(30, Opcoes.RazaoSocial); //Nome do Favorecido
                b.AppendDinheiro(13, rat.ValorRateio); //Valor para Partilha
                b.Append(new string(' ', 13)); //Brancos
            }
            else
            {
                b.Append(new string('0', 10));
                b.Append(' ');
                b.Append(new string('0', 11));
                b.Append(new string(' ', 31));
                b.Append(new string('0', 13));
                b.Append(new string(' ', 13));
            }
        }

        private void EscreverDocumentoFavorecido(StringBuilder b, RateioCredito rat)
        {
            if (rat != null)
            {
                b.Append('4'); //Tipo de documento do favorecido
                b.AppendTexto(14, rat.CnpjRecebedor); //Numero do documento do favorecido
            }
            else
            {
                b.Append(new string('0', 15));
            }
        }
        private string FazerNossoNumero(string convenio, string nossoNumero)
        {
            var texto = new StringBuilder();

            if (Opcoes.Carteira == "11" || Opcoes.Carteira == "31" || Opcoes.Carteira == "51")
            {
                texto.Append(new string('0', 17));
            }
            else if (Opcoes.Carteira == "12" || Opcoes.Carteira == "15" || Opcoes.Carteira == "17")
            {
                texto.AppendNumero(7, convenio);
                texto.AppendNumero(10, nossoNumero);
            }
            return (texto.ToString());
        }
    }
}