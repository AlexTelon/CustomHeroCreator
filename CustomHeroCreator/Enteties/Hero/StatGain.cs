using System;
using CustomHeroCreator.CLI;
using CustomHeroCreator.Repository;

namespace CustomHeroCreator.Enteties
{
    public class StatGain
    {
        // Some stats for the Hero
        public double Str { get; internal set; } = 1;
        public double Agi { get; internal set; } = 1;
        public double Int { get; internal set; } = 1;

        public string StatsGain => " Strength: " + Str + " Agility: " + Agi + " Intelligence: " + Int;

        internal void Print()
        {
            var console = DataHub.Instance.ConsoleWrapper;

            console.Write("Str:");
            CommandLineTools.PrintWithColor(" +" + Str.ToString("0.##"), System.ConsoleColor.Red);
            console.Write(" Agi:");
            CommandLineTools.PrintWithColor(" +" + Agi.ToString("0.##"), System.ConsoleColor.Green);
            console.Write(" Int:");
            CommandLineTools.PrintWithColor(" +" + Int.ToString("0.##"), System.ConsoleColor.Cyan);
        }
    }
}