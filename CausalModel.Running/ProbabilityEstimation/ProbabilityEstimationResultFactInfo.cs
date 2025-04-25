using CausalModel.Running.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Running.ProbabilityEstimation
{
    public class ProbabilityEstimationResultFactInfo<TFactValue> : FactInfo<TFactValue>
        where TFactValue : class
    {
        public float EstimatedProbability { get; set; }

        public ProbabilityEstimationResultFactInfo(string factLocalId, TFactValue? factValue) : base(factLocalId, factValue)
        {
        }
    }
}
