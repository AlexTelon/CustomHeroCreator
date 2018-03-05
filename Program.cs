//#define DEBUG
#undef DEBUG

using CustomHeroCreator.AI;
using CustomHeroCreator.CLI;
using CustomHeroCreator.Fighters;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using static CustomHeroCreator.Hero;


namespace CustomHeroCreator
{
    class Program
    {

        private static readonly int MAX_NR_OF_GENERATIONS = 40;
        private static readonly int HERO_STARTING_LEVEL = 10;
        private static readonly int NR_OF_HEROES_IN_EACH_GENERATION = 10;

        private static readonly int MAX_LEVEL = 500;


        private static readonly double MUTATION_CHANCE = 0.2;
        private static readonly double MUTATION_AMPLITUDE = 0.1;

        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.WindowWidth, Console.LargestWindowHeight / 2);

            Console.WriteLine("Welcome to Custom Hero Creator!");

            //Console.WriteLine("Display results? (y/N)");
            //var displayStuff = Console.ReadLine() == "y";
            var displayStuff = false;

            Console.WriteLine("Add human player? (y/N)");
            var HasHumanPlayer = Console.ReadLine() == "y";
            //displayStuff = AddHumanPlayer; // if we have a player, show stuff!

            var generations = new List<List<Hero>>();

            // Create the first generation
            List<Hero> heroes = new List<Hero>();
            var newGeneration = CreateNewGeneration(NR_OF_HEROES_IN_EACH_GENERATION);

            // Create the arena in which the heroes fitness will be evaluated
            Arena arena = new Arena();

            for (int i = 0; i < MAX_NR_OF_GENERATIONS; i++)
            {
                //Add new generation
                generations.Add(newGeneration);

                // The heroes get to level up a few times before they run into their trials
                LevelUpHeroes(generations[i], HERO_STARTING_LEVEL);

                // Let the heroes fight to gauge their Fitness
                //RunTournamentTrial(generations[i]);

                // Run the game on the heroes
                // fight against increasingly strong enemies, survive as long as you can!
                RunSinglePlayerTrials(arena, generations[i]);

#if (DEBUG)
                Console.WriteLine("Generation: " + i);
                CommandLineTools.PrintTable(newGeneration.Select(x => x.Fitness).ToList(), ConsoleColor.Cyan, true);
                Console.WriteLine();
#endif

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


            // we have no found the best AI (refactor out the above parts so this is not the mother of all functions in the end!)

            // Introduce a player and let it fight against the same NPCs, then compare its result against the AI generations
            if (HasHumanPlayer)
            {
                var player = new Hero();

                // The heroes get to level up a few times before they run into their trials
                LevelUpHero(player, HERO_STARTING_LEVEL);

                // fight against increasingly strong enemies, survive as long as you can!
                RunSinglePlayerTrial(arena, player);


                Console.WriteLine("Your result: ");
                Console.WriteLine("Level: " + player.Level);
                player.PrintStats();
                Console.WriteLine();
            }


            CommandLineTools.PrintTable(averagePerGeneration, ConsoleColor.Cyan, true);

            var bestHero = bestInEachGenration.MaxBy(x => x.Fitness);
            Console.WriteLine("Best Configuraion");
            Console.WriteLine("Level: " + bestHero.Level);
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
        private static void RunSinglePlayerTrials(Arena arena, List<Hero> heroes)
        {
            foreach (var hero in heroes)
            {
                RunSinglePlayerTrial(arena, hero);
            }
        }

        /// <summary>
        /// Gauge the Fitness of a hero by letting it fight a series of "NPCs"
        /// </summary>
        /// <param name="list"></param>
        private static void RunSinglePlayerTrial(Arena arena, Hero hero)
        {
            var enemy = new DeterministicScrub();

            // create an agent that is fully random (has no intelligent preferences for one stat over another)
            //var agent = new Agent();
            //for (int i = 0; i < agent.Weights.Count(); i++)
            //{
            //    agent.Weights[i] = 1;
            //}
            //enemy.AI = agent;

            var level = 1;
            while (hero.IsAlive && level < MAX_LEVEL)
            {
                //Make sure they are not dead before the fight
                enemy.Restore();
                hero.Restore();

                enemy.LevelUp();
                hero.LevelUp();

                if (hero.IsPlayer)
                {
                    Console.Clear();
                    Console.WriteLine("Level " + level);
                    Console.Write("Enemy has grown stronger: ");
                    enemy.PrintStats(new List<StatTypes>() { StatTypes.MaxHealth, StatTypes.AttackDmg, StatTypes.Armor }, " ");
                    Console.WriteLine();
                }

                arena.Fight(hero, enemy);
#if (DEBUG)
                Console.Write("\rLevel " + level);
#endif
                level++;
            }

            // The fitness is how many rounds of enemies our hero survived
            hero.Fitness = enemy.Level;
        }

        private static void RunTournamentTrial(Arena arena, List<Hero> heroes)
        {
            // Run tournament style trial
            // all vs all
            for (int i = 0; i < heroes.Count(); i++)
            {
                var hero = heroes[i];
                for (int j = i + 1; j < heroes.Count(); j++)
                {
                    var enemy = heroes[j];
                    arena.Fight(hero, enemy);

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

            var weights = new List<Weight>();

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
                    var constSize = weights[i].Constants.Count();

                    // give random constant a modification 
                    // TODO (this never adds new constants, maybe it should?)
                    // TODO give Weight class breed and mutation functionality, this is ugly
                    weights[i].Constants[rnd.Next(0, constSize - 1 )] += (rnd.NextDouble() * MUTATION_AMPLITUDE) - MUTATION_AMPLITUDE / 2;
                }
            }

            c.Weights = weights;
            child.AI = c;

            return child;
        }

        private static void LevelUpHeroes(List<Hero> heroes, int lvl)
        {
            foreach (var hero in heroes)
            {
                LevelUpHero(hero, lvl);
            }
        }

        private static void LevelUpHero(Hero hero, int lvl)
        {
            for (int i = 0; i < lvl; i++)
            {
                if (hero.IsPlayer)
                {
                    Console.Clear();
                }
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
