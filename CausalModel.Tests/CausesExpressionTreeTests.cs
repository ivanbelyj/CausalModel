using CausalModel.CausesExpressionTree;
using CausalModel.Model;
using CausalModel.Factors;
using CausalModel.Fixation;
using CausalModel.Facts;
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
            var falseEdge = TestUtils.NewFalseFactor();
            var trueEdge = TestUtils.NewTrueFactor();

            var factCol = new FactCollection<string>();
            var fixator = new Fixator<string>();
            var generator = new CausalGenerator<string>(new CausalModel<string>()
            {
                Facts = factCol,
            }, 123, fixator);

            var nullFactor = TestUtils.NewNullFactor();

            Assert.False(new FactorLeaf(falseEdge).Evaluate(factCol, fixator, generator));
            Assert.True(new FactorLeaf(trueEdge).Evaluate(factCol, fixator, generator));
            Assert.Null(new FactorLeaf(nullFactor).Evaluate(factCol, fixator, generator));
        }

        [Fact]
        public void ConjunctionOperationTest()
        {
            var falseEdge = TestUtils.NewFalseFactor();
            var falseEdge1 = TestUtils.NewFalseFactor();
            var trueEdge = TestUtils.NewTrueFactor();
            var trueEdge1 = TestUtils.NewTrueFactor();

            var nullFactor = TestUtils.NewNullFactor();

            var factCol = new FactCollection<string>();
            var fixator = new Fixator<string>();
            var generator = new CausalGenerator<string>(new CausalModel<string>()
            {
                Facts = factCol,
            }, 123, fixator);

            Assert.False(Expressions.And(falseEdge, falseEdge1).Evaluate(factCol, fixator, generator));
            Assert.False(Expressions.And(falseEdge, trueEdge).Evaluate(factCol, fixator, generator));
            Assert.False(Expressions.And(trueEdge, falseEdge1).Evaluate(factCol, fixator, generator));
            Assert.True(Expressions.And(trueEdge, trueEdge1).Evaluate(factCol, fixator, generator));

            Assert.False(Expressions.And(nullFactor, falseEdge1).Evaluate(factCol, fixator, generator));
            Assert.False(Expressions.And(falseEdge, nullFactor).Evaluate(factCol, fixator, generator));
            Assert.Null(Expressions.And(nullFactor, trueEdge1).Evaluate(factCol, fixator, generator));
            Assert.Null(Expressions.And(trueEdge, nullFactor).Evaluate(factCol, fixator, generator));
            Assert.Null(Expressions.And(nullFactor, nullFactor).Evaluate(factCol, fixator, generator));
        }

        [Fact]
        public void DisjunctionOperationTest()
        {
            var falseEdge = TestUtils.NewFalseFactor();
            var falseEdge1 = TestUtils.NewFalseFactor();
            var trueEdge = TestUtils.NewTrueFactor();
            var trueEdge1 = TestUtils.NewTrueFactor();

            var nullFactor = TestUtils.NewNullFactor();

            var factCol = new FactCollection<string>();
            var fixator = new Fixator<string>();
            var generator = new CausalGenerator<string>(new CausalModel<string>()
            {
                Facts = factCol,
            }, 123, fixator);

            Assert.False(Expressions.Or(falseEdge, falseEdge1).Evaluate(factCol, fixator, generator));
            Assert.True(Expressions.Or(falseEdge, trueEdge).Evaluate(factCol, fixator, generator));
            Assert.True(Expressions.Or(trueEdge, falseEdge1).Evaluate(factCol, fixator, generator));
            Assert.True(Expressions.Or(trueEdge, trueEdge1).Evaluate(factCol, fixator, generator));

            Assert.Null(Expressions.Or(nullFactor, falseEdge1).Evaluate(factCol, fixator, generator));
            Assert.Null(Expressions.Or(falseEdge, nullFactor).Evaluate(factCol, fixator, generator));
            Assert.True(Expressions.Or(nullFactor, trueEdge1).Evaluate(factCol, fixator, generator));
            Assert.True(Expressions.Or(trueEdge, nullFactor).Evaluate(factCol, fixator, generator));
            Assert.Null(Expressions.Or(nullFactor, nullFactor).Evaluate(factCol, fixator, generator));
        }

        [Fact]
        public void InversionTest()
        {
            var falseEdge = TestUtils.NewFalseFactor();
            var trueEdge = TestUtils.NewTrueFactor();

            var factCol = new FactCollection<string>();
            var fixator = new Fixator<string>();
            var generator = new CausalGenerator<string>(new CausalModel<string>()
            {
                Facts = factCol,
            }, 123, fixator);

            var nullFactor = TestUtils.NewNullFactor();

            Assert.True(Expressions.Not(falseEdge).Evaluate(factCol, fixator, generator));
            Assert.False(Expressions.Not(trueEdge).Evaluate(factCol, fixator, generator));
            Assert.False(Expressions.Not(trueEdge).Evaluate(factCol, fixator, generator));

            Assert.Null(Expressions.Not(nullFactor).Evaluate(factCol, fixator, generator));
        }

        [Fact]
        public void EdgesTest()
        {
            const int TEST_SIZE = 5;

            var edges = new ProbabilityFactor[TEST_SIZE];
            for (int i = 0; i < TEST_SIZE; i++)
                edges[i] = TestUtils.NewTrueFactor();

            var or = Expressions.Or(edges);
            var and = Expressions.And(edges);
            var not = Expressions.Not(TestUtils.NewFalseFactor());

            Assert.Equal(TEST_SIZE, or.GetEdges().Count());
            Assert.Equal(TEST_SIZE, and.GetEdges().Count());
            Assert.Single(not.GetEdges());
            Assert.Single(Expressions.Edge(TestUtils.NewTrueFactor()).GetEdges());
        }
    }
}
