using CausalModel.CausesExpressionTree;
using CausalModel.Edges;
using CausalModel.Nests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Tests
{
    public static class TestUtils
    {
        public static ProbabilityEdge NewFalseEdge() => new ProbabilityEdge(0, null, 0.5f);
        public static ProbabilityEdge NewTrueEdge() => new ProbabilityEdge(1, null, 0.5f);

        // public static ProbabilityEdge NewRootEdge() => new ProbabilityEdge(1, null, 0.5);
        public static ProbabilityEdge NewNotRootEdge()
        {
            var rootNode = NodeUtils.CreateNode(1, "root node", null);
            return new ProbabilityEdge(1, rootNode.Id, 0.5f);
        }

        public static ProbabilityNest NewRootNest()
        {
            var expression = Expressions.Or(new ProbabilityEdge(1, null),
                new ProbabilityEdge(1, null));
            return new ProbabilityNest(expression);
        }

        public static ProbabilityNest NewNotRootNest()
        {
            var rootNode = NodeUtils.CreateNode(1, "root", null);

            var notRootEdge = new ProbabilityEdge(1, rootNode.Id);

            var expression1 = Expressions.Or(notRootEdge, new ProbabilityEdge(1, null));
            return new ProbabilityNest(expression1);
        }
    }
}
