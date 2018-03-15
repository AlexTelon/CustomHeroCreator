using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator.AI
{
    public static class AgentFactory
    {
        public enum AgentType
        {
            PolyAgent,
            FastAgent
        }

        public static IAgent Create(AgentType type)
        {
            switch (type)
            {
                case AgentType.PolyAgent:
                    return new Agent();
                case AgentType.FastAgent:
                    return new FastAgent();
                default:
                    throw new NotSupportedException();
            }
        }

    }
}
