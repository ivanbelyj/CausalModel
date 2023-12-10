using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Factors
{
    /// <summary>
    /// A weighted edge makes a certain implementation of an abstract entity
    /// more favorable for selection
    /// </summary>
    public class WeightFactor : Factor
    {
        /// <summary>
        /// The weight that determines the favorability of a certain implementation
        /// of the abstract entity for selection.
        /// The value is greater than or equal to 0, 
        /// set relative to the weights of other implementations.
        /// At 0, the weighted edge is not considered,
        /// and the behavior is not defined for negative values
        /// </summary>
        public float Weight { get; set; }

        public WeightFactor(float weight, string? causeId = null)
        {
            Weight = weight;
            CauseId = causeId;
        }
    }
}
