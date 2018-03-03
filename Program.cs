using CustomHeroCreator.AI;
using CustomHeroCreator.CLI;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomHeroCreator
{
    class Program
    {
        private static readonly int MAX_NR_OF_TRIALS = 100;
        private static readonly int MAX_NR_OF_GENERATIONS = 40;
        private static readonly int NR_OF_HEROES_IN_EACH_GENERATION = 10;


        private static readonly double MUTATION_CHANCE = 0.2;
        private static readonly double MUTATION_AMPLITUDE = 0.1;

        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.WindowWidth, Console.LargestWindowHeight / 2);

            Console.WriteLine("Welcome to Custom Hero Creator!");

            Console.WriteLine("Display results? (y/N)");
            var displayStuff = Console.ReadLine() == "y";

            var generations = new List<List<Hero>>();

            // Create the first generation
            List<Hero> heroes = new List<Hero>();
            var newGeneration = CreateNewGeneration(NR_OF_HEROES_IN_EACH_GENERATION);


            for (int i = 0; i < MAX_NR_OF_GENERATIONS; i++)
            {
                //Add new generation
                generations.Add(newGeneration);

                //Let the AI choose a set of upgrades to each hero
                PrepareHeroes(generations[i]);

                // Let the heroes fight to gauge their Fitness
                RunTrials(generations[i]);

                //Select the most elite heroes
                List<Hero> elites = SelectElites(generations[i]);

                // The best heroes get to breed
                // And are added to the next generation
                newGeneration = Breed(elites, generations[i].Count());

                //display results
                if (displayStuff)
                {
                    Console.WriteLine("========================");
                    Console.WriteLine("Generation " + i + ": ");
                    DisplayHeroTrialResults(heroes);
                    Console.WriteLine("========================");
                }
            }

            var averagePerGeneration = new List<double>();

            foreach (var generation in generations)
            {
                var average = generation.Average(x => x.Fitness);
                averagePerGeneration.Add(average);
            }

            CommandLineTools.PrintTable(averagePerGeneration, ConsoleColor.Cyan, true);
        }


        private static void RunTrials(List<Hero> heroes)
        {
            // run static trials for each hero
            //foreach (Hero hero in heroes)
            //{
            //    RunTrial(hero);
            //}

            // Run tournament style trial
            // all vs all

            for (int i = 0; i < heroes.Count(); i++)
            {
                var hero = heroes[i];
                for (int j = i + 1; j < heroes.Count(); j++)
                {
                    var enemy = heroes[j];
                    Arena.Fight(hero, enemy);
                    
                    // The heroes Fitness is increased by how much health it has left at the end
                    hero.Fitness += hero.CurrentHealth;
                    enemy.Fitness += enemy.CurrentHealth;

                    //Make sure they are not dead afterwards
                    hero.Restore();
                    enemy.Restore();
                }
            }
        }

        /// <summary>
        /// Let a hero run a series of trials against monsters to gauge its fitness
        /// </summary>
        /// <param name="hero"></param>
        private static void RunMonsterTrial(Hero hero)
        {
            //gauge the fitness of the hero by way of combat

        }

        private static List<Hero> SelectElites(List<Hero> heroes)
        {
            // first lets remove the bottom half of the heroes from further consideration
            var sortedHeroes = heroes.OrderBy(x => x.Fitness, OrderByDirection.Descending).ToList();

            // the most elite of heroes
            var elite = sortedHeroes.Take(sortedHeroes.Count() / 2).ToList();

            return elite;
        }

        /// <summary>
        /// Take the fittest heroes and create a new generation out of them
        /// </summary>
        /// <param name="heroes"></param>
        private static List<Hero> Breed(List<Hero> elites, int sizeOfNewGeneration)
        {
            var rnd = new Random();
            var newGeneration = new List<Hero>();

            var eliteSize = elites.Count();

            // breed enough for a new generation
            for (int i = 0; i < sizeOfNewGeneration; i++)
            {
                var mother = elites[rnd.Next(0, eliteSize)];
                var father = elites[rnd.Next(0, eliteSize)];

                var child = Breed(mother, father);
                newGeneration.Add(child);
            }

            return newGeneration;
        }




        private static Hero Breed(Hero mother, Hero father)
        {
            var child = new Hero();

            // really the AI is the one we are breeding
            Agent a = mother.AI;
            Agent b = father.AI;

            // merge the two with a chance for mutation
            var rnd = new Random();
            Agent c = new Agent();

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

        private static void PrepareHeroes(List<Hero> heroes)
        {
            foreach (var hero in heroes)
            {
                // run trials on the hero
                PrepareHero(hero);
            }
        }

        private static void PrepareHero(Hero hero)
        {
            for (int i = 0; i < MAX_NR_OF_TRIALS; i++)
            {
                hero.LevelUp();
            }
        }


        private static List<Hero> CreateNewGeneration(int nrOfAgents)
        {
            var heroes = new List<Hero>();

            for (int i = 0; i < nrOfAgents; i++)
            {
                Agent ai = new Agent();

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




    }
}
