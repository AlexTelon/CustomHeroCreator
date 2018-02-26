using System;
using System.Collections.Generic;

namespace CustomHeroCreator
{
    class Program
    {
        private static readonly int MAX_NR_OF_TRIALS = 100;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Custom Hero Creator!");

            Console.WriteLine("How many agents?");
            int NrOfAgents = int.Parse(Console.ReadLine());

            bool useAI = true;

            if (NrOfAgents == 0)
            {
                useAI = false;
            }

            var heroes = new List<Hero>();

            for (int i = 0; i < NrOfAgents; i++)
            {
                AI ai = null;
                if (useAI)
                {
                    ai = new AI();
                }

                Hero hero = new Hero
                {
                    AI = ai
                };

                // run trials on the hero
                RunTrials(hero);

                heroes.Add(hero);
            }

            Console.Clear();

            DisplayHeroTrialResults(heroes);
        }

        private static void DisplayHeroTrialResults(List<Hero> heroes)
        {
            int i = 1;
            foreach (var hero in heroes)
            {
                Console.WriteLine();
                Console.WriteLine("Hero " + i++ + ":");

                hero.PrintStats();

                hero.PrintFitness();
                Console.WriteLine();
            }
        }

        private static void RunTrials(Hero hero)
        {
            for (int i = 0; i < MAX_NR_OF_TRIALS; i++)
            {
                hero.LevelUp();
            }
        }
    }
}
