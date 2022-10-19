namespace NRFramework
{
    static public partial class StringExtension
    {
        public static string SurroundBy(this string str, string start, string end)
        {
            return str.Insert(str.Length, end).Insert(0, start);
        }
        
        public static string SurroundByQuotes(this string str)
        {
            return str.SurroundBy("\"", "\"");  //双引号 : Quotes
        }
    }
}

