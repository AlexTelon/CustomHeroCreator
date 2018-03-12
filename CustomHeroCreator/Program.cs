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
using CustomHeroCreator.GameModes;

namespace CustomHeroCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Always use . instead of , as a comma.
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.SetWindowSize(Console.WindowWidth, Console.LargestWindowHeight / 2);

#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

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
            evo.AllwaysRunAllGenerations = false;

            evo.AgentType = AgentFactory.AgentType.FastAgent;


            evo.RunEvolution(trials, arena);
#if DEBUG
            Console.WriteLine("The value of the different skills:");
            evo.BestHero.AI.PrintInternalDebugInfo();
            Console.WriteLine();

            Console.ReadKey();
#endif
            // we have now trained an AI to know how strong each stat is

            var skillTreeGenerator = new SkillTreeGenerator(rnd);
            skillTreeGenerator.ChoicesPerLevel = 3;
            skillTreeGenerator.MeanStrengthOfOptions = 3;
            skillTreeGenerator.MaxStrengthOptionDiff = 0;
            skillTreeGenerator.RoundStatValues = true;

            skillTreeGenerator.Agent = evo.BestHero.AI;

            var treeRootNode = skillTreeGenerator.GenerateSkillTree(10);

#if FALSE
            // Start second evolutionary algo to evaluate the SkillTreeGenerator that has been created
            evo = new Evolution(rnd, skillTreeGenerator: skillTreeGenerator);
            evo.NrOfHeroesInEachGeneration = 100;
            evo.MaxGenerations = 20;
            evo.HeroStartingLevel = 10;
            evo.AllwaysRunAllGenerations = true;


            evo.RunEvolution(trials, arena);

            Console.WriteLine("The value of the different skills:");
            evo.BestHero.AI.PrintInternalDebugInfo();
            Console.WriteLine();

            Console.ReadKey();
#endif
            var player = new Hero(rnd);
            player.SkillTreeGenerator = skillTreeGenerator;

            IGame game = new PlayerGame(rnd, player);

#if DEBUG
            Console.WriteLine("Play the game?");
            var play = Console.ReadLine() == "y";
#else
            var play = true;
            game.Init();
#endif
            if (play)
            {

                game.Start();
            }



#if DEBUG
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
#endif
        }
    }
}
