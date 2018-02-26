using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomHeroCreator
{
    class Program
    {
        private static readonly int MAX_NR_OF_TRIALS = 100;
        private static readonly int MAX_NR_OF_GENERATIONS = 100;
        private static readonly double MUTATION_CHANCE = 0.2;
        private static readonly double MUTATION_AMPLITUDE = 0.1;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Custom Hero Creator!");

            Console.WriteLine("How many agents?");

            int nrOfAgents = int.Parse(Console.ReadLine());

            var generations = new List<List<Hero>>();

            List<Hero> heroes = new List<Hero>();
            heroes = CreateNewGeneration(nrOfAgents);
            generations.Add(heroes);


            for (int i = 0; i < MAX_NR_OF_GENERATIONS; i++)
            {
                // Run trials on the new generation of heroes
                RunTrials(generations[i]);

                // The best heroes get to breed
                // And are added to the next generation
                var newGeneration = Breed(generations[i]);
                generations.Add(newGeneration);


                //display results
                Console.WriteLine("========================");
                Console.WriteLine("Generation " + i + ": ");
                DisplayHeroTrialResults(heroes);
                Console.WriteLine("========================");
            }

            var averagePerGeneration = new List<double>();

            foreach (var generation in generations)
            {
                var average = generation.Average(x => x.Fitness);
                averagePerGeneration.Add(average);
            }

            //Console.Clear();

        }

        /// <summary>
        /// Take the fittest heroes and create a new generation out of them
        /// </summary>
        /// <param name="heroes"></param>
        private static List<Hero> Breed(List<Hero> heroes)
        {
            var rnd = new Random();

            var newGeneration = new List<Hero>();

            var generationSize = heroes.Count();

            // first lets remove the bottom half of the heroes from further consideration
            var sortedHeroes = heroes.OrderBy(x => x.Fitness, OrderByDirection.Descending).ToList();

            // the most elite of heroes
            var elite = sortedHeroes.Take(sortedHeroes.Count() / 2).ToList();
            var eliteSize = elite.Count();

            // breed enough for a new generation
            for (int i = 0; i < generationSize; i++)
            {
                var mother = elite[rnd.Next(0, eliteSize)];
                var father = elite[rnd.Next(0, eliteSize)];

                var child = Breed(mother, father);
                newGeneration.Add(child);
            }

            return newGeneration;
        }

        private static Hero Breed(Hero mother, Hero father)
        {
            var child = new Hero();

            // really the AI is the one we are breeding
            AI a = mother.AI;
            AI b = father.AI;

            // merge the two with a chance for mutation
            var rnd = new Random();
            AI c = new AI();

            var weights = new List<double>();

            // create a child that is a perfect mix of the parents
            for (int i = 0; i < a.Weights.Count(); i++)
            {
                weights.Add(rnd.Next(0, 1) == 0 ? a.Weights[i] : b.Weights[i]);
            }


            // now add some random mutations
            for (int i = 0; i < weights.Count(); i++)
            {
                if (rnd.NextDouble() > MUTATION_CHANCE)
                {
                    weights[i] += (rnd.NextDouble() * MUTATION_AMPLITUDE) - MUTATION_AMPLITUDE / 2;
                }
            }

            c.Weights = weights;
            child.AI = c;

            return child;
        }

        private static void RunTrials(List<Hero> heroes)
        {
            foreach (var hero in heroes)
            {
                // run trials on the hero
                RunTrials(hero);
            }
        }

        private static List<Hero> CreateNewGeneration(int nrOfAgents)
        {
            var heroes = new List<Hero>();

            for (int i = 0; i < nrOfAgents; i++)
            {
                AI ai = new AI();

                Hero hero = new Hero
                {
                    AI = ai
                };

                heroes.Add(hero);
            }

            return heroes;
        }

        private static void DisplayHeroTrialResults(List<Hero> heroes)
        {
            int i = 1;
            foreach (var hero in heroes)
            {
                Console.WriteLine();
                Console.WriteLine("Hero " + i++ + ":");

                //hero.PrintStats();

                hero.PrintFitness();

                hero.AI.PrintWeights();

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
