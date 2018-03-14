using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator.CLI
{
    /// <summary>
    /// A Console that has auto-responses to ReadLine like requrests.
    /// The idea is that Tests and AI can use this. Also nice for debugging certain situations quickly
    /// </summary>
    public class AutoResponseConsole : IConsole
    {
        public List<String> AutomatedResponses = new List<String>();
        private int counter = 0;

        private readonly bool LoopResponses = true;

        public ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        public void Clear()
        {
        }

        public string ReadLine()
        {
            if (!LoopResponses)
            {
                throw new NotImplementedException("LoopResponses being false is not implemented yet");
            }

            string result = AutomatedResponses[counter];
            counter = (counter + 1) % AutomatedResponses.Count;
            return result;
        }

        public void Write(string message = "")
        {
            Console.Write(message);
        }

        public void WriteLine(string message = "")
        {
            Console.WriteLine(message);
        }
    }
}
