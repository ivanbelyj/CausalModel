using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
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
        public void IsRootCauseTest()
        {
            var rootNode = new Fact<string>()
            {
                CausesExpression = TestUtils.NewCausesExpression(),
                NodeValue = "root"
            };
            var notRootNode = new Fact<string>()
            {
                CausesExpression = TestUtils.NewNotRootCausesExpression(),
                NodeValue = "not root"
            };

            Assert.True(rootNode.IsRootCause());
            Assert.False(notRootNode.IsRootCause());
        }

        [Fact]
        public void GetCausesTest()
        {
            const int TEST_SIZE = 5;

            var edges = new ProbabilityFactor[TEST_SIZE];
            for (int i = 0; i < TEST_SIZE; i++)
                edges[i] = TestUtils.NewTrueFactor();

            var or = Expressions.Or(edges);
            var and = Expressions.And(edges);
            var not = Expressions.Not(TestUtils.NewFalseFactor());

            var expr = Expressions.Or(or, and, not);

            Fact<string> node = new Fact<string>()
            {
                CausesExpression = expr, NodeValue = "root"
            };
            Assert.Equal(TEST_SIZE * 2 + 1, node.GetCauses().Count());
        }
    }
}
