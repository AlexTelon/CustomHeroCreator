#define DEBUG
#undef DEBUG

using CustomHeroCreator.AI;
using CustomHeroCreator.CLI;
using CustomHeroCreator.Fighters;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomHeroCreator.Logging;
using static CustomHeroCreator.Hero;
using CustomHeroCreator.Helpers;

using System.Diagnostics;
using System.Threading;
using CustomHeroCreator.Generators;

namespace CustomHeroCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Always use . instead of , as a comma.
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.SetWindowSize(Console.WindowWidth, Console.LargestWindowHeight / 2);

            Console.WriteLine("Welcome to Custom Hero Creator!");

            //Console.WriteLine("Display results? (y/N)");
            //var displayStuff = Console.ReadLine() == "y";
            //var displayStuff = false;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var rnd = new Random();

            // Create the arena in which the heroes fitness will be evaluated
            Arena arena = new Arena();

            // Trials decide what type of trials the heroes will meet (single player survival mode or gladiator arena like stuff)
            Trials trials = new Trials();
            trials.MaxLevel = 5000;

            var evo = new Evolution(rnd, skillTreeGenerator: null);
            evo.NrOfHeroesInEachGeneration = 100;
            evo.MaxGenerations = 20;
            evo.HeroStartingLevel = 10;
            evo.AllwaysRunAllGenerations = true;

            evo.AgentType = AgentFactory.AgentType.FastAgent;


            evo.RunEvolution(trials, arena);

            Console.WriteLine("The value of the different skills:");
            evo.BestHero.AI.PrintInternalDebugInfo();
            Console.WriteLine();

            Console.ReadKey();

            // we have now trained an AI to know how strong each stat is

            var skillTreeGenerator = new SkillTreeGenerator(rnd);
            skillTreeGenerator.ChoicesPerLevel = 3;
            skillTreeGenerator.MeanStrengthOfOptions = 3;
            skillTreeGenerator.MaxStrengthOptionDiff = 0;

            skillTreeGenerator.Agent = evo.BestHero.AI;

            var treeRootNode = skillTreeGenerator.GenerateSkillTree(10);

#if DEBUG
            // Start second evolutionary algo to evaluate the SkillTreeGenerator that has been created
            evo = new Evolution(rnd, skillTreeGenerator: skillTreeGenerator);
            evo.NrOfHeroesInEachGeneration = 100;
            evo.MaxGenerations = 20;
            evo.HeroStartingLevel = 10;
            evo.AllwaysRunAllGenerations = true;


            evo.RunEvolution(trials, arena);

            Console.WriteLine("The value of the different skills:");
            evo.BestHero.AI.PrintWeights();
            Console.WriteLine();

            Console.ReadKey();
#endif


            Console.WriteLine("Add human player? (y/N)");
            var HasHumanPlayer = Console.ReadLine() == "y";
            //displayStuff = AddHumanPlayer; // if we have a player, show stuff!

            // Introduce a player and let it fight against the same NPCs, then compare its result against the AI generations
            if (HasHumanPlayer)
            {
                var player = new Hero(rnd);

                player.SkillTreeGenerator = skillTreeGenerator;

                // The heroes get to level up a few times before they run into their trials
                Trials.LevelUpHero(player, evo.HeroStartingLevel);

                // fight against increasingly strong enemies, survive as long as you can!
                trials.RunSinglePlayerTrial(arena, player);


                Console.WriteLine("Your result: ");
                Console.WriteLine("Level: " + player.Level);
                player.PrintStats();
                Console.WriteLine();
            }


            Console.WriteLine("Best Configuraion");
            Console.WriteLine("Level: " + evo.BestHero.Level);
            evo.BestHero.PrintStats();

            Console.WriteLine();
            evo.BestHero.AI.PrintInternalDebugInfo();

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


            sw.Stop();
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("Execution time: ");
            CommandLineTools.PrintWithColor("" + sw.Elapsed, ConsoleColor.Red);

            Console.ReadKey();
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Printing Skill Tree");

            treeRootNode.Print(3);

        }
    }
}
