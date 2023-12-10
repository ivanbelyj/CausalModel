using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation
{
    /// <summary>
    /// A component generating random values for causal model fixation purposes
    /// </summary>
    public interface IRandomProvider
    {
        /// <summary>
        /// Next random float from min (inclusive) and max (exclusive)
        /// </summary>
        public float NextDouble(float min = 0, float max = 1);
    }
}
