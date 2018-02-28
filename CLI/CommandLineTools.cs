using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator.CLI
{
    public static class CommandLineTools
    {
        /// <summary>
        /// Prints a list of doubles as a table to the command line
        /// </summary>
        /// <param name="items"></param>
        internal static void PrintTable(List<double> items, ConsoleColor color = ConsoleColor.Cyan)
        {
            int i = 1;
            foreach (var item in items)
            {
                PrintWithColor(i++ + ": ", ConsoleColor.White);
                PrintWithColor("" + item, color);
                Console.WriteLine();
            }
        }

        public static void PrintWithColor(string message, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.Write(message);

            Console.ForegroundColor = originalColor;
        }
    }
}
