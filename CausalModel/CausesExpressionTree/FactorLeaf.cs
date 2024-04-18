using CausalModel.Factors;
using CausalModel.Fixation;
using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CausalModel.Model;

namespace CausalModel.CausesExpressionTree
{
    /// <summary>
    /// An element of a logical expression that is calculated based on the factor
    /// </summary>
    public class FactorLeaf : CausesExpression
    {
        public ProbabilityFactor Edge { get; set; }

        public override IEnumerable<ProbabilityFactor> GetEdges()
            => new List<ProbabilityFactor>() { Edge };

        public FactorLeaf(ProbabilityFactor edge)
        {
            Edge = edge;
        }

        public override bool? Evaluate<TFactValue>(
            IModelProvider<TFactValue> factProvider,
            IFixatedProvider happenedProvider,
            IRandomProvider fixingValueProvider)
            where TFactValue : class
        {
            bool probabilityHappened = ProbabilityFactor.IsHappened(Edge.Probability,
                (float)fixingValueProvider.NextDouble());
            
            bool isExistingCauseHappened = false;
            if (Edge.CauseId != null)
            {
                var causeId = factProvider
                    .GetModelFact(Edge.CauseId)
                    .InstanceFactId;
                bool? isHappened = happenedProvider.IsFixated(causeId);
                // If there is a cause, but it is not fixated
                if (isHappened == null)
                {
                    return null;
                }
                isExistingCauseHappened = isHappened.Value;
            }

            // If there is no cause, then it is enough to just fulfill
            // the factor itself based on probability
            return probabilityHappened &&
                (Edge.CauseId == null || isExistingCauseHappened);
        }
    }
}
