using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator
{
    class Hero
    {
        public readonly uint SKILL_OPTIONS_PER_LVL_UP = 3;

        public string Name { get; set; }

        public uint Level { get; private set; } = 1;
        public bool IsActive { get; internal set; } = true;


        public string Stats => "Strength: " + Str + " Agility: " + Agi + " Intelligence: " + Int;

        // Some stats for the Hero

        public int Str { get; private set; } = 1;
        public int Agi { get; private set; } = 1;
        public int Int { get; private set; } = 1;




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


                var i = 1;
                foreach (var skill in skillOptions)
                {
                    Console.WriteLine("[" + i++ + "]: " + skill.Key + " +" + skill.Value);
                }


                var input = Console.ReadLine();

                // abort
                if (input == "q" || input == "Q")
                {
                    IsActive = false;
                    hasChoosen = true;
                    break;
                }

                int option = int.Parse(input);

                switch (option)
                {
                    case 1:
                        Str += skillOptions["str"];
                        hasChoosen = true;
                        break;
                    case 2:
                        Agi += skillOptions["agi"];
                        hasChoosen = true;
                        break;
                    case 3:
                        Int += skillOptions["int"];
                        hasChoosen = true;
                        break;

                    default:
                        hasChoosen = false;
                        break;
                }

                if (!hasChoosen)
                {
                    Console.WriteLine();
                    Console.WriteLine("Please choose a valid option");
                }
                Console.WriteLine();
            }
        }

        private Dictionary<string, int> GenerateRandomSkills()
        {
            var skills = new Dictionary<string, int>();

            Random rnd = new Random();

            skills.Add("str", rnd.Next(1, 5));
            skills.Add("agi", rnd.Next(1, 5));
            skills.Add("int", rnd.Next(1, 5));

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
