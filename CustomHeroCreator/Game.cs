using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator
{
    class Game
    {
        private Random _rnd;

        public Hero Player { get; set; }
        public int StartingLevel { get; internal set; }

        // Create the arena in which the Player will fight
        Arena Arena = new Arena();

        // Trials decide what type of trials the heroes will meet (single player survival mode or gladiator arena like stuff)
        Trials Trials = new Trials();

        public Game(Random rnd, Hero player)
        {
            _rnd = rnd;
            Player = player;

            Trials.MaxLevel = 100;
        }


        /// <summary>
        /// Introduce the player to the game
        /// </summary>
        public void PlayerIntro()
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
    }
}
