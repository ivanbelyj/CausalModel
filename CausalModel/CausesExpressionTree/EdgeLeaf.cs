using CausalModel.Edges;
using CausalModel.Nodes;
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
    public class EdgeLeaf : CausesExpression
    {
        public ProbabilityEdge Edge { get; set; }

        public override IEnumerable<ProbabilityEdge> GetEdges()
            => new List<ProbabilityEdge>() { Edge };

        public EdgeLeaf(ProbabilityEdge edge)
        {
            Edge = edge;
        }

        public override bool Evaluate<TNodeValue>(IFactProvider<TNodeValue> factProvider,
            IHappenedProvider happenedProvider, IFixingValueProvider fixingValueProvider)
        {
            bool probabilityHappened = ProbabilityEdge.IsHappened(Edge.Probability,
                fixingValueProvider.GetFixingValue());
            // Если причины нет, значит достаточно лишь выполнения самого фактора
            // на основе вероятности
            bool isCauseHappened = Edge.CauseId == null
                || happenedProvider.IsHappened(Edge.CauseId.Value);
            return probabilityHappened && isCauseHappened;
        }
    }
}
