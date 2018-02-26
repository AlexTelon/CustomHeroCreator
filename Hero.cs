using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomHeroCreator
{
    class Hero
    {
        public static readonly uint SKILL_OPTIONS_PER_LVL_UP = 3;
        public static readonly ConsoleColor DEFAULT_TEXT_COLOR = ConsoleColor.Gray;


        public string Name { get; set; }

        public uint Level { get; private set; } = 1;
        public bool IsActive { get; internal set; } = true;

        // Some stats for the Hero
        public string Stats => "HitPoints: " + Hp + " Strength: " + Str + " Agility: " + Agi + " Intelligence: " + Int;
        public int Str { get; private set; } = 1;
        public int Agi { get; private set; } = 1;
        public int Int { get; private set; } = 1;

        public int Hp { get; set; } = 100;

        public enum StatTypes
        {
            Str, Agi, Int, Hp
        };

        /// <summary>
        /// If this hero has an AI or if it is to be controlled by user
        /// </summary>
        public bool HasAI { get; set; } = false;


        /// <summary>
        /// Level up the user (and give the user the option to choose new skills
        /// </summary>
        public void LevelUp()
        {
            Console.Clear();
            Console.WriteLine("========================");
            PrintStats();
            ChooseNewSkill();
            Console.WriteLine("========================");

            Level++;
        }


        private void ChooseNewSkill()
        {
            Console.WriteLine();
            Console.WriteLine("Choose one of the following or press Q to abort");

            var skillOptions = GenerateRandomSkills();

            var hasChoosen = false;

            while (!hasChoosen)
            {
                var i = 1;

                Console.WriteLine();
                foreach (var skill in skillOptions)
                {
                    Console.Write("[" + i++ + "]: " + skill.Key);
                    PrintWithColor(" +" + skill.Value + "    ", StatToColor(skill.Key));
                }
                Console.WriteLine();

                // get input from user or from AI
                string input = GetInput();

                // abort
                if (input == "q" || input == "Q")
                {
                    IsActive = false;
                    break;
                }

                // user supplies a 1 indexed number
                int option = int.Parse(input) - 1;

                if (option >= SKILL_OPTIONS_PER_LVL_UP)
                {
                    // invalid option
                    Console.WriteLine();
                    Console.WriteLine("Please choose a valid option");
                    continue;
                }

                var type = skillOptions.Keys.ElementAt(option);
                var value = skillOptions[type];

                switch (type)
                {
                    case StatTypes.Str:
                        Str += value;
                        break;
                    case StatTypes.Agi:
                        Agi += value;
                        break;
                    case StatTypes.Int:
                        Int += value;
                        break;
                    case StatTypes.Hp:
                        Hp += value;
                        break;
                    default:
                        break;
                }

                hasChoosen = true;
                Console.WriteLine();
            }
        }


        private string GetInput()
        {
            if (HasAI)
            {
                return "1";
            } else
            {
                //controlled by user
                return Console.ReadLine();
            }
        }

        private Dictionary<StatTypes, int> GenerateRandomSkills()
        {
            var skills = new Dictionary<StatTypes, int>();

            Random rnd = new Random();

            skills.Add(StatTypes.Str, rnd.Next(1, 5));
            skills.Add(StatTypes.Agi, rnd.Next(1, 5));
            skills.Add(StatTypes.Int, rnd.Next(1, 5));

            //every now and again remove one of the standard stat options and add health instead
            if (rnd.Next(1, 5) == 4)
            {
                skills.Remove(StatTypes.Str);
                skills.Add(StatTypes.Hp, rnd.Next(1, 10));
            }

            return skills;
        }

        public void PrintStats()
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("HitPoints: ");
            Console.ForegroundColor = StatToColor(StatTypes.Hp);
            Console.Write(Hp);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" Strength: ");
            Console.ForegroundColor = StatToColor(StatTypes.Str);
            Console.Write(Str);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" Agility: ");
            Console.ForegroundColor = StatToColor(StatTypes.Agi);
            Console.Write(Agi);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" Intelligence: ");
            Console.ForegroundColor = StatToColor(StatTypes.Int);
            Console.Write(Int);

            Console.ForegroundColor = originalColor;
        }

        public static ConsoleColor StatToColor(StatTypes type)
        {
            switch (type)
            {
                case StatTypes.Str:
                    return ConsoleColor.Red;
                case StatTypes.Agi:
                    return ConsoleColor.Green;
                case StatTypes.Int:
                    return ConsoleColor.Cyan;
                case StatTypes.Hp:
                    return ConsoleColor.Magenta;
                default:
                    return DEFAULT_TEXT_COLOR;
            }
        }


        public static void PrintWithColor(string message, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.Write(message);

            Console.ForegroundColor = originalColor;
        }


        public override string ToString()
        {
            return "Name: " + Name + " Level: " + Level + " Stats: " + Stats;
        }

    }
}
