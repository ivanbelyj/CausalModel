using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
using CausalModel.Facts;
using CausalModel.Fixation;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CausalModel.Blocks.Resolving;
using CausalModel.Model.Resolving;
using CausalModel.Model.Instance;

namespace CausalModel.Tests;
internal static class TestUtils
{
    public static ProbabilityFactor CreateFalseFactor() => new ProbabilityFactor(0, null);
    public static ProbabilityFactor CreateTrueFactor() => new ProbabilityFactor(1, null);
    public static (Fact<string> cause, Fact<string> consequence)
        CreateCauseAndConsequence()
    {
        var cause = FactsBuilding.CreateFact(1,
            "Cause", null);

        var consequence = FactsBuilding.CreateFact(1, "Consequence", cause.Id);
        return (cause, consequence);
    }

    // public static ProbabilityEdge NewRootEdge() => new ProbabilityEdge(1, null, 0.5);
    //public static ProbabilityFactor NewNotRootEdge()
    //{
    //    var rootNode = FactsBuilding.CreateFact(1, "root node", null);
    //    return new ProbabilityFactor(1, rootNode.Id);
    //}

    public static CausesExpression CreateRootCausesExpression()
    {
        var expression = Expressions.Or(new ProbabilityFactor(1, null),
            new ProbabilityFactor(1, null));
        return expression;
    }

    public static CausesExpression CreateNotRootCausesExpression()
    {
        var rootNode = FactsBuilding.CreateFact(1, "root", null);

        var notRootEdge = new ProbabilityFactor(1, rootNode.Id);

        var expression1 = Expressions.Or(notRootEdge, new ProbabilityFactor(1, null));
        return expression1;
    }

    public static CausalModel<TFactValue> CreateCausalModel<TFactValue>(
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
        ResolvedModelProvider<TFactValue> provider,
        BlockResolver<TFactValue> resolver)
        CreateMockGenerator<TFactValue>(params Fact<TFactValue>[] facts)
    {
        return CreateMockGenerator(facts.ToList());
    }

    public static
        (CausalGenerator<TFactValue> generator,
        Fixator<TFactValue> fixator,
        ResolvedModelProvider<TFactValue> provider,
        BlockResolver<TFactValue> resolver)
        CreateMockGenerator<TFactValue>(List<Fact<TFactValue>>? facts = null)
    {
        var model = new CausalModel<TFactValue>();
        if (facts != null)
            model.Facts = facts;

        var builder = new CausalGeneratorBuilder<TFactValue>(model);
        var gen = builder.Build();

        return (gen,
            builder.Fixator,
            builder.ResolvedModelProvider!,
            builder.BlockResolver!);
    }
}
