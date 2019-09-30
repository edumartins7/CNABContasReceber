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
            b.Append("100000 000000000000 0");//1-20
            b.Append(Opcoes.Carteira.PadLeft(3, '0'));
            b.AppendNumero(5, Opcoes.NumeroAgencia);
            b.AppendNumero(7, Opcoes.NumeroContaCorrente);//30-36
            b.AppendNumero(1, Opcoes.DigitoContaCorrente);//37-37
            b.AppendNumero(25, titulo.NumeroTitulo); //38-62
            b.Append("000"); //63-65
            b.Append(Opcoes.CobraMulta ? "2" : "0"); //66-66
            b.AppendNumero(4, Math.Round(Opcoes.PercentualMulta, 2).ToString()); //67-70
            b.AppendNumero(12, CalculaNossoNumero(Opcoes.NumeroAgencia, Opcoes.NumeroContaCorrente, Opcoes.Carteira, titulo.NumeroTitulo));
            b.Append("0000000000"); //82-92
            b.Append(Opcoes.BancoEnviaBoleto ? "1" : "2"); //93-93 1=banco emite boleto e processa. 2=empresa emite boleto e banco processa
            b.Append("N"); //94-94
            b.Append(new string(' ', 11)); //95-105
            b.Append("0");
            b.Append(new string(' ', 2));
            b.Append("01");
            b.AppendNumero(10, ++Opcoes.ContadorTitulos);
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
            b.AppendTexto(40, titulo.NomePagador);
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


        public string CalculaNossoNumero(string agencia, string conta, string carteira, string idTitulo)
        {
            // função para retonar o nosso numero com o digito verificador
            string b = "";
            long resto;
            long total = 0;
            long parcial = 0;


            var nossoNumero = string.Format("00000000", idTitulo);
            b = agencia + conta + carteira + string.Format("00000000", idTitulo);

            for (var i = 1; i <= b.Length; i++)
            {
                var c = int.Parse(b[i].ToString());

                if (i / (double)2 == i / 2)
                    parcial = c * 2;
                else
                    parcial = c;
                while (parcial >= 10)
                    parcial = int.Parse(parcial.ToString().Substring(0, 1)) + int.Parse(parcial.ToString().Substring(parcial.ToString().Length - 1));
                total = total + parcial;
            }
            resto = 10 - total % 10;

            var teste = nossoNumero + resto.ToString();

            return teste;
        }

    }
}