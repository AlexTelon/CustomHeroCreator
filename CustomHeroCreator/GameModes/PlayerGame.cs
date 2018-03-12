using CustomHeroCreator.AI;
using CustomHeroCreator.Generators;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator.GameModes
{
    class PlayerGame : IGame
    {
        private Random _rnd;

        public Hero Player { get; set; }
        public int StartingLevel { get; internal set; } = 10;

        // Create the arena in which the Player will fight
        private Arena Arena { get; set; } = new Arena();

        // Trials decide what type of trials the heroes will meet (single player survival mode or gladiator arena like stuff)
        private Trials Trials { get; set; } = new Trials();

        private Evolution Evo { get; set; }

        public PlayerGame()
        {
            _rnd = new Random();
            Player = new Hero(_rnd);

            Trials.MaxLevel = 100;
        }


        /// <summary>
        /// Introduce the player to the game
        /// </summary>
        public void Init()
        {
            // Create the arena in which the heroes fitness will be evaluated
            Arena arena = new Arena();

            // Trials decide what type of trials the heroes will meet (single player survival mode or gladiator arena like stuff)
            Trials trials = new Trials();
            trials.MaxLevel = 5000;

            Evo = new Evolution(_rnd, skillTreeGenerator: null);
            Evo.NrOfHeroesInEachGeneration = 100;
            Evo.MaxGenerations = 20;
            Evo.HeroStartingLevel = 10;
            Evo.AllwaysRunAllGenerations = false;

            Evo.AgentType = AgentFactory.AgentType.FastAgent;

            Evo.RunEvolution(trials, arena);

            // we have now trained an AI to know how strong each stat is
            // Now a skill tree generator can be created
            var skillTreeGenerator = new SkillTreeGenerator(_rnd);
            skillTreeGenerator.ChoicesPerLevel = 3;
            skillTreeGenerator.MeanStrengthOfOptions = 3;
            skillTreeGenerator.MaxStrengthOptionDiff = 0;
            skillTreeGenerator.RoundStatValues = true;

            skillTreeGenerator.Agent = Evo.BestHero.AI;

            // we have no created a good skill tree generator
            // lets give one to the player
            Player.SkillTreeGenerator = skillTreeGenerator;

            ShowPlayerIntro();
        }

        private void ShowPlayerIntro()
        {
            Console.WriteLine("Welcome to Custom Hero Creator!");
            Console.WriteLine("You will begin by selecting a hero!");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("You have choosen a X hero!");
            Console.WriteLine("...");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("You prepare yourself for the comming battles!");
            Console.WriteLine("...");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("Good luck!");
            Console.WriteLine("...");
            Console.ReadLine();
        }

        public void Start()
        {
            // The heroes get to level up a few times before they run into their trials
            Trials.LevelUpHero(Player, StartingLevel);

            // fight against increasingly strong enemies, survive as long as you can!
            Trials.RunSinglePlayerTrial(Arena, Player);

            if (Player.IsAlive)
            {
                WinScreen();
            } else
            {
                LooseScreen();
            }
        }

        private void LooseScreen()
        {
            Console.WriteLine("You lost");
            PrintEndGameStatistics();
        }

        private void WinScreen()
        {
            Console.WriteLine("You won!");
            PrintEndGameStatistics();
        }

        private void PrintEndGameStatistics()
        {
            Console.WriteLine("Your result: ");
            Console.WriteLine("Level: " + Player.Level);
            Player.PrintStats();
            Console.WriteLine();
        }

        public void End()
        {

        }
    }
}
