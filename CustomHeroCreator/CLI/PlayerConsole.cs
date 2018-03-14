using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator.CLI
{
    /// <summary>
    /// A real Console that a player can use.
    /// </summary>
    public class PlayerConsole : IConsole
    {
        public ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        public void Write(string message = "")
        {
            Console.Write(message);
        }

        public void WriteLine(string message = "")
        {
            Console.WriteLine(message);
        }

        public void Clear()
        {
            Console.Clear();
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}
