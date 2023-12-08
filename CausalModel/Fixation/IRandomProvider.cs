using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation
{
    public interface IRandomProvider
    {
        /// <summary>
        /// Next double in [min, max)
        /// </summary>
        public double NextDouble(double min = 0, double max = 1);
    }
}
