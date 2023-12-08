using CausalModel.Model;
using CausalModel.Factors;
using CausalModel.Fixation;
using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.CausesExpressionTree
{
    /// <summary>
    /// Элемент логического выражения, который вычисляется на основе причинного ребра
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

        public override bool? Evaluate<TNodeValue>(IFactProvider<TNodeValue> factProvider,
            IFixatedProvider happenedProvider, IRandomProvider fixingValueProvider)
        {
            bool probabilityHappened = ProbabilityFactor.IsHappened(Edge.Probability,
                (float)fixingValueProvider.NextDouble());
            
            bool isExistingCauseHappened = false;
            if (Edge.CauseId != null)
            {
                bool? isHappened = happenedProvider.IsFixated(Edge.CauseId);
                // Если причина есть, но не зафиксирована
                if (isHappened == null)
                {
                    return null;
                }
                isExistingCauseHappened = isHappened.Value;
            }

            // Если причины нет, значит, достаточно лишь выполнения самого фактора
            // на основе вероятности
            return probabilityHappened &&
                (Edge.CauseId == null || isExistingCauseHappened);
        }
    }
}
