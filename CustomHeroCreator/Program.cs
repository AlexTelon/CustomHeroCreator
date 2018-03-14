using System;
using System.Threading;
using CustomHeroCreator.CLI;
using CustomHeroCreator.GameModes;
using CustomHeroCreator.Repository;

namespace CustomHeroCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Always use . instead of , as a comma.
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
#if DEBUG
            // Running SetWindowsSize below in a linux docker containter gives a plattform not supported exception
            Console.SetWindowSize(Console.WindowWidth, Console.LargestWindowHeight / 2);
#endif

            // Select what type of console we want
#if DEBUG
            var console = new AutoResponseConsole();
            console.AutomatedResponses.Add("1");
#else
            var console = new PlayerConsole();

#endif

            DataHub.Instance.ConsoleWrapper = console;

            //var game = new AIGame(playerConsole);
            var game = new PlayerGame();
            game.Difficulty = 3;

            game.Init();
            game.Start();
            game.End();
        }
    }
}
