using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Waddle.Core.Symbols;

namespace Waddle.Test
{
    internal static class StringExtensions
    {
        public static CharReader CharwiseWithTrimmedLines(this string str)
        {
            return new CharReader(new StringReader(str.AutoTrim()));
        }

        public static CharReader Charwise(this string str)
        {
            return new CharReader(new StringReader(str));
        }

        public static string AutoTrim(this string str)
        {
            var lines = str.Lines().SkipWhile(string.IsNullOrEmpty).ToArray();
            var indent = lines.Min(CountLeadingWhitespace);
            return string.Join(Environment.NewLine, lines.Select(line => line[indent..]));
        }

        private static IEnumerable<string> Lines(this string str)
        {
            using var reader = new StringReader(str);
            for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                yield return line;
            }
        }

        private static int CountLeadingWhitespace(this string str)
        {
            var count = 0;
            foreach (var ch in str)
            {
                if (char.IsWhiteSpace(ch))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }
    }
}