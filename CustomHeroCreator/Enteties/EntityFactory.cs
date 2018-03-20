using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator.Enteties
{
    class EntityFactory
    {
        public enum EntityType
        {
            Hero,
            DetermenisticScrub
        }

        public static IEntity Create(EntityType type)
        {
            switch (type)
            {
                case EntityType.Hero:
                    return new Hero();
                case EntityType.DetermenisticScrub:
                    return new DeterministicScrub();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
