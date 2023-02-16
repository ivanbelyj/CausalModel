using CausalModel.CausesExpressionTree;
using CausalModel.Edges;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CausalModel.Tests
{
    public class CausesExpressionTreeTests
    {
        [Fact]
        public void EdgeLeafTest()
        {
            var falseEdge = TestUtils.NewFalseEdge();
            var trueEdge = TestUtils.NewTrueEdge();

            var factCol = new FactCollection<string>();
            var model = new CausalModel<string>(factCol, 123);

            Assert.False(new EdgeLeaf(falseEdge).Evaluate(factCol, model, model));
            Assert.True(new EdgeLeaf(trueEdge).Evaluate(factCol, model, model));
        }

        [Fact]
        public void ConjunctionOperationTest()
        {
            var falseEdge = TestUtils.NewFalseEdge();
            var falseEdge1 = TestUtils.NewFalseEdge();
            var trueEdge = TestUtils.NewTrueEdge();
            var trueEdge1 = TestUtils.NewTrueEdge();

            var factCol = new FactCollection<string>();
            var model = new CausalModel<string>(factCol, 123);

            Assert.False(Expressions.And(falseEdge, falseEdge1).Evaluate(factCol, model, model));
            Assert.False(Expressions.And(falseEdge, trueEdge).Evaluate(factCol, model, model));
            Assert.False(Expressions.And(trueEdge, falseEdge1).Evaluate(factCol, model, model));
            Assert.True(Expressions.And(trueEdge, trueEdge1).Evaluate(factCol, model, model));
        }

        [Fact]
        public void DisjunctionOperationTest()
        {
            var falseEdge = TestUtils.NewFalseEdge();
            var falseEdge1 = TestUtils.NewFalseEdge();
            var trueEdge = TestUtils.NewTrueEdge();
            var trueEdge1 = TestUtils.NewTrueEdge();

            var factCol = new FactCollection<string>();
            var model = new CausalModel<string>(factCol, 123);

            Assert.False(Expressions.Or(falseEdge, falseEdge1).Evaluate(factCol, model, model));
            Assert.True(Expressions.Or(falseEdge, trueEdge).Evaluate(factCol, model, model));
            Assert.True(Expressions.Or(trueEdge, falseEdge1).Evaluate(factCol, model, model));
            Assert.True(Expressions.Or(trueEdge, trueEdge1).Evaluate(factCol, model, model));
        }

        [Fact]
        public void InversionTest()
        {
            var falseEdge = TestUtils.NewFalseEdge();
            var trueEdge = TestUtils.NewTrueEdge();

            var factCol = new FactCollection<string>();
            var model = new CausalModel<string>(factCol, 123);

            Assert.True(Expressions.Not(falseEdge).Evaluate(factCol, model, model));
            Assert.False(Expressions.Not(trueEdge).Evaluate(factCol, model, model));
        }


        [Fact]
        public void EdgesTest()
        {
            const int TEST_SIZE = 5;

            var edges = new ProbabilityEdge[TEST_SIZE];
            for (int i = 0; i < TEST_SIZE; i++)
                edges[i] = TestUtils.NewTrueEdge();

            var or = Expressions.Or(edges);
            var and = Expressions.And(edges);
            var not = Expressions.Not(TestUtils.NewFalseEdge());

            Assert.Equal(TEST_SIZE, or.GetEdges().Count());
            Assert.Equal(TEST_SIZE, and.GetEdges().Count());
            Assert.Single(not.GetEdges());
            Assert.Single(Expressions.Edge(TestUtils.NewTrueEdge()).GetEdges());
        }
    }
}
