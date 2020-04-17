using System;
using System.Globalization;

namespace CNABContasReceber.Retorno
{
    public static class StringExtensions
    {
        private static CultureInfo _culture = new CultureInfo("pt-BR");
            
        /// <summary>
        /// O start começa em 1, ao invés de zero, pra ficar mais fácil de bater com a documentacao. Melhor que subtrair 1 mentalmente toda vez enquanto lê...
        /// </summary>
        public static string Slice(this string txt, int start, int end)
        {
            start--;
            int len = end - start;

            return txt.Substring(start, len);
        }


        public static object Slice<T>(this string txt, int start, int end)
        {
            var part = Slice(txt, start, end);

            if (typeof(T) == typeof(decimal))
                part = part.Insert(part.Length - 2, ",");

            else if (typeof(T) == typeof(DateTime))
            {
                DateTime.TryParseExact(part, "ddMMyy", _culture, DateTimeStyles.None, out var date);
                return date;
            }

            return (T) Convert.ChangeType(part, typeof(T), _culture); 
        }
    }
}
