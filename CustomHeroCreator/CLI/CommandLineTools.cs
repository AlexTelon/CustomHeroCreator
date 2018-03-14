using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using CustomHeroCreator.Repository;

namespace CustomHeroCreator.CLI
{
    public static class CommandLineTools
    {

        internal static void PrintTableHeader()
        {
            var console = DataHub.Instance.ConsoleWrapper;
            PrintWithColor("Row\tValue\tBar", ConsoleColor.Green);
            console.WriteLine();
            PrintWithColor("===========================", ConsoleColor.Gray);
            console.WriteLine();
        }

        /// <summary>
        /// Prints a list of doubles as a table to the command line
        /// </summary>
        /// <param name="items"></param>
        internal static void PrintTable(List<double> items, ConsoleColor color = ConsoleColor.Cyan, bool showExtra = false)
        {
            var console = DataHub.Instance.ConsoleWrapper;

            var max = items.Max();
            var min = items.Min();

            // print table header:
            PrintTableHeader();

            int i = 1;
            foreach (var item in items)
            {
                PrintWithColor(i++ + ":\t", ConsoleColor.White);
                PrintWithColor("" + item.ToString("#.##"), color);

                if (showExtra)
                {
                    console.Write("\t");
                    PrintVerticalBar(item, min, max, ConsoleColor.Red);
                }

                console.WriteLine();
            }
        }

        internal static void PrintVerticalBar(double value, double min, double max, ConsoleColor color)
        {
            var console = DataHub.Instance.ConsoleWrapper;

            var ratio = 1 - (max - value) / (max - min);

            var MAX_NR_OF_BARS = 20;

            var bars = (int)Math.Round(ratio * MAX_NR_OF_BARS);

            StringBuilder builder = new StringBuilder();

            builder.Append("|");
            for (int i = 0; i < bars; i++)
            {
                builder.Append("=");
            }

            // add whitespace padding so all bars are the same "width"
            // It makes spacing text out behind bars easier
            var padding = MAX_NR_OF_BARS - bars;
            for (int i = 0; i < padding; i++)
            {
                builder.Append(" ");
            }


            PrintWithColor(builder.ToString(), color);
        }

        public static void PrintWithColor(string message = "", ConsoleColor color = ConsoleColor.White)
        {
            var console = DataHub.Instance.ConsoleWrapper;

            var originalColor = console.ForegroundColor;

            console.ForegroundColor = color;
            console.Write(message);

            console.ForegroundColor = originalColor;
        }

    }
}
