using System.Text;


namespace Assets.Scripts.Core
{
    public static class StringExtensions
    {
        public static string ToASCII(this string text)
        {
            
            return text.Replace("\\u0027", "\'");
        }
    }
}
