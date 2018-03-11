﻿using System;
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

        public static IAgent Create(AgentType type, Random rnd)
        {
            switch (type)
            {
                case AgentType.PolyAgent:
                    return new Agent(rnd);
                case AgentType.FastAgent:
                    return new FastAgent(rnd);
                default:
                    throw new NotSupportedException();
            }
        }

    }
}
