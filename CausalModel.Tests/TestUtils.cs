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
using CausalModel.Common;

namespace CausalModel.Tests;
internal static class TestUtils
{
    public static ProbabilityFactor CreateFalseFactor() => new ProbabilityFactor(0, null);
    public static ProbabilityFactor CreateTrueFactor() => new ProbabilityFactor(1, null);
    public static (Fact<string> cause, Fact<string> consequence)
        CreateCauseAndConsequence()
    {
        var cause = FactBuilding.CreateFact(1,
            "Cause", null);

        var consequence = FactBuilding.CreateFact(1, "Consequence", cause.Id);
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
        var rootNode = FactBuilding.CreateFact(1, "root", null);

        var notRootEdge = new ProbabilityFactor(1, rootNode.Id);

        var expression1 = Expressions.Or(notRootEdge, new ProbabilityFactor(1, null));
        return expression1;
    }

    public static CausalModel<TFactValue> CreateCausalModel<TFactValue>(
        List<Fact<TFactValue>>? facts = null)
        where TFactValue : class
    {
        var res = new CausalModel<TFactValue>("Name");
        if (facts != null)
            res.Facts = facts;
        return res;
    }

    public static FixationFacade<TFactValue> CreateFixationFacade<TFactValue>(
        params Fact<TFactValue>[] facts)
        where TFactValue : class
    {
        return CreateFixationFacade(facts.ToList());
    }

    public static FixationFacade<TFactValue> CreateFixationFacade<TFactValue>(
        List<Fact<TFactValue>>? facts = null)
        where TFactValue : class
    {
        var model = new CausalModel<TFactValue>("Name");
        if (facts != null)
            model.Facts = facts;

        var builder = new FixationFacadeBuilder<TFactValue>(new CausalBundle<TFactValue>()
        {
            CausalModels = new[] { model }
        });
        var facade = builder.Build();

        return facade;
    }
}
