using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
using CausalModel.Nests;
using CausalModel.Facts;
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
            var rootNode = new ProbabilityFact<string>(TestUtils.NewRootNest(), "root");
            var notRootNode = new ProbabilityFact<string>(TestUtils.NewNotRootNest(), "not root");

            Assert.True(rootNode.IsRootNode());
            Assert.False(notRootNode.IsRootNode());
        }

        [Fact]
        public void GetEdgesTest()
        {
            const int TEST_SIZE = 5;

            var edges = new ProbabilityFactor[TEST_SIZE];
            for (int i = 0; i < TEST_SIZE; i++)
                edges[i] = TestUtils.NewTrueFactor();

            var or = Expressions.Or(edges);
            var and = Expressions.And(edges);
            var not = Expressions.Not(TestUtils.NewFalseFactor());

            var expr = Expressions.Or(or, and, not);

            Fact<string> node = new ProbabilityFact<string>(new ProbabilityNest(expr),
                "root");
            Assert.Equal(TEST_SIZE * 2 + 1, node.GetCauses().Count());
        }
    }
}
