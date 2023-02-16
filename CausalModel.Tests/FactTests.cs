using CausalModel.CausesExpressionTree;
using CausalModel.Edges;
using CausalModel.Nests;
using CausalModel.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CausalModel.Tests
{
    public class FactTests
    {
        [Fact]
        public void IsRootNodeTest()
        {
            var rootNode = new Fact<string>(TestUtils.NewRootNest(), "root");
            var notRootNode = new Fact<string>(TestUtils.NewNotRootNest(), "not root");

            Assert.True(rootNode.IsRootNode());
            Assert.False(notRootNode.IsRootNode());
        }

        [Fact]
        public void GetEdgesTest()
        {
            const int TEST_SIZE = 5;

            var edges = new ProbabilityEdge[TEST_SIZE];
            for (int i = 0; i < TEST_SIZE; i++)
                edges[i] = TestUtils.NewTrueEdge();

            var or = Expressions.Or(edges);
            var and = Expressions.And(edges);
            var not = Expressions.Not(TestUtils.NewFalseEdge());

            var expr = Expressions.Or(or, and, not);

            Fact<string> node = new Fact<string>(new ProbabilityNest(expr),
                "root");
            Assert.Equal(TEST_SIZE * 2 + 1, node.GetEdges().Count());
        }
    }
}
