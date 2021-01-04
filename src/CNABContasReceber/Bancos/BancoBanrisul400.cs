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
    public class BancoBanrisul400 : IBanco
    {
        private int _index = 1;

        private decimal _valorTotal = 0;

        public BancoBanrisul400(Opcoes opcoes)
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
                t.CalcularDescontos(t);

                Detalhe1(b, t);

                if (Opcoes.BancoEnviaBoleto)
                    throw new NotImplementedException("BancoEnviaBoleto");
                _valorTotal += t.Valor;
            }

            Trailer(b);

            return b.ToString();
        }

        public void Header(StringBuilder b)
        {
            b.Append("01REMESSA");//01-09
            b.Append(new string(' ', 17)); //10-26
            b.AppendNumero(13, FazCodigoBeneficiario()); //27-39
            b.Append(new string(' ', 7)); //40-46
            b.AppendTexto(30, Opcoes.RazaoSocial); //47-76
            b.Append("041BANRISUL"); //77-87
            b.Append(new string(' ', 7)); //88-94
            b.AppendData(DateTime.Now); //95-100
            b.Append(new string(' ', 9)); //101-109
            b.AppendNumero(4, FazCodigoServico(Opcoes.Carteira)); //110-113            
            b.Append(' '); //114-114
            b.Append(FazTipoProcessamento(Opcoes.Carteira)); //115-115
            b.Append(' '); //116-116
            b.Append(new string(' ', 10)); //117-126
            b.Append(new string(' ', 268)); //127-394
            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine);
        }

        public void Detalhe1(StringBuilder b, TituloReceber titulo)
        {
            TituloReceber.Desconto desconto1 = titulo.Descontos.FirstOrDefault();

            b.Append('1');//1-1
            b.Append(new string(' ', 16)); //2-17
            b.AppendNumero(13, FazCodigoBeneficiario()); //18-30
            b.Append(new string(' ', 7)); //31-37
            b.AppendTexto(25, titulo.NossoNumero); //38-62
            b.AppendNumero(10, 0); //63-72 NOSSO NUMERO
            b.AppendTexto(32, Opcoes.Msg1); //73-104
            b.Append(new string(' ', 3)); //105-107
            b.Append('1'); //108-108 COBRAÇA SIMPLES
            b.Append("01"); //109-110 REMESSA
            b.AppendNumero(10, titulo.NossoNumero); //111-120 SEU NUMERO
            b.AppendData(titulo.Vencimento); //121-126
            b.AppendDinheiro(13, titulo.Valor); //127-139
            b.Append("041"); //140-142
            b.Append(new string(' ', 5)); //143-147
            if (Opcoes.BancoEnviaBoleto)
                b.Append("06"); //148-149 TIPO DE DOCUMENTO
            else
                b.Append("08"); //148-149 TIPO DE DOCUMENTO
            b.Append('N'); //150-150
            b.AppendData(titulo.Emissao); //151-156
            if (titulo.CobraMulta) //157-158
                b.Append("18"); //Multa após vencimento
            else if(titulo.ProtestavelAposVencimento)
                b.Append("09"); //Protestavel após N dias
            else
                b.Append("23"); //Não Protestar
            b.Append("00"); //159-160
            b.Append('0'); //161-161
            b.AppendDinheiro(12, Math.Round(titulo.PercentualMoraDiaAtraso * titulo.Valor / 100, 2, MidpointRounding.AwayFromZero)); // 162-173

            if (desconto1 != null)
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
            b.AppendTexto(35, titulo.NomePagador); //235-269
            b.Append(new string(' ', 5)); //270-274
            b.AppendTexto(40, titulo.EnderecoCompleto); //275-314
            b.Append(new string(' ', 7)); //315-321
            b.AppendDinheiroUmaCasa(3, titulo.PercentualMulta); //322-324
            b.Append("30"); //325-326
            b.AppendNumero(8, titulo.Cep); //327-334
            b.AppendTexto(15, titulo.Cidade); //335-349
            b.AppendTexto(2, titulo.UF); //350-351
            b.Append(new string(' ', 18)); //352-369
            if(titulo.CobraMulta || titulo.ProtestavelAposVencimento)
                b.AppendNumero(2, titulo.DiasParaProtestar); //370-371
            else
                b.Append("00"); //370-371
            b.Append(new string(' ', 23)); //372-394
            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine);
        }

        public void Detalhe2(StringBuilder b, TituloReceber titulo)
        {
            b.Append('1');//1-1
            b.AppendNumero(2, "02"); //2-3
            b.AppendNumero(14, Opcoes.CnpjBeneficiario); //4-17
            b.AppendNumero(13, FazCodigoBeneficiario()); //18-30
            b.Append(new string(' ', 7)); //31-37
            b.AppendTexto(25, titulo.NossoNumero); //38-62
            b.AppendNumero(10, titulo.NossoNumero); //63-72 NOSSO NUMERO
            b.Append(new string(' ', 25)); //73-107
            b.Append('1'); //108-108 COBRAÇA SIMPLES
            b.Append("98"); //109-110
            b.Append('1');
            b.AppendTexto(90, Opcoes.Msg1);
            b.Append(' ');
            b.Append(new string(' ', 90));
            b.Append(' ');
            b.Append(new string(' ', 90));
            b.Append(new string(' ', 11));
            b.AppendNumero(6, _index++); //395-400
            b.Append(Environment.NewLine);
        }

        public void Trailer(StringBuilder b)
        {
            b.Append("9"); //1-1
            b.Append(new string(' ', 26)); //2-27
            b.AppendDinheiro(13, _valorTotal); //28-40
            b.Append(new string(' ', 354)); //41-394
            b.AppendNumero(6, _index++);
            b.Append(Environment.NewLine);
        }

        private string FazCodigoBeneficiario()
        {
            var codigoBeneficiario = new StringBuilder();
            codigoBeneficiario.AppendNumero(4, Opcoes.NumeroAgencia);
            codigoBeneficiario.AppendTexto(9, Opcoes.NumeroConvenio);
            return codigoBeneficiario.ToString();
        }
        
        private string FazCodigoServico(string carteira)
        {
            if (CarteiraTipoRSX(carteira))
                return "0808";
            return new string(' ', 4);
        }

        private char FazTipoProcessamento(string carteira)
        {
            if (CarteiraTipoRSX(carteira))
                return 'P';
            return ' ';
        }

        private bool CarteiraTipoRSX(string carteira)
        {
            if (carteira == "R" || carteira == "S" || carteira == "X" || carteira == "r" || carteira == "s" || carteira == "x")
                return true;
            return false;
        }

        public string NomearArquivo(DateTime? dt = null, int arquivosHoje = 0)
        {
            throw new NotImplementedException();
        }
    }

}