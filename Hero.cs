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
        public string Stats => "MaxHealth: " + MaxHealth + " Strength: " + Str + " Agility: " + Agi + " Intelligence: " + Int;
        public int Str { get; private set; } = 1;
        public int Agi { get; private set; } = 1;
        public int Int { get; private set; } = 1;

        public double MaxHealth { get; set; } = 100;

        public double CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                _currentHealth = value;

                if (value <= 0)
                {
                    IsAlive = false;
                    _currentHealth = 0;
                }

                if (value > MaxHealth)
                {
                    _currentHealth = MaxHealth;
                }

            }
        }
        private double _currentHealth = 100;

        public bool IsAlive { get; private set; } = true;


        public double AttackDmg { get; set; } = 10;
        public double CritChance { get; set; } = 0.1;
        public double CritMultiplier { get; set; } = 1.1;

        public double AttackSpeed
        {
            get => _attacksPerSecond;
            set => _attacksPerSecond = value;
        }
        private double _attacksPerSecond = 1;

        public double Armor { get; set; }


        public enum StatTypes
        {
            Str, Agi, Int, MaxHealth, AttackDmg, AttackSpeed, Armor
        };


        public double GetHitDmg()
        {
            double dmg = AttackDmg;

            var rnd = new Random();
            if (rnd.NextDouble() > CritChance)
            {
                dmg *= CritMultiplier;
            }
            return dmg;
        }

        public void TakeHitDmg(double dmg)
        {
            var dmgTaken = (dmg - Armor);
            // dmg is always at least 1
            if (dmgTaken <= 0)
            {
                dmgTaken = 1;
            }

            this.CurrentHealth -= dmgTaken;
        }


        public void Attack(Hero enemy)
        {
            var dmg = GetHitDmg();
            enemy.TakeHitDmg(dmg);
        }


        /// <summary>
        /// How powerful is this hero?
        /// </summary>
        public double Fitness { get; set; } = 0;


        /// <summary>
        /// If this hero has an AI or if it is to be controlled by user
        /// </summary>
        public bool HasAI => AI != null;

        public AI AI { get; set; }


        /// <summary>
        /// Restores a hero back to prime condition
        /// </summary>
        public void Restore()
        {
            CurrentHealth = MaxHealth;
            IsAlive = true;
        }

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
                    case StatTypes.MaxHealth:
                        MaxHealth += value;
                        break;
                    case StatTypes.AttackDmg:
                        AttackDmg += value;
                        break;
                    case StatTypes.AttackSpeed:
                        AttackSpeed += value;
                        break;
                    case StatTypes.Armor:
                        Armor += value;
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

            foreach (StatTypes stat in Enum.GetValues(typeof(StatTypes)))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" " + Enum.GetName(typeof(StatTypes), stat) + ": ");
                Console.ForegroundColor = StatToColor(stat);
                Console.Write(GetStatValue(stat));
            }

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
                case StatTypes.MaxHealth:
                    return ConsoleColor.Magenta;
                default:
                    return DEFAULT_TEXT_COLOR;
            }
        }

        public double GetStatValue(StatTypes type)
        {
            switch (type)
            {
                case StatTypes.Str:
                    return Str;
                case StatTypes.Agi:
                    return Agi;
                case StatTypes.Int:
                    return Int;
                case StatTypes.MaxHealth:
                    return MaxHealth;
                case StatTypes.AttackDmg:
                    return AttackDmg;
                case StatTypes.AttackSpeed:
                    return AttackSpeed;
                case StatTypes.Armor:
                    return Armor;
                default:
                    return -1;
            }
        }


        public override string ToString()
        {
            var result = "";

            result += "Fitness: " + Fitness + " ";

            foreach (StatTypes stat in Enum.GetValues(typeof(StatTypes)))
            {
                result += Enum.GetName(typeof(StatTypes), stat) + ":" + GetStatValue(stat) + " ";
            }
            return result;
        }
    }
}
