﻿using CustomHeroCreator.AI;
using CustomHeroCreator.CLI;
using CustomHeroCreator.Generators;
using CustomHeroCreator.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using CustomHeroCreator.Repository;

namespace CustomHeroCreator.Enteties
{
    public class Hero : IEntity
    {
        private static readonly ConsoleColor DEFAULT_TEXT_COLOR = ConsoleColor.Gray;
        private static int SkillOptionsPerLevelUp => Enum.GetNames(typeof(StatTypes)).Count();

        protected static readonly Array ALL_STAT_TYPES = Enum.GetValues(typeof(StatTypes));


        public uint Level { get; protected set; } = 1;
        public bool IsActive { get; internal set; } = true;

        public StatGain StatGain { get; set; } = new StatGain();

        public double MaxHealth { get; set; } = 100;

        public double CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                _currentHealth = value;

                if (value <= 0)
                {
                    _currentHealth = 0;
                }

                if (value > MaxHealth)
                {
                    _currentHealth = MaxHealth;
                }

            }
        }
        private double _currentHealth = 100;

        public bool IsAlive => CurrentHealth > 0;

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

        public Hero()
        {
        }

        public enum StatTypes
        {
            Str, Agi, Int, MaxHealth, AttackDmg, CritChance, CritMultiplier, Armor
        };
        //Str, Agi, Int, MaxHealth, AttackDmg, AttackSpeed, CritChance, CritMultiplier, Armor

        public double GetHitDmg()
        {
            double dmg = AttackDmg;

            if (DataHub.Instance.RandomSource.NextDouble() < CritChance)
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

        public IAgent AI { get; set; }

        public SkillTreeGenerator SkillTreeGenerator { get; internal set; }

        /// <summary>
        /// Restores a hero back to prime condition
        /// </summary>
        public void Restore()
        {
            CurrentHealth = MaxHealth;
        }

        /// <summary>
        /// Level up the user (and give the user the option to choose new skills
        /// </summary>
        public virtual void LevelUp()
        {
            ChooseNewSkill();
            Level++;

            // some simple rules for how stat gain works
            MaxHealth += StatGain.Str;
            AttackDmg += StatGain.Agi;
            CritChance += StatGain.Int;
        }

        internal virtual StatNode GenerateRandomSkills()
        {
            var rnd = DataHub.Instance.RandomSource;
            var rootNode = new StatNode();

            //var skills = new Dictionary<StatTypes, double>();
            foreach (StatTypes stat in ALL_STAT_TYPES)
            {
                var child = new StatNode();
                child.Stat = stat;
                switch (stat)
                {
                    case StatTypes.Str:
                    case StatTypes.Agi:
                    case StatTypes.Int:
                        child.Value = rnd.Next(1, 5);
                        break;
                    case StatTypes.MaxHealth:
                        child.Value = rnd.Next(1, 5);
                        break;
                    case StatTypes.AttackDmg:
                        child.Value = rnd.Next(1, 5);
                        break;
                    //case StatTypes.AttackSpeed:
                    //    child.Value = rnd.Next(1, 5);
                    //    break;
                    case StatTypes.CritChance:
                        // only allow for 1-5% increases
                        child.Value = rnd.Next(1, 20) * 0.01;
                        break;
                    case StatTypes.CritMultiplier:
                        // only allow for 1-5% increases
                        child.Value = rnd.Next(1, 20) * 0.01;
                        break;
                    case StatTypes.Armor:
                        child.Value = rnd.Next(1, 5);
                        break;
                    default:
                        break;
                }
                rootNode.Children.Add(child);
            }

            return rootNode;
        }

        internal Hero BreedWith(Hero partner, double mutationRate, double mutationChange)
        {
            var child = new Hero();
            child.SkillTreeGenerator = SkillTreeGenerator;

            // really the AI is the one we are breeding
            IAgent a = this.AI;
            IAgent b = partner.AI;

            // merge the two with a chance for mutation
            IAgent c = a.BreedWith(b, mutationRate, mutationChange);
            child.AI = c;

            return child;
        }

        private void ChooseNewSkill()
        {
            StatNode options;

            if (SkillTreeGenerator == null)
            {
                options = GenerateRandomSkills();
            }
            else
            {
                options = SkillTreeGenerator.GenerateUniqueSkillOptions();
            }

            ChoooseNewSkillFromTree(options);
        }

        private void ChoooseNewSkillFromTree(StatNode statNode)
        {
            var console = DataHub.Instance.ConsoleWrapper;

            var hasChoosen = false;

            while (!hasChoosen)
            {
                var i = 1;

                if (!HasAI)
                {
                    console.WriteLine();
                    console.WriteLine();
                    console.Write("Current Level: ");
                    CommandLineTools.PrintWithColor("" + Level, ConsoleColor.Green);
                    console.WriteLine();
                    console.WriteLine("You:");
                    this.PrintStats();
                    console.WriteLine();
                    console.WriteLine("Choose one of the following or press Q to abort");

                    console.WriteLine();
                    foreach (var node in statNode.Children.OrderBy(x => (int)x.Stat))
                    {
                        CommandLineTools.PrintWithColor("[" + i++ + "]: " + node.Stat, ConsoleColor.White);

                        CommandLineTools.PrintWithColor(" " + GetStatValue(node.Stat).ToString("0.#"), ConsoleColor.Gray);

                        CommandLineTools.PrintWithColor(" + " + node.Value.ToString("0.#") + "    ", StatToColor(node.Stat));
                        console.WriteLine();
                    }
                    console.WriteLine();
                }
                int option = ChooseOption(statNode);

                if (option >= statNode.Children.Count())
                {
                    // invalid option
                    console.WriteLine();
                    console.WriteLine("Please choose a valid option");
                    continue;
                }

                //We have choosen an option, lets select it by letting it become the new current node!
                if (!HasAI)
                {
                    statNode = statNode.Children.OrderBy(x => x.Stat).ToList()[option];
                } else
                {
                    statNode = statNode.Children[option];
                }

                var type = statNode.Stat;
                var value = statNode.Value;

                SetStatValue(type, value);

                hasChoosen = true;

                if (!HasAI)
                {
                    console.WriteLine();
                }
            }

        }

        private int ChooseOption(StatNode node)
        {
            if (HasAI)
            {
                return AI.ChooseOption(this, node);
            }
            else
            {
                int option = -1;
                // ask for input until we get correct input
                while (true)
                {
                    string input = DataHub.Instance.ConsoleWrapper.ReadLine();

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

                //controlled by user
                return option;
            }
        }



        internal void PrintFitness()
        {
            CommandLineTools.PrintWithColor("Fitness: ", ConsoleColor.White);
            CommandLineTools.PrintWithColor("" + this.Fitness, ConsoleColor.Yellow);
            DataHub.Instance.ConsoleWrapper.WriteLine();
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
            foreach (StatTypes stat in typesToPrint)
            {
                CommandLineTools.PrintWithColor(delimiter + Enum.GetName(typeof(StatTypes), stat) + ": ", ConsoleColor.White);
                CommandLineTools.PrintWithColor(GetStatValue(stat).ToString("0.00"), StatToColor(stat));
            }
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
                //case StatTypes.AttackSpeed:
                //    return ConsoleColor.DarkGreen;
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
                    StatGain.Str += value;
                    break;
                case StatTypes.Agi:
                    StatGain.Agi += value;
                    break;
                case StatTypes.Int:
                    StatGain.Int += value;
                    break;
                case StatTypes.MaxHealth:
                    MaxHealth += value;
                    break;
                case StatTypes.AttackDmg:
                    AttackDmg += value;
                    break;
                //case StatTypes.AttackSpeed:
                //    AttackSpeed += value;
                //    break;
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
                    return StatGain.Str;
                case StatTypes.Agi:
                    return StatGain.Agi;
                case StatTypes.Int:
                    return StatGain.Int;
                case StatTypes.MaxHealth:
                    return MaxHealth;
                case StatTypes.AttackDmg:
                    return AttackDmg;
                //case StatTypes.AttackSpeed:
                //    return AttackSpeed
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
                result += Enum.GetName(typeof(StatTypes), stat) + ": " + GetStatValue(stat) + " ";
            }
            return result;
        }
    }
}
