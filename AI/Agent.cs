using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            // randomize the choise (for now) on how we choose an option
            var rnd = new Random();

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

                // constant or linear by giving weight 1 or 2 constants
                for (int x = 0; x < rnd.Next(1, 2); x++)
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

            Console.BackgroundColor = ConsoleColor.DarkRed;

            Console.WriteLine(String.Join(" ", Weights.Select(x => x.ToString())));

            Console.BackgroundColor = originalBackground;
        }
    }
}
