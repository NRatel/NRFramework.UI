using System.Text;

namespace NRFramework
{
    static public partial class StringBuilderExtension
    {
        public static StringBuilder SurroundBy(this StringBuilder builder, string start, string end)
        {
            return builder.Insert(builder.Length, end).Insert(0, start);
        }
        
        public static StringBuilder SurroundByBraces(this StringBuilder builder)
        {
            return builder.SurroundBy("{", "}");    //大括号 : Braces,
        }
        
        public static StringBuilder SurroundByBrackets(this StringBuilder builder)
        {
            return builder.SurroundBy("[", "]");    //中括号 : Brackets, 
        }
    }
}
