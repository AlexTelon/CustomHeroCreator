using CustomHeroCreator.CLI;
using CustomHeroCreator.Helpers;
using CustomHeroCreator.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CustomHeroCreator.Hero;

namespace CustomHeroCreator.AI
{
    class Agent
    {
        protected List<Weight> Weights { get; set; } = new List<Weight>();

        private Random _rnd;

        public Agent(Random rnd)
        {
            _rnd = rnd;
            InitRandomWeights();
        }

        internal double GetScore(StatTypes type, double value)
        {
            var weight = Weights[(int)type];

            // calculate the "score" (the y-value) of the function with the given input
            var score = weight.GetScore(value);
            return score;
        }

        internal string ChooseOption(Hero hero, StatNode node)
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

            // +1 since we are 1 indexed
            return "" + (maxIndex + 1);
        }

        internal void InitRandomWeights()
        {
            // Init AI weights
            for (int i = 0; i < Enum.GetNames(typeof(Hero.StatTypes)).Count(); i++)
            {
                var weight = new Weight();

                if (i == 2 || i == 3)
                {
                    // Remember, rnd.Next is exlusive on the upper end.
                    for (int x = 0; x < _rnd.Next(1, 3); x++)
                    {
                        weight.Constants.Add(_rnd.NextDouble() * 10);
                    }
                }
                else
                {

                    // Remember, rnd.Next is exlusive on the upper end.
                    for (int x = 0; x < _rnd.Next(1, 3); x++)
                    {
                        weight.Constants.Add(_rnd.NextDouble());
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


        internal Agent BreedWith(Agent partner, double mutationRate, double mutationChange)
        {
            var child = new Agent(_rnd);

            var weights = new List<Weight>();

            // create a child that is a perfect mix of the parents
            for (int i = 0; i < this.Weights.Count(); i++)
            {
                var choose = RandomHelper.RandomBool(_rnd);
                weights.Add(choose ? this.Weights[i] : partner.Weights[i]);
            }


            // now add some random mutations
            for (int i = 0; i < weights.Count(); i++)
            {
                if (_rnd.NextDouble() < mutationRate)
                {
                    var constSize = weights[i].Constants.Count();

                    // give random constant a modification 
                    // TODO (this never adds new constants, maybe it should?)
                    // TODO give Weight class breed and mutation functionality, this is ugly
                    weights[i].Constants[_rnd.Next(0, constSize - 1)] += (_rnd.NextDouble() * mutationChange) - mutationChange / 2;
                }
            }

            child.Weights = weights;

            return child;
        }
    }
}
