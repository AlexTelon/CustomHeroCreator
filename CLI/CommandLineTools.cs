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
        internal static void PrintTable(List<double> items)
        {
            int i = 1;
            foreach (var item in items)
            {
                Console.WriteLine(i++ + ": " + item);
            }

        }
    }
}
