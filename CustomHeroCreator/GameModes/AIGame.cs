using CustomHeroCreator.AI;
using CustomHeroCreator.CLI;
using CustomHeroCreator.Generators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CustomHeroCreator.GameModes
{
    class AIGame : IGame
    {
        public Evolution Evo { get; set; }
        public Stopwatch StopWatch { get; set; } = new Stopwatch();


        public void Init()
        {
            StopWatch.Start();

            var rnd = new Random();

            // Create the arena in which the heroes fitness will be evaluated
            Arena arena = new Arena();

            // Trials decide what type of trials the heroes will meet (single player survival mode or gladiator arena like stuff)
            Trials trials = new Trials();
            trials.MaxLevel = 5000;

            Evo = new Evolution(rnd, skillTreeGenerator: null);
            Evo.NrOfHeroesInEachGeneration = 100;
            Evo.MaxGenerations = 20;
            Evo.HeroStartingLevel = 10;
            Evo.AllwaysRunAllGenerations = false;

            Evo.AgentType = AgentFactory.AgentType.FastAgent;

            Evo.RunEvolution(trials, arena);

            Console.WriteLine("The value of the different skills:");
            Evo.BestHero.AI.PrintInternalDebugInfo();
            Console.WriteLine();

            Console.ReadKey();

            // we have now trained an AI to know how strong each stat is

            var skillTreeGenerator = new SkillTreeGenerator(rnd);
            skillTreeGenerator.ChoicesPerLevel = 3;
            skillTreeGenerator.MeanStrengthOfOptions = 3;
            skillTreeGenerator.MaxStrengthOptionDiff = 0;
            skillTreeGenerator.RoundStatValues = true;

            skillTreeGenerator.Agent = Evo.BestHero.AI;

            var treeRootNode = skillTreeGenerator.GenerateSkillTree(10);

            Console.ReadKey();
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Printing Skill Tree");

            treeRootNode.Print(3);
        }

        public void Start()
        {
            Console.WriteLine("Play the game?");
            var play = Console.ReadLine() == "y";
        }

        public void End()
        {


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

            Console.WriteLine("Best Configuraion");
            Console.WriteLine("Level: " + Evo.BestHero.Level);
            Evo.BestHero.PrintStats();

            Console.WriteLine();
            Evo.BestHero.AI.PrintInternalDebugInfo();

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


            StopWatch.Stop();
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("Execution time: ");
            CommandLineTools.PrintWithColor("" + StopWatch.Elapsed, ConsoleColor.Red);

            Console.ReadKey();
        }
    }
}
