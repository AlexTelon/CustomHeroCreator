using CustomHeroCreator.CLI;
using CustomHeroCreator.Generators;
using System;
using System.Collections.Generic;
using System.Text;
using static CustomHeroCreator.Enteties.Hero;

namespace CustomHeroCreator.Trees
{
    public class StatNode
    {
        public List<StatNode> Children { get; set; } = new List<StatNode>();

        public StatTypes Stat { get; set; }

        public double Value { get; set; }

        internal void Print(int depth)
        {
            depth--;

            // only print children if we still have depth to go
            if (depth >= 0)
            {
                foreach (var child in Children)
                {
                    child.Print(depth);
                }
            }

            Console.Write("Depth: ");
            // shift the color at the end not to ever include 0 since that is black and to skip the first few since they are dark
            // and then let it wrap around at 15 in those rare cases we want to print deeper than 6 or smth
            CommandLineTools.PrintWithColor("" + (depth + 1), (ConsoleColor) ((depth + 3) % 15));
            Console.Write("\t");

            var message = Enum.GetName(typeof(StatTypes), Stat) + ": ";
            var color = (ConsoleColor)((int)(Stat + 1) % 15); // + 1 to avoid black.
            CommandLineTools.PrintWithColor(message, color);
            Console.WriteLine(Value.ToString("0.00"));
        }
    }
}
