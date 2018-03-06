using System;
using System.Collections.Generic;

namespace CustomHeroCreator.AI
{
    /// <summary>
    /// A weight determines the likelyhood that an option/action is choosen/done
    /// However since we dont want simple scalar weights only but the possiblity for any polynomial (and maybe other functions) weight functions a Weight class is provided
    /// </summary>
    class Weight
    {
        /// <summary>
        /// The constants the polynomial, constants not added here are considered 0
        /// So the number of constants determine the degree of the polynomial
        /// </summary>
        public List<double> Constants { get; set; } = new List<double>();


        /// <summary>
        /// Supply x to the polynomial
        /// might want to provide a cached or a lookup table for this method?
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        internal double GetScore(double x)
        {
            double result = 0;
            var power = 0;
            foreach (var constant in Constants)
            {
                result += constant * Math.Pow(x, power);
                power++;
            }

            return result;
        }

        /// <summary>
        /// Ugly ToString but it will do
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var strings = new List<string>();
            foreach (var constant in Constants)
            {
                strings.Add(constant.ToString("0.00"));
            }
            return String.Join(", ", strings);
        }

    }
}
