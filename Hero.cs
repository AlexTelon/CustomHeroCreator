using CustomHeroCreator.AI;
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

        public uint Level { get; protected set; } = 1;
        public bool IsActive { get; internal set; } = true;

        // Some stats for the Hero
        public string Stats => "MaxHealth: " + MaxHealth + " Strength: " + Str + " Agility: " + Agi + " Intelligence: " + Int;
        public double Str { get; private set; } = 1;
        public double Agi { get; private set; } = 1;
        public double Int { get; private set; } = 1;

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
            Str, Agi, Int, MaxHealth, AttackDmg, AttackSpeed, CritChance, CritMultiplier, Armor
        };


        public double GetHitDmg()
        {
            double dmg = AttackDmg;

            var rnd = new Random();
            if (rnd.NextDouble() < CritChance)
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
        public bool IsPlayer => !HasAI;

        public Agent AI { get; set; }


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
        public virtual void LevelUp()
        {
            ChooseNewSkill();
            Level++;
        }

        internal virtual Dictionary<StatTypes, double> GenerateRandomSkills()
        {
            var skills = new Dictionary<StatTypes, double>();

            Random rnd = new Random();

            foreach (StatTypes stat in Enum.GetValues(typeof(StatTypes)))
            {
                double value = 0;
                switch (stat)
                {
                    case StatTypes.Str:
                    case StatTypes.Agi:
                    case StatTypes.Int:
                        value = rnd.Next(1, 5);
                        break;
                    case StatTypes.MaxHealth:
                        value = rnd.Next(1, 5);
                        break;
                    case StatTypes.AttackDmg:
                        value = rnd.Next(1, 5);
                        break;
                    case StatTypes.AttackSpeed:
                        value = rnd.Next(1, 5);
                        break;
                    case StatTypes.CritChance:
                        // only allow for 1-5% increases
                        value = rnd.Next(1, 5) * 0.01;
                        break;
                    case StatTypes.CritMultiplier:
                        // only allow for 1-5% increases
                        value = rnd.Next(1, 5) * 0.01;
                        break;
                    case StatTypes.Armor:
                        value = rnd.Next(1, 5);
                        break;
                    default:
                        break;
                }

                skills.Add(stat, value);
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
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Current Level: ");
                    CommandLineTools.PrintWithColor("" + Level, ConsoleColor.Green);
                    Console.WriteLine();
                    Console.WriteLine("Choose one of the following or press Q to abort");

                    Console.WriteLine();
                    foreach (var skill in skillOptions)
                    {
                        CommandLineTools.PrintWithColor("[" + i++ + "]: " + skill.Key, ConsoleColor.White);

                        CommandLineTools.PrintWithColor(" " + GetStatValue(skill.Key), ConsoleColor.Gray);

                        CommandLineTools.PrintWithColor(" + " + skill.Value + "    ", StatToColor(skill.Key));
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }


                int option = -1;

                // ask for input until we get correct input
                while (true)
                {
                    // get input from user or from AI
                    string input = ChooseOption(skillOptions);


                    // abort
                    if (input == "q" || input == "Q")
                    {
                        IsActive = false;
                        break;
                    }

                    if (int.TryParse(input, out int tmp))
                    {
                        // if we have correct input the exit the loop
                        // user supplies a 1 indexed number so we take 1
                        option = tmp - 1;
                        break;
                    }
                }

                if (option >= SkillOptionsPerLevelUp)
                {
                    // invalid option
                    Console.WriteLine();
                    Console.WriteLine("Please choose a valid option");
                    continue;
                }

                var type = skillOptions.Keys.ElementAt(option);
                var value = skillOptions[type];

                SetStatValue(type, value);

                hasChoosen = true;

                if (!HasAI)
                {
                    Console.WriteLine();
                }
            }
        }

      

        private string ChooseOption(Dictionary<StatTypes, double> options)
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
            // include all stats
            var listOfStats = Enum.GetValues(typeof(StatTypes)).OfType<StatTypes>().ToList();
            PrintStats(listOfStats, " ");
        }

        public void PrintStats(string delimiter)
        {
            // include all stats
            var listOfStats = Enum.GetValues(typeof(StatTypes)).OfType<StatTypes>().ToList();
            PrintStats(listOfStats, delimiter);
        }

        public void PrintStats(List<StatTypes> typesToPrint, string delimiter = " ")
        {
            var originalColor = Console.ForegroundColor;

            foreach (StatTypes stat in typesToPrint)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(delimiter + Enum.GetName(typeof(StatTypes), stat) + ": ");
                Console.ForegroundColor = StatToColor(stat);
                Console.Write(GetStatValue(stat));
            }

            Console.ForegroundColor = originalColor;
        }


        /// <summary>
        /// Alternative to PrintStats, you get all the info and can choose what to show and how
        /// </summary>
        /// <returns></returns>
        public List<string> GetStatsAsStrings()
        {
            var result = new List<string>();

            foreach (StatTypes stat in Enum.GetValues(typeof(StatTypes)))
            {
                var statString = "";
                statString += Enum.GetName(typeof(StatTypes), stat) + ": ";
                statString += GetStatValue(stat);
                result.Add(statString);
            }

            return result;
        }


        protected static ConsoleColor StatToColor(StatTypes type)
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
                case StatTypes.AttackDmg:
                    return ConsoleColor.DarkRed;
                case StatTypes.AttackSpeed:
                    return ConsoleColor.DarkGreen;
                case StatTypes.CritChance:
                    return ConsoleColor.Yellow;
                case StatTypes.CritMultiplier:
                    return ConsoleColor.DarkYellow;
                case StatTypes.Armor:
                    return ConsoleColor.Blue;
                default:
                    return DEFAULT_TEXT_COLOR;
            }
        }

        protected void SetStatValue(StatTypes type, double value)
        {
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
                case StatTypes.CritChance:
                    CritChance += value;
                    break;
                case StatTypes.CritMultiplier:
                    CritMultiplier += value;
                    break;
                case StatTypes.Armor:
                    Armor += value;
                    break;
                default:
                    break;
            }
        }

        protected double GetStatValue(StatTypes type)
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
                case StatTypes.CritChance:
                    return CritChance;
                case StatTypes.CritMultiplier:
                    return CritMultiplier;
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
            result += "Level: " + Level + " ";
            result += "CurrentHp: " + CurrentHealth + " ";

            foreach (StatTypes stat in Enum.GetValues(typeof(StatTypes)))
            {
                result += Enum.GetName(typeof(StatTypes), stat) + ":" + GetStatValue(stat) + " ";
            }
            return result;
        }
    }
}
