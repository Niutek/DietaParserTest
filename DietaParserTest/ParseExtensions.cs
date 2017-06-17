using System.Text.RegularExpressions;

namespace DietaParserTest
{
    static class ParseExtensions
    {
        public static int ToInt(this Match val, int index)
        {
            return int.Parse(val.Groups[index].Value);
        }
    }
}
