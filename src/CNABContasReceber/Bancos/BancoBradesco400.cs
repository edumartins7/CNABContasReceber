﻿using CnabContasReceber.Interfaces;
using CnabContasReceber.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CnabContasReceber.Util;

namespace CnabContasReceber.Bancos
{
    public class BancoBradesco400 : IBanco
    {
        private int _index = 1;

        public BancoBradesco400(Opcoes opcoes)
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
            b.AppendNumero(20, Opcoes.CodigoEmpresa); //27-46
            b.AppendTexto(30, Opcoes.RazaoSocial); //47-76
            b.Append("237"); //77-79
            b.AppendTexto(15, "BRADESCO"); //80-94
            b.AppendData(DateTime.Now); //95-100
            b.Append(new string(' ', 8)); 
            b.Append("MX"); //109-110
            b.AppendNumero(7, Opcoes.NumeroSequencialRemessaCnab); //111-117
            b.Append(new string(' ', 277));
            b.AppendNumero(6, _index++);
            b.Append(Environment.NewLine);
        }

        public void Detalhe1(StringBuilder b, TituloReceber titulo)
        {
            b.Append("100000 000000000000 0");//1-20
            b.Append(Opcoes.Carteira.PadLeft(3, '0'));
            b.AppendNumero(5, Opcoes.NumeroAgencia);
            b.AppendNumero(7, Opcoes.NumeroContaCorrente);//30-36
            b.Append(Opcoes.DigitoContaCorrente);//37-37
            b.AppendNumero(25, titulo.NumeroTitulo); //38-62
            b.Append("000"); //63-65
            b.Append(titulo.CobraMulta ? "2" : "0"); //66-66
            b.AppendNumero(4, Math.Round(titulo.PercentualMulta, 2).ToString("#.00", CultureInfo.InvariantCulture)); //67-70
            b.AppendNumero(11, titulo.NossoNumero); //71-82
            b.Append(CalcularDVNossoNumero(Opcoes.Carteira, titulo.NossoNumero.PadLeft(11, '0')));
            b.Append("0000000000"); //82-92
            b.Append(Opcoes.BancoEnviaBoleto ? "1" : "2"); //93-93 1=banco emite boleto e processa. 2=empresa emite boleto e banco processa
            b.Append("N"); //94-94
            b.Append(new string(' ', 10)); //95-104
            b.Append(Opcoes.CobrancaCompartilhada ? "R" : " "); //105 indicador rateio crédito
            b.Append("0"); //106
            b.Append(new string(' ', 2));//108
            b.Append("01");//110
            b.AppendNumero(10, ++Opcoes.ContadorTitulos);
            b.AppendData(titulo.Vencimento); //121-126
            b.AppendDinheiro(13, titulo.Valor); //127-139
            b.Append("00000000");
            b.Append("04N");
            b.AppendData(titulo.Emissao);
            b.Append("0000");
            b.AppendDinheiro(13, Math.Round(titulo.PercentualMoraDiaAtraso * titulo.Valor / 100, 2, MidpointRounding.AwayFromZero));
            b.Append("000000000000000000000000000000000000000000000");
            b.AppendNumero(2, titulo.PessoaJuridica() ? "02" : "01");
            b.AppendNumero(14, titulo.CpfCnpj);
            b.AppendTexto(40, titulo.NomePagador);
            b.AppendTexto(40, titulo.EnderecoCompleto);
            b.AppendTexto(12, Opcoes.Msg1);
            b.AppendNumero(8, titulo.Cep);
            b.AppendTexto(60, Opcoes.Msg2);
            b.AppendNumero(6, _index++);
            b.Append(Environment.NewLine);
        }


        public void Detalhe3(StringBuilder b, string nossoNumero, IEnumerable<RateioCredito> rateios)
        {
            var primeiroRateio = rateios.ElementAtOrDefault(0);
            var segundoRateio = rateios.ElementAtOrDefault(1);
            var terceiroRateio = rateios.ElementAtOrDefault(2);

            b.Append("3"); //1-1
            b.Append(Opcoes.Carteira.PadLeft(3, '0')); //02-04
            b.AppendNumero(5, Opcoes.NumeroAgencia); //05-09
            b.AppendNumero(7, Opcoes.NumeroContaCorrente); //16-16
            b.Append(Opcoes.DigitoContaCorrente); //17-17
            b.AppendNumero(11, nossoNumero); //18-28
            b.Append(CalcularDVNossoNumero(Opcoes.Carteira, nossoNumero.PadLeft(11, '0'))); //29-29
            b.Append("2"); //30-30            1- valor cobrado 2-Valor do registro 3-Rateio pelo menor valor (registrado ou pago)
            b.Append("2"); //31-31            1- percentual 2-valor
            b.Append(new string(' ', 12)); //32-43

            EscreverRateioCredito(b, primeiroRateio);
            EscreverRateioCredito(b, segundoRateio);
            EscreverRateioCredito(b, terceiroRateio);

            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine); 
        }


        public void Trailer(StringBuilder b)
        {
            b.Append("9");
            b.Append(new string(' ', 393));
            b.AppendNumero(6, _index++);
        }
        
        public string CalcularDVNossoNumero(string carteira, string nossoNumero)
        {
            carteira = carteira.PadLeft(2, '0');

            if (nossoNumero.Length != 11 || carteira.Length > 2)
                throw new IndexOutOfRangeException();

            string texto = carteira + nossoNumero;

            string digito;
            int pesoMaximo = 7, soma = 0, peso = 2;
            for (var i = texto.Length - 1; i >= 0; i--)
            {
                soma = soma + (int)char.GetNumericValue(texto[i]) * peso;
                if (peso == pesoMaximo)
                    peso = 2;
                else
                    peso = peso + 1;
            }
            var resto = soma % 11;
            switch (resto)
            {
                case 0:
                    digito = "0";
                    break;
                case 1:
                    digito = "P";
                    break;
                default:
                    digito = (11 - resto).ToString();
                    break;
            }
            return digito;
        }

        private void EscreverRateioCredito(StringBuilder b, RateioCredito rat)
        {
            if (rat != null)
            {
                b.Append("237");
                b.AppendNumero(5, rat.AgenciaSemDigito);
                b.Append(rat.DigitoAgencia);
                b.AppendNumero(12, rat.ContaCorrente);
                b.Append(rat.DigitoContaCorrente);
                b.AppendDinheiro(15, rat.ValorRateio);
                b.AppendTexto(40, Opcoes.RazaoSocial);
                b.Append(new string(' ', 31));
                b.Append(new string(' ', 6));
                b.Append("001");
            }
            else
            {
                b.Append("237");
                b.Append(new string('0', 34));
                b.Append(new string(' ', 40));
                b.Append(new string(' ', 31));
                b.Append(new string(' ', 6));
                b.Append("000");
            }
        }

        public string NomearArquivo(DateTime? dt = null, int arquivosHoje = 0)
        {
            throw new NotImplementedException();
        }
    }

    public static class MyExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items,
                                                           int maxItems)
        {
            return items.Select((item, inx) => new { item, inx })
                        .GroupBy(x => x.inx / maxItems)
                        .Select(g => g.Select(x => x.item));
        }
    }
}