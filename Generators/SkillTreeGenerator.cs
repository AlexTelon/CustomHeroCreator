using CustomHeroCreator.AI;
using CustomHeroCreator.Trees;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static CustomHeroCreator.Hero;

namespace CustomHeroCreator.Generators
{
    // Will generate a skill tree with the given stats
    class SkillTreeGenerator
    {
        protected static readonly Array ALL_STAT_TYPES = Enum.GetValues(typeof(StatTypes));
        protected static readonly int ALL_STAT_TYPES_COUNT = Enum.GetNames(typeof(StatTypes)).Count();


        public int ChoicesPerLevel { get; set; }

        public double MeanStrengthOfOptions { get; set; }

        /// <summary>
        /// How far from the mean the strength of an individual option is allowed to be. Range [0..1]
        /// </summary>
        public double MaxStrengthOptionDiff { get; internal set; }


        /// <summary>
        /// The agent that makes the choices on how to generate new stats
        /// </summary>
        public Agent Agent {
            get => _agent;
            set
            {
                _agent = value;

                // WE ASSUME THE WEIGHTS are at most a linear thing
                Debug.Assert(_agent.Weights[0].Constants.Count() <= 2);
            }
        }

        private Agent _agent;


        public StatTypes GetRandomStatType() => (StatTypes)_rnd.Next(0, ALL_STAT_TYPES_COUNT);

        protected StatNode GetRandomStat()
        {
            //var template = StatTemplates[_rnd.Next(0, StatTemplates.Count())];

            var node = new StatNode
            {
                Stat = GetRandomStatType(),
            };

            double constantFactor = Agent.Weights[(int)node.Stat].Constants[0]; // power of 0
            double k = 1;
            if (Agent.Weights[(int)node.Stat].Constants.Count() >= 2)
            {
                k = Agent.Weights[(int)node.Stat].Constants[1]; // power of 1
            }

            // calculate a "mean" that might lie some maximum distance away from the true mean
            var randomShift = (_rnd.NextDouble() * 2 * MaxStrengthOptionDiff) - MaxStrengthOptionDiff;
            // shift is now between -maxDiff and + maxDiff

            var mean = MeanStrengthOfOptions * (1 + randomShift);


            // mean = constantFactor + k * value
            // value = (mean - constantFactor) / k
            node.Value = (mean - constantFactor) / k;

            return node;
        }

        private Random _rnd;

        public SkillTreeGenerator(Random rnd)
        {
            _rnd = rnd;
        }

        /// <summary>
        /// Slow! But only used for players now so that is fine
        /// </summary>
        /// <returns></returns>
        public StatNode GenerateUniqueSkillOptions()
        {
            var RootNode = new StatNode();
            RootNode.Stat = Hero.StatTypes.MaxHealth;
            RootNode.Value = 0;

            for (int i = 0; i < ChoicesPerLevel; i++)
            {
                var child = GetRandomStat();
                // make sure the child is not of a type that is already added
                while (RootNode.Children.Select(x => x.Stat).Contains(child.Stat))
                {
                    child = GetRandomStat();
                }

                RootNode.Children.Add(child);
            }

            return RootNode;
        }


        /// <summary>
        /// Generates a skill tree built with the stat types available in this object
        /// </summary>
        public StatNode GenerateSkillTree(int depth)
        {
            var RootNode = new StatNode();
            RootNode.Stat = Hero.StatTypes.MaxHealth;
            RootNode.Value = 0;

            depth--;
            for (int i = 0; i < ChoicesPerLevel; i++)
            {
                RootNode.Children.Add(GenerateSubTree(depth));
            }

            return RootNode;
        }

        private StatNode GenerateSubTree(int depth)
        {
            var node = GetRandomStat();

            if (depth == 0)
            {
                return node;
            }

            depth--;
            for (int i = 0; i < ChoicesPerLevel; i++)
            {
                node.Children.Add(GenerateSubTree(depth));
            }

            return node;
        }
    }
}
