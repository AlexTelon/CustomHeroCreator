//#define DEBUG
#undef DEBUG

using CustomHeroCreator.AI;
using CustomHeroCreator.CLI;
using CustomHeroCreator.Generators;
using CustomHeroCreator.Helpers;
using CustomHeroCreator.Logging;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomHeroCreator
{
    class Evolution
    {
        public int MaxGenerations { get; set; } = 100;
        public int HeroStartingLevel { get; set; } = 10;
        public int NrOfHeroesInEachGeneration { get; set; } = 100;

        private static readonly double MUTATION_CHANCE = 0.01;
        private static readonly double MUTATION_AMPLITUDE = 0.01;

        public bool AllwaysRunAllGenerations { get; set; } = false;

        public List<List<Hero>> Generations { get; private set; } = new List<List<Hero>>();

        public SkillTreeGenerator SkillTreeGenerator { get; set; }

        public Hero BestHero;

        private Random _rnd;


        public Evolution(Random rnd, SkillTreeGenerator skillTreeGenerator)
        {
            _rnd = rnd;
            SkillTreeGenerator = skillTreeGenerator;
            BestHero = new Hero(_rnd);
        }

        internal void RunEvolution(Trials trials, Arena arena)
        {
            var newGeneration = this.CreateNewGeneration();

            CommandLineTools.PrintTableHeader();
            var averagePerGeneration = new List<double>();
            var bestInEachGenration = new List<Hero>();

            for (int i = 0; i < MaxGenerations; i++)
            {
                //Add new generation
                Generations.Add(newGeneration);

                // The heroes get to level up a few times before they run into their trials
                Trials.LevelUpHeroes(Generations[i], HeroStartingLevel);

                // Run the game on the heroes
                // fight against increasingly strong enemies, survive as long as you can!
                trials.RunSinglePlayerTrials(arena, Generations[i]);

                //Stat stuff
                Logger.Instance.Log(Generations[i]);

                var average = Generations[i].Average(x => x.Fitness);
                averagePerGeneration.Add(average);

                CommandLineTools.PrintVerticalBar(average, 0, trials.MaxLevel, ConsoleColor.Red);
                Console.WriteLine();

                BestHero = Generations[i].MaxBy(x => x.Fitness);
                bestInEachGenration.Add(BestHero);

                //If we have an AI that can beat the max level we are done
                if (!AllwaysRunAllGenerations)
                {
                    if (BestHero.Fitness >= trials.MaxLevel)
                    {
                        break;
                    }
                }


#if (DEBUG)
                Console.WriteLine("Generation: " + i);
                CommandLineTools.PrintTable(newGeneration.Select(x => x.Fitness).ToList(), ConsoleColor.Cyan, true);
                Console.WriteLine();
#endif

                //Select the most elite heroes
                List<Hero> elites = SelectElites(Generations[i]);

                // The best heroes get to breed
                // And are added to the next generation
                newGeneration = Breed(elites, Generations[i].Count());


                ////display results
                //if (displayStuff)
                //{
                //    Console.WriteLine("========================");
                //    Console.WriteLine("Generation " + i + ": ");
                //    DisplayHeroTrialResults(heroes);
                //    Console.WriteLine("========================");
                //}
            }

            Console.Clear();
            Console.WriteLine();
            CommandLineTools.PrintTable(averagePerGeneration, ConsoleColor.Cyan, true);

        }


        internal List<Hero> SelectElites(List<Hero> heroes)
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
        internal List<Hero> Breed(List<Hero> elites, int sizeOfNewGeneration)
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

        internal Hero Breed(Hero mother, Hero father)
        {
            var rnd = new Random();
            var child = new Hero(rnd);
            child.SkillTreeGenerator = SkillTreeGenerator;

            // really the AI is the one we are breeding
            Agent a = mother.AI;
            Agent b = father.AI;

            // merge the two with a chance for mutation
            Agent c = new Agent();

            var weights = new List<Weight>();

            // create a child that is a perfect mix of the parents
            for (int i = 0; i < a.Weights.Count(); i++)
            {
                var choose = RandomHelper.RandomBool(rnd);
                weights.Add(choose ? a.Weights[i] : b.Weights[i]);
            }


            // now add some random mutations
            for (int i = 0; i < weights.Count(); i++)
            {
                if (rnd.NextDouble() < MUTATION_CHANCE)
                {
                    var constSize = weights[i].Constants.Count();

                    // give random constant a modification 
                    // TODO (this never adds new constants, maybe it should?)
                    // TODO give Weight class breed and mutation functionality, this is ugly
                    weights[i].Constants[rnd.Next(0, constSize - 1)] += (rnd.NextDouble() * MUTATION_AMPLITUDE) - MUTATION_AMPLITUDE / 2;
                }
            }

            c.Weights = weights;
            child.AI = c;

            return child;
        }


        internal List<Hero> CreateNewGeneration()
        {
            var rnd = new Random();
            var heroes = new List<Hero>();

            for (int i = 0; i < NrOfHeroesInEachGeneration; i++)
            {
                Agent ai = new Agent();

                Hero hero = new Hero(rnd)
                {
                    AI = ai
                };

                heroes.Add(hero);
            }

            return heroes;
        }

        //private static void DisplayHeroTrialResults(List<Hero> heroes)
        //{
        //    int i = 1;
        //    foreach (var hero in heroes)
        //    {
        //        Console.WriteLine();
        //        Console.WriteLine("Hero " + i++ + ":");

        //        //hero.PrintStats();

        //        hero.PrintFitness();

        //        hero.AI.PrintWeights();

        //        Console.WriteLine();
        //    }
        //}


    }
}
