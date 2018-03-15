using CustomHeroCreator.AI;
using CustomHeroCreator.CLI;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator.Fighters
{
    class DeterministicScrub : Hero
    {
        public double StatGain { get; set; } = 1.5;

        /// <summary>
        /// Create a scrub that always stronger in the same deterministic manner when it levels up
        /// </summary>
        public DeterministicScrub() : base()
        {
            // dummy AI
            this.AI = new Agent();
        }
        ///// <summary>
        ///// A scrub gets weeker options
        ///// </summary>
        ///// <returns></returns>
        //internal override Dictionary<StatTypes, double> GenerateRandomSkills()
        //{
        //    var skills = new Dictionary<StatTypes, double>();

        //    Random rnd = new Random();

        //    foreach (StatTypes stat in Enum.GetValues(typeof(StatTypes)))
        //    {
        //        double value = 0;
        //        switch (stat)
        //        {
        //            case StatTypes.Str:
        //            case StatTypes.Agi:
        //            case StatTypes.Int:
        //            case StatTypes.MaxHealth:
        //            case StatTypes.AttackDmg:
        //            case StatTypes.AttackSpeed:
        //                value = 1;
        //                break;
        //            case StatTypes.CritChance:
        //                // only allow for 1-5% increases
        //                value = rnd.Next(1, 5) * 0.001;
        //                break;
        //            case StatTypes.CritMultiplier:
        //                // only allow for 1-5% increases
        //                value = rnd.Next(1, 5) * 0.001;
        //                break;
        //            case StatTypes.Armor:
        //                value = 1;
        //                break;
        //            default:
        //                break;
        //        }

        //        skills.Add(stat, value);
        //    }

        //    return skills;
        //}

        /// <summary>
        /// The scrub always gets stronger in the same deterministic manner
        /// </summary>
        public override void LevelUp()
        {
            foreach (StatTypes statType in ALL_STAT_TYPES)
            {
                if (statType == StatTypes.CritChance || statType == StatTypes.CritMultiplier)
                {
                    // no extra crit at all for now
                    //SetStatValue(statType, 0.001);
                }
                else
                {
                    SetStatValue(statType, StatGain);
                }
            }

            Level++;
        }


    }
}
