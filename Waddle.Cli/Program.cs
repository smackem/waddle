using System;
using System.Collections.Generic;

namespace Waddle.Cli
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            int? i = args.Length > 0 ? int.Parse(args[0]) : null;
            int? j = args.Length > 1 ? int.Parse(args[1]) : null;

            var x = (i, j) switch
            {
                (10, 10) => "1010",
                (< 100, null) or (null, < 100) => "belowAndNull",
                _ => "",
            };

            Console.WriteLine($"{x}");

            if (i is null or > 1)
            {
                Console.WriteLine();
            }

            if (i == null || i > 1)
            {
                Console.WriteLine();
            }

            return 123;
        }
    }
}