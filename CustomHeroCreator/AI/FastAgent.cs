using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomHeroCreator.Helpers;
using CustomHeroCreator.Trees;
using static CustomHeroCreator.Hero;

namespace CustomHeroCreator.AI
{
    /// <summary>
    /// A fast agent
    /// </summary>
    public class FastAgent : IAgent
    {
        private Random _rnd;

        public FastAgent(Random rnd)
        {
            _rnd = rnd;

            var nrOfStats = Enum.GetNames(typeof(StatTypes)).Count();

            for (int i = 0; i < nrOfStats; i++)
            {
                if (i == 2 || i == 3)
                {
                    weights.Add(_rnd.NextDouble() * 10);
                }
                else
                {
                    weights.Add(_rnd.NextDouble());
                }
            }
        }

        /// <summary>
        /// One weight per stat
        /// </summary>
        protected List<double> weights = new List<double>();

        public IAgent BreedWith(IAgent partner, double mutationRate, double mutationChange)
        {
            var child = new FastAgent(_rnd);

            var weights = new List<double>();

            if (partner is Agent)
            {
                FastAgent other = partner as FastAgent;
                // create a child that is a perfect mix of the parents
                for (int i = 0; i < this.weights.Count(); i++)
                {
                    var choose = RandomHelper.RandomBool(_rnd);
                    weights.Add(choose ? this.weights[i] : other.weights[i]);
                }
            }

            // now add some random mutations
            for (int i = 0; i < weights.Count(); i++)
            {
                if (_rnd.NextDouble() < mutationRate)
                {
                    // give random constant a modification 
                    // TODO (this never adds new constants, maybe it should?)
                    // TODO give Weight class breed and mutation functionality, this is ugly
                    weights[i] += (_rnd.NextDouble() * mutationChange) - mutationChange / 2;
                }
            }
            child.weights = weights;
            return child;
        }

        public int ChooseOption(Hero hero, StatNode node)
        {
            int maxIndex = -1;
            double maxValue = double.MinValue;
            for (int i = 0; i < node.Children.Count; i++)
            {
                var score = GetScore(node.Children[0].Stat, node.Children[0].Value);
                if ((maxIndex < 0) || (score > maxValue))
                {
                    maxValue = score;
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        public double GetScore(Hero.StatTypes type, double value) => weights[(int)type] * value;

        public void PrintInternalDebugInfo()
        {
            throw new NotImplementedException();
        }
    }
}
