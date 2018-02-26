using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomHeroCreator
{
    class Hero
    {
        public readonly uint SKILL_OPTIONS_PER_LVL_UP = 3;

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
        /// Level up the user (and give the user the option to choose new skills
        /// </summary>
        public void LevelUp()
        {
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
                var i = 0;
                foreach (var skill in skillOptions)
                {
                    Console.WriteLine("[" + i++ + "]: " + skill.Key + " +" + skill.Value);
                }

                var input = Console.ReadLine();

                // abort
                if (input == "q" || input == "Q")
                {
                    IsActive = false;
                    break;
                }

                int option = int.Parse(input);

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

        private Dictionary<StatTypes, int> GenerateRandomSkills()
        {
            var skills = new Dictionary<StatTypes, int>();

            Random rnd = new Random();

            skills.Add(StatTypes.Str, rnd.Next(1, 5));
            skills.Add(StatTypes.Agi, rnd.Next(1, 5));
            skills.Add(StatTypes.Int, rnd.Next(1, 5));

            //every now and again remove one of the standard stat options and add health instead
            if (rnd.Next(1,5) == 4)
            {
                skills.Remove(StatTypes.Str);
                skills.Add(StatTypes.Hp, rnd.Next(1, 10));
            }

            return skills;
        }

        public void PrintStats()
        {
            Console.WriteLine();
            Console.WriteLine(this.ToString());
            Console.WriteLine();
        }

        public override string ToString()
        {
            return "Name: " + Name + " Level: " + Level + " Stats: " + Stats;
        }

    }
}
