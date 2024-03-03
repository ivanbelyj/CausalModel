using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Factors
{
    /// <summary>
    /// Represents a causal-consequential edge that indicates with what probability
    /// one of the consequence factors occurs in the case of the occurrence
    /// of the cause.
    /// </summary>
    public class ProbabilityFactor : Factor
    {
        private float probability;
        
        /// <summary>
        /// The probability that the causal-consequential relationship will lead to
        /// an event factor. The value ranges from 0 (the factor never occurs)
        /// to 1.0 (the factor always occurs) inclusively
        /// </summary>
        public float Probability
        {
            get => probability;
            set
            {
                if (value >= 0 && value <= 1.0)
                {
                    probability = value;
                }
                else
                    throw new ArgumentOutOfRangeException(nameof(Probability),
                        "Incorrect probability value. It must be in [0, 1]");
            }
        }

        public ProbabilityFactor(float probability, string? causeId = null)
        {
            Probability = probability;
            CauseId = causeId;
        }

        public override string ToString()
        {
            string str = $"(probability: {Probability}, "
                + $"causeId: {CauseId})";
            return str;
        }

        /// <summary>
        /// Function that determines whether the causal-consequential relationship
        /// lead to an event factor with given probability and fixing value.
        /// The probability value prevails over the fixing value
        /// (if the probability is defined as 0 or 1, any fixing value cannot
        /// affect the outcome of the factor)
        /// </summary>
        public static bool IsHappened(float probability, float fixingValue)
            => probability - fixingValue > 0;
    }
}
