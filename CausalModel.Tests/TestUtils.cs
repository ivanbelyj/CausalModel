using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
using CausalModel.Nests;
using CausalModel.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Tests
{
    public static class TestUtils
    {
        public static ProbabilityFactor NewFalseFactor() => new ProbabilityFactor(0, null);
        public static ProbabilityFactor NewTrueFactor() => new ProbabilityFactor(1, null);
        public static ProbabilityFactor NewNullFactor()
        {
            var notFixedNode = FactUtils.CreateNode(1,
                "Пока не известно, произошло, или нет", null);
            // Причина неопределена (ее нет в factCollection),
            // поэтому операции будут работать с троичной логикой и иногда выдавать
            // null
            var nullFactor = new ProbabilityFactor(1, notFixedNode.Id);
            return nullFactor;
        }

        // public static ProbabilityEdge NewRootEdge() => new ProbabilityEdge(1, null, 0.5);
        public static ProbabilityFactor NewNotRootEdge()
        {
            var rootNode = FactUtils.CreateNode(1, "root node", null);
            return new ProbabilityFactor(1, rootNode.Id);
        }

        public static ProbabilityNest NewRootNest()
        {
            var expression = Expressions.Or(new ProbabilityFactor(1, null),
                new ProbabilityFactor(1, null));
            return new ProbabilityNest(expression);
        }

        public static ProbabilityNest NewNotRootNest()
        {
            var rootNode = FactUtils.CreateNode(1, "root", null);

            var notRootEdge = new ProbabilityFactor(1, rootNode.Id);

            var expression1 = Expressions.Or(notRootEdge, new ProbabilityFactor(1, null));
            return new ProbabilityNest(expression1);
        }
    }
}
