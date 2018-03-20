using CustomHeroCreator.CLI;
using CustomHeroCreator.Enteties;
using CustomHeroCreator.Helpers;
using CustomHeroCreator.Repository;
using CustomHeroCreator.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CustomHeroCreator.Enteties.Hero;

namespace CustomHeroCreator.AI
{
    public class Agent : IAgent
    {
        protected List<Weight> Weights { get; set; } = new List<Weight>();

        public Agent()
        {
            InitRandomWeights();
        }

        public double GetScore(StatTypes type, double value)
        {
            var weight = Weights[(int)type];

            // calculate the "score" (the y-value) of the function with the given input
            var score = weight.GetScore(value);
            return score;
        }

        public int ChooseOption(Hero hero, StatNode node)
        {
            // get internal state of the hero, use that later

            // the score each option gets
            var scores = new List<double>();

            int i = 0;
            foreach (var option in node.Children)
            {
                // calculate the "score" (the y-value) of the function with the given input
                var score = GetScore(option.Stat, option.Value);

                scores.Add(score);
                i++;
            }

            var maxScore = scores.Max();
            var maxIndex = scores.IndexOf(maxScore);

            return maxIndex;
        }

        private void InitRandomWeights()
        {
            var rnd = DataHub.Instance.RandomSource;

            // Init AI weights
            for (int i = 0; i < Enum.GetNames(typeof(Hero.StatTypes)).Count(); i++)
            {
                var weight = new Weight();

                if (i == (int)StatTypes.CritChance || i == (int)StatTypes.CritMultiplier)
                {
                    // Remember, rnd.Next is exlusive on the upper end.
                    for (int x = 0; x < rnd.Next(1, 3); x++)
                    {
                        weight.Constants.Add(rnd.NextDouble() * 10);
                    }
                }
                else
                {

                    // Remember, rnd.Next is exlusive on the upper end.
                    for (int x = 0; x < rnd.Next(1, 3); x++)
                    {
                        weight.Constants.Add(rnd.NextDouble());
                    }
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

        public void PrintInternalDebugInfo()
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


        public IAgent BreedWith(IAgent partner, double mutationRate, double mutationChange)
        {
            var rnd = DataHub.Instance.RandomSource;

            var child = new Agent();

            var weights = new List<Weight>();

            if (partner is Agent)
            {
                Agent other = partner as Agent;
                // create a child that is a perfect mix of the parents
                for (int i = 0; i < this.Weights.Count(); i++)
                {
                    var choose = RandomHelper.RandomBool();
                    weights.Add(choose ? this.Weights[i] : other.Weights[i]);
                }
            }


            // now add some random mutations
            for (int i = 0; i < weights.Count(); i++)
            {
                if (rnd.NextDouble() < mutationRate)
                {
                    var constSize = weights[i].Constants.Count();

                    // give random constant a modification 
                    // TODO (this never adds new constants, maybe it should?)
                    // TODO give Weight class breed and mutation functionality, this is ugly
                    weights[i].Constants[rnd.Next(0, constSize - 1)] += (rnd.NextDouble() * mutationChange) - mutationChange / 2;
                }
            }

            child.Weights = weights;

            return child;
        }
    }
}
