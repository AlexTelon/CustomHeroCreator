using CustomHeroCreator.CLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomHeroCreator
{
    class Hero
    {
        public static readonly ConsoleColor DEFAULT_TEXT_COLOR = ConsoleColor.Gray;
        private static int SkillOptionsPerLevelUp => Enum.GetNames(typeof(StatTypes)).Count();


        public string Name { get; set; }

        public uint Level { get; private set; } = 1;
        public bool IsActive { get; internal set; } = true;

        // Some stats for the Hero
        public string Stats => "HitPoints: " + Hp + " Strength: " + Str + " Agility: " + Agi + " Intelligence: " + Int;
        public int Str { get; private set; } = 1;
        public int Agi { get; private set; } = 1;
        public int Int { get; private set; } = 1;

        public int Hp { get; set; } = 100;

        public double Attack { get; set; } = 10;

        public double AttackSpeed
        {
            get => _attacksPerSecond;
            set => _attacksPerSecond = value;
        }
        private double _attacksPerSecond = 1;

        public double Armor { get; set; }


        public enum StatTypes
        {
            Str, Agi, Int, Hp, Attack, AttackSpeed, Armor
        };


        /// <summary>
        /// How powerful is this hero?
        /// </summary>
        public double Fitness
        {
            get
            {
                // start with a Fitness that clearly rewards Hp over the other attributes
                var power = Hp * 10 + Str + Agi + Int;

                // normalize the results so the Fitness is the average power increase per level
                return power / Level;
            }
        }


        /// <summary>
        /// If this hero has an AI or if it is to be controlled by user
        /// </summary>
        public bool HasAI => AI != null;

        public AI AI { get; set; }



        /// <summary>
        /// Level up the user (and give the user the option to choose new skills
        /// </summary>
        public void LevelUp()
        {
            if (!HasAI)
            {
                Console.Clear();
                Console.WriteLine("========================");
                PrintStats();
            }

            ChooseNewSkill();

            if (!HasAI)
            {
                Console.WriteLine("========================");
            }

            Level++;
        }

        private Dictionary<StatTypes, int> GenerateRandomSkills()
        {
            var skills = new Dictionary<StatTypes, int>();

            Random rnd = new Random();

            foreach (StatTypes stat in Enum.GetValues(typeof(StatTypes)))
            {
                skills.Add(stat, rnd.Next(1, 5));
            }

            return skills;
        }



        private void ChooseNewSkill()
        {
            var skillOptions = GenerateRandomSkills();

            var hasChoosen = false;

            while (!hasChoosen)
            {
                var i = 1;

                if (!HasAI)
                {
                    Console.WriteLine("Choose one of the following or press Q to abort");

                    Console.WriteLine();
                    foreach (var skill in skillOptions)
                    {
                        CommandLineTools.PrintWithColor("[" + i++ + "]: " + skill.Key, ConsoleColor.White);
                        CommandLineTools.PrintWithColor(" +" + skill.Value + "    ", StatToColor(skill.Key));
                    }
                    Console.WriteLine();
                }

                // get input from user or from AI
                string input = ChooseOption(skillOptions);

                // abort
                if (input == "q" || input == "Q")
                {
                    IsActive = false;
                    break;
                }

                // user supplies a 1 indexed number
                int option = int.Parse(input) - 1;

                if (option >= SkillOptionsPerLevelUp)
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

                if (!HasAI)
                {
                    Console.WriteLine();
                }

            }
        }

        private string ChooseOption(Dictionary<StatTypes, int> options)
        {
            if (HasAI)
            {
                return AI.ChooseOption(this, options);
            }
            else
            {
                //controlled by user
                return Console.ReadLine();
            }
        }




        internal void PrintFitness()
        {
            CommandLineTools.PrintWithColor("Fitness: ", ConsoleColor.White);
            CommandLineTools.PrintWithColor("" + this.Fitness, ConsoleColor.Yellow);
            Console.WriteLine();
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
            Console.WriteLine();

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

        public override string ToString()
        {
            return "Name: " + Name + " Level: " + Level + " Fitness: " + Fitness + " Stats: " + Stats;
        }
    }
}
