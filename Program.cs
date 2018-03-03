using CustomHeroCreator.AI;
using CustomHeroCreator.CLI;
using CustomHeroCreator.Fighters;
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
                //RunTournamentTrial(generations[i]);

                // Gauge the Fitness of each hero by fighting against "NPCs"
                // This gives a fitness that is comparable across generations 
                // since the NPCs are equally challanging for all
                RunSinglePlayerTrials(generations[i]);

                
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
            var bestInEachGenration = new List<Hero>();


            foreach (var generation in generations)
            {
                var average = generation.Average(x => x.Fitness);
                averagePerGeneration.Add(average);

                bestInEachGenration.Add(generation.MaxBy(x => x.Fitness));
            }

            CommandLineTools.PrintTable(averagePerGeneration, ConsoleColor.Cyan, true);

            var bestHero = bestInEachGenration.MaxBy(x => x.Fitness);
            Console.WriteLine("Best Configuraion");
            bestHero.PrintStats();
            Console.WriteLine();


            // in the end let the top performer from each generation all fight each other
            //bestInEachGenration = new List<Hero>();
            //foreach (var generation in generations)
            //{
            //    var bestInClass = generation.MaxBy(x => x.Fitness);
            //    bestInClass.Fitness = 0;
            //    bestInEachGenration.Add(bestInClass);
            //}

            //RunTournamentTrial(bestInEachGenration);



        }

        /// <summary>
        /// Gauge the Fitness of each hero by letting them fight a series of "NPCs"
        /// </summary>
        /// <param name="list"></param>
        private static void RunSinglePlayerTrials(List<Hero> heroes)
        {
            foreach (var hero in heroes)
            {
                RunSinglePlayerTrial(hero);
            }
        }

        /// <summary>
        /// Gauge the Fitness of a hero by letting it fight a series of "NPCs"
        /// </summary>
        /// <param name="list"></param>
        private static void RunSinglePlayerTrial(Hero hero)
        {
            var enemy = new DeterministicScrub();

            // create an agent that is fully random (has no intelligent preferences for one stat over another)
            //var agent = new Agent();
            //for (int i = 0; i < agent.Weights.Count(); i++)
            //{
            //    agent.Weights[i] = 1;
            //}
            //enemy.AI = agent;

            while (hero.IsAlive)
            {
                //Make sure they are not dead before the fight
                enemy.Restore();
                hero.Restore();

                // each iteration the enemy grows stronger
                enemy.LevelUp();

                Arena.Fight(hero, enemy);
            }

            // The fitness is how long our hero survived
            hero.Fitness = enemy.Level;
        }

        private static void RunTournamentTrial(List<Hero> heroes)
        {
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
