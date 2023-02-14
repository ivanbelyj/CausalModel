﻿using CausalModel.Edges;
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
        protected bool EvaluateNecessary()
        {
            if (Edge.FixingValue is null)
                throw new InvalidOperationException("Фиксирующее значение не установлено");

            return Edge.IsHappened;
        }

        protected bool EvaluateSufficient()
        {
            if (Edge.FixingValue is null)
                throw new InvalidOperationException("Фиксирующее значение не установлено");

            var cause = (IHappenable?)Edge.Cause;

            // if (cause is not null && cause.IsHappened is null)
            //     throw new NullReferenceException("Не определено, произошла ли причина");

            // Исход ребер, не имеющих причин, зависит лишь от них самих
            bool condA = cause is null ||
                (/*cause.IsHappened is not null && */cause.IsHappened);
            //bool condB = ProbabilityEdge.IsActuallyHappened(Edge.Probability,
            //    Edge.FixingValue.Value);
            return condA; // && condB;
        }

        public override bool Evaluate() => EvaluateNecessary() && EvaluateSufficient();
    }
}
