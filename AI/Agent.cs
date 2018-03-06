using CustomHeroCreator.CLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CustomHeroCreator.Hero;

namespace CustomHeroCreator.AI
{
    class Agent
    {
        public List<Weight> Weights { get; set; } = new List<Weight>();

        public Agent()
        {
            InitRandomWeights();
        }

        internal string ChooseOption(object hero, Dictionary<Hero.StatTypes, double> options)
        {
            // get internal state of the hero, use that later


            // see what options we have

            // the score each option gets
            var scores = new List<double>();

            int i = 0;
            foreach (var option in options)
            {
                //get the corresponding weight function
                var weight = Weights[(int)option.Key];

                // calculate the "score" (the y-value) of the function with the given input
                var score = weight.GetScore(option.Value);

                scores.Add(score);
                i++;
            }

            var maxScore = scores.Max();
            var maxIndex = scores.IndexOf(maxScore);

            // +1 since we are 1 indexed
            return "" + (maxIndex + 1);
        }

        internal void InitRandomWeights()
        {
            var rnd = new Random();

            // Init AI weights
            for (int i = 0; i < Enum.GetNames(typeof(Hero.StatTypes)).Count(); i++)
            {
                var weight = new Weight();

                // Remember, rnd.Next is exlusive on the upper end.
                for (int x = 0; x < rnd.Next(1, 3); x++)
                {
                    weight.Constants.Add(rnd.NextDouble());
                }

                Weights.Add(weight);

                // cheating by making sure that the weight is scaled according to the type of value that it is
                // here crit chance and crit multiplier are multiplicative things are increased by much smaller margins 
                // and hence need to be compensated for in some way when each weight is just a scalar.
                // This is probable fixed by making each weight a n-polynomial where the AI gets to experiment with the degree and the weights.
                //if (i == 2 || i == 3)
                //{
                //    Weights.Add(rnd.NextDouble() * 10);
                //}
                //else
                //{
                //    Weights.Add(rnd.NextDouble() * 1);
                //}
            }
        }

        internal void PrintWeights()
        {
            var originalBackground = Console.BackgroundColor;

            CommandLineTools.PrintWithColor("Weights:", ConsoleColor.White);
            Console.WriteLine();

            Console.BackgroundColor = ConsoleColor.DarkRed;

            var strings = new List<string>();
            var i = 0;
            foreach (var weight in Weights)
            {
                var statName = Enum.GetName(typeof(StatTypes), i);
                strings.Add(statName + ": " + weight.ToString());
                i++;
            }
            Console.WriteLine(String.Join("\n", strings));

            Console.BackgroundColor = originalBackground;
        }
    }
}
