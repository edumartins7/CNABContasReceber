using CnabContasReceber.Models;
using System.Collections.Generic;
using System.Text;

namespace CnabContasReceber.Interfaces
{
    public interface IBanco
    {
        string MontarArquivo(IEnumerable<TituloReceber> titulos);
        void Header(StringBuilder b);
        void Detalhe1(StringBuilder b, TituloReceber titulo);
        void Trailer(StringBuilder b);

        Opcoes Opcoes { get; set; }
    }
}
