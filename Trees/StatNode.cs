using CustomHeroCreator.Generators;
using System;
using System.Collections.Generic;
using System.Text;
using static CustomHeroCreator.Hero;

namespace CustomHeroCreator.Trees
{
    class StatNode
    {
        public StatTypes Stat { get; set; }

        public double Value { get; set; }

        public List<StatNode> Children { get; set; } = new List<StatNode>();

    }
}
