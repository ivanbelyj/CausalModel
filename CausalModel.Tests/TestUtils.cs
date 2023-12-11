using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
using CausalModel.Facts;
using CausalModel.Fixation;
using CausalModel.Model.Providers;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CausalModel.Blocks.Resolving;

namespace CausalModel.Tests;
internal static class TestUtils
{
    public static ProbabilityFactor NewFalseFactor() => new ProbabilityFactor(0, null);
    public static ProbabilityFactor NewTrueFactor() => new ProbabilityFactor(1, null);
    public static ProbabilityFactor NewNullFactor()
    {
        var notFixedNode = FactsBuilding.CreateFact(1,
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
        var rootNode = FactsBuilding.CreateFact(1, "root node", null);
        return new ProbabilityFactor(1, rootNode.Id);
    }

    public static CausesExpression NewCausesExpression()
    {
        var expression = Expressions.Or(new ProbabilityFactor(1, null),
            new ProbabilityFactor(1, null));
        return expression;
    }

    public static CausesExpression NewNotRootCausesExpression()
    {
        var rootNode = FactsBuilding.CreateFact(1, "root", null);

        var notRootEdge = new ProbabilityFactor(1, rootNode.Id);

        var expression1 = Expressions.Or(notRootEdge, new ProbabilityFactor(1, null));
        return expression1;
    }

    public static (ResolvingModelProvider<TFactValue> provider,
        BlockResolver<TFactValue> resolver) CreateModelProvider<TFactValue>(
        CausalModel<TFactValue> model,
        BlockResolvingMap<TFactValue> conventions)
    {
        var resolver = new BlockResolver<TFactValue>(conventions);
        var provider = new ResolvingModelProvider<TFactValue>(model, resolver);
        return (provider, resolver);
    }

    public static CausalModel<TFactValue> CreateMockCausalModel<TFactValue>(
        List<Fact<TFactValue>>? facts = null)
    {
        var res = new CausalModel<TFactValue>();
        if (facts != null)
            res.Facts = facts;
        return res;
    }

    public static 
        (CausalGenerator<TFactValue> generator,
        Fixator<TFactValue> fixator,
        ResolvingModelProvider<TFactValue> provider,
        BlockResolver<TFactValue> resolver)
        CreateMockGenerator<TFactValue>(List<Fact<TFactValue>>? facts = null)
    {
        var fixator = new Fixator<TFactValue>();
        var (provider, resolver) = CreateModelProvider(CreateMockCausalModel(facts),
            new BlockResolvingMap<TFactValue>());
        CausalGenerator<TFactValue> gen =
            new(provider, fixator);
        return (gen, fixator, provider, resolver);
    }
}
