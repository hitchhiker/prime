using System;
using System.Collections.Generic;
using System.Linq;

namespace Prime.Core
{
    public class Rnd
    {
        private Rnd() { }

        public static Rnd I { get { return Lazy.Value; } }
        private static readonly Lazy<Rnd> Lazy = new Lazy<Rnd>(() => new Rnd());

        /// <summary>
        /// Returns a random number between 0 and max. This will NOT return the max value.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public int Next(int max) { return Next(0, max); }

        /// <summary>
        /// Helper method, just rolls a virtual 100 sided dice. 'True' you won, 'False' you lost.
        /// </summary>
        /// <param name="percentageWin">The percentage of rolls you'd like to win.</param>
        /// <returns></returns>
        public bool DiceRoll(int percentageWin)
        {
            return Next(0, 99) < percentageWin;
        }

        /// <summary>
        /// Returns a random number between and including min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public int Next(int min, int max)
        {
            return min == max ? min : BaseRandom.Next(min, max);
        }

        public string Next(params string[] pars)
        {
            if (pars == null || pars.Length == 0)
                return null;

            return pars[Next(0, pars.Length - 1)];
        }

        /// <summary>
        /// http://stackoverflow.com/a/12388092/1318333
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public decimal Next(decimal from, decimal to)
        {
            var fromScale = 20;
            var toScale = 20;

            var scale = (byte)(fromScale + toScale);
            if (scale > 28)
                scale = 28;

            var r = new decimal(BaseRandom.Next(), BaseRandom.Next(), BaseRandom.Next(), false, scale);
            if (Math.Sign(from) == Math.Sign(to) || from == 0 || to == 0)
                return decimal.Remainder(r, to - from) + from;

            var getFromNegativeRange = (double)from + BaseRandom.NextDouble() * ((double)to - (double)from) < 0;
            return getFromNegativeRange ? decimal.Remainder(r, -from) + from : decimal.Remainder(r, to);
        }

        public T PickOne<T>(IEnumerable<T> objects)
        {
            return objects.OrderByRandom().FirstOrDefault();
        }

        public double Next(double min, double max)
        {
            return BaseRandom.NextDouble() * (max - min) + min;
        }

        public decimal Money(decimal min, decimal max)
        {
            return min == max ? min : (BaseRandom.Next((int)(min * 100), (int)(max * 100)) / (decimal)100);
        }

        private Random _baseRandom;
        public Random BaseRandom
        {
            get { return _baseRandom ?? (_baseRandom = new System.Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber))); }
        }

        /// <summary>
        /// Attempts to get a random value from the given Enum type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Enum<T>() where T : struct, IConvertible
        {
            return EnumExtensionMethods.RndEnum<T>();
        }
    }
}