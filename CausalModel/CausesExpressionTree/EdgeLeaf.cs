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

        public override bool Evaluate(float fixingValue, bool isCauseHappened)
        {
            return ProbabilityEdge.IsHappened(Edge.Probability, fixingValue)
                && (Edge.CauseId is null || isCauseHappened);
        }
    }
}
