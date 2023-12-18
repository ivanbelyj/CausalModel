using CausalModel.CausesExpressionTree;
using CausalModel.Model;
using CausalModel.Factors;
using CausalModel.Fixation;
using CausalModel.Facts;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using CausalModel.Model.Resolving;

namespace CausalModel.Tests
{
    public class CausesExpressionTreeTests
    {
        [Fact]
        public void EdgeLeafTest()
        {
            var falseEdge = TestUtils.CreateFalseFactor();
            var trueEdge = TestUtils.CreateTrueFactor();

            var (cause, factWithCause) = TestUtils.CreateCauseAndConsequence();

            var (generator, fixator, resolvedModelProvider, _)
                = TestUtils.CreateMockGenerator<string>(cause, factWithCause);
            var provider = resolvedModelProvider.GetRootModelProvider();

            Assert.False(new FactorLeaf(falseEdge).Evaluate(provider, fixator, generator));
            Assert.True(new FactorLeaf(trueEdge).Evaluate(provider, fixator, generator));
            Assert.Null(factWithCause.CausesExpression.Evaluate(provider, fixator, generator));
        }

        [Fact]
        public void ConjunctionOperationTest()
        {
            var falseEdge = TestUtils.CreateFalseFactor();
            var falseEdge1 = TestUtils.CreateFalseFactor();
            var trueEdge = TestUtils.CreateTrueFactor();
            var trueEdge1 = TestUtils.CreateTrueFactor();

            var (cause, factWithCause) = TestUtils.CreateCauseAndConsequence();

            var (generator, fixator, resolvedModelProvider, _)
                = TestUtils.CreateMockGenerator<string>(cause, factWithCause);
            var provider = resolvedModelProvider.GetRootModelProvider();

            Assert.False(Expressions.And(falseEdge, falseEdge1).Evaluate(provider, fixator, generator));
            Assert.False(Expressions.And(falseEdge, trueEdge).Evaluate(provider, fixator, generator));
            Assert.False(Expressions.And(trueEdge, falseEdge1).Evaluate(provider, fixator, generator));
            Assert.True(Expressions.And(trueEdge, trueEdge1).Evaluate(provider, fixator, generator));

            // Todo: ?
            //Assert.False(Expressions.And(factWithCause, falseEdge1).Evaluate(provider, fixator, generator));
            //Assert.False(Expressions.And(falseEdge, factWithCause).Evaluate(provider, fixator, generator));
            //Assert.Null(Expressions.And(factWithCause, trueEdge1).Evaluate(provider, fixator, generator));
            //Assert.Null(Expressions.And(trueEdge, factWithCause).Evaluate(provider, fixator, generator));
            //Assert.Null(Expressions.And(factWithCause, factWithCause).Evaluate(provider, fixator, generator));
        }

        [Fact]
        public void DisjunctionOperationTest()
        {
            var falseEdge = TestUtils.CreateFalseFactor();
            var falseEdge1 = TestUtils.CreateFalseFactor();
            var trueEdge = TestUtils.CreateTrueFactor();
            var trueEdge1 = TestUtils.CreateTrueFactor();

            var (cause, factWithCause) = TestUtils.CreateCauseAndConsequence();

            var (generator, fixator, resolvedModelProvider, _)
                = TestUtils.CreateMockGenerator<string>(cause, factWithCause);
            var provider = resolvedModelProvider.GetRootModelProvider();

            Assert.False(Expressions.Or(falseEdge, falseEdge1).Evaluate(provider, fixator, generator));
            Assert.True(Expressions.Or(falseEdge, trueEdge).Evaluate(provider, fixator, generator));
            Assert.True(Expressions.Or(trueEdge, falseEdge1).Evaluate(provider, fixator, generator));
            Assert.True(Expressions.Or(trueEdge, trueEdge1).Evaluate(provider, fixator, generator));

            //Assert.Null(Expressions.Or(nullFactor, falseEdge1).Evaluate(provider, fixator, generator));
            //Assert.Null(Expressions.Or(falseEdge, nullFactor).Evaluate(provider, fixator, generator));
            //Assert.True(Expressions.Or(nullFactor, trueEdge1).Evaluate(provider, fixator, generator));
            //Assert.True(Expressions.Or(trueEdge, nullFactor).Evaluate(provider, fixator, generator));
            //Assert.Null(Expressions.Or(nullFactor, nullFactor).Evaluate(provider, fixator, generator));
        }

        [Fact]
        public void InversionTest()
        {
            var falseEdge = TestUtils.CreateFalseFactor();
            var trueEdge = TestUtils.CreateTrueFactor();

            var (cause, factWithCause) = TestUtils.CreateCauseAndConsequence();

            var (generator, fixator, resolvedModelProvider, _)
                = TestUtils.CreateMockGenerator<string>(cause, factWithCause);
            var provider = resolvedModelProvider.GetRootModelProvider();

            Assert.True(Expressions.Not(falseEdge).Evaluate(provider, fixator, generator));
            Assert.False(Expressions.Not(trueEdge).Evaluate(provider, fixator, generator));
            Assert.False(Expressions.Not(trueEdge).Evaluate(provider, fixator, generator));

            //Assert.Null(Expressions.Not(nullFactor).Evaluate(provider, fixator, generator));
        }

        [Fact]
        public void EdgesTest()
        {
            const int TEST_SIZE = 5;

            var edges = new ProbabilityFactor[TEST_SIZE];
            for (int i = 0; i < TEST_SIZE; i++)
                edges[i] = TestUtils.CreateTrueFactor();

            var or = Expressions.Or(edges);
            var and = Expressions.And(edges);
            var not = Expressions.Not(TestUtils.CreateFalseFactor());

            Assert.Equal(TEST_SIZE, or.GetEdges().Count());
            Assert.Equal(TEST_SIZE, and.GetEdges().Count());
            Assert.Single(not.GetEdges());
            Assert.Single(Expressions.Edge(TestUtils.CreateTrueFactor()).GetEdges());
        }
    }
}
