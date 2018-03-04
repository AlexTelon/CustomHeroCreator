using System.Linq;
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
        internal static void PrintTable(List<double> items, ConsoleColor color = ConsoleColor.Cyan, bool showExtra = false)
        {
            var max = items.Max();
            var min = items.Min();

            // print table header:
            PrintWithColor("Row\tValue\tBar", ConsoleColor.Green);
            Console.WriteLine();
            PrintWithColor("===========================", ConsoleColor.Gray);
            Console.WriteLine();

            int i = 1;
            foreach (var item in items)
            {
                PrintWithColor(i++ + ":\t", ConsoleColor.White);
                PrintWithColor("" + item.ToString("#.##"), color);

                if (showExtra)
                {
                    Console.Write("\t");
                    PrintVerticalBar(item, min, max, ConsoleColor.Red);
                }

                Console.WriteLine();
            }
        }

        internal static void PrintVerticalBar(double value, double min, double max, ConsoleColor color)
        {
            var ratio = 1 - (max - value) / (max - min);

            var MAX_NR_OF_BARS = 20;

            var bars = (int)Math.Round(ratio * MAX_NR_OF_BARS);

            StringBuilder builder = new StringBuilder();

            builder.Append("|");
            for (int i = 0; i < bars; i++)
            {
                builder.Append("=");
            }
            PrintWithColor(builder.ToString(), color);
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
