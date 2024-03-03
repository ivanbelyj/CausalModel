using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Common
{
    internal static class RandomExtensions
    {
        /// <summary>
        /// Generates a random double from min (inclusive) to max (exclusive)
        /// </summary>
        public static double NextDouble(this Random random, double min, double max)
            => (max - min) * random.NextDouble() + min;
    }
}
