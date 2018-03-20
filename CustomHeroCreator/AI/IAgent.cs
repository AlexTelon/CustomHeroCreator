using CustomHeroCreator.Enteties;
using CustomHeroCreator.Trees;
using System;
using System.Collections.Generic;
using System.Text;
using static CustomHeroCreator.Enteties.Hero;

namespace CustomHeroCreator.AI
{
    public interface IAgent
    {
        int ChooseOption(Hero hero, StatNode node);

        double GetScore(StatTypes type, double value);

        IAgent BreedWith(IAgent partner, double mutationRate, double mutationChange);

        void PrintInternalDebugInfo();

    }
}
