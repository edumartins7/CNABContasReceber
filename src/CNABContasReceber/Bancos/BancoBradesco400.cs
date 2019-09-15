using CnabContasReceber.Interfaces;
using CnabContasReceber.Models;
using System;
using System.Collections.Generic;
using System.Text;

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

            foreach(var t in titulos)
            {
                Detalhe1(b, t);

                if (Opcoes.BancoEnviaBoleto)
                    Detalhe2(b, t);
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
            b.Append("100000 000000000000 ");//1-20
            b.AppendNumero(17, "0" + Opcoes.Carteira.PadLeft(3, '0') + Opcoes.NumeroAgencia + Opcoes.NumeroContaCorrente);//21-37
            b.AppendNumero(25, titulo.NumeroTitulo); //38-62
            b.Append("000"); //63-65
            b.Append(Opcoes.CobraMulta ? "2" : "0"); //66-66
            b.AppendNumero(4, Math.Round(Opcoes.PercentualMulta, 2).ToString()); //67-70
            b.Append("0000000000000000000000"); //71-92
            b.Append(Opcoes.BancoEnviaBoleto ? "1" : "2"); //93-93 1=banco emite boleto e processa. 2=empresa emite boleto e banco processa
            b.Append("N"); //94-94
            b.Append(new string(' ', 11)); //95-105
            b.Append("0");
            b.Append(new string(' ', 2));
            b.Append("01");
            b.AppendNumero(10, Opcoes.ContadorTitulos++);
            b.AppendData(titulo.Vencimento);
            b.AppendDinheiro(13, titulo.Valor);
            b.Append("00000000");
            b.Append("04N");
            b.AppendData(titulo.Emissao);
            b.Append("0000");
            b.AppendDinheiro(13, Math.Round(Opcoes.PercentualMoraDiaAtraso * titulo.Valor / 100, 2, MidpointRounding.AwayFromZero));
            b.Append("000000000000000000000000000000000000000000000");
            b.AppendNumero(2, titulo.PessoaJuridica() ? "02" : "01");
            b.AppendNumero(14, titulo.CpfCnpj);
            b.AppendNumero(40, titulo.NomePagador);
            b.AppendTexto(40, titulo.EnderecoCompleto);
            b.AppendTexto(12, Opcoes.Msg1);
            b.AppendNumero(8, titulo.Cep);
            b.AppendTexto(60, Opcoes.Msg2);
            b.AppendNumero(6, _index++);
            b.Append(Environment.NewLine);
        }

        public void Detalhe2(StringBuilder b, TituloReceber titulo)
        {
            b.Append(Environment.NewLine);
        }

        public void Trailer(StringBuilder b)
        {
            b.Append("9");
            b.Append(new string(' ', 393));
            b.AppendNumero(6, _index++);
        }
    }
}